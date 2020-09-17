using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class ItemService:Singleton<ItemService>
    {
        public ItemService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ItemBuyRequest>(this.OnItemBuy);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ItemEquipRequest>(this.OnItemEquip);

        }

       

        public void Init()
        {

        }
        private void OnItemBuy(NetConnection<NetSession> sender, ItemBuyRequest message)
        {
            Character character = sender.Session.Character;//取当前会话的角色

            Log.InfoFormat("OnItemBuy::Character:{0} shop:{1} shopItem:{2}", character.Id, message.shopId, message.shopItemId);
            var result = ShopManager.Instance.BuyItem(sender, message.shopId, message.shopItemId);//扣钱添加物品
            sender.Session.Response.itemBuy = new ItemBuyResponse();
            sender.Session.Response.itemBuy.Result = result;
            sender.SendResponse();

        }
        private void OnItemEquip(NetConnection<NetSession> sender, ItemEquipRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnItemEquip::character:{0} Slot:{1} Item:{2} Equip:{3}", character.Id, message.Slot, message.itemId, message.isEquip);
            var result = EquipManager.Instance.EquipItem(sender, message.Slot, message.itemId, message.isEquip);
            sender.Session.Response.itemEquip = new ItemEquipResponse();
            sender.Session.Response.itemEquip.Result = result;
            sender.SendResponse();

        }

    }
}
