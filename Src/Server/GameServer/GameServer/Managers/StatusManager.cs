using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class StatusManager
    {
        Character Owner;

        private List<Nstatus> Status { get; set; }

        public bool HasStatus
        {
            get { return this.Status.Count > 0; }

        }
        public StatusManager(Character owner)
        {
            this.Owner = owner;
            this.Status = new List<Nstatus>();
        }

        public void AddStatus(StatusType type,int id,int value,StatusAction action)
        {
            this.Status.Add(new Nstatus()
            {
                Type = type,
                Id = id,
                Value = value,
                Action=action,
            });
        }

        public void AddGoldChange(int goldDelta)//goldDelta金币差值
        {
            if (goldDelta>0)
            {
                this.AddStatus(StatusType.Money, 0, goldDelta, StatusAction.Add);
            }
            if (goldDelta<0)
            {
                this.AddStatus(StatusType.Money, 0, goldDelta, StatusAction.Delete);
            }

        }
        public void AddItemChange(int id,int count,StatusAction action)
        {
            this.AddStatus(StatusType.Item, id, count, action);
        }
        /// <summary>
        /// 将状态列表的状态全部传入消息，并清空状态列表
        /// </summary>
        /// <param name="message"></param>
        public void ApplyResponse(NetMessageResponse message)
        {
            if (message.statusNotify==null)
            {
                message.statusNotify = new StatusNotify();
            }
            foreach (var item in Status)
            {
                message.statusNotify.Status.Add(item);
            }
            this.Status.Clear();
        }
        
    }
}
