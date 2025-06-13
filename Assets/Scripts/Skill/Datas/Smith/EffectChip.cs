
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Skills
{
    [CreateAssetMenu(fileName = "Effect", menuName = "RPG_003/Skills/Smith/Effect")]
    public class EffectChip : SkillData
    {
        [SerializeField] private float _load;
        [SerializeField] private List<DamageData> _damage;
        [SerializeField] private float _consumePower;

        public float Load => _load;
        public List<DamageData> Damage => _damage;
        public float ConsumePower => _consumePower;
    }
}