using Assets.Scripts.Managers;
using Assets.Scripts.Services;
using Assets.Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuildApplyList : UIWindow {

    public GameObject itemPrefab;
    public ListView listMain;
    public Transform itemRoot;

	// Use this for initialization
	void Start () {
        GuildService.Instance.OnGuildUpdate += UpDateList;
        GuildService.Instance.SendGuildListRequest();
        this.UpDateList();
	}
    private void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate -= UpDateList;
    }
    private void UpDateList()
    {
        Clearlist();
        InitItem();
        Debug.Log("公会审批列表刷新");
    }

    private void Clearlist()
    {
        this.listMain.RemoveAll();
    }

    private void InitItem()
    {
        foreach (var item in GUildManager.Instance.guildInfo.Applies)
        {
            GameObject go = Instantiate(itemPrefab, itemRoot);
            go.SetActive(true);
            UIGuildApplyItem ui = go.GetComponent<UIGuildApplyItem>();
            
            ui.SetItemInfo(item);
            this.listMain.AddItem(ui);
        }
    }
}
