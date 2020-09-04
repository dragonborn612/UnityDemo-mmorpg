using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkillBridge.Message;

namespace Common.Data
{
    public enum NpcType
    {
        None=0,
        Functional,
        Task,
    }
    public enum NpcFunction
    {
        None=0,
        InvokeShop=1,
        InvokeInsrance=2,
    }
    public class NPCDefine
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Descript { get; set;}
        public NVector3 Position { get; set; }
        public NpcType Type { get; set; }
        public NpcFunction Function { get; set; }
        public int Param { get; set; }
    }
}
