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
       
        public unsafe void Init(NBagInfo info)
        {
            this.Info = info;
            this.Unlocked = info.Unlocked;
            Items = new BagItem[Unlocked];
            if (info.Items!=null&&info.Items.Length>=Unlocked)
            {
                Anlyze(info.Items);
            }
            else
            {
                info.Items = new byte[sizeof(BagItem) * this.Unlocked];
                Reset();
            }
            Debug.Log("BagManagerInit");
        }

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
                        Items[i].Count = (ushort)kv.Value.Count;
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
