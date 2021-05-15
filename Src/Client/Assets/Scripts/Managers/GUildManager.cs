using Assets.Scripts.UI;
using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Managers
{
    public class GUildManager:Singleton<GUildManager>
    {
        public NGuildInfo guildInfo;

        public NGuildMemberInfo myMemberInfo;
        public bool HasGuild//有没有公会，有公会信息就有
        {
            get
            {
                return this.guildInfo != null;
            }
            
        }

        /// <summary>
        /// 初始化，绑定管理器的公会信息
        /// </summary>
        /// <param name="guild"></param>
        public void Init(NGuildInfo guild)
        {
            
            this.guildInfo = guild;
            if (guild==null)
            {
                myMemberInfo = null;
                return;
            }
            foreach (var item in guild.Members)
            {
                if (item.characterId==User.Instance.CurrentCharacter.Id)
                {
                    myMemberInfo = item;
                    break;
                }
            }
        }

        /// <summary>
        /// 如果有公会，显示公会界面，否则显示加入/创建公会界面
        /// </summary>
        public void ShowGuild()
        {
            if (this.HasGuild)
            {
                UIManager.Instance.Show<UIGuild>();
            }
            else
            {
               var win= UIManager.Instance.Show<UIGuildPopNoGuild>();
                win.Onclose += PopNoGuild_OnClose;
            }
        }

        private void PopNoGuild_OnClose(UIWindow sender, WindowResult windowResult)
        {
            //如果点击了创建公会
            if (windowResult==WindowResult.Yes)
            {
                UIManager.Instance.Show<UIGuildPopCreate>();
            }
            else if (windowResult == WindowResult.No)
            {
                UIManager.Instance.Show<UIGuildList>();
            }
        }
    }
}
