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
    class FriendService:Singleton<FriendService>
    {
       public FriendService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendAddRequest>(this.OnFriendAddRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendAddResponse>(this.OnFriendAddResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendRemoveRequest>(this.OnFriendRemove);
        }
        public void Init()
        {

        }
       
        /// <summary>
        /// 收到好友请求消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnFriendAddRequest(NetConnection<NetSession> sender, FriendAddRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnFriendAddRequest:FromID:{0} FromName:{1} ToID:{2} ToName:{3}", request.FromId, request.FromName, request.ToId, request.ToId);

            if (request.ToId==0)
            {//如果没有传入ID则用用户名查找
                foreach (var cha in CharacterManager.Instance.Characters)
                {
                    if (cha.Value.Data.Name==request.ToName)
                    {
                        request.ToId = cha.Key;
                        break;
                    }
                }
            }
            NetConnection<NetSession> friend = null;
            if (request.ToId>0)//用ID查找
            {
                if (character.FriendManager.GetFriendInfo(request.ToId)!=null)//是不是在好友列表里
                {
                    sender.Session.Response.friendAddRes = new FriendAddResponse();
                    sender.Session.Response.friendAddRes.Result = Result.Failed;
                    sender.Session.Response.friendAddRes.Errormsg = "已经是好友了";
                    sender.SendResponse();
                    return;
                }
                friend = SessionManager.Instance.GetSession(request.ToId);
                if (friend ==null)//不存在或者不在线
                {
                    sender.Session.Response.friendAddRes = new FriendAddResponse();
                    sender.Session.Response.friendAddRes.Result = Result.Failed;
                    sender.Session.Response.friendAddRes.Errormsg = "好友不存在或不在线";
                    sender.SendResponse();
                    return;
                }

                Log.InfoFormat("ForwardRequst :FromId:{0} FromName:{1} ToId:{2} ToName:{3}", request.FromId, request.FromName, request.ToId, request.ToId);
                friend.Session.Response.friendAddReq = request;
                friend.SendResponse();
            }
        }
        /// <summary>
        /// 收到好友响应消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnFriendAddResponse(NetConnection<NetSession> sender, FriendAddResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnFriendAddResponse::Character:{0} Result:{1} FromId:{2} ToId:{3}", character.Id, response.Result, response.Request.FromId, response.Request.ToId);
            sender.Session.Response.friendAddRes = response;
            var requester = SessionManager.Instance.GetSession(response.Request.FromId);
            if (response.Result==Result.Success)
            {//好友接受了请求
                
                if (requester==null)//没找到session
                {
                    
                    sender.Session.Response.friendAddRes.Result = Result.Failed;
                    sender.Session.Response.friendAddRes.Errormsg = "请求者已下线";

                }
                else//有session
                {
                    character.FriendManager.AddFriend(requester.Session.Character);
                    requester.Session.Character.FriendManager.AddFriend(character);//互相加好友进好友列表
                    DBService.Instance.Save();
                    sender.SendResponse();
                    requester.Session.Response.friendAddRes = response;
                    requester.Session.Response.friendAddRes.Result = Result.Success;
                    requester.Session.Response.friendAddRes.Errormsg = "添加好友成功";
                    requester.SendResponse();

                }
            }
            else
            {//好友拒绝了请求
                if (requester!=null)
                {
                    requester.Session.Response.friendAddRes = response;
                    requester.Session.Response.friendAddRes.Result = Result.Failed;
                    requester.Session.Response.friendAddRes.Errormsg = "对方拒绝了你的请求";
                    requester.SendResponse();
                }
            }
        }

        private void OnFriendRemove(NetConnection<NetSession> sender, FriendRemoveRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnFriendRemove::Character:{0} FriendReletionID:{1}", character.Id, message.Id);
            sender.Session.Response.friendRemove = new FriendRemoveResponse();
            sender.Session.Response.friendRemove.Id = message.Id;

            
            if (character.FriendManager.RemoveFriendById(message.Id))//删除自己的好友
            {
                sender.Session.Response.friendRemove.Result = Result.Success;
                //删除别人好友列表中的自己
                var friend = SessionManager.Instance.GetSession(message.frienId);
                if (friend!=null)
                {//好友在线
                    friend.Session.Character.FriendManager.RemoveFriendByFriendId(character.Id);
                }
                else
                {//好友不在线
                    this.RemoveFriend(message.frienId, character.Id);
                }
            }
            else
            {
                sender.Session.Response.friendRemove.Result = Result.Failed;
            }
            DBService.Instance.Save();
            sender.SendResponse();
        }

        private void RemoveFriend(int characterId, int FriendId)
        {
            var remveItem = DBService.Instance.Entities.TCharacterFriends.FirstOrDefault(v => v.CharacterID == characterId && v.FriendID == FriendId);
            if (remveItem!=null)
            {
                DBService.Instance.Entities.TCharacterFriends.Remove(remveItem);
            }
        }
    }
}
