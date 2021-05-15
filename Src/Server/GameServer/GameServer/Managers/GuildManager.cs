using Common;
using Common.Utils;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class GuildManager : Singleton<GuildManager>
    {
        public Dictionary<int, Guild> Guilds = new Dictionary<int, Guild>();
        private HashSet<string> GuildNames = new HashSet<string>();//用来查询

        internal void Init()
        {
            this.Guilds.Clear();
            foreach (var guild in DBService.Instance.Entities.TGuilds)
            {
                this.AddGuild(new Guild(guild));
            }
        }

        private void AddGuild(Guild guild)
        {
            this.Guilds.Add(guild.Id, guild);
            this.GuildNames.Add(guild.Name);
            guild.timestemp = TimeUtil.timestamp;
        }

        internal bool CheckNameExisted(string guildName)
        {
            return GuildNames.Contains(guildName);
        }

        public bool CreateGuild(string name,string notice,Character leader)
        {
            DateTime now = DateTime.Now;
            TGuild dbGuild = DBService.Instance.Entities.TGuilds.Create();
            dbGuild.Name = name;
            dbGuild.Notice = notice;
            dbGuild.LeaderID = leader.Id;
            dbGuild.LeaderName = leader.Data.Name;
            dbGuild.CreateTime = now;
            DBService.Instance.Entities.TGuilds.Add(dbGuild);

            Guild guild = new Guild(dbGuild);
            guild.AddMember(leader.Id, leader.Data.Name, leader.Data.Class, leader.Data.Level, GuildTitle.President);
            leader.Guild = guild;
            DBService.Instance.Save();
            leader.Data.GuildId = dbGuild.Id;
            DBService.Instance.Save();
            this.AddGuild(guild);

            return true;
        }

        internal Guild GetGuild(int guildID)
        {
            if (guildID==0)
            {
                return null;
            }

            Guild guild = null;
            this.Guilds.TryGetValue(guildID, out guild);
            return guild;
        }
        /// <summary>
        /// 将Guild类型的字典转化我NGuildInfo的列表
        /// </summary>
        /// <returns></returns>
        internal List<NGuildInfo> GetGuildsInfo()
        {
            List<NGuildInfo> result = new List<NGuildInfo>();
            foreach (var item in this.Guilds)
            {
                result.Add(item.Value.GuildInfo(null));
            }
            return result;
        }
    }
}
