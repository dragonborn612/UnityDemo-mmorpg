using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Common;

namespace GameServer.Services
{
    class DBService : Singleton<DBService>
    {
        ExtremeWorldEntities entities;

        public ExtremeWorldEntities Entities
        {
            get { return this.entities; }
        }

        public void Init()
        {
            entities = new ExtremeWorldEntities();
        }
        public void Save(bool isAsync=false)
        {
            
            if (isAsync)
            {
                entities.SaveChangesAsync();
            }
            else
            {
                entities.SaveChanges();//异步保存
            }
            
        }
    }
}
