using Assets.Scripts.Managers;
using Assets.Scripts.UI;
using Common.Data;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : UIWindow{
    public Text title;//商店标题
    public Text money;

    public GameObject shopItem;
    ShopDefine shop;
    public Transform[] itemRoot;
	// Use this for initialization
	void Start () {
        StartCoroutine(InitItems());
	}
    
    /// <summary>
    /// 生成商店项目 并对UIShopItem赋值
    /// </summary>
    /// <returns></returns>
    IEnumerator InitItems()
    {
        int count = 0;
        int page = 0;
        foreach (var kv in DataManager.Instance.ShopItems[this.shop.ID])
        {
            if (kv.Value.Status>0)
            {
                GameObject go = Instantiate(shopItem, itemRoot[page]);
                UIShopItem uiItem = go.GetComponent<UIShopItem>();
                uiItem.SetShopItem(kv.Key, kv.Value, this);
                //分页
                //count++;
                //if (count>=10)
                //{
                //    count = 0;
                //    page++;
                //    itemRoot[page].gameObject.SetActive(true);
                //}
            }
        }

        yield return null;
    }
    public void SetShop(ShopDefine shop)
    {
        this.shop = shop;
        this.title.text = shop.Name;
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }

    private UIShopItem selectedItem;
    public void SelectShopItem(UIShopItem item)
    {
        //把上一个选中的项目改为否
        if (selectedItem!=null)
        {
            selectedItem.Selected = false;
        }
        //绑定新的选中项目
        selectedItem = item;
    }
   
    public void OnClickBuy()
    {
        if (this.selectedItem==null)
        {
            MessageBox.Show("请选择购买的道具", "购买提示");
            return;
        }
        if (!ShopManager.Instance.BuyItem(this.shop.ID,selectedItem.ShopItemID))
        {
            MessageBox.Show("请求购买失败", "购买提示");
        }
    }
    public override void OnClikeClose()
    {
        base.OnClikeClose();
        ShopManager.Instance.uiShop = null;
        

    }
}
