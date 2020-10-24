using GameServer.Entities;
using GameServer.Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class MonsterManager
    {
        private Map Map;
        public Dictionary<int, Monster> Mosters = new Dictionary<int, Monster>();

        public void Init(Map map)
        {
            this.Map = map;
        }

        internal Monster Create(int spawnMonID,int spawnLevel, NVector3 position,NVector3 direction)
        {
            Monster monster = new Monster(spawnMonID, spawnLevel, position, direction);
            EntityManager.Instance.AddEneity(this.Map.ID, monster);
            monster.Info.Id = monster.entityId;
            monster.Info.mapId = this.Map.ID;
            Mosters[monster.Id] = monster;

            this.Map.MonsterEnter(monster);
            return monster;
        }
    }
}
