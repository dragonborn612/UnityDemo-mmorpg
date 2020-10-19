using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Services
{
    class QuestService:Singleton<QuestService>
    {
        public QuestService()
        {
            MessageDistributer.Instance.Subscribe<QuestAcceptResponse>(this.OnQuestAccept);
            MessageDistributer.Instance.Subscribe<QuestSubmitResponse>(this.OnQuestSubmit);
        }
        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<QuestAcceptResponse>(this.OnQuestAccept);
            MessageDistributer.Instance.Unsubscribe<QuestSubmitResponse>(this.OnQuestSubmit);
        }
        public void Init()
        {

        }
        public bool SendQuestAccpet(Quest quest)
        {
            Debug.Log("SendQuestAccpet");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.questAccept = new QuestAcceptRequest();

            message.Request.questAccept.QuestId = quest.Define.ID;
            NetClient.Instance.SendMessage(message);
            return true;

        }
        public bool SendQuestSubmit(Quest quest)
        {
            Debug.Log("SendQuestSubmit");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.questSubmit = new QuestSubmitRequest();

            message.Request.questSubmit.QuestId = quest.Define.ID;
            NetClient.Instance.SendMessage(message);
            return true;
        }

        private void OnQuestAccept(object sender, QuestAcceptResponse message)
        {
            Debug.LogFormat("QuestAccept:{0},ERR:{1}", message.Result, message.Errormsg);
            if (message.Result==Result.Success)
            {
                QuestManager.Instance.OnQuestAccpectd(message.Quest);
            }
            else
            {
                MessageBox.Show("任务接受失败", "错误", MessageBoxType.Error);
            }
        }
        private void OnQuestSubmit(object sender, QuestSubmitResponse message)
        {
            Debug.LogFormat("QuestSubmit:{0},ERR:{1}", message.Result, message.Errormsg);
            if (message.Result == Result.Success)
            {
                QuestManager.Instance.OnQuestSubmited(message.Quest);
            }
            else
            {
                MessageBox.Show("任务提交失败", "错误", MessageBoxType.Error);
            }
        }
    }
}
