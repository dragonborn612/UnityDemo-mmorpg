using Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Managers
{

    class NPCManager:Singleton<NPCManager>
    {
        public delegate bool NpcActionHanger(NPCDefine nPCDefine);
        
        Dictionary<NpcFunction, NpcActionHanger> eventMap = new Dictionary<NpcFunction, NpcActionHanger>();

        public void RegisterNpcEvent(NpcFunction npcFunction,NpcActionHanger  npcActionHanger)
        {
            if (!eventMap.ContainsKey(npcFunction) )//如果不存在 添加字典
            {
                eventMap[npcFunction] = npcActionHanger;
            }
            else//是不是应该去掉？
                eventMap[npcFunction] += npcActionHanger;//如果存在注册委托
        }
        public NPCDefine GetNpcDefine(int npcID)
        {
            NPCDefine npc = null;

            DataManager.Instance.NPCs.TryGetValue(npcID,out npc);
            return npc;
        }
        public bool Interactive(int npcID)
        {
            if (DataManager.Instance.NPCs.ContainsKey(npcID))
            {
                NPCDefine npc = GetNpcDefine(npcID);
                return Interactive(npc);
            }
            else
                return false;           
        }
        //交互
        public bool Interactive(NPCDefine nPCDefine)
        {
            if (nPCDefine.Type ==NpcType.Task)
            {
              return  DoTaskInteractive(nPCDefine);
            }
            if (nPCDefine.Type==NpcType.Functional)
            {
                return DoFuctionInteractive(nPCDefine);
            }
            return false;
        }
        public bool DoTaskInteractive(NPCDefine nPCDefine)
        {
            MessageBox.Show("点击了npc：" + nPCDefine.Name, "npc对话");
            return true;
        }
        public bool DoFuctionInteractive(NPCDefine nPCDefine)
        {
            if (nPCDefine.Type!=NpcType.Functional)
            {
                return false;
            }
            if (!eventMap.ContainsKey(nPCDefine.Function))
            {
                return false;
            }
           return eventMap[nPCDefine.Function].Invoke(nPCDefine);
        }
    }
}
