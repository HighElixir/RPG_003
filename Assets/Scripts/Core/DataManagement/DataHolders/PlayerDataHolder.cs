using RPG_003.Battle.Characters;
using RPG_003.Battle.Characters.Player;
using RPG_003.Skills;
using Sirenix.Serialization;
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
        [OdinSerialize] private List<SkillDataHolder> _skills = new();

        // == Property ==
        public CharacterData CharacterData => _characterData;
        public List<SkillDataHolder> Skills => _skills;

        // === Public ===
        public void SetCharacter(CharacterData characterData) => _characterData = characterData;
        public void SetSkills(List<SkillDataHolder> skills) => _skills = skills;

        public PlayerData Convert()
        {
            var p = new PlayerData();
            p.SetCharcter(CharacterData);
            p.SetSkillDataInBattles(Skills.ConvertAll((h) =>
            {
                var data = h.ConvartData();
                data.SetVFX(h.SoundVFXData);
                data.SetIcon(h.Icon);
                return data;
            }));
            return p;
        }

        // === Constracter ===
        public PlayerDataHolder(CharacterData characterData, List<SkillDataHolder> skills)
        {
            _characterData = characterData;
            _skills = skills;
        }
    }
}