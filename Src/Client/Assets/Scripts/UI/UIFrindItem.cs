using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class UIFrindItem:ListView.ListViewItem
    {
        public Text nickname;
        public Text @class;
        public Text level;
        public Text status;

        public Image background;
        public Sprite normalBg;
        public Sprite selectBg;

        public NFriendInfo Info;
        public override void onSelected(bool selected)
        {
            this.background.overrideSprite = selected ? selectBg : normalBg;
        }

        public void SetFriendInfo(NFriendInfo item)
        {
            this.Info = item;
            if (this.nickname != null) this.nickname.text = this.Info.friendInfo.Name;
            if (this.@class != null) this.@class.text = this.Info.friendInfo.Class.ToString();
            if (this.level != null) this.level.text = this.Info.friendInfo.Level.ToString();
            if (this.status != null) this.status.text = this.Info.Status == 1 ? "在线" : "离线";
           
        }
    }
}
