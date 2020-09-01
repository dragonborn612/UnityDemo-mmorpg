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
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapTeleportRequest>(OnMapTeleport);

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
        private void OnMapTeleport(NetConnection<NetSession> sender, MapTeleportRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnMapTeleporter:CharacterId:{0}:{1} TeleporterId:{2}", character.Id, character.Data.Name, message.teleporterId);
            if (!DataManager.Instance.Teleporters.ContainsKey(message.teleporterId))
            {
                Log.WarningFormat("Source TeleporterId:[{0}]not existed!", message.teleporterId);
                return;
            }
            TeleporterDefine source = DataManager.Instance.Teleporters[message.teleporterId];
            if (source.LinkTo==0||!DataManager.Instance.Teleporters.ContainsKey(source.LinkTo))
            {
                Log.ErrorFormat("Source TeleporterId:[{1}] LinkeTo Id:[{1}]not existed!", message.teleporterId, source.LinkTo);
                
            }
            TeleporterDefine target = DataManager.Instance.Teleporters[source.LinkTo];

            MapManager.Instance[source.MapID].CharacterLeave(character);
            character.Position = target.Position;
            character.Direction = target.Direction;
            MapManager.Instance[target.MapID].CharacterEnter(sender, character);
        }
    }
}
