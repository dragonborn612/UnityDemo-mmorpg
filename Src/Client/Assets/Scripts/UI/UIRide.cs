using Assets.Scripts.Managers;
using Assets.Scripts.UI;
using Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRide :UIWindow {

    public Text descript;
    public GameObject itemPrefan;
    public ListView ListMain;
    private UIRideItem selectedItem;
    // Use this for initialization
    void Start()
    {
        RefreshUI();
        this.ListMain.onItemSelected += this.OnItemSelected;

    }

    private void OnItemSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIRideItem;
        this.descript.text = this.selectedItem.item.itemDefine.Description;
    }

    
    private void RefreshUI()
    {
        ClearItems();
        InitItems();
    }

   

    private void InitItems()
    {
        foreach (var kv in ItemManager.Instance.Items)
        {
            if (kv.Value.itemDefine.Type == ItemType.Ride /*&& kv.Value.itemDefine.LimitClass == CharacterClass.None|| kv.Value.itemDefine.LimitClass ==User.Instance.CurrentCharacter.Class*/)
            {
              
               
                GameObject go = Instantiate(itemPrefan, this.ListMain.transform);
                go.SetActive(true);
                UIRideItem ui = go.GetComponent<UIRideItem>();
                ui.SetRideItem( kv.Value, this, false);
                this.ListMain.AddItem(ui);
            }
        }
    }
    private void ClearItems()
    {
        this.ListMain.RemoveAll();
    }

    public void DoRide()
    {
        if (this.selectedItem==null)
        {
            MessageBox.Show("请选择要召唤的坐骑", "提示");
            return;
        }
        User.Instance.Ride(this.selectedItem.item.Id);
    }

  
}
