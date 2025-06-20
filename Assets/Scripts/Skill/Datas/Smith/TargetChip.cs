
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace RPG_003.Skills
{
    [CreateAssetMenu(fileName = "Target", menuName = "RPG_003/Skills/Smith/Target")]
    public class TargetChip : SmithChip
    {
        [SerializeField] private TargetData _target;
        public TargetData Target => _target;
    }
}