using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.UI;
using Common.Data;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    class TestManager:Singleton<TestManager>
    {
        public void Intit()
        {
            NPCManager.Instance.RegisterNpcEvent(NpcFunction.InvokeInsrance, OnNpcInvokeInsrance);
            NPCManager.Instance.RegisterNpcEvent(NpcFunction.InvokeShop, OnNpcInvokeShop);
        }

        private bool OnNpcInvokeInsrance(NPCDefine nPCDefine)
        {
            Debug.LogFormat("TestManager.OnNpcInvokeInsrance：npc:[{0}:{1}],type:{2},func:{3}", nPCDefine.ID, nPCDefine.Name, nPCDefine.Type, nPCDefine.Function);
            UITest test= UIManager.Instance.Show<UITest>();
            return true;
        }

        private bool OnNpcInvokeShop(NPCDefine nPCDefine)
        {
            Debug.LogFormat("TestManager.OnNpcInvokeShop：npc:[{0}:{1}],type:{2},func:{3}", nPCDefine.ID, nPCDefine.Name, nPCDefine.Type, nPCDefine.Function);
            MessageBox.Show("点击" + nPCDefine.Name, "noc对话");
            return true;

        }
    }
}
