using Assets.Scripts.Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Services
{
    class StatusService:Singleton<StatusService>,IDisposable
    {
       

        public delegate bool StatusNotifyHandler(Nstatus status);
        Dictionary<StatusType, StatusNotifyHandler> eventMap = new Dictionary<StatusType, StatusNotifyHandler>();
        HashSet<StatusNotifyHandler> handlers = new HashSet<StatusNotifyHandler>();
        public void Init()
        {

        }
        public StatusService()
        {
            MessageDistributer.Instance.Subscribe<StatusNotify>(OnStatusNotify);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<StatusNotify>(OnStatusNotify);
        }


        /// <summary>
        /// 注册事事件
        /// </summary>
        /// <param name="function"></param>
        /// <param name="action"></param>
        public void RegisterStatusNotify(StatusType function,StatusNotifyHandler action)
        {
            if (handlers.Contains(action))
            {
                return;
            }
            if (!eventMap.ContainsKey(function))
            {
                eventMap[function] = action;
            }
            else
            {
                eventMap[function] += action;
            }
            handlers.Add(action);
        }
        private void OnStatusNotify(object sender, StatusNotify message)
        {
            foreach (var item in message.Status)
            {
                Notify(item);
            }
        }

        private void Notify(Nstatus status)
        {
            Debug.LogFormat("StatusNotify:[{0}][{1}]{2}:{3}", status.Type, status.Action, status.Id, status.Value);
            if (status.Type==StatusType.Money)
            {
                if (StatusAction.Add==status.Action)
                {
                    User.Instance.AddGold(status.Value);
                    
                }
                else if (status.Action==StatusAction.Delete)
                {
                    User.Instance.AddGold(status.Value);//传过的是负数

                }
                MonemyUpdata();
            }

            StatusNotifyHandler handler;
            if (eventMap.TryGetValue(status.Type,out handler))
            {
                handler.Invoke(status);
            }
        }
        private void MonemyUpdata()
        {
            if (ShopManager.Instance.uiShop != null)
            {
                ShopManager.Instance.uiShop.money.text = User.Instance.CurrentCharacter.Gold.ToString();
                Debug.Log("商店面板金币更新");

            }
            if (BagManager.Instance.uIBag != null)
            {
                BagManager.Instance.uIBag.money.text = User.Instance.CurrentCharacter.Gold.ToString();
                Debug.Log("背包面板金币更新");
            }
            if (EquipMananger.Instance.uiCharaterEquip!=null)
            {
                EquipMananger.Instance.uiCharaterEquip.money.text= User.Instance.CurrentCharacter.Gold.ToString();
                Debug.Log("装备面板金币更新");
            }
        }
    }
}
