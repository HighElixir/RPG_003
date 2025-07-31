using System;
using System.Collections.Generic;
namespace RPG_003.DataManagements.Datas
{
    [Serializable]
    public struct EnemySkillData
    {
        public string id;
        public string name;
        public string description;
        public string vfxPath; // 例："VFX/Skill/Fireball"
        public List<DamageData> damages;
        public List<EffectData> effects;
        public TargetData target;
    }
}