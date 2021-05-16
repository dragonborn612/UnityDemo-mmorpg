using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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
        public MapDefine currenMapData { get; set; }
        public PlayerInputerController currentCharacterObject { get; set; }

        public NTeamInfo TeamInfo { get; set; }

        public void AddGold(int gold)
        {
            this.CurrentCharacter.Gold += gold;
        }

        public int CurrentRide = 0;
        public void  Ride(int id)
        {
            if (CurrentRide != id)
            {
                CurrentRide = id;
                currentCharacterObject.SendEntityEvent(EntityEvent.Ride, CurrentRide);
            }
            else
            {
                CurrentRide = 0;
                currentCharacterObject.SendEntityEvent(EntityEvent.Ride, 0);
            }
        }
    }
}
