using Common;
using GameServer.Entities;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class BagService : Singleton<BagService>
    {
        public BagService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<BagSaveRequst>(this.OnBagSave);
        }
        public void Init()
        {

        }
        private void OnBagSave(NetConnection<NetSession> sender, BagSaveRequst message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("BagSaveRequset::Character:{0}:Unlocked{1}", character.Id,message.BagInfo.Unlocked);
            if (message.BagInfo!=null)
            {
                character.Data.TCharacterBag.Items = message.BagInfo.Items;
                DBService.Instance.Save();
            }
        }
    }
}
