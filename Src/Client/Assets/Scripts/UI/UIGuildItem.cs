using System;
using System.Collections;
using System.Collections.Generic;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildItem : ListView.ListViewItem {
    public NGuildInfo Info { get; internal set; }

    public Text guildId;
    public Text guildName;
    public Text memberNum;
    public Text leaderName;


    public Image background;
    public Sprite normalBg;
    public Sprite selectedBg;

    public override void onSelected(bool selected)
    {
        this.background.overrideSprite = selected ? selectedBg : normalBg;
    }

    public void SetGuildInfo(NGuildInfo item)
    {
        this.Info = item;
        if (guildId != null) guildId.text = item.Id.ToString();
        if (guildName!= null) guildName.text = item.GuildName;
        if (memberNum != null) memberNum.text = item.memberCont.ToString();
        if (leaderName != null) leaderName.text = item.leaderName;
       
    }
}
