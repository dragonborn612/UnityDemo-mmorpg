using Common.Utils;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    class Guild
    {
        public TGuild Date;

        public int Id { get { return this.Date.Id; } }

       // private Character leader;

        public string Name { get { return this.Date.Name; } }

        //public List<Character> Members = new List<Character>();

        public double timestemp;

        public Guild(TGuild guild)
        {
            this.Date = guild;
        }

        /// <summary>
        /// 检测有没有申请过，添加一条公会申请记录
        /// </summary>
        /// <param name="apply"></param>
        /// <returns></returns>
        internal bool JoinApply(NguildApplyInfo apply)
        {
            var oldApply = this.Date.Applies.FirstOrDefault(v => v.CharacterId == apply.characterID);//检测申请的玩家之前有没有申请过本公会
            if (oldApply!=null)
            {
                return false;
            }
            //向数据库表中添加一条数据
            var dbApply = DBService.Instance.Entities.TGuildApplies.Create();
            dbApply.GuildId = apply.GuildId;
            dbApply.CharacterId = apply.characterID;
            dbApply.Name = apply.Name;
            dbApply.Class = apply.Class;
            dbApply.Level = apply.Level;
            dbApply.ApplyTime = DateTime.Now;
            DBService.Instance.Entities.TGuildApplies.Add(dbApply);

            this.Date.Applies.Add(dbApply);

            DBService.Instance.Save();

            this.timestemp = TimeUtil.timestamp;
            return true;
        }

        internal bool JoinAppove(NguildApplyInfo apply)
        {
            var oldApply = this.Date.Applies.FirstOrDefault(v => v.CharacterId == apply.characterID && v.Result == 0);
            if (oldApply==null)
            {
                return false;
            }

            oldApply.Result = (int)apply.Result;
            if (apply.Result==ApplyResult.Accpet)
            {
                this.AddMember(apply.characterID, apply.Name, apply.Class, apply.Level, GuildTitle.Nome);

            }
            DBService.Instance.Save();
            this.timestemp = TimeUtil.timestamp;
            return true;
        }
        public void AddMember(int characterId,string name,int @class,int level,GuildTitle title)
        {
            DateTime now = DateTime.Now;
            TGuildMember dbMember = new TGuildMember()
            {
                CharacterID=characterId,
                Name=name,
                Class=@class,
                Level=level,
                Title=(int)title,
                JoinTime=now,
                LastTime=now
            };
            this.Date.Members.Add(dbMember);
            var character = CharacterManager.Instance.GetCharacter(characterId);
            if (character!=null)
            {
                character.Data.GuildId = this.Id;
            }
            else
            {
                TCharacter dbchar = DBService.Instance.Entities.Characters.SingleOrDefault(C => C.ID == characterId);
                dbchar.GuildId = this.Id;
            }
            timestemp = TimeUtil.timestamp;
        }

     
        public void PostProcess(Character from ,NetMessageResponse message)
        {
            if (message.Guild==null)
            {
                message.Guild = new GuildResponse();
                message.Guild.Result = Result.Success;
                message.Guild.guildInfo = this.GuildInfo(from);
            }
        }

        internal NGuildInfo GuildInfo(Character from)
        {
            NGuildInfo info = new NGuildInfo()
            {
                Id = this.Id,
                GuildName=this.Name,
                Notice=this.Date.Notice,
                leaderId=this.Date.LeaderID,
                leaderName=this.Date.LeaderName,
                createTime=(long)TimeUtil.GetTimestamp(this.Date.CreateTime),
                memberCont=this.Date.Members.Count
            };
            if (from!=null)
            {
                info.Members.AddRange(GetMemberInfos());
                if (from.Id==this.Date.LeaderID)//假如是会长
                {
                    info.Applies.AddRange(GetApplyInfos());
                }
            }
            return info;
        }

        private List<NGuildMemberInfo> GetMemberInfos()
        {
            List<NGuildMemberInfo> members = new List<NGuildMemberInfo>();
            foreach (var member in this.Date.Members)
            {
                var memberInfo = new NGuildMemberInfo()
                {
                    Id = member.Id,
                    characterId = member.CharacterID,
                    Titlr = (GuildTitle)member.Title,
                    joinTime = (long)TimeUtil.GetTimestamp(member.JoinTime),
                    Lasttime =(long)TimeUtil.GetTimestamp(member.LastTime)
                };
                //应该增加更多检查
                var character = CharacterManager.Instance.GetCharacter(member.CharacterID);
                if (character!=null)
                {
                    memberInfo.Info = character.GetBasicInfo();
                    memberInfo.Status = 1;
                    member.Level = character.Data.Level;
                    member.Name = character.Data.Name;
                    member.LastTime = DateTime.Now;
                }
                else
                {
                    memberInfo.Info = GetMemberInfo(member);
                    memberInfo.Status = 0;                    
                }
                members.Add(memberInfo);
            }
            return members;
        }
       private NCharacterInfo GetMemberInfo(TGuildMember member)
        {
            return new NCharacterInfo()
            {
                Id = member.CharacterID,
                Name = member.Name,
                Class = (CharacterClass)member.Class,
                Level=member.Level,
            };
        }
        private List<NguildApplyInfo> GetApplyInfos()
        {
            List<NguildApplyInfo> applies = new List<NguildApplyInfo>();
            foreach (var apply in this.Date.Applies)
            {
                if (apply.Result != (int)ApplyResult.Nome)
                {
                    continue;
                }
                applies.Add(new NguildApplyInfo()
                {
                    characterID=apply.CharacterId,
                    GuildId=apply.GuildId,
                    Class=apply.Class,
                    Level=apply.Level,
                    Name=apply.Name,
                    Result=(ApplyResult)apply.Result
                });
            }
            return applies; 
        }
        TGuildMember GetDBMember(int characterId)
        {
            foreach (var member in this.Date.Members)
            {
                if (member.CharacterID==characterId)
                {
                    return member;
                }
            }
            return null; 
        }
        internal void ExecuteAdmin(GuildAdminCommand command, int targetId, int sourceId)
        {
            var target = GetDBMember(targetId);
            var source = GetDBMember(sourceId);

            switch (command)
            {
                case GuildAdminCommand.Kickout:
                    //自己写
                    break;
                case GuildAdminCommand.Promote:
                    target.Title = (int)GuildTitle.VicePresident;
                    break;
                case GuildAdminCommand.Depost:
                    target.Title = (int)GuildTitle.Nome;
                    break;
                case GuildAdminCommand.Transfer:
                    target.Title = (int)GuildTitle.President;
                    source.Title = (int)GuildTitle.Nome;
                    break;
                default:
                    break;
            }
            DBService.Instance.Save();
            timestemp = TimeUtil.timestamp;
        }
        //作业，自己写
        internal void Leave(Character character)
        {
           
        }

       
    }
}
