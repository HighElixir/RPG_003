using UnityEngine;

namespace RPG_003.Skills
{
    public abstract class SmithChip : SkillData
    {
        [SerializeField] protected float _load;
        [SerializeField] protected float _power; // consume or production

        public float Load => _load;
        public float Power => _power;
    }
}