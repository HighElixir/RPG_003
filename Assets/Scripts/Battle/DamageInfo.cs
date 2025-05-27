using RPG_001.Characters;
using System;

namespace RPG_001.Battle
{
    [Serializable]
    public class DamageInfo
    {
        public ICharacter Source { get; set; }
        public float Damage { get; set; }
        public bool IsCritical { get; set; }
        public bool IsMissed { get; set; }
        public bool IsDodged { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsResisted { get; set; }
        public DamageInfo(ICharacter source, float damage, bool isCritical = false, bool isMissed = false,
                          bool isDodged = false, bool isBlocked = false,
                          bool isResisted = false)
        {
            Source = source;
            Damage = damage;
            IsCritical = isCritical;
            IsMissed = isMissed;
            IsDodged = isDodged;
            IsBlocked = isBlocked;
            IsResisted = isResisted;
        }
    }
}