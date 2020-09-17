using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using Assets.Scripts.UI;
using Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharEquip : UIWindow
{

    public Text title;
    public Text money;
    public Text characterName;


    public GameObject itemPrefab;
    public GameObject itemEquipedPrefab;

    public Transform itemListRoot;

    public List<Transform> slots;
	// Use this for initialization
	void Start ()
    {
        RefreshUI();
        EquipMananger.Instance.OnEquipChange += RefreshUI;
        characterName.text = "Lv"+User.Instance.CurrentCharacter.Level+" "+User.Instance.CurrentCharacter.Name.ToString();
        money.text = User.Instance.CurrentCharacter.Gold.ToString();
        
    }

    private void OnDestroy()
    {
        EquipMananger.Instance.OnEquipChange -= RefreshUI;
    }
    private void RefreshUI()
    {
        ClearAllEquipList();
        InitAllEquipItems();
        ClearEquipedList();
        InitEquipedItems();
    }

    /// <summary>
    /// 初始化所有装备列表
    /// </summary>
    private void InitAllEquipItems()
    {
        foreach (var kv in ItemManager.Instance.Items)
        {
            if (kv.Value.itemDefine.Type==ItemType.Equip&&kv.Value.itemDefine.LimitClass==User.Instance.CurrentCharacter.Class)
            {
                //已经装备的不显示
                if (EquipMananger.Instance.Contains(kv.Key))
                {
                    continue;
                }
                GameObject go = Instantiate(itemPrefab, itemListRoot);
                UIEquipItem ui = go.GetComponent<UIEquipItem>();
                ui.SetEquipItem(kv.Key, kv.Value, this, false);
            }
        }
    }

    void ClearAllEquipList()
    {
        foreach (var item in itemListRoot.GetComponentsInChildren<UIEquipItem>())
        {
            Destroy(item.gameObject);
        }
    }

    void ClearEquipedList()
    {
        foreach (var item in slots)
        {
            if (item.childCount>0)
            {
                Destroy(item.GetChild(0).gameObject);
            }
        }
    }

    /// <summary>
    /// 初始化已装备的列表
    /// </summary>
    void InitEquipedItems()
    {
        for (int i = 0; i < (int)EqipSlot.SoltMax; i++)
        {
            var item = EquipMananger.Instance.Equips[i];
            {
                if (item!=null)
                {
                    GameObject go = Instantiate(itemEquipedPrefab, slots[i]);
                    UIEquipItem ui = go.GetComponent<UIEquipItem>();
                    ui.SetEquipItem(i, item, this, true);
                }
            }
        }
    }
    internal void DoEquip(Item item)
    {
        EquipMananger.Instance.EquipItem(item);
    }

    internal void UnEquip(Item item)
    {
        EquipMananger.Instance.UnEquipItem(item);
    }
    public override void OnClikeClose()
    {
        this.Close();
        EquipMananger.Instance.uiCharaterEquip = null;
    }
}

