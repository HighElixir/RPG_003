using System;
using System.Collections.Generic;
using UnityEngine;
using RPG_003.Status;

namespace RPG_003.Battle
{
    [Serializable]
    public class PlayerData
    {
        [SerializeField] private StatusData _statusData;
        [SerializeField] private List<SkillDataInBattle> _SkillDataInBattles = new List<SkillDataInBattle>();
        [SerializeField] private Sprite _icon;
        public StatusData StatusData => _statusData;
        public List<SkillDataInBattle> Skills => _SkillDataInBattles;
        public Sprite Icon => _icon;
        public PlayerData() { }
        public PlayerData(StatusData data, List<SkillDataInBattle> skills)
        {
            _statusData = data;
            _SkillDataInBattles.AddRange(skills);
        }
        public PlayerData SetIcon(Sprite icon)
        {
            _icon = icon;
            return this;
        }
    }
}