using Entities;
using Gameobject;
using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GammaObjectManager : MonoSingleton<GammaObjectManager> {

    Dictionary<int, GameObject> Characters = new Dictionary<int, GameObject>();

    // Use this for initialization
    protected override void OnStart()
    {
        StartCoroutine(InintGameObject());
        CharacterManager.Instance.OnCharacterEnter+= OnCharacterEnter;//注册委托
        CharacterManager.Instance.OnCharacterLeave+=OnCharacterLeave;

    }

    

    private void OnDestroy()
    {
        CharacterManager.Instance.OnCharacterEnter -= OnCharacterEnter;//销毁时解除注册
        CharacterManager.Instance.OnCharacterLeave -= OnCharacterLeave;
    }


    // Update is called once per frame
   
    IEnumerator InintGameObject()
    {
        foreach (var cha in CharacterManager.Instance.Characters.Values)
        {
            CreateCharacterObject(cha);
            yield return null; 
        }
    }
    private void OnCharacterEnter(Character cha)
    {
        Debug.Log("角色添加委托执行");
        CreateCharacterObject(cha);
    }
    private void OnCharacterLeave(Character cha)
    {
        if (!Characters.ContainsKey(cha.entityId))
        {
            return;
        }
        if (Characters[cha.entityId]!=null)
        {
            Destroy(Characters[cha.entityId]);
            Characters.Remove(cha.entityId);
        }
    }
    /// <summary>
    /// 在unity场景中创建角色
    /// </summary>
    /// <param name="cha"></param>
    private void CreateCharacterObject(Character cha)
    {
        if (!Characters.ContainsKey(cha.entityId)||Characters[cha.entityId] ==null)
        {
            UnityEngine.Object obj = Resloader.Load<UnityEngine.Object>(cha.characterDefine.Resource);
            if (obj==null)
            {
                Debug.LogErrorFormat("Character[{0}] Resourse[{1}] not existed", cha.characterDefine.TID, cha.characterDefine.Resource);
                return;
            }
            GameObject go =(GameObject) Instantiate(obj,this.transform) ;
            go.name = "Character_" + cha.Id + "_" + cha.Name;

            go.transform.position = GameObjectTool.LogicToWorld(cha.position);
            go.transform.forward = GameObjectTool.LogicToWorld(cha.direction);

            Characters[cha.entityId] = go;

            
            UIWorldElementManager.Instance.AddCharacterNameBar(go.transform, cha); 
        }
        InitGameObject(cha,Characters[cha.entityId]);
    }
    private void InitGameObject(Character cha,GameObject go)
    {
        EntiyController ec = go.GetComponent<EntiyController>();
        if (ec != null)
        {
            ec.entity = cha;
            ec.isPlayer = cha.IsCurrentPlayer;
        }

        PlayerInputerController pc = go.GetComponent<PlayerInputerController>();
        if (pc != null)
        {
            //确定是否为当前控制角色
            if (cha.IsCurrentPlayer)
            {
                User.Instance.currentCharacterObject = go;
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
    }
}
