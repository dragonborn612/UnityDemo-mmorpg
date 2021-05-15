using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Models;
using SkillBridge.Message;
using UnityEngine;


namespace Assets.Scripts.Managers
{
    class BagManager : Singleton<BagManager>
    {
        public int Unlocked;
        public BagItem[] Items;
        NBagInfo Info;
        public UIBag uIBag;


        public unsafe void Init(NBagInfo info)
        {
            this.Info = info;
            this.Unlocked = info.Unlocked;
            Items = new BagItem[Unlocked];
            if (info.Items!=null&&info.Items.Length>=Unlocked)
            {
                Anlyze(info.Items);
                Reset();
            }
            else
            {
                info.Items = new byte[sizeof(BagItem) * this.Unlocked];
                Reset();
            }
            Debug.Log("BagManagerInit");
        }
        /// <summary>
        /// 显示背包
        /// </summary>
        public void ShowBag()
        {
            uIBag = UIManager.Instance.Show<UIBag>();
        }
        /// <summary>
        /// 排列背包
        /// </summary>
        public void Reset()
        {
            int i = 0;
            foreach (var kv in ItemManager.Instance.Items)
            {
                if (kv.Value.Count<=kv.Value.itemDefine.StackLimit)
                {
                    Items[i].Count =(ushort) kv.Value.Count;
                    Items[i].ItemId =(ushort) kv.Value.Id;
                }
                else
                {
                    int count = kv.Value.Count;
                    while (count>kv.Value.itemDefine.StackLimit)
                    {
                        Items[i].Count = (ushort)kv.Value.itemDefine.StackLimit;
                        Items[i].ItemId = (ushort)kv.Value.Id;
                        i++;
                        count -= kv.Value.itemDefine.StackLimit;
                    }
                    Items[i].Count = (ushort)count;
                    Items[i].ItemId = (ushort)kv.Value.Id;
                }
                i++;
            }
        }

        public void RemoveItem(int itemId, int value)
        {
            
        }

        public void AddItem(int itemId,int count)
        {
            //如果添加数量大于堆叠限制；
            ushort addCount = (ushort)count;
            for (int i = 0; i < Items.Length; i++)
            {
                if (this.Items[i].ItemId==itemId)
                {
                    ushort canAdd = (ushort)(DataManager.Instance.Items[itemId].StackLimit - Items[i].Count);
                    if (canAdd>=addCount)
                    {
                        Items[i].Count += addCount;
                        addCount = 0;
                    }
                    else
                    {
                        Items[i].Count += canAdd;
                        addCount -= canAdd;
                    }
                }
            }
            if (addCount>0)
            {
                for (int i = 0; i < Items.Length; i++)
                {
                    if (Items[i].ItemId==0)
                    {
                        Items[i].ItemId = (ushort)itemId;
                        Items[i].Count = addCount;
                        break;
                    }
                   
                }
            }
            if (BagManager.Instance.uIBag != null)//背包打开时
            {
                BagManager.Instance.uIBag.UpdateBag();//刷新背包

            }
        }

        public unsafe void Anlyze(byte[] data)
        {
            fixed(byte* pt = data)
            {
                for (int i = 0; i < this.Unlocked; i++)
                {
                    BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));
                    Items[i] = *item;
                }
            }
        }
        public unsafe NBagInfo GetNBagInfo()
        {
            fixed(byte* pt = this.Info.Items)
            {
                for (int i = 0; i < this.Unlocked; i++)
                {
                    BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));
                    *item = Items[i];
                }
            }

            return this.Info;
        }
       
    }
}
