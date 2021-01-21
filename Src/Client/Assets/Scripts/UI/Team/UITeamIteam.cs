using Assets.Scripts.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Team
{
    public class UITeamIteam : ListView.ListViewItem
    {
        public Text nickname;
        public Image classIcon;
        public Image leaderIcon;

        public Image background;

        public override void onSelected(bool selected)
        {
            this.background.enabled = selected ? true : false;
        }

        public int idx;
        public NCharacterInfo Info;

        private void Start()
        {
            this.background.enabled = false;
        }

        public void SetMemberInfo(int idx,NCharacterInfo item,bool isLeader) //member 成员
        {
            this.idx = idx;
            this.Info = item;
            if (nickname != null) this.nickname.text = this.Info.Level.ToString().PadRight(4) + this.Info.Name;
            if (this.classIcon != null) this.classIcon.overrideSprite = SpriteManager.Instance.classIcons[(int)this.Info.Class - 1];
            if (this.leaderIcon != null) this.leaderIcon.gameObject.SetActive(isLeader);
        }
    }
}
