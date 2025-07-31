using HighElixir;
using RPG_003.Battle;
using RPG_003.Battle.Factions;
using RPG_003.DataManagements.Datas;
using RPG_003.DataManagements.Datas.Helper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RPG_003.DataManagements.Holders
{
    public class SmithHolder : SkillHolder
    {
        // Serialized Fields
        [SerializeField] private SkillSlotData _skillSlotData;
        [SerializeField] private List<EffectChip> _effect = new();
        [SerializeField] private List<CostChip> _cost = new();
        [SerializeField] private TargetChip _target;

        // Public Properties
        public SkillSlotData SlotData => _skillSlotData;
        public List<EffectChip> Effect => _effect;
        public List<CostChip> Cost => _cost;
        public TargetChip Target => _target;

        /// <summary>
        /// 現在の総負荷を計算して返します。
        /// </summary>
        public float Load
        {
            get
            {
                float l = 0f;
                _effect.ForEach(e => l += e.Load);
                _cost.ForEach(c => l += c.Load);
                if (_target != null) l += _target.Load;
                return l;
            }
        }

        /// <summary>
        /// 消費する動力の合計を計算して返します（マイナス値のみ）。
        /// </summary>
        public float ConsumePower => _effect.Where(e => e.Power < 0).Sum(e => e.Power)
                                         + _cost.Where(c => c.Power < 0).Sum(c => c.Power)
                                         + (_target.Power < 0 ? _target.Power : 0);

        /// <summary>
        /// 生産する動力の合計を計算して返します（プラス値のみ）。
        /// </summary>
        public float ProductPower => _effect.Where(e => e.Power > 0).Sum(e => e.Power)
                                          + _cost.Where(c => c.Power > 0).Sum(c => c.Power)
                                          + (_target.Power > 0 ? _target.Power : 0);

        public float Power => ProductPower + ConsumePower;

        // Override Base Class Properties
        public override SkillData SkillData => _skillSlotData;
        public override Sprite Icon => _custonIcon ?? _skillSlotData.DefaultIcon;
        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_custonName))
                {
                    if (_skillSlotData != null)
                        return _skillSlotData.Name;
                    else
                        return "";
                }
                return _custonName;
            }
        }
        public override string Desc
        {
            get
            {
                if (string.IsNullOrEmpty(_custonDesc))
                {
                    if (_skillSlotData != null)
                        return _skillSlotData.Description;
                    else
                        return "";
                }
                return _custonDesc;
            }
        }

        /// <summary>
        /// スロット、チップ数、負荷、動力収支を検証し、エラーコードを返します。
        /// </summary>
        public override bool IsValid(out int errorCode)
        {
            if (_skillSlotData == null) { errorCode = -1; return false; }
            if (_effect.Count > _skillSlotData.EffectSlotCount || _effect.Count < 1) { errorCode = -2; return false; }
            if (_cost.Count > _skillSlotData.CostSlotCount || _cost.Count < 1) { errorCode = -3; return false; }
            if (_target == null) { errorCode = -4; return false; }
            if (Load > _skillSlotData.MaximumLoad) { errorCode = -5; return false; }
            if (Power < 0) { errorCode = -6; return false; }
            errorCode = 0; return true;
        }

        /// <summary>
        /// 与えられたSkillDataがこのHolderにセット可能か判定します。
        /// </summary>
        public override bool CanSetSkillData(SkillData data)
        {
            if (!data.IsMatch(SkillMaker.SkillType.Smith)) return false;
            if (data is SkillSlotData) return true;
            if (data is SmithChip chip)
            {
                if (_skillSlotData == null) return false;
                if (chip.Load + Load > _skillSlotData.MaximumLoad) return false;
                if (chip is EffectChip) return _effect.Count < _skillSlotData.EffectSlotCount;
                if (chip is CostChip) return _cost.Count < _skillSlotData.CostSlotCount;
                if (chip is TargetChip) return true;
            }
            return false;
        }

        /// <summary>
        /// 登録されたEffectChipのダメージデータから平均クリティカル率を計算して返します。
        /// </summary>
        public override float GetCriticalRate()
        {
            if (_effect.Count <= 0) return 0f;
            var list = new List<DamageData>();
            _effect.ForEach(e => list.AddRange(e.Damage));
            return list.CalcCritRateAverage();
        }

        /// <summary>
        /// 登録されたEffectChipのダメージデータから平均クリティカルダメージを計算して返します。
        /// </summary>
        public override float GetCriticalDamage()
        {
            if (_effect.Count <= 0) return 0f;
            var list = new List<DamageData>();
            _effect.ForEach(e => list.AddRange(e.Damage));
            return list.CalcCritDamageAverage();
        }

        /// <summary>
        /// 現在の設定に基づきSkillDataInBattleを生成して返します。
        /// </summary>
        public override SkillDataInBattle ConvartData()
        {
            var damage = new List<DamageData>();
            var cost = new List<CostData>();
            var effect = new List<EffectData>();
            _effect.ForEach(e => {
                damage.AddRange(e.Damage);
                effect.AddRange(e.Effects);
            });
            _cost.ForEach(c => cost.AddRange(c.Cost));
            return SkillDataInBattle.Create()
                .SetName(Name)
                .SetDescription(Desc)
                .SetDamageDatas(damage)
                .SetCostDatas(cost)
                .SetEffectDatas(effect)
                .SetTarget(_target.Target)
                .SetSprite(Icon)
                .SetVFX(SoundVFXData);
        }

        /// <summary>
        /// SkillSlotDataまたはSmithChipを追加・更新します。
        /// </summary>
        public override void SetSkillData(SkillData data)
        {
            if (data is SkillSlotData slotData)
            {
                _skillSlotData = slotData;
                Debug.Log($"Set SkillSlotData: {_skillSlotData.Name}");
            }
            else if (data is SmithChip chip)
            {
                if (chip is EffectChip effectChip && _effect.Count < _skillSlotData.EffectSlotCount)
                {
                    _effect.Add(effectChip);
                    Debug.Log($"Added EffectChip: {effectChip.Name}");
                }
                else if (chip is CostChip costChip && _cost.Count < _skillSlotData.CostSlotCount)
                {
                    _cost.Add(costChip);
                    Debug.Log($"Added CostChip: {costChip.Name}");
                }
                else if (chip is TargetChip targetChip)
                {
                    _target = targetChip;
                    Debug.Log($"Set TargetChip: {targetChip.Name}");
                }
            }
        }

        /// <summary>
        /// 指定したSkillDataを除去し、除去に成功したかどうかを返します。
        /// </summary>
        public override bool RemoveSkillData(SkillData data)
        {
            if (data is EffectChip effect && _effect.Contains(effect))
            {
                Debug.Log($"Removed EffectChip: {effect.Name}");
                return _effect.Remove(effect);
            }
            if (data is CostChip cost && _cost.Contains(cost))
            {
                Debug.Log($"Removed CostChip: {cost.Name}");
                return _cost.Remove(cost);
            }
            if (data is TargetChip target && _target == target)
            {
                Debug.Log($"Removed TargetChip: {target.Name}");
                _target = null;
                return true;
            }
            if (data is SkillSlotData && _skillSlotData != null)
            {
                _skillSlotData = null;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 除去したSkillDataのリストをoutで返しつつ、指定データを除去します。
        /// </summary>
        public override bool RemoveSkillData(SkillData data, out List<SkillData> list)
        {
            list = new List<SkillData>();
            bool removed = RemoveSkillData(data);
            if (data is SkillSlotData)
            {
                foreach (var item in GetSkillDatas())
                {
                    if (RemoveSkillData(item))
                        list.Add(item);
                }
            }
            return removed;
        }

        /// <summary>
        /// 現在セットされている全SkillDataをリスト形式で返します。
        /// </summary>
        public override IReadOnlyList<SkillData> GetSkillDatas()
        {
            var datas = new List<SkillData>(_effect);
            datas.AddRange(_cost);
            datas.Add(_skillSlotData);
            datas.Add(_target);
            return datas.AsReadOnly();
        }

        /// <summary>
        /// Holderの内容を日本語テキストで表現して返します。
        /// </summary>
        public override string ToString()
        {
            if (!IsValid(out var errorCode))
            {
                return errorCode switch
                {
                    -1 => "スロットデータが設定されていません。",
                    -2 => $"エフェクト数は1～{_skillSlotData.EffectSlotCount}の間でなければなりません。",
                    -3 => $"コスト数は1～{_skillSlotData.CostSlotCount}の間でなければなりません。",
                    -4 => "ターゲットが設定されていません。",
                    -5 => $"負荷が最大値({_skillSlotData.MaximumLoad})を超えています。",
                    -6 => "動力収支がマイナスです。",
                    _ => "無効なスキル構成です。"
                };
            }
            var data = ConvartData();
            var sb = new StringBuilder();
            sb.AppendLine($"スキル名: {Name}");
            sb.AppendLine($"説明: {Desc}");
            sb.AppendLine("ダメージ:");
            foreach (var damage in data.DamageDatas)
                sb.AppendLine($"  - {damage.element.ToJapanese()}: {damage.type.ToJapanese()}{damage.amount * 100}% + {damage.fixedAmount}{damage.amountAttribute.ToJapanese()}");
            sb.AppendLine($"  - 基礎会心率 :{GetCriticalRate() * 100}%");
            sb.AppendLine($"  - 基礎会心ダメージ :{GetCriticalDamage() * 100}%");
            sb.AppendLine("コスト:");
            float hp = 0, mp = 0;
            foreach (var cost in data.CostDatas)
            {
                if (cost.isHP) hp += cost.amount;
                else mp += cost.amount;
            }
            sb.AppendLine($"  - HP: {hp}");
            sb.AppendLine($"  - MP: {mp}");
            sb.AppendLine($"ターゲット: {(_target.Target.isSelf ? "自己" : _target.Target.faction.ToJapanese())}の{_target.Target.count}体");
            sb.AppendLine($"負荷/最大負荷: {Load} / {_skillSlotData.MaximumLoad}");
            sb.AppendLine($"動力収支: 生産({ProductPower}) / 消費({Mathf.Abs(ConsumePower)})");
            return sb.ToString();
        }

        /// <summary>
        /// 新しいSkillDataをセットする際、既存データを置き換える必要があるか判定し、置換対象を返します。
        /// </summary>
        public override bool IsNeedReplace(SkillData newItem, out List<SkillData> oldItems)
        {
            oldItems = new List<SkillData>();
            if (_skillSlotData == null) return false;
            if (!newItem.IsMatch(SkillMaker.SkillType.Smith)) return false;
            if (!newItem.IsSlot())
            {
                if (newItem.IsEffect() && _effect.TryGetOverItem(_skillSlotData.EffectSlotCount, out var res))
                {
                    oldItems.AddRange(res);
                    return true;
                }
                if (newItem.IsCost() && _cost.TryGetOverItem(_skillSlotData.CostSlotCount, out var res1))
                {
                    oldItems.AddRange(res1);
                    return true;
                }
                if (newItem.IsTarget() && _target != null)
                {
                    oldItems.Add(_target);
                    return true;
                }
            }
            else
            {
                oldItems.AddRange(GetSkillDatas());
                return true;
            }
            return false;
        }
    }
}
