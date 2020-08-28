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
        /// <summary>
        /// 加载小地图图片
        /// </summary>
        /// <returns></returns>
        public Sprite LoadcurrentMiniMap()
        {
            return Resloader.Load<Sprite>("UI/Minimap" + User.Instance.currenMapData.MiniMap);
        }
    }
}
