using RPG_003.Battle;
using System.Collections.Generic;
using System.Linq;
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
                foreach (EffectChip chip in _effect) p += chip.Power;
                foreach (CostChip chip in _cost) p += chip.Power;
                return p;
            }
        }
        public override SkillData SkillData => null;
        public override Sprite Icon => _custonIcon == null ? _skillSlotData.DefaultIcon : _custonIcon;
        public override string Name => string.IsNullOrEmpty(_custonName) ? _skillSlotData.Name : _custonName;
        public override string Desc => string.IsNullOrEmpty(_custonDesc) ? _skillSlotData.Description : _custonDesc;
        public override bool IsValid(out string errorMessage)
        {
            if (_skillSlotData == null)
            {
                errorMessage = "SkillSlotData is not set.";
                return false;
            }
            if (_effect.Count > _skillSlotData.EffectSlotCount || _effect.Count < 1)
            {
                errorMessage = $"Effect count must be between 1 and {_skillSlotData.EffectSlotCount}.";
                return false;
            }
            if (_cost.Count > _skillSlotData.CostSlotCount || _cost.Count < 1)
            {
                errorMessage = $"Cost count must be between 1 and {_skillSlotData.CostSlotCount}.";
                return false;
            }
            if (_target == null)
            {
                errorMessage = "Target is not set.";
                return false;
            }
            if (Load > _skillSlotData.MaximumLoad)
            {
                errorMessage = $"Load exceeds maximum limit of {_skillSlotData.MaximumLoad}.";
                return false;
            }
            if (Power < 0)
            {
                errorMessage = "Power cannot be negative.";
                return false;
            }
            errorMessage = string.Empty;
            return true;
        }
        public override bool CanSetSkillData(SkillData data)
        {
            if (!data.IsMatch(SkillMaker.SkillType.Smith)) return false;
            if (data is SkillSlotData) return true;
            if (data is SmithChip smith)
            {
                if (_skillSlotData == null) return false;
                if (smith.Load + Load > _skillSlotData.MaximumLoad) return false;
                if (smith is EffectChip) return _effect.Count < _skillSlotData.EffectSlotCount;
                if (smith is CostChip) return _cost.Count < _skillSlotData.CostSlotCount;
            }
            return false;
        }

        public override SkillDataInBattle ConvartData()
        {
            List<DamageData> damage = new();
            List<CostData> cost = new();
            foreach (EffectChip chip in _effect) damage.AddRange(chip.Damage);
            foreach (CostChip chip in _cost) cost.AddRange(chip.Cost);
            var s = new SkillDataInBattle(Name, Desc, Icon, damage, cost, _target.Target, SoundVFXData);
            return s;
        }

        public override void SetSkillData(SkillData data)
        {
            if (data is SkillSlotData slotData)
            {
                _skillSlotData = slotData;
                Debug.Log($"Set SkillSlotData: {_skillSlotData.Name}");
                _effect.Clear();
                _cost.Clear();
                _target = null;
            }
            else if (data is SmithChip chip)
            {
                if (chip is EffectChip effectChip)
                {
                    if (_effect.Count < _skillSlotData.EffectSlotCount)
                    {
                        _effect.Add(effectChip);
                        Debug.Log($"Added EffectChip: {effectChip.Name}");
                    }
                }
                else if (chip is CostChip costChip)
                {
                    if (_cost.Count < _skillSlotData.CostSlotCount)
                    {
                        _cost.Add(costChip);
                        Debug.Log($"Added CostChip: {costChip.Name}");
                    }
                }
                else if (chip is TargetChip targetChip)
                {
                    _target = targetChip;
                    Debug.Log($"Set TargetChip: {targetChip.Name}");
                }
            }
        }

        public override bool RemoveSkillData(SkillData data)
        {
            if (data is EffectChip effect && _effect.Contains(effect))
            {
                Debug.Log($"Removed EffectChip: {effect.Name}");
                return _effect.Remove(effect);
            }
            else if (data is CostChip cost && _cost.Contains(cost))
            {
                Debug.Log($"Removed CostChip: {cost.Name}");
                return _cost.Remove(cost);
            }
            else if (data is TargetChip target && _target == target)
            {
                Debug.Log($"Removed TargetChip: {target.Name}");
                _target = null;
                return true;
            }
            return false;
        }

        public override IReadOnlyList<SkillData> GetSkillDatas()
        {
            List<SkillData> datas = new List<SkillData>(_effect);
            datas.Add(_skillSlotData);
            datas.Add(_target);
            datas.AddRange(_cost);
            return datas.AsReadOnly();
        }
        public override float GetCriticalRate()
        {
            if (_effect.Count <= 0)
            {
                return 0f; // No damage data available
            }
            return _effect.Average(e => e.Damage.Average(d => d.criticalRate));
        }

        public override float GetCriticalDamage()
        {
            if (_effect.Count <= 0)
            {
                return 0f; // No damage data available
            }
            return _effect.Average(e => e.Damage.Average(d => d.criticalRateBonus));
        }
    }
}