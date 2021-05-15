using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Services;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildApplyItem : ListView.ListViewItem {

    public Text nickNmae;
    public Text @class;
    public Text level;

    public NguildApplyInfo info;
	// Use this for initialization

    internal void SetItemInfo(NguildApplyInfo item)
    {
        this.info = item;
        if (this.nickNmae != null) nickNmae.text = info.Name;
        if (this.@class != null) @class.text = info.Class.ToString();
        if (this.level != null) level.text = info.Level.ToString();
    }

    public void OnClickAccpet()
    {
      var ui=  MessageBox.Show(string.Format("要通过【{0}】的公会申请吗？", this.info.Name), "审核申请", MessageBoxType.Confirm, "同意加入", "取消").OnYes =
            () => { GuildService.Instance.SendGuildJoinApply(true, this.info); /*Debug.Log("确认按钮绑定成功")*/; };
    }
    public void OnClickDecline()
    {
        MessageBox.Show(string.Format("要拒绝【{0}】的公会申请吗？", this.info.Name), "审核申请", MessageBoxType.Confirm, "拒绝加入", "取消").OnYes =
            () => { GuildService.Instance.SendGuildJoinApply(false, this.info); };
    }
}
