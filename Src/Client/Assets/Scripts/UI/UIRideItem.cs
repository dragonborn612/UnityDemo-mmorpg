using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRideItem :ListView.ListViewItem {

    public Image icon;
    public Text title;
    public Text level;

    public Image Background;
    public Sprite normalBg;
    public Sprite selectBg;

    public override void onSelected(bool selected)
    {
        this.Background.overrideSprite = Selected ? selectBg : normalBg;
    }

    public Item item;
    public void SetRideItem( Item item, UIRide owner, bool equiped)
    {
      
        this.item = item;
        

        if (this.title != null) this.title.text = this.item.itemDefine.Name;
        if (this.level != null) this.level.text = item.itemDefine.Level.ToString();       
        if (this.icon != null) this.icon.overrideSprite = Resloader.Load<Sprite>(this.item.itemDefine.Icon);
    }
}
