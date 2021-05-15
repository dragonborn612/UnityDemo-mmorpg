using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using Assets.Scripts.UI;
using Common.Data;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBag : UIWindow
{
    public Text money;
    public Transform[] pages;
    public GameObject bagItem;
    private List<GameObject> UiItems=new List<GameObject>();
    /// <summary>
    /// 背包格子
    /// </summary>
   // [SerializeField]
    List<Image> slots=null;

    private void Start()
    {
        if (slots==null)
        {
            //把格子添加到格子列表内
            slots = new List<Image>();
            for (int page = 0; page < this.pages.Length; page++)
            {
                slots.AddRange(this.pages[page].GetComponentsInChildren<Image>(true));
            }

            StartCoroutine(InitBag());
        }
        money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }
    IEnumerator InitBag()
    {   //生成物品
        for (int i = 0; i <BagManager.Instance.Items.Length; i++)
        {
            BagItem item = BagManager.Instance.Items[i];
            if (item.ItemId>0&&item.Count>0)
            {
                GameObject go = Instantiate(bagItem, slots[i].transform);
                go.SetActive(true);
                UiItems.Add(go);
                UIIconItem ui = go.GetComponent<UIIconItem>();
                ItemDefine define = ItemManager.Instance.Items[item.ItemId].itemDefine;
                ui.SetMainIcon(define.Icon, item.Count.ToString());
            }
        }
        //未解锁的格子变灰
        for (int i = BagManager.Instance.Items.Length; i < slots.Count; i++)
        {
            slots[i].color = Color.gray;
        }
        yield return null;
    }
    public void SetTitle(string title)
    {
        this.money.text = User.Instance.CurrentCharacter.Id.ToString();//应该有问题 原代码
    }
    public void OnReset()
    {
        //BagManager.Instance.Reset();
    }
    public override void OnClikeClose()
    {
        base.OnClikeClose();
        BagManager.Instance.uIBag = null;

    }
    public void UpdateBag()
    {
        foreach (var item in UiItems)
        {
            Destroy(item);
        }
        UiItems.Clear();
        BagManager.Instance.Reset();
        StartCoroutine(InitBag());

    }
}
