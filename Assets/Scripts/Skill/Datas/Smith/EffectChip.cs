
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Skills
{
    [CreateAssetMenu(fileName = "Effect", menuName = "RPG_003/Skills/Smith/Effect")]
    public class EffectChip : SmithChip
    {
        [SerializeField] private List<DamageData> _damage;
        [SerializeReference] private List<EffectData> _effects; // スキルの効果
        public List<DamageData> Damage => _damage;
        public List<EffectData> Effects => _effects;
    }
}