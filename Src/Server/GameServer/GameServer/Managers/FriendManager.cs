using Common;
using GameServer.Entities;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class FriendManager
    {
        Character Owner;
        List<NFriendInfo> friends = new List<NFriendInfo>();

        bool friendChange = false;
        public FriendManager( Character owner)
        {
            this.Owner = owner;
            this.InitFriends();
        }

        /// <summary>
        /// 把manager的friends提取到list
        /// </summary>
        /// <param name="list"></param>
        public void GetFriendInfos(List<NFriendInfo> list)
        {
            foreach (var f in this.friends)
            {
                list.Add(f);
            }
        }
        /// <summary>
        /// 从玩家数据库提取好友列表
        /// </summary>
        private void InitFriends()
        {
            this.friends.Clear();
            foreach (var friend in this.Owner.Data.Friends)
            {
                this.friends.Add(GetFriendInfo(friend));
            }
        }

        public NFriendInfo GetFriendInfo(TCharacterFriend friend)
        {
            NFriendInfo friendInfo = new NFriendInfo();
            Character character = CharacterManager.Instance.GetCharacter(friend.FriendID);
            friendInfo.friendInfo = new NCharacterInfo();
            friendInfo.Id = friend.Id;
            if (character==null)//角色管理器里没有
            {
                friendInfo.friendInfo.Id = friend.FriendID;
                friendInfo.friendInfo.Name = friend.FriendName;
                friendInfo.friendInfo.Class =(CharacterClass) friend.Class;
                friendInfo.friendInfo.Level = friend.Level;
                friendInfo.Status = 0;//不在线
            }
            else
            {
                friendInfo.friendInfo =character.GetBasicInfo();
                friendInfo.friendInfo.Name = character.Info.Name;
                friendInfo.friendInfo.Class = character.Info.Class;
                friendInfo.friendInfo.Level = character.Info.Level;
                if (friend.Level!=character.Info.Level)
                {
                    friend.Level = character.Info.Level;
                }
                character.FriendManager.UpdateFrindInfo(this.Owner.Info, 1);//更新好友的好友列表里本玩家的在线状态
                friendInfo.Status = 1;

            }
            //通知好友自己上线
            //var friendSession = SessionManager.Instance.GetSession(friend.FriendID);
            //if (friendSession!=null)
            //{
            //    friendSession.Session.Character.FriendManager.UpdateFrindInfo(friendSession.Session.Character.Info, 1);
            //}
            Log.InfoFormat("{0}:{1} GetFriendInfo:{2}:{3} Status:{4}", this.Owner.Info.Name, friendInfo.friendInfo.Name, Owner.Info.Id, friendInfo.friendInfo.Id, friendInfo.Status);
            return friendInfo;
        }

        public NFriendInfo GetFriendInfo(int friendId)
        {
            foreach (var f in this.friends)
            {
                if (f.friendInfo.Id==friendId)
                {
                    return f;
                }
            }
            return null;
        }
        /// <summary>
        /// 更新在线状态
        /// </summary>
        /// <param name="friendInfo"></param>
        /// <param name="Status"></param>
        public void UpdateFrindInfo(NCharacterInfo friendInfo, int Status)
        {
            foreach (var f in this.friends)
            {
                if (f.friendInfo.Id==friendInfo.Id)
                {
                    f.Status = Status;
                    break;
                }
            }
            this.friendChange = true;
        }
        public void OfflineNotify()
        {
            foreach (var friendInfo in this.friends)
            {
                var friend = CharacterManager.Instance.GetCharacter(friendInfo.friendInfo.Id);
                if (friend!=null)
                {
                    friend.FriendManager.UpdateFrindInfo(this.Owner.Info, 0);
                }
            }
        }


        /// <summary>
        /// 向本玩家的数据库好友列表里加好友
        /// </summary>
        /// <param name="friend"></param>
        public void AddFriend(Character friend)
        {
            TCharacterFriend tf = new TCharacterFriend()
            {
                FriendID = friend.Id,
                FriendName = friend.Data.Name,
                Class = friend.Data.Class,
                Level = friend.Data.Level

            };
            this.Owner.Data.Friends.Add(tf);
            friendChange = true;
        }

        public bool RemoveFriendByFriendId(int frienId)
        {
            var removeItem = this.Owner.Data.Friends.FirstOrDefault(v => v.FriendID == frienId);//数据库查询
            if (removeItem!=null)
            {
                DBService.Instance.Entities.TCharacterFriends.Remove(removeItem);//数据库删除
            }
            friendChange = true;
            return true;
        }

        public bool RemoveFriendById(int id)
        {
            var removeItem = this.Owner.Data.Friends.FirstOrDefault(v => v.Id == id);//数据库查询
            if (removeItem != null)
            {
                DBService.Instance.Entities.TCharacterFriends.Remove(removeItem);//数据库删除
            }
            friendChange = true;
            return true;
        }

        /// <summary>
        /// 后处理，好友列表变动后发送friendList，并从新初始化
        /// </summary>
        /// <param name="mesage"></param>
        public void PostProcess(NetMessageResponse mesage)
        {
            if (friendChange)
            {
                Log.InfoFormat("PostProcess>FriendManager:characterID:{0}:{1}", this.Owner.Id, this.Owner.Info.Name);
                this.InitFriends();
                if (mesage.friendList==null)
                {
                    mesage.friendList = new FriendListResponse();
                    mesage.friendList.Friends.AddRange(this.friends);
                }
                friendChange = false;
            }
        }
    }
}
