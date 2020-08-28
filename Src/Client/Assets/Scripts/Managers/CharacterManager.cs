using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using SkillBridge.Message;
using Assets.Scripts.Managers;

namespace Managers
{
    class CharacterManager : Singleton<CharacterManager>, IDisposable
    {
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();

        public UnityAction<Character> OnCharacterEnter;
        public UnityAction<Character> OnCharacterLeave;

        public void Dispose()
        {
            
        }
        public CharacterManager()
        {

        }
        public void Clear()
        {
            int[] keys = Characters.Keys.ToArray();
            foreach (var item in keys)
            {
                this.RemoceCharacter(item);
            }
            Characters.Clear();
        }
        public void AddCharacter(NCharacterInfo nCharacterInfo)
        {
            Debug.LogFormat("AddCharacter:{0}:{1} Map:{2} Entity:{3}", nCharacterInfo.Id, nCharacterInfo.Name, nCharacterInfo.mapId, nCharacterInfo.Entity.ToString());
            Character character = new Character(nCharacterInfo);
            this.Characters[nCharacterInfo.Id] = character;
            EntityManager.Instance.AddEntity(character);
            if (OnCharacterEnter!=null)
            {
                OnCharacterEnter.Invoke(character);
            }
        }
        public void RemoceCharacter(int characterId)
        {
            Debug.LogFormat("RemoveCharacter:{0}", characterId);
            if (Characters.ContainsKey(characterId))
            {
                EntityManager.Instance.RemoveEntity(Characters[characterId].nCharacterInfo.Entity);     
                if (OnCharacterLeave != null)
                {
                    OnCharacterLeave.Invoke(Characters[characterId]);
                }
                this.Characters.Remove(characterId);
            }    
        }
    }
}
