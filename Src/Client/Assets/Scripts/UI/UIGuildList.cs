using Assets.Scripts.Managers;
using Assets.Scripts.Services;
using Assets.Scripts.UI;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuildList : UIWindow
{

    public GameObject itemPrefab;
    public ListView listMain;
    public Transform itemRoot;
    public UIGuildInfo uiInfo;
    public UIGuildItem selectedItem;

    private void Start()
    {
        this.listMain.onItemSelected += this.OnGuildMemberSelected;
        this.uiInfo.Info = null;
        GuildService.Instance.OnGuildListResult += UpDateGuildList;
        GuildService.Instance.SendGuildListRequest();
        GuildService.Instance.OnGuildJoinSucess += OnJoinSucess;
    }

    private void OnJoinSucess()
    {
        Destroy(this);
        UIManager.Instance.Show<UIGuild>();
    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildListResult -= UpDateGuildList;
        GuildService.Instance.OnGuildJoinSucess -= OnJoinSucess;
    }
    private void OnGuildMemberSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIGuildItem;
        this.uiInfo.Info = this.selectedItem.Info;
    }

    private void UpDateGuildList(List<NGuildInfo> guilds)
    {
        ClearList();
        InitItems(guilds);
        Debug.Log("公会列表刷新");
    }

    private void InitItems(List<NGuildInfo> guilds)
    {
        foreach (var item in guilds)
        {
            GameObject go = Instantiate(itemPrefab, this.listMain.transform);
            go.SetActive(true);
            UIGuildItem ui = go.GetComponent<UIGuildItem>();
            ui.SetGuildInfo(item);
            this.listMain.AddItem(ui);
        }
    }

    private void ClearList()
    {
        this.listMain.RemoveAll();
    }
    /// <summary>
    /// 加入按钮点击事件
    /// </summary>
    public void OnClickJoin()
    {
        //如果没有选中
        if (selectedItem==null)
        {
            MessageBox.Show("请选中一个公会");
            return;
        }
        var form = MessageBox.Show(string.Format("确认要加入【{0}】吗？", selectedItem.Info.GuildName), "申请加入公会", MessageBoxType.Confirm
            , "确定", "取消");
        form.OnYes = () =>
        {
            GuildService.Instance.SendGuildJoinRequest(selectedItem.Info.Id);
        };
    }
    
}
