
using RPG_003.Battle.Skills;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;

namespace RPG_003.Battle.Characters.Player
{
    public class Player : CharacterBase
    {
        public PlayerData PlayerData { get; private set; }
        public List<Skill> Skills { get; private set; } = new List<Skill>();
        public override CharacterData Data => PlayerData.CharacterData;

        public void SetPlayerData(PlayerData playerData) 
        {
            PlayerData = playerData;
            Skills.Clear();
            foreach (var skillData in playerData.Skills)
            {
                var skill = new Skill(skillData, this);
                Skills.Add(skill);
            }
        }
    }
}