using RPG_003.Skills;
using RPG_003.Battle;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Core
{
    [Serializable]
    public class PlayerDataHolder
    {
        // == ==
        [SerializeField] private CharacterData _characterData = new();
        [SerializeField] private List<SkillDataHolder> _skills = new();

        // == Property ==
        public CharacterData CharacterData => _characterData;
        public List<SkillDataHolder> Skills => _skills ??= new List<SkillDataHolder>();


        // === Public ===
        public void SetCharacter(CharacterData characterData) => _characterData = characterData;
        public void SetSkills(List<SkillDataHolder> skills) => _skills = skills;

        public PlayerData Convert()
        {
            return new PlayerData(CharacterData, Skills.ConvertAll(h => { return h.ConvartData(); }));
        }

        // === Constracter ===
        public PlayerDataHolder(CharacterData characterData)
        {
            _characterData = characterData;
            _skills = new List<SkillDataHolder>();
        }
    }
}