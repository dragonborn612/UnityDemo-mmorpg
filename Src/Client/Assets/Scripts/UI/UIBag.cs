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
    /// <summary>
    /// 背包格子
    /// </summary>
    List<Image> slots;
    private void Start()
    {
        if (slots==null)
        {
            slots = new List<Image>();
            for (int page = 0; page < this.pages.Length; page++)
            {
                slots.AddRange(this.pages[page].GetComponentsInChildren<Image>(true));
            }
            StartCoroutine(InitBag());
        }
    }
    IEnumerator InitBag()
    {
        for (int i = 0; i <BagManager.Instance.Items.Length; i++)
        {
            BagItem item = BagManager.Instance.Items[i];
            if (item.ItemId>0&&item.Count>0)
            {
                GameObject go = Instantiate(bagItem, slots[i].transform);
                go.SetActive(true);
                UIIconItem ui = go.GetComponent<UIIconItem>();
                ItemDefine define = ItemManager.Instance.Items[item.ItemId].itemDefine;
                ui.SetMainIcon(define.Icon, item.Count.ToString());
            }
        }
        for (int i = BagManager.Instance.Items.Length; i < slots.Count; i++)
        {
            slots[i].color = Color.gray;
        }
        yield return null;
    }
    public void SetTitle(string title)
    {
        this.money.text = User.Instance.CurrentCharacter.Id.ToString();
    }
    public void OnReset()
    {
        BagManager.Instance.Reset();
    }
}
