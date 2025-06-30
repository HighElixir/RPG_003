using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Skills
{
    [CreateAssetMenu(fileName = "Basic", menuName = "RPG_003/Skills/Basic")]
    public class BasicData : SkillData
    {
        [SerializeField] private List<DamageData> _damage;
        [SerializeField] private List<CostData> _consDatas;
        [SerializeField] private TargetData _target;

        public List<DamageData> DamageDatas => _damage;
        public List<CostData> CostDatas => _consDatas;
        public TargetData Target => _target;
    }
}