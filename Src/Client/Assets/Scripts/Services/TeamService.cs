using Assets.Scripts.Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Services
{
    class TeamService : Singleton<TeamService>,IDisposable
    {
        public void Init()
        {

        }
        public TeamService()
        {
            MessageDistributer.Instance.Subscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer.Instance.Subscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer.Instance.Subscribe<TeamInfoResponse>(this.OnTeamInfo);
            MessageDistributer.Instance.Subscribe<TeamLeaveResponse>(this.OnTeamLeave);
            
        }
        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer.Instance.Unsubscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer.Instance.Unsubscribe<TeamInfoResponse>(this.OnTeamInfo);
            MessageDistributer.Instance.Unsubscribe<TeamLeaveResponse>(this.OnTeamLeave);
        }

        private void OnTeamInfo(object sender, TeamInfoResponse message)
        {
            Debug.Log("OnTeamInfo");
            TeamManager.Instance.UpdateTeamInfo(message.Team);
        }

        private void OnTeamLeave(object sender, TeamLeaveResponse message)
        {
            if (message.Result == Result.Success)
            {
                TeamManager.Instance.UpdateTeamInfo(null);
                MessageBox.Show("退出成功", "退出队伍");
            }
            else
            {
                MessageBox.Show("退出失败", "退出队伍", MessageBoxType.Error);
            }
        }

        private void OnTeamInviteResponse(object sender, TeamInviteResponse message)
        {
            if (message.Result==Result.Success)
            {
                MessageBox.Show(message.Request.ToName + " 加入您的队伍", "邀请组队成功");
            }
            else
            {
                MessageBox.Show(message.Errormsg, "邀请组队失败");
            }
        }

        /// <summary>
        /// 收到添加队伍请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnTeamInviteRequest(object sender, TeamInviteRequest message)
        {
            var confirm = MessageBox.Show(string.Format("{0} 邀请您加入队伍", message.FromName), "组队请求", MessageBoxType.Confirm, "接受", "拒绝");
            confirm.OnYes = () =>
            {
                this.SendTeamInviteResponse(true, message);
            };
            confirm.OnNo = () =>
            {
                this.SendTeamInviteResponse(false, message);
            };
        }

       

        internal void SendTeamLeaveRequest(int id)
        {
            Debug.Log("SendTeamLeaveRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.teamLeave = new TeamLeaveRequest();
            message.Request.teamLeave.CharacterId = User.Instance.CurrentCharacter.Id;
            message.Request.teamLeave.TeamId= User.Instance.TeamInfo.Id;
            NetClient.Instance.SendMessage(message);
        }
        public void SendTeamInvitRequest(int friendId,string friendName)
        {
            Debug.Log("SendTeamInviteRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.teamInviteReq = new TeamInviteRequest();
            message.Request.teamInviteReq.FromId = User.Instance.CurrentCharacter.Id;
            message.Request.teamInviteReq.FromName = User.Instance.CurrentCharacter.Name;
            message.Request.teamInviteReq.ToId = friendId;
            message.Request.teamInviteReq.ToName = friendName;

            NetClient.Instance.SendMessage(message);

        }
        public void SendTeamInviteResponse(bool accpt,TeamInviteRequest request)
        {
            Debug.Log("SendTeamInviteResponse");
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.teamInviteRes = new TeamInviteResponse();
            message.Response.teamInviteRes.Result = accpt ? Result.Success : Result.Failed;
            message.Response.teamInviteRes.Errormsg = accpt ? "组队成功" : "对方拒绝了组队请求";
            message.Response.teamInviteRes.Request = request;
            NetClient.Instance.SendMessage(message);

        }
    }
}
