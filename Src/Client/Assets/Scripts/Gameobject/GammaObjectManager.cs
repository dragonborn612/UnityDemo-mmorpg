using Entities;
using Gameobject;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GammaObjectManager : MonoBehaviour {

    Dictionary<int, GameObject> Characters = new Dictionary<int, GameObject>();

	// Use this for initialization
	void Start () {
        StartCoroutine(InintGameObject());
        CharacterManager.Instance.OnCharacterEnter = OnCharacterEnter;//注册委托
	}

    private void OnDestroy()
    {
        CharacterManager.Instance.OnCharacterEnter = null;//销毁时解除注册
    }


    // Update is called once per frame
    void Update () {
		
	}
    IEnumerator InintGameObject()
    {
        foreach (var cha in CharacterManager.Instance.Characters.Values)
        {
            CreateCharacterObject(cha);
            yield return null; 
        }
    }
    private void OnCharacterEnter(Character arg0)
    {
        
    }

    /// <summary>
    /// 在unity场景中创建角色
    /// </summary>
    /// <param name="cha"></param>
    private void CreateCharacterObject(Character cha)
    {
        if (!Characters.ContainsKey(cha.nCharacterInfo.Id)||Characters[cha.nCharacterInfo.Id]==null)
        {
            UnityEngine.Object obj = Resloader.Load<UnityEngine.Object>(cha.characterDefine.Resource);
            if (obj==null)
            {
                Debug.LogErrorFormat("Character[{0}] Resourse[{1}] not existed", cha.characterDefine.TID, cha.characterDefine.Resource);
                return;
            }
            GameObject go =(GameObject) Instantiate(obj) ;
            go.name = "Character_" + cha.nCharacterInfo.Id + "_" + cha.nCharacterInfo.Name;

            go.transform.position = GameObjectTool.LogicToWorld(cha.position);
            go.transform.forward = GameObjectTool.LogicToWorld(cha.direction);

            Characters[cha.nCharacterInfo.Id] = go;

            EntiyController ec = go.GetComponent<EntiyController>();
            if (ec!=null)
            {
                ec.entity = cha;
                ec.isPlayer = cha.IsPlayer;
            }

            PlayerInputerController pc = go.GetComponent<PlayerInputerController>();
            if (pc != null)
            {
                //确定是否为当前控制角色
                if (cha.nCharacterInfo.Id==Models.User.Instance.CurrentCharacter.Id)
                {
                    MainPlayerCamera.Instance.player = go;
                    pc.enabled = true;
                    pc.isPlayer = true;
                    pc.character = cha;
                    pc.entiyController = ec;
                }
                else
                {
                    pc.enabled = false;
                }
            }
            UIWorldElementManager.Instance.AddCharacterNameBar(go.transform, cha); 
        }
    }
}
