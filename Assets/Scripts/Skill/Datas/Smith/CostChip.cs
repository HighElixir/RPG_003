
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace RPG_003.Skills
{
    [CreateAssetMenu(fileName = "Cost", menuName = "RPG_003/Skills/Smith/Cost")]
    public class CostChip : SmithChip
    {
        [SerializeField] private List<CostData> _cost;
        public List<CostData> Cost => _cost;
    }
}