using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Battle.Characters.Player
{
    [Serializable]
    public class PlayerData
    {
        [SerializeField] private CharacterData _characterData;
        [SerializeField] private List<SkillData> _skillDatas;
        public CharacterData CharacterData => _characterData;
        public List<SkillData> Skills => _skillDatas;

        public PlayerData(CharacterData characterData, List<SkillData> skills)
        {
            _characterData = characterData;
            _skillDatas = skills;
        }
    }
}