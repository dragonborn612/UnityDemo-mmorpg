using Common;
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
    class ItemManager
    {
        Character Owner;
        public Dictionary<int, Item> Items = new Dictionary<int, Item>();
        public ItemManager(Character owenr)
        {
            this.Owner = owenr;
            foreach (var item in owenr.Data.Items)
            {
                this.Items.Add(item.Id, new Item(item));
            }
        }

        public bool UseItem(int itemId,int count = 1)
        {
            Log.InfoFormat("[{0}]UseItem[{1}:{2}]", this.Owner.Data.ID, itemId, count);
            Item item = null;
            if (this.Items.TryGetValue(itemId,out item))
            {
                if (item.Count<count)
                {
                    return false;
                }
                //添加使用逻辑
                return true;
            }
            return false;
        }
        public bool HasItem(int itemId)
        {
            Item item = null;
            if (this.Items.TryGetValue(itemId, out item))
            {
                return item.Count > 0;
            }
            return false;
        }
        public Item GetItem(int itemId)
        {
            Item item = null;
            this.Items.TryGetValue(itemId, out item);
            Log.InfoFormat("[{0}]GetItem[{1}:{2}", this.Owner.Data.ID, item.ID, item);
            return item;
        }
        public bool AddItem(int itemId,int count)
        {
            Item item = null;
            //字典有加数目，没有数据库和角色、字典添加道具
            if (this.Items.TryGetValue(itemId, out item))
            {
                item.Add(count);
            }
            else
            {
                TCharacterItem dbItem = new TCharacterItem();
                dbItem.CharacterID = this.Owner.Data.ID;
                dbItem.Character = this.Owner.Data;
                dbItem.ItemID = itemId;
                dbItem.ItemCount = count;
                this.Owner.Data.Items.Add(dbItem);
                item = new Item(dbItem);
                Items.Add(itemId, item);
            }
            Log.InfoFormat("[{0}]AddItem[{1}] addCount:{2}", this.Owner.Data.ID, itemId, count);
            DBService.Instance.Save();
            return true;
        }
        public bool RemoveItem(int itemId,int count)
        {
            if (!Items.ContainsKey(itemId))
            {
                return false;
            }
            Item item = Items[itemId];
            if (item.Count<count)
            {
                return false;
            }
            item.Remove(count);
            Log.InfoFormat("[{0}]RemoveItem[{1}] removeCount:{2}", this.Owner.Data.ID, itemId, count);
            return true;
        }
        internal void GetItemInfos(List<NItemInfo> list)
        {
            foreach (var item in this.Items)
            {
                list.Add(new NItemInfo() { Id = item.Value.ID, Count = item.Value.Count });
            }
        }
    }
}
