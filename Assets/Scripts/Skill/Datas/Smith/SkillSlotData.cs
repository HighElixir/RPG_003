
using UnityEngine;

namespace RPG_003.Skills
{
    [CreateAssetMenu(fileName = "Slot", menuName = "RPG_003/Skills/Smith/Slot")]
    public class SkillSlotData : SkillData
    {
        [SerializeField] float _MaximumLoad;
        [SerializeField] int _effectSlotCount;
        [SerializeField] int _costSlotCount;
        [SerializeField] float _addLoadScale = 1f; // 各チップの負荷にかかる係数

        public float MaximumLoad => _MaximumLoad;
        public int EffectSlotCount => _effectSlotCount;
        public int CostSlotCount => _costSlotCount;
        public float AddLoadScale => _addLoadScale;
    }
}