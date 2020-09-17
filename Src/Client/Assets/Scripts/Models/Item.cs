using System;
using System.Collections.Generic;
using System.Linq;
using SkillBridge.Message;
using System.Text;
using Common.Data;

namespace Assets.Scripts.Models
{
    public class Item
    {
        public int Id;
        public int Count;
        public ItemDefine itemDefine;
        public EquipDefine EquipInfo;
        public Item(NItemInfo item):this(item.Id,item.Count)
        {
            
        }
        public Item(int id,int count)
        {
            this.Id = id;
            this.Count = count;
           

            DataManager.Instance.Items.TryGetValue(this.Id,out this.itemDefine);
            DataManager.Instance.Equips.TryGetValue(Id, out EquipInfo);
        }
        public override string ToString()
        {
            return string.Format("Id:{0},Count{1}", this.Id, this.Count);
        }
    }
}
