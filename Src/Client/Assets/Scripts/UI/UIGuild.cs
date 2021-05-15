using Assets.Scripts.Managers;
using Assets.Scripts.Services;
using Assets.Scripts.UI;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuild : UIWindow
{
    public GameObject itemPrefab;
    public ListView listMain;
    public Transform itemRoot;
    public UIGuildInfo uiInfo;
    public UIGuildMemberItem selectedItem;

    public GameObject panelAdmin;
    public GameObject panelLeader;
    // Use this for initialization
    void Start() {
        GuildService.Instance.OnGuildUpdate+= UpdateUI;
        this.listMain.onItemSelected += this.OnGuildMemberSelected;
        this.UpdateUI();
    }
    private void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate -= UpdateUI;
    }

    private void OnGuildMemberSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIGuildMemberItem;
    }

    private void UpdateUI()
    {
        this.uiInfo.Info = GUildManager.Instance.guildInfo;

        ClearList();
        InitItems();

        //控制管理按钮的显示
        this.panelAdmin.SetActive(GUildManager.Instance.myMemberInfo.Titlr > GuildTitle.Nome);
        this.panelLeader.SetActive(GUildManager.Instance.myMemberInfo.Titlr == GuildTitle.President);
        Debug.Log("公会成员列表刷新");
    }

    private void InitItems()
    {
        foreach (var item in GUildManager.Instance.guildInfo.Members)
        {
            GameObject go = Instantiate(itemPrefab, this.listMain.transform);
            go.SetActive(true);
            UIGuildMemberItem ui = go.GetComponent<UIGuildMemberItem>();
            ui.SetGuildmemberInfo(item);
            this.listMain.AddItem(ui);

        }
    }

    private void ClearList()
    {
        this.listMain.RemoveAll();
    }


    //各种点击事件

    public void OnClickAppliesList()//申请列表
    {
        UIManager.Instance.Show<UIGuildApplyList>();
    }
    public void OnClickLeave()
    {
        //扩展
    }
    public void OnClickChat()
    {

    }
    public void OnClickKickout()//踢出
    {
        if (selectedItem==null)
        {
            MessageBox.Show("请选择要踢出的成员");
            return;
        }
        MessageBox.Show(string.Format("要踢【{0}】出公会吗？", this.selectedItem.Info.Info.Name), "踢出公会", MessageBoxType.Confirm).OnYes =
            () => { GuildService.Instance.SendAdminCommand(GuildAdminCommand.Kickout, this.selectedItem.Info.Info.Id); };
    }
    public void OnClickPromote()//提升
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要晋升的成员");
            return;
        }
        if (selectedItem.Info.Titlr!=GuildTitle.Nome)
        {
            MessageBox.Show("对方已经身份尊贵");
            return;
        }
        MessageBox.Show(string.Format("要晋升【{0}】为公会副会长吗？", this.selectedItem.Info.Info.Name), "晋升", MessageBoxType.Confirm).OnYes =
            () => { GuildService.Instance.SendAdminCommand(GuildAdminCommand.Promote, this.selectedItem.Info.Info.Id); };
    }
    public void OnClickDepose()//免职
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要罢免的成员");
            return;
        }
        if (selectedItem.Info.Titlr == GuildTitle.Nome)
        {
            MessageBox.Show("对方已经没有职务");
            return;
        }
        if (selectedItem.Info.Titlr == GuildTitle.President)
        {
            MessageBox.Show("会长可不是你能动的哦！");
            return;
        }
        MessageBox.Show(string.Format("要罢免【{0}】的公会职务吗？", this.selectedItem.Info.Info.Name), "罢免", MessageBoxType.Confirm).OnYes =
            () => { GuildService.Instance.SendAdminCommand(GuildAdminCommand.Depost, this.selectedItem.Info.Info.Id); };
    }
    public void OnClickTransfer()//转让
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要把会长转让的成员");
            return;
        }
        if (selectedItem.Info.Titlr == GuildTitle.President)
        {
            MessageBox.Show("不能转给自己");
            return;
        }
        MessageBox.Show(string.Format("要把会长转让给【{0}】吗？", this.selectedItem.Info.Info.Name), "转让会长", MessageBoxType.Confirm).OnYes =
            () => { GuildService.Instance.SendAdminCommand(GuildAdminCommand.Transfer, this.selectedItem.Info.Info.Id); };
    }
    public void OnClickSetNotice()
    {
        //扩展；
    }
}

