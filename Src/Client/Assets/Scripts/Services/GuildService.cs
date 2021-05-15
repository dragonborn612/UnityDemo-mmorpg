using Assets.Scripts.Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Services
{
    public class GuildService : Singleton<GuildService>, IDisposable
    {
        public UnityAction OnGuildUpdate;
        public UnityAction<bool> OnGuildCreateResult;
        public UnityAction OnGuildJoinSucess;

        public UnityAction<List<NGuildInfo>> OnGuildListResult;

        public void Init()
        {

        }
        public GuildService()
        {
            MessageDistributer.Instance.Subscribe<GuildCreateResponse>(this.OnGuildcreate);
            MessageDistributer.Instance.Subscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer.Instance.Subscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Subscribe<GuildLeaveResponse>(this.OnGuildLeave);
            MessageDistributer.Instance.Subscribe<GuildAdminResponse>(this.OnGuildAdmin);
        }

       
        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<GuildCreateResponse>(this.OnGuildcreate);
            MessageDistributer.Instance.Unsubscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Unsubscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer.Instance.Unsubscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer.Instance.Unsubscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Unsubscribe<GuildLeaveResponse>(this.OnGuildLeave);
            MessageDistributer.Instance.Unsubscribe<GuildAdminResponse>(this.OnGuildAdmin);
        }
        

       
        /// <summary>
        /// 收到创建公会响应时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildcreate(object sender, GuildCreateResponse message)
        {
            Debug.LogFormat("OnGuildCreateResonse:{0}", message.Result);
            if (OnGuildCreateResult!=null)
            {
                this.OnGuildCreateResult.Invoke(message.Result == Result.Success);
            }
            if (message.Result==Result.Success)
            {
                GUildManager.Instance.Init(message.guildInfo);
                MessageBox.Show(string.Format("{0} 公会创建成功", message.guildInfo.GuildName), "公会");

            }
            else
            {
                MessageBox.Show(string.Format("{0} 公会创建失败", message.guildInfo.GuildName), "公会");
            }
           
        }

        /// <summary>
        /// 发送创建公会请求
        /// </summary>
        /// <param name="guildName"></param>
        /// <param name="notice"></param>
        public void SendGuildCreate(string guildName,string notice)
        {
            Debug.Log("SendGuildCreate");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildCreate = new GuildCreateRequest();
            message.Request.guildCreate.GuildName = guildName;
            message.Request.guildCreate.GuildNotice = notice;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 成员发送加入公会请求
        /// </summary>
        public void SendGuildJoinRequest(int guildId)
        {
            Debug.Log("SendguildJoinrequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinReq = new GuildJoinRequest();
            message.Request.guildJoinReq.Apply = new NguildApplyInfo();
            message.Request.guildJoinReq.Apply.GuildId = guildId;


            NetClient.Instance.SendMessage(message);

        }
        /// <summary>
        /// 当会长收到成员加入公会请求时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildJoinRequest(object sender, GuildJoinRequest message)
        {
            var confirm = MessageBox.Show(string.Format("{0} 申请加入公会", message.Apply.Name), "公会申请", MessageBoxType.Confirm, "接受", "拒绝");
            confirm.OnYes = () =>
            {
                this.SendGuildJoinResponse(true, message);
            };
            confirm.OnNo = () =>
            {
                this.SendGuildJoinResponse(false, message);
            };

        }
        /// <summary>
        /// 会长发送对成员加入公会请求的响应
        /// </summary>
        /// <param name="accpt"></param>
        /// <param name="request"></param>
        public void SendGuildJoinResponse(bool accpt,GuildJoinRequest request)
        {
            Debug.Log("SendGuildJoinResponse");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinRes = new GuildJoinResponse();
            message.Request.guildJoinRes.Result = Result.Success;
            message.Request.guildJoinRes.Apply = request.Apply;
            message.Request.guildJoinRes.Apply.Result = accpt ? ApplyResult.Accpet : ApplyResult.Reject;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 当成员收到加入公会回复的响应时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildJoinResponse(object sender, GuildJoinResponse message)
        {
            Debug.LogFormat("OnGuildJoinResponse:{0}", message.Result);

            if (message.Result==Result.Success)
            {
                MessageBox.Show("加入公会成功", "公会").OnYes=()=> {
                    if (this.OnGuildJoinSucess != null)
                    {
                        this.OnGuildJoinSucess.Invoke();
                    }
                };
            }
            else
            {
                MessageBox.Show("加入公会失败", "公会");
            }
        }
        
        /// <summary>
        /// 收到公会信息时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuild(object sender, GuildResponse message)
        {
            Debug.LogFormat("OnGuild:{0} {1}:{2}", message.Result, message.guildInfo.Id, message.guildInfo.GuildName);
            GUildManager.Instance.Init(message.guildInfo);//更新当前公会
            Debug.Log("当前公会信信与成员列表更新");
            if (this.OnGuildUpdate!=null)
            {
                this.OnGuildUpdate.Invoke();
            }

        }

        public void SendGuildLeaveRequest()
        {
            Debug.Log("SendGuildLeaveRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildLeave = new GuildLeaveRequest();
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 收到服务器离开公会响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildLeave(object sender, GuildLeaveResponse message)
        {
            if (message.Result==Result.Success)
            {
                GUildManager.Instance.Init(null);//更新当前公会信息
                MessageBox.Show("离开公会成功", "工会");
            }
            else
            {
                MessageBox.Show("离开工会失败", "公会",MessageBoxType.Error);
            }
        }
        public void SendGuildListRequest()
        {
            Debug.Log("SendGuildListRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildList = new GuildListRequest();
            NetClient.Instance.SendMessage(message);
        }
        /// <summary>
        /// 收到服务器发送的加入公会时的公会列表响应时；
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildList(object sender, GuildListResponse message)
        {
            Debug.Log("OnGuildList");
           
            if (OnGuildListResult != null)
            {
                this.OnGuildListResult.Invoke(message.Guilds);
            }
        }
        private void OnGuildAdmin(object sender, GuildAdminResponse message)
        {
            Debug.LogFormat("GuildAdmin:{0}:{1}", message.Command, message.Result);
            MessageBox.Show(string.Format("执行操作:{0} 结果:{1} {2}", message.Command, message.Result, message.Errormsg));

        }

        /// <summary>
        /// 发送加入公会请求的申批
        /// </summary>
        /// <param name="accept"></param>
        /// <param name="apply"></param>
        public void SendGuildJoinApply(bool accept,NguildApplyInfo apply)
        {
            Debug.Log("SendGuildJoinApply");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinRes = new GuildJoinResponse();
            message.Request.guildJoinRes.Result = Result.Success;
            message.Request.guildJoinRes.Apply = apply;
            message.Request.guildJoinRes.Apply.Result = accept ? ApplyResult.Accpet : ApplyResult.Reject;
            NetClient.Instance.SendMessage(message);
        }

        public void SendAdminCommand(GuildAdminCommand command,int characterID)
        {
            Debug.LogFormat("SendAdminCommand:{0} characterId:{1}", command, characterID);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildAdmin = new GuildAdminRequest();
            message.Request.guildAdmin.Command = command;
            message.Request.guildAdmin.Target = characterID;
            NetClient.Instance.SendMessage(message);
        }
    }
}
