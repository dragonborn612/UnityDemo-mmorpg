using Common.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIShopItem : MonoBehaviour,ISelectHandler {


    public Image icon;
    public Text title;
    public Text price;
    public Text limitClass;
    public Text count;

    public Image Background;
    public Sprite normalBg;
    public Sprite selectBg;
    public bool selected;
   public bool Selected
    {
        get { return selected; }
        set
        {
            selected = value;
            this.Background.sprite = selected ? selectBg : normalBg;//选中修改背景
        }
    }
    public int ShopItemID { get; set; }

    private UIShop shop;
    private ItemDefine item;
    private ShopItemDefine ShopItem { get; set; }


    public void SetShopItem(int id,ShopItemDefine shopItem,UIShop owner)
    {
        this.shop = owner;
        this.ShopItemID = id;
        this.ShopItem = shopItem;
        this.item = DataManager.Instance.Items[this.ShopItem.ItemID];

        this.title.text = this.item.Name;
        this.count.text = ShopItem.Count.ToString();
        this.limitClass.text = item.LimitClass.ToString();
        this.price.text = ShopItem.Price.ToString();
        icon.overrideSprite = Resloader.Load<Sprite>(item.Icon);
    }


    public void OnSelect(BaseEventData eventData)
    {
        this.Selected = true;
        this.shop.SelectShopItem(this);
    }
}
