using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Battle
{
    [Serializable]
    public class PlayerData
    {
        [SerializeField] private CharacterData _characterData;
        [SerializeField] private List<SkillDataInBattle> _SkillDataInBattles = new List<SkillDataInBattle>();
        public CharacterData CharacterData => _characterData;
        public List<SkillDataInBattle> Skills => _SkillDataInBattles;

        public PlayerData() { }
        public PlayerData(CharacterData data, List<SkillDataInBattle> skills)
        {
            _characterData = data;
            _SkillDataInBattles.AddRange(skills);
        }
        public void SetCharcter(CharacterData characterData)
        {
            _characterData = characterData;
        }

        public void AddSkillDataInBattle(SkillDataInBattle SkillDataInBattle)
        {
            if (_SkillDataInBattles.Count < 3)
            {
                _SkillDataInBattles.Add(SkillDataInBattle);
            }
        }

        public void SetSkillDataInBattles(List<SkillDataInBattle> SkillDataInBattles)
        {
            _SkillDataInBattles = new(SkillDataInBattles);
        }
    }
}