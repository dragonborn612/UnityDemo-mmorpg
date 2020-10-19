using Assets.Scripts.Models;
using Assets.Scripts.Services;
using Assets.Scripts.UI;
using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Managers
{
    public enum NpcQuestStatus
    {    /// <summary>
         /// 无任务
         /// </summary>
        None = 0,
        /// <summary>
        /// 拥有已完成可提交任务
        /// </summary>
        Complete,
        /// <summary>
        /// 拥有可接受任务
        /// </summary>
        Available,
        /// <summary>
        /// /拥有未完成任务
        /// </summary>
        Incomplete,
    }
    class QuestManager:Singleton<QuestManager>
    {
        /// <summary>
        /// 所有有效任务
        /// </summary>
        public List<NQuestInfo> questInfos;

        public Dictionary<int, Quest> allQuests = new Dictionary<int, Quest>();
        //第一个key为NPCid
        public Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>> npcQuests = new Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>>();
        public delegate void QuestStatusChangeHandl(Quest quest);
        public event QuestStatusChangeHandl onQuestStatusChange ;
        public void Init(List<NQuestInfo> quests)
        {
            this.questInfos = quests;
            allQuests.Clear();
            this.npcQuests.Clear();
            InitQuests();
        }
        void InitQuests()
        {
            //初始化已有任务,将服务器传来已接的任务列表转化存储
            foreach (var info in this.questInfos)
            {
                Quest quest = new Quest(info);
                this.allQuests[quest.Info.QuestId] = quest;
            }
            this.CheckAvailableQuests();
            //把任务添加到NPC
            foreach (var item in allQuests)
            {
                this.AddNpcQuest(item.Value.Define.AcceptNPC, item.Value);
                this.AddNpcQuest(item.Value.Define.SubmitNPC, item.Value);
            }
        }
        /// <summary>
        /// 初始化配置表中的可用任务
        /// </summary>
        private void CheckAvailableQuests()
        {
            foreach (var kv in DataManager.Instance.Quests)
            {
                if (kv.Value.LimitClass!=CharacterClass.None&&kv.Value.LimitClass!=User.Instance.CurrentCharacter.Class)
                {
                    continue;//不符合职业
                }
                if (kv.Value.LimitLevel>User.Instance.CurrentCharacter.Level)
                {
                    continue;//等级不符
                }
                if (this.allQuests.ContainsKey(kv.Key))
                {
                    continue;//任务已存在
                }
                if (kv.Value.PreQuest>0)//有前置任务
                {
                    Quest preQuest;
                    if (allQuests.TryGetValue(kv.Value.PreQuest,out preQuest))//获取了前置任务
                    {
                        if (preQuest.Info==null)
                        {
                            continue;//前置任务没有接取
                        }
                        if (preQuest.Info.Status != QuestStatus.Finished)
                        {
                            continue;//前置任务未完成
                        }
                       
                    }
                    else
                        continue;//前置任务还没接
                }
                Quest quest = new Quest(kv.Value);
                this.allQuests[quest.Define.ID] = quest;
            }
        }
        /// <summary>
        /// 添加任务到 npcQuests
        /// </summary>
        /// <param name="npcId"></param>
        /// <param name="quest"></param>
        private void AddNpcQuest(int npcId ,Quest quest)
        {
            if (!this.npcQuests.ContainsKey(npcId))
            {
                npcQuests[npcId] = new Dictionary<NpcQuestStatus, List<Quest>>();
            }
            
            List<Quest> availables;//可接受任务
            List<Quest> complates;//完成可提交的任务
            List<Quest> incomplates;//已接，未完成任务

            if (!npcQuests[npcId].TryGetValue(NpcQuestStatus.Available,out availables))
            {
                availables = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Available] = availables;
            }
            if (!npcQuests[npcId].TryGetValue(NpcQuestStatus.Complete, out complates))
            {
                complates = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Complete] = complates;
            }
            if (!npcQuests[npcId].TryGetValue(NpcQuestStatus.Incomplete, out incomplates))
            {
                incomplates = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Incomplete] = incomplates;
            }

            if (quest.Info==null)//任务未被接取
            {
                if (npcId==quest.Define.AcceptNPC&&!this.npcQuests[npcId][NpcQuestStatus.Available].Contains(quest))
                {
                    this.npcQuests[npcId][NpcQuestStatus.Available].Add(quest);
                }
            }
            else
            {
                if (quest.Define.SubmitNPC==npcId&&quest.Info.Status==QuestStatus.Complated)
                {
                    if (!npcQuests[npcId][NpcQuestStatus.Complete].Contains(quest))
                    {
                        npcQuests[npcId][NpcQuestStatus.Complete].Add(quest);
                    }
                }
                if (quest.Define.SubmitNPC == npcId && quest.Info.Status == QuestStatus.InProgress)
                {
                    if (!npcQuests[npcId][NpcQuestStatus.Incomplete].Contains(quest))
                    {
                        npcQuests[npcId][NpcQuestStatus.Incomplete].Add(quest);
                    }
                }
            }
        }
        /// <summary>
        /// 获取NPC任务状态
        /// </summary>
        /// <param name="npcId"></param>
        /// <returns></returns>
        public NpcQuestStatus GetQuestStatusByNpc(int npcId)
        {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
            if (this.npcQuests.TryGetValue(npcId,out status))//获取NPC任务
            {
                if (status[NpcQuestStatus.Complete].Count>0)
                {
                    return NpcQuestStatus.Complete;
                }
                if (status[NpcQuestStatus.Available].Count > 0)
                {
                    return NpcQuestStatus.Available;
                }
                if (status[NpcQuestStatus.Incomplete].Count > 0)
                {
                    return NpcQuestStatus.Incomplete;
                }
            }
            return NpcQuestStatus.None;
        }

        public bool OpenNpcQuest(int npcid)
        {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
            if (npcQuests.TryGetValue(npcid,out status))//获取NPC任务
            {
                if (status[NpcQuestStatus.Complete].Count>0)
                {
                    return ShowQuestDialog(status[NpcQuestStatus.Complete].First());
                }
                if (status[NpcQuestStatus.Available].Count > 0)
                {
                    return ShowQuestDialog(status[NpcQuestStatus.Available].First());
                }
                if (status[NpcQuestStatus.Incomplete].Count > 0)
                {
                    return ShowQuestDialog(status[NpcQuestStatus.Incomplete].First());
                }
            }
            return false;
        }

        private bool ShowQuestDialog(Quest quest)
        {
            if (quest.Info==null||quest.Info.Status==QuestStatus.Complated) //任务没接，或者完成未提交
            {
                UIQuestDialog dlg = UIManager.Instance.Show<UIQuestDialog>();
                dlg.SetQuest(quest);
                dlg.Onclose += OnQuestDialogClose;
                return true;
            }
            if (quest.Info!=null||quest.Info.Status==QuestStatus.Complated)//任务未完成
            {
                if (!string.IsNullOrEmpty(quest.Define.DialogIncomplete))//判断字符串是否为“”和null
                {
                    MessageBox.Show(quest.Define.DialogIncomplete);
                }
            }
            return true;
        }

        private void OnQuestDialogClose(UIWindow sender, WindowResult result)
        {
            UIQuestDialog dlg = (UIQuestDialog)sender;
            if (result==WindowResult.Yes)//接受任务 或提交
            {
                if (dlg.quest.Info==null)//提交
                {
                    QuestService.Instance.SendQuestAccpet(dlg.quest);
                    
                }
                else//接受
                {
                    QuestService.Instance.SendQuestSubmit(dlg.quest);
                   
                }
                
            }
            else if (result==WindowResult.No)//拒绝任务
            {
                MessageBox.Show(dlg.quest.Define.DialogDeny);
            }
        }
        private Quest RefreshQuestStatus(NQuestInfo quest)
        {
            this.npcQuests.Clear();
            Quest result;
            if (allQuests.ContainsKey(quest.QuestId))
            {
                //更新任务状态
                allQuests[quest.QuestId].Info = quest;
                result = allQuests[quest.QuestId];
            }
            else
            {
                result = new Quest(quest);
                allQuests[quest.QuestId] = result;
            }
            CheckAvailableQuests();
            //把任务添加到NPC
            foreach (var item in allQuests)
            {
                this.AddNpcQuest(item.Value.Define.AcceptNPC, item.Value);
                this.AddNpcQuest(item.Value.Define.SubmitNPC, item.Value);
            }

            if (onQuestStatusChange!=null)
            {
                onQuestStatusChange.Invoke(result);
            }
            return result;
        }

        public void OnQuestAccpectd(NQuestInfo info)
        {
            var quest = this.RefreshQuestStatus(info);
            MessageBox.Show(quest.Define.DialogAccept);
        }
        public void OnQuestSubmited(NQuestInfo info)
        {
            var quest = this.RefreshQuestStatus(info);
            MessageBox.Show(quest.Define.DialogFinish);
        }
    }
}
