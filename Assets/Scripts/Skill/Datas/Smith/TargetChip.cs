using UnityEngine;

namespace RPG_003.Skills
{
    [CreateAssetMenu(fileName = "Target", menuName = "RPG_003/Skills/Smith/Target")]
    public class TargetChip : SmithChip
    {
        [SerializeField] private TargetData _target;
        public TargetData Target => _target;
    }
}