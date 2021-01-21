using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class TeamService:Singleton<TeamService>
    {
        public TeamService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamLeaveRequest>(this.OnTeamLeave);
        }
        public void Init()
        {
            TeamManager.Instance.Init();
        }

        private void OnTeamLeave(NetConnection<NetSession> sender, TeamLeaveRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnTeamLeave: :character:{0} TeamID:{1}:{2}", character.Id, message.TeamId, message.CharacterId);
            sender.Session.Response.teamLeave = new TeamLeaveResponse();
            sender.Session.Response.teamLeave.Result = Result.Success;
            sender.Session.Response.teamLeave.CharacterId = message.CharacterId;
            character.Team.Leave(character);
            sender.SendResponse();

        }

        private void OnTeamInviteResponse(NetConnection<NetSession> sender, TeamInviteResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnTeamInviteResponse ::character:{0} Result:{1} FromID:{2} ToID:{3}", character.Id, response.Result, response.Request.FromId, response.Request.ToId);
            sender.Session.Response.teamInviteRes = response;
            if (response.Result==Result.Success)
            {
                var requester = SessionManager.Instance.GetSession(response.Request.FromId);
                if (requester==null)
                {
                    sender.Session.Response.teamInviteRes.Result = Result.Failed;
                    sender.Session.Response.teamInviteRes.Errormsg = "请求者已下线";

                }
                else
                {
                    TeamManager.Instance.AddTeamMember(requester.Session.Character, character);
                    requester.Session.Response.teamInviteRes = response;
                    requester.SendResponse();
                }
            }
            sender.SendResponse();
        }

        private void OnTeamInviteRequest(NetConnection<NetSession> sender, TeamInviteRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnTeamInviteRequest ::FromdID:{0} FromName:{1} ToId:{2} ToName:{3}", message.FromId, message.FromName, message.ToId, message.ToName);

            var target = SessionManager.Instance.GetSession(message.ToId);
            if (target==null)
            {
                sender.Session.Response.teamInviteRes = new TeamInviteResponse();
                sender.Session.Response.teamInviteRes.Result = Result.Failed;
                sender.Session.Response.teamInviteRes.Errormsg = "对方不在线";
                sender.SendResponse();
                return;

            }
            if (target.Session.Character.Team!=null)
            {
                sender.Session.Response.teamInviteRes = new TeamInviteResponse();
                sender.Session.Response.teamInviteRes.Result = Result.Failed;
                sender.Session.Response.teamInviteRes.Errormsg = "对方已经有队伍";
                sender.SendResponse();
                return;
            }
            Log.InfoFormat("ForwardTeamInviteRequest::FromId:{0} FromName:{1} ToID:{2} ToName:{3}", message.FromId, message.FromName, message.ToId, message.ToName);
            target.Session.Response.teamInviteReq = message;
            target.SendResponse();
        }
    }
}
