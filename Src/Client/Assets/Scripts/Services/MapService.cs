using Common;
using Network;
using SkillBridge.Message;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;
using Managers;

namespace Services
{
    class MapService : Singleton<MapService>, IDisposable
    {
        public int CurrentMapId { get; private set; }

        public MapService()//创建时执行
        {

            Debug.Log("MapService()");
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);

        }

       

        public void Dispose()//销毁时执行
        {
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);

        }

        public void Init()
        {

        }

        private void OnMapCharacterEnter(object sender, MapCharacterEnterResponse message)
        {
            Debug.LogFormat("OnMapCharacterEnter:Map:{0} Count:{1}", message.mapId, message.Characters.Count);
            foreach (var cha in message.Characters)
            {
                if (User.Instance.CurrentCharacter.Id==cha.Id)
                {
                    User.Instance.CurrentCharacter = cha;//更新角色的其他信息
                    //加入角色管理器
                    CharacterManager.Instance.AddCharacter(cha);

                  
                }
                //判断是否是当前地图
                //不是，进入地图，更改当前地图
                if (CurrentMapId!=message.mapId)
                {
                    EnterMap(message.mapId);
                    CurrentMapId = message.mapId;
                }

            }

        }

        private void OnMapCharacterLeave(object sender, MapCharacterLeaveResponse message)//切换地图
        {
            
        }
        /// <summary>
        /// 进入指定id的地图
        /// </summary>
        /// <param name="mapId"></param>
        private void EnterMap(int mapId)
        {
            if (DataManager.Instance.Maps.ContainsKey(mapId))
            {
                SceneManager.Instance.LoadScene(DataManager.Instance.Maps[mapId].Resource);
                Debug.LogFormat("EnterMap:map:{0} success", mapId);
            }
            else
            {
                Debug.LogErrorFormat("EnterMap:map:{0} not existed", mapId);
            }
        }
       
    }
}
