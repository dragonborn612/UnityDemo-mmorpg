using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    class MiniMapManager:Singleton<MiniMapManager>
    {
        public UIMiniMap miniMap;
        private Collider miniMapBroundingBox;
        public Collider MiniMapBroundingBox
        {
            get
            {
                return miniMapBroundingBox;
            }           
        }

        public Transform PlayerTansform
        {
            get
            {
                if (User.Instance.CurrentCharacter==null)
                {
                    return null;
                }
                return User.Instance.currentCharacterObject.transform;
            }
        }

        

        /// <summary>
        /// 加载小地图图片
        /// </summary>
        /// <returns></returns>
        public Sprite LoadcurrentMiniMap()
        {
            return Resloader.Load<Sprite>("UI/Minimap/" + User.Instance.currenMapData.MiniMap);
        }
        /// <summary>
        /// 切换场景时更新小地图
        /// </summary>
        /// <param name="minimapBoundingBox"></param>
        public void UpdtaMinimap(Collider minimapBroundingBox)
        {
            this.miniMapBroundingBox = minimapBroundingBox;
            if (this.miniMap!=null)
            {
                this.miniMap.UpdataMap();
            }
        }
    }
}
