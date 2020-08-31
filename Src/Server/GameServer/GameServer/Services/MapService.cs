using Common;
using Common.Data;
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
    class MapService : Singleton<MapService>
    {
        public MapService()
        {
            //  MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapCharacterEnterRequest>(this.OnMapCharacterEnter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapEntitySyncRequest>(OnMapEntitySync);

        }

        

        public void Init()
        {
            MapManager.Instance.Init();
        }
        private void OnMapEntitySync(NetConnection<NetSession> sender, MapEntitySyncRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnMapEntitySync: characterId:{0}:{1} entityId:{2} evnt:{3} Entity:{4}", character.Info.Id, character.Info.Name,
                message.entitySync.Id, message.entitySync.Event, message.entitySync.Entity.String());
            MapManager.Instance[character.Info.mapId].UpdataEntity(message.entitySync);
        }

        internal void SendEntityUpdata(NetConnection<NetSession> connection, NEntitySync nEntitySync)
        {
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.mapEntitySync = new MapEntitySyncResponse();

            message.Response.mapEntitySync.entitySyncs.Add(nEntitySync);
            byte[] data = PackageHandler.PackMessage(message);
            connection.SendData(data,0,data.Length);
        }
    }
}
