using Assets.Scripts.Models;
using Assets.Scripts.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Managers
{
    class EquipMananger:Singleton<EquipMananger>
    {
        public delegate void OnEquipChangeHandler();
        public event OnEquipChangeHandler OnEquipChange;

        public Item[] Equips = new Item[(int)EqipSlot.SoltMax];

        byte[] Data;
        public UICharEquip uiCharaterEquip;
        unsafe public void Init(byte[] data)
        {
            this.Data = data;
            this.ParseEquipData(data);
        }
        /// <summary>
        /// Equips中是否包含
        /// </summary>
        /// <param name="equipId"></param>
        /// <returns></returns>
        public bool Contains(int equipId)
        {
            for (int i = 0; i < Equips.Length; i++)
            {
                if (Equips[i]!=null&&Equips[i].Id==equipId)
                {
                    return true;
                }
            }
            return false;
        }

        public Item GetEquip(EqipSlot slot)
        {
            return Equips[(int)slot];
        }


        /// <summary>
        /// 解析装备数据
        /// </summary>
        /// <param name="data"></param>
        unsafe void ParseEquipData(byte[] data)
        {
            fixed (byte* pt = data)
            {
                for (int i = 0; i < Equips.Length; i++)
                {
                    int itemId = *(int*)(pt + i * sizeof(int));
                    if (itemId>0)
                    {
                        Equips[i] = ItemManager.Instance.Items[itemId];
                    }
                    else
                    {
                        Equips[i] = null;
                    }
                }
            }
        }


        unsafe public byte[] GetEquipData()
        {
            fixed (byte* pt = Data)
            {
                for (int i = 0; i < Equips.Length; i++)
                {
                    int* itemId = (int*)(pt + i * sizeof(int));
                    if (Equips[i]==null)
                    {
                        *itemId = 0;
                    }
                    else
                    {
                        *itemId = Equips[i].Id;
                    }
                }
            }
            return Data;
        }


        /// <summary>
        /// 发穿装备消息
        /// </summary>
        /// <param name="equip"></param>
        public void EquipItem(Item equip)
        {
            ItemService.Instance.SendEquipItem(equip, true);
        }
        /// <summary>
        /// 发脱装备消息
        /// </summary>
        /// <param name="equip"></param>
        public void UnEquipItem(Item equip)
        {
            ItemService.Instance.SendEquipItem(equip, false);
        }
        public void OnEquipItem(Item equip)
        {
            if (Equips[(int)equip.EquipInfo.Slot]!= null&& Equips[(int)equip.EquipInfo.Slot].Id==equip.Id)//如果装备一样的装备 返回空
            {
                return ;
            }
            this.Equips[(int)equip.EquipInfo.Slot] = ItemManager.Instance.Items[equip.Id];

            if (OnEquipChange!=null)
            {
                OnEquipChange.Invoke();
            }
        }
        public void OnUnEquipItem(EqipSlot slot)
        {
            if (Equips[(int)slot] != null)
            {
                Equips[(int)slot] = null;
            }
            if (OnEquipChange != null)
            {
                OnEquipChange.Invoke();
            }
        }

        public void ShowUiEquip()
        {
            uiCharaterEquip = UIManager.Instance.Show<UICharEquip>();
        }
    }
}
