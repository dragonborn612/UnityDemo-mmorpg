using Common;
using GameServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class EntityManager: Singleton<EntityManager>
    {
        private int index = 0;
        public List<Entity> AllEntities = new List<Entity>();
        public Dictionary<int, List<Entity>> MapEntities = new Dictionary<int, List<Entity>>();

        public void AddEneity(int mapId,Entity entity)
        {
            
            //加入管理器唯一id
            entity.EntityData.Id = ++this.index;
            AllEntities.Add(entity);

            List<Entity> entities = null;
            if (!MapEntities.TryGetValue(mapId,out entities))//如果不存在key为mapID的entities
            {
                entities = new List<Entity>();
                MapEntities[mapId] = entities;
            }
            entities.Add(entity);
         }
        public void RemoveEneity(int mapId, Entity entity)
        {
            AllEntities.Remove(entity);
            MapEntities.Remove(mapId);
        }
    }
}
