using Common.Data;
using GameServer.Entities;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class QuestManager
    {
        Character Owner;
        public QuestManager(Character owner)
        {
            this.Owner = owner;
        }
        /// <summary>
        /// 把数据库里的任务提取出来
        /// </summary>
        /// <param name="list"></param>
        public void GetQuestInfos(List<NQuestInfo> list)
        {
            foreach (var quest in this.Owner.Data.Quests)
            {
                list.Add(GetQuestInfo(quest));
            }
        }
        /// <summary>
        /// 把表任务转化为网络任务
        /// </summary>
        /// <param name="quest"></param>
        /// <returns></returns>
        private NQuestInfo GetQuestInfo(TCharacterQuest quest)
        {
            return new NQuestInfo()
            {
                QuestId = quest.QuestID,
                QuestGuid = quest.Id,
                Status = (QuestStatus)quest.Status,
                Targets=new int[3]
                {
                    quest.Target1,
                    quest.Target2,
                    quest.Target3,
                }
            };
        }

        public Result AccpetQuest(NetConnection<NetSession> sender,int questId)
        {
            Character character = sender.Session.Character;

            QuestDefine quest;
            if (DataManager.Instance.Quests.TryGetValue(questId,out quest))
            {
                var dbquest = DBService.Instance.Entities.TCharacterQuests.Create();
                dbquest.QuestID = quest.ID;
                if (quest.Target1==QuestTarget.None)
                {
                    //没有目标直接完成
                    dbquest.Status = (int)QuestStatus.Complated;
                }
                else
                {
                    //有目标的
                    dbquest.Status = (int)QuestStatus.InProgress;
                }
                sender.Session.Response.questAccept.Quest = this.GetQuestInfo(dbquest);//向协议中添加接受的任务
                character.Data.Quests.Add(dbquest);//数据库添加任务
                DBService.Instance.Save();
                return Result.Success;

            }
            else
            {
                sender.Session.Response.questAccept.Errormsg = "任务不存在";
                return Result.Failed;
            }
        }
        public Result SubmitQuest(NetConnection<NetSession> sender, int questId)
        {
            Character character = sender.Session.Character;
            QuestDefine quest;
            if (DataManager.Instance.Quests.TryGetValue(questId, out quest))
            {
                var dbQuest = character.Data.Quests.Where(q => q.QuestID == questId).FirstOrDefault();//查数据库 接取才会在数据库
                if (dbQuest!=null)
                {
                    if (dbQuest.Status!=(int)QuestStatus.Complated)
                    {//还不是完成状态
                        sender.Session.Response.questSubmit.Errormsg = "任务未完成";
                        return Result.Failed;
                    }
                    dbQuest.Status = (int)QuestStatus.Finished;
                    sender.Session.Response.questSubmit.Quest = this.GetQuestInfo(dbQuest);
                    DBService.Instance.Save();

                    //处理任务奖励
                    if (quest.RewardGold>0)
                    {
                        character.Gold += quest.RewardGold;
                    }
                    if (quest.RewardExp>0)
                    {
                        //character.Exp += quest.RewardExp;
                    }
                    if (quest.RewardItem1>0)
                    {
                        character.ItemManager.AddItem(quest.RewardItem1, quest.RewardItem1Count);
                    }
                    if (quest.RewardItem2 > 0)
                    {
                        character.ItemManager.AddItem(quest.RewardItem2, quest.RewardItem2Count);
                    }
                    if (quest.RewardItem3 > 0)
                    {
                        character.ItemManager.AddItem(quest.RewardItem3, quest.RewardItem3Count);
                    }
                    DBService.Instance.Save();
                    return Result.Success;
                }
                sender.Session.Response.questSubmit.Errormsg = "任务不存在【2】";
                return Result.Failed;
            }
            else
            {
                sender.Session.Response.questSubmit.Errormsg = "任务不存在【1】";
                return Result.Failed;
            }
        }

    }
}
