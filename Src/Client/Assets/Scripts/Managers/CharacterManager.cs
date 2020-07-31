using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using SkillBridge.Message;


namespace Managers
{
    class CharacterManager : Singleton<CharacterManager>, IDisposable
    {
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();

        public UnityAction<Character> OnCharacterEnter;

        public void Dispose()
        {
            
        }
        public CharacterManager()
        {

        }
        public void Clear()
        {
            Characters.Clear();
        }
        public void AddCharacter(NCharacterInfo nCharacterInfo)
        {
            Debug.LogFormat("AddCharacter:{0}:{1} Map:{2} Entity:{3}", nCharacterInfo.Id, nCharacterInfo.Name, nCharacterInfo.mapId, nCharacterInfo.Entity.ToString());
            Character character = new Character(nCharacterInfo);
            this.Characters[nCharacterInfo.Id] = character;
            if (OnCharacterEnter!=null)
            {
                OnCharacterEnter.Invoke(character);
            }
        }
        public void RemoceCharacter(int characterId)
        {
            Debug.LogFormat("RemoveCharacter:{0}", characterId);
            this.Characters.Remove(characterId);
        }
    }
}
