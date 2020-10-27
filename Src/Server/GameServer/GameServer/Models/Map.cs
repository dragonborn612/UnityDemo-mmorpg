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

        /// <summary>
        /// 地图中的角色，以CharacterId为key
        /// </summary>
        Dictionary<int, MapCharacter> MapCharacters = new Dictionary<int, MapCharacter>();

        /// <summary>
        /// 刷怪管理器
        /// </summary>
        SpawnManager SpawnManager = new SpawnManager();
        public MonsterManager MonsterManager = new MonsterManager();

        internal Map(MapDefine define)
        {
            this.Define = define;
            this.SpawnManager.Init(this);
            this.MonsterManager.Init(this);
        }

        internal void Update()
        {
            SpawnManager.Update();
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
            this.MapCharacters[character.Id] = new MapCharacter(conn, character);

            conn.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            conn.Session.Response.mapCharacterEnter.mapId = this.Define.ID;

            //自己和其他人进入地图
            foreach (var kv in this.MapCharacters)
            {
                conn.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.character.Info);
                if (kv.Value.character!=character)
                {
                    this.AddCharacterEnterMap(kv.Value.connection, character.Info);
                }
                
            }
            //怪物进入地图
            foreach (var kv in this.MonsterManager.Mosters)
            {
                conn.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.Info);
            }


            conn.SendResponse();
        }
        internal void CharacterLeave( Character character)
        {
            Log.InfoFormat("ChaaracterLeave:Map:{0} characterId:{1},", this.Define.ID, character.Info.Id);
            foreach (var item in MapCharacters)
            {
                SendCharacterLeaveMap(item.Value.connection, character);
            }
            MapCharacters.Remove(character.Id);
        }

        

        void AddCharacterEnterMap(NetConnection<NetSession> conn, NCharacterInfo character)
        {
            if (conn.Session.Response.mapCharacterEnter==null)
            {
                conn.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
                conn.Session.Response.mapCharacterEnter.mapId = this.Define.ID;
            }
            conn.Session.Response.mapCharacterEnter.Characters.Add(character);
            conn.SendResponse();
        }
        private void SendCharacterLeaveMap(NetConnection<NetSession> conn, Character character)
        {
            conn.Session.Response.mapCharacterLeave = new MapCharacterLeaveResponse();
            conn.Session.Response.mapCharacterLeave.entityId = character.entityId;
            conn.SendResponse();
        }

        /// <summary>
        /// 怪物进入地图
        /// </summary>
        /// <param name="monster"></param>
        public void MonsterEnter(Monster monster)
        {
            Log.InfoFormat("MonsterEnter:Map:{0} monsterId:{1}", this.Define.ID, monster.Id);
            foreach (var item in this.MapCharacters)//每个角色刷怪
            {
                this.AddCharacterEnterMap(item.Value.connection, monster.Info);
            }
        }
    }
}
