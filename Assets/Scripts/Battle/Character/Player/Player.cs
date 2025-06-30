using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Battle
{
    public class Player : CharacterObject
    {
        public PlayerData PlayerData { get; private set; }
        public List<Skill> Skills { get; private set; } = new List<Skill>();
        public override CharacterData Data => PlayerData.CharacterData;

        public void SetPlayerData(PlayerData playerData)
        {
            PlayerData = playerData;
            Skills.Clear();
            foreach (var SkillDataInBattle in playerData.Skills)
            {
                Debug.Log($"Init Skill, Name: {SkillDataInBattle.Name}");
                var skill = new Skill(SkillDataInBattle, this);
                Skills.Add(skill);
            }
        }
    }
}