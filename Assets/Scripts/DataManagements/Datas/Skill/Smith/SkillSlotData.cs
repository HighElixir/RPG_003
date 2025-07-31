using UnityEngine;

namespace RPG_003.DataManagements.Datas
{
    [CreateAssetMenu(fileName = "Slot", menuName = "RPG_003/Skills/Smith/Slot")]
    public class SkillSlotData : SkillData
    {
        [SerializeField] private float _MaximumLoad;
        [SerializeField] private int _effectSlotCount;
        [SerializeField] private int _costSlotCount;
        [SerializeField] private float _addLoadScale = 1f; // 各チップの負荷にかかる係数

        public float MaximumLoad => _MaximumLoad;
        public int EffectSlotCount => _effectSlotCount;
        public int CostSlotCount => _costSlotCount;
        public float AddLoadScale => _addLoadScale;
    }
}