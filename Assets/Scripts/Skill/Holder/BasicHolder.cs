using RPG_003.Battle;
using RPG_003.Battle.Factions;
using RPG_003.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RPG_003.Skills
{
    [Serializable]
    public class BasicHolder : SkillHolder
    {
        [SerializeField] private BasicData _skill;

        public override SkillData SkillData => _skill;
        public override Sprite Icon => _custonIcon ? _custonIcon : _skill.DefaultIcon;

        public override string Name => string.IsNullOrEmpty(_custonName) ? _skill.Name : _custonName;

        public override string Desc => string.IsNullOrEmpty(_custonDesc) ? _skill.Description : _custonDesc;

        
        // === Public ===
        public BasicHolder() { }    
        public BasicHolder(BasicData data)
        {
            _skill = data;
        }
        public override bool IsValid(out int errorCode)
        {
            if (_skill != null)
            {
                errorCode = 0;
                return true;
            }
            else
            {
                errorCode = -1;
                return false;
            }
        }
        public override bool CanSetSkillData(SkillData data)
        {
            return data is BasicData;
        }
        public override void SetSkillData(SkillData data)
        {
            _skill = data as BasicData;
            //Debug.Log($"Set SkillData: {_skill.Name}");
        }
        public override SkillDataInBattle ConvartData()
        {
            var s = SkillDataInBattle.Create()
                .SetName(Name)
                .SetDescription(Desc)
                .SetDamageDatas(_skill.DamageDatas)
                .SetCostDatas(_skill.CostDatas)
                .SetEffectDatas(_skill.EffectDatas)
                .SetTarget(_skill.Target)
                .SetSprite(Icon)
                .SetVFX(SoundVFXData);
            return s;
        }

        public override IReadOnlyList<SkillData> GetSkillDatas()
        {
            return new List<SkillData> { _skill }.AsReadOnly();
        }

        public override bool RemoveSkillData(SkillData data)
        {
            if (data is BasicData basicData && _skill == basicData)
            {
                _skill = null;
                return true;
            }
            return false;
        }

        public override bool RemoveSkillData(SkillData data, out List<SkillData> list)
        {
            list = new List<SkillData>();
            return RemoveSkillData(data);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"スキル名: {Name}");
            sb.AppendLine($"説明: {Desc}");
            sb.AppendLine($"ダメージ:");
            foreach (var damage in _skill.DamageDatas)
            {
                sb.AppendLine($"  - {damage.element.ToJapanese()}: {damage.type.ToJapanese()}{damage.amount * 100}% + {damage.fixedAmount}{damage.amountAttribute.ToJapanese()}");
            }
            sb.AppendLine($"  - 基礎会心率 :{GetCriticalRate() * 100}%");
            sb.AppendLine($"  - 基礎会心ダメージ :{GetCriticalDamage() * 100}%");
            sb.AppendLine($"コスト:");
            float hp = 0;
            float mp = 0;
            foreach (var cost in _skill.CostDatas)
            {
                if (cost.isHP)
                {
                    hp += cost.amount;
                }
                else
                {
                    mp += cost.amount;
                }
            }
            sb.AppendLine($"  - HP: {hp}");
            sb.AppendLine($"  - MP: {mp}");
            sb.AppendLine($"ターゲット: {(_skill.Target.IsSelf ? "自己" : _skill.Target.Faction.ToJapanese())}の{_skill.Target.Count}体");
            return sb.ToString();
        }

        public override float GetCriticalRate()
        {
            if (_skill == null || _skill.DamageDatas == null || _skill.DamageDatas.Count == 0)
            {
                return 0f; // No damage data available
            }
            return _skill.DamageDatas.CalcCritRateAverage();
        }

        public override float GetCriticalDamage()
        {
            if (_skill == null || _skill.DamageDatas == null || _skill.DamageDatas.Count == 0)
            {
                return 0f; // No damage data available
            }
            return _skill.DamageDatas.CalcCritDamageAverage();
        }

        public override bool IsNeedReplace(SkillData newItem, out List<SkillData> oldItems)
        {
            oldItems = new List<SkillData>();
            if (newItem  != null && newItem is BasicData && SkillData != null)
            {
                oldItems.Add(SkillData);
                return true;
            }
            return false;
        }
    }
}