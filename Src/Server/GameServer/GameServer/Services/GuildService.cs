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
    class GuildService:Singleton<GuildService>
    {
        public GuildService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildCreateRequest>(this.OnGuildCreate);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildListRequest>(this.OnGuildList);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinResponse>(this.OnJoinResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildLeaveRequest>(this.OnGuildLeave);

            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildAdminRequest>(this.OnGuildAdmin);

        }

        public void Init()
        {
            GuildManager.Instance.Init();
        }
        private void OnGuildCreate(NetConnection<NetSession> sender, GuildCreateRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildCreate:: GuildName:{0} character:[{1}]:{2}", request.GuildName, character.Id, character.Data.Name);
            sender.Session.Response.guildCreate = new GuildCreateResponse();
            if (character.Guild!=null)//已经有公会
            {
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.Session.Response.guildCreate.Errormsg = "已经有公会";
                sender.SendResponse();
                return;
            }
            if (GuildManager.Instance.CheckNameExisted(request.GuildName))
            {
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.Session.Response.guildCreate.Errormsg = "公会名称已存在";
                sender.SendResponse();
                return;
            }
            GuildManager.Instance.CreateGuild(request.GuildName, request.GuildNotice, character);
            sender.Session.Response.guildCreate.guildInfo = character.Guild.GuildInfo(character);
            sender.Session.Response.guildCreate.Result = Result.Success;
            sender.SendResponse();
        }

        /// <summary>
        /// 收到成员加入公会请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildJoinRequest(NetConnection<NetSession> sender, GuildJoinRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildJoinRequest::GuilfId:{0} ", request.Apply.GuildId,character.Id,character.Name);
            var guild = GuildManager.Instance.GetGuild(request.Apply.GuildId);
            if (guild==null)
            {
                sender.Session.Response.guildJoinRes = new GuildJoinResponse();
                sender.Session.Response.guildJoinRes.Result = Result.Failed;
                sender.Session.Response.guildJoinRes.Errormsg = "公会不存在";
                sender.SendResponse();
                return;
            }
            request.Apply.characterID = character.Data.ID;
            request.Apply.Name = character.Data.Name;
            request.Apply.Class = character.Data.Class;
            request.Apply.Level = character.Data.Level;
            if (guild.JoinApply(request.Apply))
            {
                var leader = SessionManager.Instance.GetSession(guild.Date.LeaderID);
                if (leader!=null)//会长在线
                {
                    //给会长发送请求加入申请
                    leader.Session.Response.guildJoinReq = request;
                    leader.SendResponse();
                }
            }
            else
            {
                sender.Session.Response.guildJoinRes = new GuildJoinResponse();
                sender.Session.Response.guildJoinRes.Result = Result.Failed;
                sender.Session.Response.guildJoinRes.Errormsg = "请勿重复申请";
                sender.SendResponse();
            }
        }

        /// <summary>
        /// 收到会长对请求公会的响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnJoinResponse(NetConnection<NetSession> sender, GuildJoinResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildJoinResponse::Guild:{0} character:[{1}]{2}", response.Apply.GuildId, character.Id, character.Name);

            var guild = GuildManager.Instance.GetGuild(response.Apply.GuildId);
            if (response.Result==Result.Success)
            {
                guild.JoinAppove(response.Apply);//处理审批
            }
            var requester = SessionManager.Instance.GetSession(response.Apply.characterID);
            if (requester!=null)//请求者在线
            {
                requester.Session.Character.Guild = guild;

                requester.Session.Response.guildJoinRes = new GuildJoinResponse();
                requester.Session.Response.guildJoinRes.Result = Result.Success;
                requester.Session.Response.guildJoinRes.Errormsg = "加入公会成功";
                requester.SendResponse();
            }

            sender.Session.Response.Guild = new GuildResponse();
            sender.Session.Response.Guild.Result = Result.Success;
            sender.Session.Response.Guild.guildInfo = character.Guild.GuildInfo(character);
            sender.SendResponse();


        }

        private void OnGuildLeave(NetConnection<NetSession> sender, GuildLeaveRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildleave::character:{0}", character.Id);
            sender.Session.Response.guildLeave = new GuildLeaveResponse();

            character.Guild.Leave(character);
            sender.Session.Response.guildLeave.Result = Result.Success;

            DBService.Instance.Save();

            sender.SendResponse();
        }

        private void OnGuildList(NetConnection<NetSession> sender, GuildListRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildList: character:[{0}:{1}]", character.Id, character.Data.Name);

            sender.Session.Response.guildList = new GuildListResponse();
            sender.Session.Response.guildList.Guilds.AddRange(GuildManager.Instance.GetGuildsInfo());
            sender.Session.Response.guildList.Result = Result.Success;
            sender.SendResponse();
        }
        private void OnGuildAdmin(NetConnection<NetSession> sender, GuildAdminRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildAdmin：TragetID[{0}]",  request.Target);
            sender.Session.Response.guildAdmin = new GuildAdminResponse();
            //权限检验 是否有公会 可以多加几个检验
            if (character.Guild==null)
            {
                sender.Session.Response.guildAdmin = new GuildAdminResponse();
                sender.Session.Response.guildAdmin.Result = Result.Failed;
                sender.Session.Response.guildAdmin.Errormsg = "你没公会不要乱来";
                sender.SendResponse();
                return;
            }

            character.Guild.ExecuteAdmin(request.Command, request.Target, character.Id);

            var target = SessionManager.Instance.GetSession(request.Target);
            if (target!=null)
            {
                target.Session.Response.guildAdmin = new GuildAdminResponse();
                target.Session.Response.guildAdmin.Result = Result.Success;
                target.Session.Response.guildAdmin.Command = request;
                target.SendResponse();
            }
            sender.Session.Response.guildAdmin.Result = Result.Success;
            sender.Session.Response.guildAdmin.Command = request;
            sender.SendResponse();
        }


    }
}
