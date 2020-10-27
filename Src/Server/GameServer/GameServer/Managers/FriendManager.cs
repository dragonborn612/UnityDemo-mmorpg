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
                friendInfo.friendInfo = GetBasicInfo(character.Info);
                friendInfo.friendInfo.Name = character.Info.Name;
                friendInfo.friendInfo.Class = character.Info.Class;
                friendInfo.friendInfo.Level = character.Info.Level;
                character.FriendManager.UpdateFrindInfo(this.Owner.Info, 1);//更新好友的好友列表里本玩家的在线状态
                friendInfo.Status = 1;

            }
            //通知好友自己上线
            //var friendSession = SessionManager.Instance.GetSession(friend.FriendID);
            //if (friendSession!=null)
            //{
            //    friendSession.Session.Character.FriendManager.UpdateFrindInfo(friendSession.Session.Character.Info, 1);
            //}
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

        /// <summary>
        /// 转化为只有基本信息的NCharacterInfo
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private NCharacterInfo GetBasicInfo(NCharacterInfo info)
        {
            return new NCharacterInfo()
            {
                Id = info.Id,
                Name = info.Name,
                Class = info.Class,
                Level = info.Level,
            };
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
