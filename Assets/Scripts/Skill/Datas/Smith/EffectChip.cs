
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Skills
{
    [CreateAssetMenu(fileName = "Effect", menuName = "RPG_003/Skills/Smith/Effect")]
    public class EffectChip : SmithChip
    {
        [SerializeField] private List<DamageData> _damage;
        public List<DamageData> Damage => _damage;
    }
}