using Assets.Scripts.Models;
using Assets.Scripts.Services;
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
               this.Items.Add(item.Id, item);
                Debug.LogFormat("ItemManager:Init[{0}]", item);
            }
            StatusService.Instance.RegisterStatusNotify(StatusType.Item, OnItemNotify);
        }

        private bool OnItemNotify(Nstatus status)
        {
            if (status.Action==StatusAction.Add)
            {
                AddItem(status.Id, status.Value);
            }
            if (status.Action == StatusAction.Delete)
            {
                RemoveItem(status.Id, status.Value);
            }
            return true;
        }

        private void RemoveItem(int itemId, int value)
        {
            Item item = null;
            if (Items.TryGetValue(itemId,out item))
            {
                if (item.Count > value)
                {
                    item.Count -= value;
                    BagManager.Instance.RemoveItem(itemId, value);
                }
                else
                {
                    Items.Remove(itemId);
                    BagManager.Instance.RemoveItem(itemId, item.Count);
                }
                
            }
           
        }

        private void AddItem(int itemId, int value)
        {
            //若列表内存在加数量 不存在添加物品
            Item item = null;
            if (Items.TryGetValue(itemId, out item))
            {
                item.Count += value;
            }
            else
            {
                item = new Item(itemId, value);
                Items.Add(itemId, item);
            }
            BagManager.Instance.AddItem(itemId, value);
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
