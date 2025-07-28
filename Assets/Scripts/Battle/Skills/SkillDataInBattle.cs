using RPG_003.Battle.Factions;
using RPG_003.Effect;
using RPG_003.Skills;
using RPG_003.StatesEffect;
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
        [SerializeField] private List<DamageData> _damageDatas;
        [SerializeField] private List<CostData> _costDatas;
        [SerializeField] private List<EffectData> _effectDatas;
        [SerializeField] private TargetData _target;
        [SerializeField] private SoundVFXData _vFXData;
        public string Name => _name;
        public string Description => _description;
        public Sprite Sprite => _sprite;
        public List<DamageData> DamageDatas { get { return _damageDatas; } set { SetDamageDatas(value); } }
        public List<CostData> CostDatas { get { return _costDatas; } set { SetCostDatas(value); } }
        public List<EffectData> EffectDatas { get { return _effectDatas; } set { SetEffectDatas(value); } }

        // EffectDatas の各種リストは、EffectData の target フィールドに応じて自動的に分類される
        #region
        public List<EffectData> SelfEffect { get; private set; } = new List<EffectData>();
        public List<EffectData> TargetEffect { get; private set; } = new List<EffectData>();
        public List<EffectData> RandomTargetEffect { get; private set; } = new List<EffectData>();
        public List<EffectData> AllAllyEffect { get; private set; } = new List<EffectData>();
        public List<EffectData> AllEnemyEffect { get; private set; } = new List<EffectData>();
        public List<EffectData> AllEffect { get; private set; } = new List<EffectData>();
        #endregion
        public TargetData TargetData { get { return _target; } set { _target = value; } }
        public Faction Target => _target.Faction;
        public int TargetCount => _target.Count;
        public bool IsSelf => _target.IsSelf;
        public bool IsRandom => _target.IsRandom;
        public bool CanSelectSameTarget => _target.CanSelectSameTarget;
        public SoundVFXData VFXData => _vFXData;


        // コンストラクタはプライベートにして、Create() から呼ばせる
        private SkillDataInBattle() { }

        #region
        // チェーンのスタート用メソッド
        public static SkillDataInBattle Create() =>
            new SkillDataInBattle();

        public SkillDataInBattle SetName(string name)
        {
            _name = name;
            return this;
        }

        public SkillDataInBattle SetDescription(string desc)
        {
            _description = desc;
            return this;
        }
        public SkillDataInBattle SetDamageDatas(List<DamageData> damageDatas)
        {
            if (damageDatas == null || damageDatas.Count == 0)
            {
                // 空のリストを設定する場合は、空のリストを返す
                _damageDatas = new List<DamageData>();
                return this;
            }
            _damageDatas = new(damageDatas);
            return this;
        }
        public SkillDataInBattle SetCostDatas(List<CostData> costDatas)
        {
            if (costDatas == null || costDatas.Count == 0)
            {
                // 空のリストを設定する場合は、空のリストを返す
                _costDatas = new List<CostData>();
                return this;
            }
            _costDatas = new(costDatas);
            return this;
        }
        public SkillDataInBattle SetEffectDatas(List<EffectData> effectDatas)
        {
            if (effectDatas == null || effectDatas.Count == 0)
            {
                // 空のリストを設定する場合は、空のリストを返す
                _effectDatas = new List<EffectData>();
                return this;
            }
            _effectDatas = new(effectDatas);
            // EffectDatas の各種リストを初期化
            SelfEffect.Clear();
            TargetEffect.Clear();
            RandomTargetEffect.Clear();
            AllAllyEffect.Clear();
            AllEnemyEffect.Clear();
            AllEffect.Clear();
            // EffectDatas を分類
            foreach (var effect in _effectDatas)
            {
                switch (effect.target)
                {
                    case EffectData.EffectTarget.Self:
                        SelfEffect.Add(effect);
                        break;
                    case EffectData.EffectTarget.Target:
                        TargetEffect.Add(effect);
                        break;
                    case EffectData.EffectTarget.RandomTarget:
                        RandomTargetEffect.Add(effect);
                        break;
                    case EffectData.EffectTarget.AllAllies:
                        AllAllyEffect.Add(effect);
                        break;
                    case EffectData.EffectTarget.AllEnemies:
                        AllEnemyEffect.Add(effect);
                        break;
                    case EffectData.EffectTarget.All:
                        AllEffect.Add(effect);
                        break;
                }
            }
            return this;
        }
        public SkillDataInBattle SetTarget(TargetData targetData)
        {
            _target = targetData;
            return this;
        }
        public SkillDataInBattle SetSprite(Sprite sprite)
        {
            _sprite = sprite;
            return this;
        }
        public SkillDataInBattle SetVFX(SoundVFXData vFXData)
        {
            _vFXData = vFXData;
            return this;
        }
        #endregion
        public object Clone()
        {
            var clone = SkillDataInBattle.Create()
                .SetName(_name)
                .SetDescription(_description)
                .SetDamageDatas(_damageDatas)
                .SetCostDatas(_costDatas)
                .SetTarget(_target)
                .SetSprite(_sprite)
                .SetVFX(_vFXData);
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