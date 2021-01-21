using Common;
using Common.Data;
using GameServer.Core;
using GameServer.Managers;
using GameServer.Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entities
{
    class Character : CharacterBase,IPostResponser
    {
        /// <summary>
        /// data为数据库表角色
        /// </summary>
        public TCharacter Data;


        public ItemManager ItemManager;
        public QuestManager QuestManager;
        public StatusManager StatusManager;
        public FriendManager FriendManager;

        public Team Team;
        public int TeamUpateTS;

        public Character(CharacterType type,TCharacter cha):
            base(new Core.Vector3Int(cha.MapPosX, cha.MapPosY, cha.MapPosZ),new Core.Vector3Int(100,0,0))
        {
            //data为数据库表角色
            this.Data = cha;
            this.Id = cha.ID;
            this.Info = new NCharacterInfo();
            this.Info.Type = type;
            this.Info.Id = cha.ID;
            this.Info.EntityId = this.entityId;
            this.Info.Name = cha.Name;
            this.Info.Level = 10;//cha.Level;
            this.Info.ConfigId = cha.TID;
            this.Info.Gold = cha.Gold;
            this.Info.Class = (CharacterClass)cha.Class;
            this.Info.mapId = cha.MapID;
            this.Info.Entity = this.EntityData;
            this.Define = DataManager.Instance.Characters[this.Info.ConfigId];

            this.ItemManager = new ItemManager(this);
            this.ItemManager.GetItemInfos(this.Info.Items);
            this.Info.Bag = new NBagInfo();
            this.Info.Bag.Unlocked = this.Data.Bag.Unlocked;
            this.Info.Bag.Items = this.Data.Bag.Items;
            this.Info.Equips = this.Data.Equips;
            this.QuestManager = new QuestManager(this);
            this.QuestManager.GetQuestInfos(this.Info.Quests);
            this.StatusManager = new StatusManager(this);
            this.FriendManager = new FriendManager(this);
            this.FriendManager.GetFriendInfos(this.Info.Friends);
           
        }
        public long Gold
        {
            get
            {
                return this.Data.Gold;

            }
            set
            {
                if (this.Data.Gold==value)
                {
                    return;
                }
                this.StatusManager.AddGoldChange((int)(value - this.Data.Gold));
                this.Data.Gold = value;//修改了数据库表角色
            }
        }

        /// <summary>
        /// 转化为只有基本信息的NCharacterInfo
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public NCharacterInfo GetBasicInfo()
        {
            return new NCharacterInfo()
            {
                Id = this.Id,
                Name = Info.Name,
                Class = Info.Class,
                Level = Info.Level,
            };
        }
        public void PostProcess(NetMessageResponse message)
        {
            Log.InfoFormat("PostProcess>Character:characterID{0}:{1}", this.Id, this.Info.Name);
            this.FriendManager.PostProcess(message);

            if (this.Team!=null)
            {
                Log.InfoFormat("PostProcess>Team:characterID:{0}:{1}", this.Id, this.Info.Id, TeamUpateTS, this.Team.timestamp);
                if (TeamUpateTS<this.Team.timestamp)
                {
                    TeamUpateTS = Team.timestamp;
                    this.Team.PostProcess(message);
                }
            }

            if (this.StatusManager.HasStatus)
            {
                this.StatusManager.PostProcess(message);
            }
        }

        /// <summary>
        /// 角色离开调用 更新在线状态
        /// </summary>
        public void Clear()
        {
            this.FriendManager.OfflineNotify();
        }
    }
}
