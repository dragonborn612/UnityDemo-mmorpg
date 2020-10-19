using Assets.Scripts.Managers;
using Entities;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElementManager : MonoSingleton<UIWorldElementManager> {
    public GameObject nameBarPrefab;
    public GameObject npcStatusPrefab;
    private Dictionary<Transform, GameObject> elementName = new Dictionary<Transform, GameObject>();
    private Dictionary<Transform, GameObject> elementStatus = new Dictionary<Transform, GameObject>();
	
    public void AddCharacterNameBar(Transform owenr,Character character)
    {
        GameObject goNamebar = Instantiate(nameBarPrefab, this.transform);
        goNamebar.name = "NameBar" + character.entityId;
        goNamebar.GetComponent<UINameBar>().characeter = character;
        goNamebar.GetComponent<UIWorldElement>().owner = owenr;
        goNamebar.SetActive(true);
        elementName[owenr] = goNamebar;
    }
    
    public void RemoveCharacterNameBar(Transform owenr)
    {
        if (elementName.ContainsKey(owenr))
        {
            Destroy(elementName[owenr]);
            elementName.Remove(owenr);
        }
    }
    /// <summary>
    /// 添加或更新状态图标
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="status"></param>
    public void AddNpcQuestStatus(Transform owner,NpcQuestStatus status)
    {
        if (this.elementStatus.ContainsKey(owner))
        {
            elementStatus[owner].GetComponent<UIQuestStatus>().SetQuestStatus(status);
        }
        else
        {
            GameObject go = Instantiate(npcStatusPrefab, this.transform);
            go.name = "NpcQuestStatus" + owner.name;
            go.GetComponent<UIWorldElement>().owner = owner;
            go.GetComponent<UIQuestStatus>().SetQuestStatus(status);
            go.SetActive(true);
            this.elementStatus[owner] = go;
        }
    }
    /// <summary>
    /// 移除状态图标
    /// </summary>
    /// <param name="owner"></param>
    public void RemoveNpcQuestStatus(Transform owner)
    {
        if (this.elementStatus.ContainsKey(owner))
        {
            Destroy(this.elementStatus[owner]);
            this.elementStatus.Remove(owner);
        }
    }
}
