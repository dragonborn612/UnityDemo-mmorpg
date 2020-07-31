using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SkillBridge.Message;
using Common.Data;
using Models;

namespace Entities
{
    public class Character:Entity
    {
        public NCharacterInfo nCharacterInfo;
        /// <summary>
        /// 职业信息
        /// </summary>
        public CharacterDefine characterDefine;

        public string Name
        {
            get
            {
                if (nCharacterInfo.Type==CharacterType.Player)
                {
                    return this.nCharacterInfo.Name;
                }
                else
                {
                    return this.characterDefine.Name;
                }
            } 
        }
        /// <summary>
        /// 是否是玩家操控的角色，（有可能是其他玩家的角色）
        /// </summary>
        public bool IsPlayer
        {
            get
            {
                return this.nCharacterInfo.Id == User.Instance.CurrentCharacter.Id;
            }
        }

        public Character(NCharacterInfo Info):base(Info.Entity)
        {
            this.nCharacterInfo = Info;
            this.characterDefine = DataManager.Instance.Characters[Info.Tid];
        }
        public void MoveForward()
        {
            Debug.LogFormat("MoVeForward");
            this.speed = characterDefine.Speed;
        }
        public void MoveBack()
        {
            Debug.LogFormat("MoveBack");
            this.speed = -characterDefine.Speed;
        }
        public void Stop()
        {
            Debug.LogFormat( "Stop");
            this.speed = 0;
        }
        public void SetDirection(Vector3Int direction)
        {
            Debug.LogFormat("SetDrirection:{0}", direction);
            this.direction = direction;
        }
        public void SetPosition(Vector3Int position)
        {
            Debug.LogFormat("SetPosition", position);
            this.position = position;
        }
    }
}
