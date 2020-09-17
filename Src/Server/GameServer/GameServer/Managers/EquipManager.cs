using Common;
using GameServer.Entities;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class EquipManager:Singleton<EquipManager>
    {
        public Result EquipItem(NetConnection<NetSession> sender,int slot,int itemId,bool isEquip)
        {
            Character character = sender.Session.Character;
            //判断角色有没有这件装备
            if (!character.ItemManager.Items.ContainsKey(itemId))
            {
                return Result.Failed;
            }
            //装备
            UpdateEquip(character.Data.Equips, slot, itemId, isEquip);
            //保存数据库
            DBService.Instance.Save();
            return Result.Success;
        }

        private unsafe void UpdateEquip(byte[] equipData, int slot, int itemId, bool isEquip)
        {
            fixed(byte* pt = equipData)//指向第一个字节
            {
                int* soltid = (int*)(pt + slot * sizeof(int));//指向对应槽位的字节
                if (isEquip)
                {
                    *soltid = itemId;
                }
                else
                {
                    *soltid = 0;
                }
            }
        }
    }
}
