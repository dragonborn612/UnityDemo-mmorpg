using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Services
{
    class ItemService:Singleton<ItemService>,IDisposable
    {
        public ItemService()
        {
            MessageDistributer.Instance.Subscribe<ItemBuyResponse>(OnItemBuy);
            MessageDistributer.Instance.Subscribe<ItemEquipResponse>(OnItemEquip);

        }

        

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ItemBuyResponse>(OnItemBuy);
            MessageDistributer.Instance.Unsubscribe<ItemEquipResponse>(OnItemEquip);
        }

        public void Intit()
        {

        }
        public void SendBuyItem(int shopId,int ShopItemId)
        {
            Debug.LogFormat("发送购买请求:ShopId{0},ShopItemId{1}", shopId, ShopItemId);

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemBuy = new ItemBuyRequest();
            message.Request.itemBuy.shopId = shopId;
            message.Request.itemBuy.shopItemId = ShopItemId;
            NetClient.Instance.SendMessage(message);
        }
        private void OnItemBuy(object sender, ItemBuyResponse message)
        {
           
            MessageBox.Show("购买结果：" + message.Result + "\n" + message.Errormsg, "购买完成");
            
            
        }
        /// <summary>
        /// 准备装备的装备
        /// </summary>
        Item pendingEquip = null;
        /// <summary>
        /// 穿还是脱
        /// </summary>
        bool isEquip;
        public bool SendEquipItem(Item equip, bool isEquip)
        {
            if (pendingEquip!=null)
            {
                return false;
            }
            Debug.Log("SendEquipItem");

            this.isEquip = isEquip;
            pendingEquip = equip;

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemEquip = new ItemEquipRequest();

            message.Request.itemEquip.isEquip = isEquip;
            message.Request.itemEquip.itemId = equip.Id;
            message.Request.itemEquip.Slot = (int)equip.EquipInfo.Slot;
            NetClient.Instance.SendMessage(message);
            return true;
        }


        private void OnItemEquip(object sender, ItemEquipResponse message)
        {
            if (message.Result==Result.Success)
            {
                if (pendingEquip!=null)
                {
                    if (this.isEquip)
                    {
                        EquipMananger.Instance.OnEquipItem(pendingEquip);
                    }
                    else
                    {
                        EquipMananger.Instance.OnUnEquipItem(pendingEquip.EquipInfo.Slot);
                    }
                    pendingEquip = null;
                }
            }
        }
    }
}
