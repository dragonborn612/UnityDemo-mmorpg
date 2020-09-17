using Assets.Scripts.Services;
using Assets.Scripts.UI;
using Common;
using Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Managers
{
    class ShopManager:Singleton<ShopManager>
    {
        public UIShop uiShop;
        public void Init()
        {
            NPCManager.Instance.RegisterNpcEvent(NpcFunction.InvokeShop, OnOpenShop);
        }

        private bool OnOpenShop(NPCDefine nPCDefine)
        {
            ShowShop(nPCDefine.Param);
            return true;
        }

        public void ShowShop(int shopId)
        {
            ShopDefine shop;
            if (DataManager.Instance.Shops.TryGetValue(shopId,out shop))
            {
                uiShop = UIManager.Instance.Show<UIShop>();
                if (uiShop != null)
                {
                    uiShop.SetShop(shop);
                }
            }
            
        }
        /// <summary>
        /// 发送购买请求
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="shopItemId"></param>
        /// <returns></returns>
        public bool BuyItem(int shopId,int shopItemId)
        {
            ItemService.Instance.SendBuyItem(shopId, shopItemId);
            return true;
        }
    }
}
