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
    class QuestService : Singleton<QuestService>
    {
        public QuestService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<QuestAcceptRequest>(this.OnQuestAccept);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<QuestSubmitRequest>(this.OnQuestSubmit);
        }
        public void Init()
        {

        }
        private void OnQuestSubmit(NetConnection<NetSession> sender, QuestSubmitRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("QusetSubmitRequest::character:{0}:QuestId{1}", character.Id, message.QuestId);

            sender.Session.Response.questSubmit = new QuestSubmitResponse();

            Result result = character.QuestManager.SubmitQuest(sender, message.QuestId);
            sender.Session.Response.questSubmit.Result = result;
            sender.SendResponse();

        }

        private void OnQuestAccept(NetConnection<NetSession> sender, QuestAcceptRequest message)
        {
            
            Character character = sender.Session.Character;
            Log.InfoFormat("QusetAcceptRequest::character:{0}:QuestId{1}", character.Id, message.QuestId);

            sender.Session.Response.questAccept = new QuestAcceptResponse();

            Result result = character.QuestManager.AccpetQuest(sender, message.QuestId);
            sender.Session.Response.questAccept.Result = result;
            sender.SendResponse();
        }
    }

}
