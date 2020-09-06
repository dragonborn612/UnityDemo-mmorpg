using Assets.Scripts.Models;
using Common;
using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    class ItemManager:Singleton<ItemManager>
    {
        public Dictionary<int, Item> Items = new Dictionary<int, Item>();
        internal void Init(List<NItemInfo> items)
        {
            this.Items.Clear();
            foreach (var info in items)
            {
                Item item = new Item(info);
                Items.Add(item.Id, item);
                Debug.LogFormat("ItemManager:Init[{0}]", item);
            }

        }


        public ItemDefine GetItem(int itemId)
        {
            return null;
        }
        public bool UseItem(int itemId)
        {
            return false;
        }
        public bool useItem(ItemDefine item)
        {
            return false;
        }
    }
}
