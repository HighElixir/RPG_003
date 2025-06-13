
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace RPG_003.Skills
{
    [CreateAssetMenu(fileName = "Cost", menuName = "RPG_003/Skills/Smith/Cost")]
    public class CostChip : SkillData
    {
        [SerializeField] private float _load;
        [SerializeField] private List<CostData> _cost;
        [SerializeField] private float _productionPower;

        public float Load => _load;
        public List<CostData> Cost => _cost;
        public float ProductionPower => _productionPower;
    }
}