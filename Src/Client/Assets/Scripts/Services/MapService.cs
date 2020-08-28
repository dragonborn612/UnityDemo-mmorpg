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
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Unsubscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);

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
                }
                //加入角色管理器
                CharacterManager.Instance.AddCharacter(cha);
                //判断是否是当前地图
                //不是，进入正确地图，更改当前地图
                if (CurrentMapId!=message.mapId)
                {
                    EnterMap(message.mapId);
                    CurrentMapId = message.mapId;
                }
            }
        }

        private void OnMapCharacterLeave(object sender, MapCharacterLeaveResponse message)//切换地图
        {
            //判断是不是自己离开
            //是自己全部移除
            //别人 移除别人
            if (message.characterId == User.Instance.CurrentCharacter.Id)
            {
                CharacterManager.Instance.Clear();
            }
            else
                CharacterManager.Instance.RemoceCharacter(message.characterId);
        }
        /// <summary>
        /// 进入指定id的地图
        /// </summary>
        /// <param name="mapId"></param>
        private void EnterMap(int mapId)
        {
            if (DataManager.Instance.Maps.ContainsKey(mapId))
            {
                User.Instance.currenMapData = DataManager.Instance.Maps[mapId];
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
