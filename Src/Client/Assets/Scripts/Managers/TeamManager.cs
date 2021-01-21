using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Managers
{
    public class TeamManager:Singleton<TeamManager>
    {

        public void Init()
        {

        }
        /// <summary>
        /// 更新User 的队伍信息并显示或关闭队伍UI
        /// </summary>
        /// <param name="team"></param>
        public void UpdateTeamInfo(NTeamInfo team)
        {
            User.Instance.TeamInfo = team;
            ShowTeamUI(team != null);
        }

        private void ShowTeamUI(bool show)
        {
            if (UIMain.Instance!=null)
            {
                UIMain.Instance.ShowTeamUI(show);
            }
        }
    }
}
