using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Message;

using Common;
using Common.Data;

using Network;
using GameServer.Managers;
using GameServer.Entities;
using GameServer.Services;

namespace GameServer.Models
{
    class Map
    {
        internal class MapCharacter
        {
            public NetConnection<NetSession> connection;
            public Character character;

            public MapCharacter(NetConnection<NetSession> conn, Character cha)
            {
                this.connection = conn;
                this.character = cha;
            }
        }

        public int ID
        {
            get { return this.Define.ID; }
        }
        internal MapDefine Define;

        Dictionary<int, MapCharacter> MapCharacters = new Dictionary<int, MapCharacter>();


        internal Map(MapDefine define)
        {
            this.Define = define;
        }

        internal void Update()
        {
        }
        public void UpdataEntity(NEntitySync nEntitySync)//客户端发过来的
        {
            //遍历看那个是自己
            //是自己把自己的信息更新到服务器
            //不是自己把自己发送给别人
            foreach (var item in MapCharacters)
            {
                if (item.Value.character.entityId==nEntitySync.Id)
                {
                    item.Value.character.EntityData.Position = nEntitySync.Entity.Position;
                    item.Value.character.EntityData.Direction = nEntitySync.Entity.Direction;
                    item.Value.character.EntityData.Speed = nEntitySync.Entity.Speed;
                }
                else
                {
                    MapService.Instance.SendEntityUpdata(item.Value.connection, nEntitySync);
                }
            }

        }

        /// <summary>
        /// 角色进入地图
        /// </summary>
        /// <param name="character"></param>
        internal void CharacterEnter(NetConnection<NetSession> conn, Character character)
        {
            Log.InfoFormat("CharacterEnter: Map:{0} characterId:{1}", this.Define.ID, character.Info.Id);

            character.Info.mapId = this.ID;

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();

            message.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            message.Response.mapCharacterEnter.mapId = this.Define.ID;
            message.Response.mapCharacterEnter.Characters.Add(character.Info);

            foreach (var kv in this.MapCharacters)
            {
                message.Response.mapCharacterEnter.Characters.Add(kv.Value.character.Info);
                
            }
            foreach (var kv in this.MapCharacters)
            {
                this.SendCharacterEnterMap(kv.Value.connection, character.Info);//通知其他所有在线玩家
            }
                

            this.MapCharacters[character.Info.Id] = new MapCharacter(conn, character);

            byte[] data = PackageHandler.PackMessage(message);
            conn.SendData(data, 0, data.Length);
        }
        internal void CharacterLeave( Character character)
        {
            Log.InfoFormat("ChaaracterLeave:Map:{0} characterId:{1},", this.Define.ID, character.Info.Id);
            foreach (var item in MapCharacters)
            {
                SendCharacterLeaveMap(item.Value.connection, character.Info);
            }
            MapCharacters.Remove(character.Id);
        }

        

        void SendCharacterEnterMap(NetConnection<NetSession> conn, NCharacterInfo character)
        {
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.mapCharacterEnter = new MapCharacterEnterResponse();

            message.Response.mapCharacterEnter.mapId = this.Define.ID;
            message.Response.mapCharacterEnter.Characters.Add(character);

            byte[] data = PackageHandler.PackMessage(message);
            conn.SendData(data, 0, data.Length);
        }
        private void SendCharacterLeaveMap(NetConnection<NetSession> conn, NCharacterInfo character)
        {
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.mapCharacterLeave = new MapCharacterLeaveResponse();

            message.Response.mapCharacterLeave.characterId = character.Id;

            byte[] data = PackageHandler.PackMessage(message);
            conn.SendData(data, 0, data.Length);
        }
    }
}
