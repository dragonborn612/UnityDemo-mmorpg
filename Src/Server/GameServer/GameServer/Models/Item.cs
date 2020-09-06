using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    class Item
    {
        TCharacterItem dbItem;//数据库缓存

        public int Count;

        public int ID;
        public Item(TCharacterItem item)
        {
            this.dbItem = item;
            this.ID =(short) item.ItemID;
            this.Count = (short)item.ItemCount;
        }
        public void Add(int count)
        {
            this.Count += count;
            dbItem.ItemCount = this.Count;
        }
        public void Remove(int count)
        {
            if (this.Count>0)
            {
                this.Count -= count;
                dbItem.ItemCount = this.Count;
            }
        }
        //暂时不用
        public bool Use(int count = 1)
        {
            return false;
        }

        public override string ToString()
        {
            return string.Format("ID:{0},Count:{1}", this.ID, this.Count);
        }
    }
}
