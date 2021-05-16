using Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Managers
{
    interface IEntityNotiy
    {
        void OnEntityRemoved();//移除“事件”
        void OnEntityChange(Entity entity);
        void OnEntityEvent(EntityEvent @event,int param);
    }
    class EntityManager:Singleton<EntityManager>
    {
        Dictionary<int, Entity> entities = new Dictionary<int, Entity>();
        Dictionary<int, IEntityNotiy> notifiers = new Dictionary<int, IEntityNotiy>();

        public void RegiestEntityChangeNotify(int entityId,IEntityNotiy notify)
        {
            this.notifiers[entityId] = notify;
        }
        public void AddEntity(Entity entity)
        {
            entities[entity.entityId] = entity;
        }
        public void RemoveEntity(NEntity entity)
        {
            entities.Remove(entity.Id);
            if (notifiers.ContainsKey(entity.Id))
            {
                notifiers[entity.Id].OnEntityRemoved();
                notifiers.Remove(entity.Id);
            }
        }
        public void OnEntitySync(NEntitySync nEntitySync)
        {
            Entity entity = null;
            entities.TryGetValue(nEntitySync.Id, out entity);
            if (entity!=null)
            {
                if (nEntitySync != null)
                {
                    entity.EntityData = nEntitySync.Entity;//同步位置
                }
                if (notifiers.ContainsKey(nEntitySync.Id))
                {
                    //发消息
                    notifiers[nEntitySync.Id].OnEntityChange(entity);
                    notifiers[nEntitySync.Id].OnEntityEvent(nEntitySync.Event,nEntitySync.Param);
                }
            }
        }
    }
}
