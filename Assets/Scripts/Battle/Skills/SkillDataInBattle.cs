using RPG_003.Battle.Factions;
using RPG_003.Effect;
using RPG_003.Skills;
using RPG_003.Status;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RPG_003.Battle
{
    /// <summary>
    /// バトルマネージャーに引き渡すためのデータクラス
    /// </summary>
    [Serializable]
    public class SkillDataInBattle : ICloneable
    {

        [SerializeField] private string _name;
        [SerializeField] private string _description;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private List<DamageData> _damageData;
        [SerializeField] private List<CostData> _costDatas;
        [SerializeField] private TargetData _target;
        [SerializeField] private SoundVFXData _vFXData;
        public string Name => _name;
        public string Description => _description;
        public Sprite Sprite => _sprite;
        public List<DamageData> DamageDatas => _damageData;
        public List<CostData> CostDatas { get { return _costDatas; } set { _costDatas = value; } }
        public TargetData TargetData { get { return _target; } set { _target = value; } }
        public Faction Target => _target.Faction;
        public int TargetCount => _target.Count;
        public bool IsSelf => _target.IsSelf;
        public bool IsRandom => _target.IsRandom;
        public bool CanSelectSameTarget => _target.CanSelectSameTarget;
        public SoundVFXData VFXData => _vFXData;

        public SkillDataInBattle(string name, string desc, Sprite sprite, List<DamageData> damageDatas, List<CostData> costDatas, TargetData targetData, SoundVFXData soundVFXData = null)
        {
            _name = name;
            _description = desc;
            _damageData = damageDatas;
            _costDatas = costDatas;
            _sprite = sprite;
            _target = targetData;
            _vFXData = soundVFXData;
        }
        public void SetVFX(SoundVFXData vFXData)
        {
            _vFXData = vFXData;
        }

        public void SetSprite(Sprite sprite)
        {
            _sprite = sprite;
        }

        public object Clone()
        {
            var clone = new SkillDataInBattle(_name, _description, _sprite, _damageData, _costDatas, _target, _vFXData);
            return clone;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"スキル名: {Name}");
            sb.AppendLine($"説明: {Description}");
            sb.AppendLine($"ダメージ:");
            foreach (var damage in DamageDatas)
            {
                sb.AppendLine($"  - {damage.element.ToJapanese()}: {damage.type.ToJapanese()}{damage.amount * 100}% + {damage.fixedAmount}{damage.amountAttribute.ToJapanese()}");
            }
            sb.AppendLine($"  - 基礎会心率 :{DamageDatas.CalcCritRateAverage() * 100}%");
            sb.AppendLine($"  - 基礎会心ダメージ :{DamageDatas.CalcCritDamageAverage() * 100}%");
            sb.AppendLine($"コスト:");
            float hp = 0;
            float mp = 0;
            foreach (var cost in CostDatas)
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
            sb.AppendLine($"ターゲット: {(IsSelf ? "自己" : Target.ToJapanese())}の{TargetCount}体");
            return sb.ToString();
        }
    }
}