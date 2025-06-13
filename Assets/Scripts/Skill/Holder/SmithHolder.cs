using System.Collections.Generic;
using System.Linq;
using RPG_003.Battle.Skills;
using UnityEngine;
namespace RPG_003.Skills
{
    public class SmithHolder : SkillDataHolder
    {
        [SerializeField] private SkillSlotData _skillSlotData;
        [SerializeField] private List<EffectChip> _effect = new();
        [SerializeField] private List<CostChip> _cost = new();
        [SerializeField] private TargetChip _target = new();

        public SkillSlotData SlotData => _skillSlotData;
        public List<EffectChip> Effect => _effect;
        public List<CostChip> Cost => _cost;
        public TargetChip Target => _target;
        public float Load
        {
            get
            {
                var l = 0f;
                foreach (EffectChip chip in _effect) l += chip.Load;
                foreach (CostChip chip in _cost) l += chip.Load;
                return l;
            }
        }

        public float Power
        {
            get
            {
                var p = 0f;
                foreach (EffectChip chip in _effect) p -= chip.ConsumePower;
                foreach (CostChip chip in _cost) p += chip.ProductionPower;
                return p;
            }
        }
        public override SkillData SkillData => null;
        public override Sprite Icon => _custonIcon == null ? _skillSlotData.DefaultIcon : _custonIcon;
        public override string Name => string.IsNullOrEmpty(_custonName) ? _skillSlotData.Name : _custonName;
        public override string Desc => string.IsNullOrEmpty(_custonDesc) ? _skillSlotData.Description : _custonDesc;
        public override SkillDataInBattle ConvartData()
        {
            List<DamageData> damage = new();
            List<CostData> cost = new();
            foreach (EffectChip chip in _effect) damage.AddRange(chip.Damage);
            foreach(CostChip chip in _cost) cost.AddRange(chip.Cost);
            var s = new SkillDataInBattle(Name, Desc, damage, cost, _target.Target);
            return s;
        }
    }
}