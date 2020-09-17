using System;
using System.Collections.Generic;
using System.Linq;
using SkillBridge.Message;
using System.Text;
using Common.Data;

namespace Assets.Scripts.Models
{
    class Item
    {
        public int Id;
        public int Count;
        public ItemDefine itemDefine;
        public Item(NItemInfo item)
        {
            this.Id = item.Id;
            this.Count = item.Count;
            itemDefine = DataManager.Instance.Items[item.Id];
        }
        public override string ToString()
        {
            return string.Format("Id:{0},Count{1}", this.Id, this.Count);
        }
    }
}
