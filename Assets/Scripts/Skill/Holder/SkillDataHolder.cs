using RPG_003.Battle.Skills;
using RPG_003.Effect;
using System;
using UnityEngine;

namespace RPG_003.Skills
{
    [Serializable]
    public abstract class SkillDataHolder
    {
        [SerializeField] protected Sprite _custonIcon;
        [SerializeField] protected SoundVFXData _soundVFXData;
        [SerializeField] protected string _custonName;
        [SerializeField] protected string _custonDesc;

        // === Properties ===
        public abstract SkillData SkillData { get; } 
        public abstract Sprite Icon { get; }
        public abstract string Name { get; }
        public abstract string Desc { get; }
        public SoundVFXData SoundVFXData => _soundVFXData;

        // === Public ===
        public void SetIcon(Sprite icon)
        {
            _custonIcon = icon;
        }
        public void SetSoundVFXData(SoundVFXData soundVFXData)
        {
            _soundVFXData = soundVFXData;
        }
        public void SetCustomName(string name)
        {
            _custonName = name;
        }
        public void SetCustomDesc(string desc)
        {
            _custonDesc = desc;
        }
        public abstract SkillDataInBattle ConvartData();
    }
}