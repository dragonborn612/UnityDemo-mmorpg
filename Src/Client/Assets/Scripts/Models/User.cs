using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    class User : Singleton<User>
    {
        NUserInfo userInfo;


        public NUserInfo Info
        {
            get { return userInfo; }
        }


        public void SetupUserInfo(NUserInfo info)
        {
            this.userInfo = info;
        }
        public  void SetupUserCharacterInfo(List<NCharacterInfo> characters )
        {
            this.userInfo.Player.Characters.Clear();
            this.userInfo.Player.Characters.AddRange(characters);
        }
        public SkillBridge.Message.NCharacterInfo CurrentCharacter { get; set; }

    }
}
