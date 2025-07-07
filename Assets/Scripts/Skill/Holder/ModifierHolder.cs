using HighElixir;
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
    public class ModifierHolder : SkillHolder
    {
        [SerializeField] private ModifierData _skill;
        [SerializeField] private List<AddonData> _addons = new();
        public override SkillData SkillData => _skill;
        public List<AddonData> Addons => _addons;
        public override Sprite Icon => _custonIcon ? _custonIcon : _skill.DefaultIcon;

        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_custonName))
                {
                    if (_skill != null)
                        return _skill.Name;
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
                    if (_skill != null)
                        return _skill.Description;
                    return "";
                }
                return _custonDesc;
            }
        }

        public override bool IsValid(out int errorCode)
        {
            if (_skill != null && _addons.Count <= _skill.InstallableAddon)
            {
                errorCode = 0;
                return true;
            }
            else if (_skill == null)
            {
                errorCode = -1;
            }
            else
            {
                errorCode = -2;
            }
            return false;
        }
        public override bool CanSetSkillData(SkillData data)
        {
            if (data is ModifierData modifierData)
            {
                // ModifierDataは常に受け入れる
                return true;
            }
            if (_skill != null && data is AddonData addonData)
            {
                return _addons.Count < _skill.InstallableAddon;
            }
            // 他のスキルデータは受け入れない
            return false;
        }
        public override float GetCriticalRate()
        {
            if (_skill == null || _skill.DamageDatas == null || _skill.DamageDatas.Count == 0)
                return 0f;

            // 各ダメージデータに対して、アドオン効果を適用したクリティカル率を計算
            var adjustedRates = _skill.DamageDatas.Select(dmg =>
            {
                float rate = dmg.criticalRate;
                foreach (var addon in _addons)
                {
                    foreach (var eff in addon.ForDamages)
                    {
                        // このダメージに対して効果を乗せられるかチェック
                        if (eff.Usable(dmg))
                        {
                            // scale をかけたうえで fixedAmount を足す
                            rate = rate * eff.scale + eff.fixedAmount;
                        }
                    }
                }
                return rate;
            });

            // 最終的に平均クリティカル率を返す
            return adjustedRates.Average();
        }
        public override float GetCriticalDamage()
        {
            if (_skill == null || _skill.DamageDatas == null || _skill.DamageDatas.Count == 0)
            {
                return 0f; // No damage data available
            }
            // 各ダメージデータに対して、アドオン効果を適用したクリティカル率を計算
            var adjustedRates = _skill.DamageDatas.Select(dmg =>
            {
                float bonus = dmg.criticalRateBonus;
                foreach (var addon in _addons)
                {
                    foreach (var eff in addon.ForDamages)
                    {
                        // このダメージに対して効果を乗せられるかチェック
                        if (eff.Usable(dmg))
                        {
                            // scale をかけたうえで fixedAmount を足す
                            bonus = bonus * eff.scale + eff.fixedAmount;
                        }
                    }
                }
                return bonus;
            });
            // 最終的に平均クリティカル率を返す
            return adjustedRates.Average();
        }
        public override SkillDataInBattle ConvartData()
        {
            // 元のスキルデータをコピーしてバトル用データを生成
            var s = new SkillDataInBattle(
                Name,
                Desc,
                Icon,
                new List<DamageData>(_skill.DamageDatas),
                new List<CostData>(_skill.CostDatas),
                _skill.Target,
                SoundVFXData
            );

            foreach (var addon in _addons)
            {
                // --- ダメージ修正 ---
                for (int i = 0; i < s.DamageDatas.Count; i++)
                {
                    var dmg = s.DamageDatas[i];
                    foreach (var eff in addon.ForDamages)
                    {
                        if (eff.Usable(dmg))
                        {
                            // 【ダメージの係数】に scale をかけ、
                            dmg.amount = dmg.amount * eff.scale;
                            // 【ダメージの固定値】にも scale をかけたうえで fixedAmount を加算
                            dmg.fixedAmount = dmg.fixedAmount * eff.scale + eff.fixedAmount;
                        }
                    }
                    s.DamageDatas[i] = dmg;
                }

                // --- コスト修正 ---
                for (int i = 0; i < s.CostDatas.Count; i++)
                {
                    var cost = s.CostDatas[i];
                    foreach (var eff in addon.Costs)
                    {
                        if (eff.Usable(cost))
                        {
                            // コストも同様に scale をかけたあと fixedAmount を加算
                            cost.amount = cost.amount * eff.scale + eff.fixedAmount;
                        }
                    }
                    s.CostDatas[i] = cost;
                }

                if (addon.IsOverrideTarget) s.TargetData = addon.OverrideTarget;
                if (addon.IsOverrideTargetCount)
                    s.TargetData = new TargetData(s.TargetData.IsSelf, s.TargetData.Faction, addon.OverrideTargetCount, s.TargetData.IsRandom, s.TargetData.CanSelectSameTarget);

                foreach (var mapping in addon.OverrideElement)
                {
                    for (int j = 0; j < s.DamageDatas.Count; j++)
                    {
                        var dd = s.DamageDatas[j];
                        if (dd.element == mapping.from)
                        {
                            dd.element = mapping.to;
                            s.DamageDatas[j] = dd;
                        }
                    }
                }
            }

            return s;
        }

        public override void SetSkillData(SkillData data)
        {
            if (data is ModifierData modifierData)
            {
                _skill = modifierData;
            }
            if (data is AddonData addon)
            {
                _addons.Add(addon);
            }
        }
        public override bool RemoveSkillData(SkillData data)
        {
            if (data is AddonData addon && _addons.Contains(addon))
            {
                return _addons.Remove(addon);
            }
            else if (_skill == data as ModifierData)
            {
                _skill = null; // ModifierDataは一つしか持てないのでnullにする
                return true;
            }
            return false;
        }

        public override bool RemoveSkillData(SkillData data, out List<SkillData> list)
        {
            list = new List<SkillData>();
            if (data is ModifierData)
            {
                var clone = new List<AddonData>(_addons);
                foreach (var addon in clone)
                {
                    if (RemoveSkillData(addon))
                        list.Add(addon);
                }
            }
            return RemoveSkillData(data);
        }

        public override IReadOnlyList<SkillData> GetSkillDatas()
        {
            var res = new List<SkillData> { _skill };
            res.AddRange(_addons);
            return res.AsReadOnly();
        }

        public override string ToString()
        {
            var data = ConvartData();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"スキル名: {Name}");
            sb.AppendLine($"説明: {Desc}");
            sb.AppendLine($"ダメージ:");
            foreach (var damage in data.DamageDatas)
            {
                sb.AppendLine($"  - {damage.element.ToJapanese()}: {damage.type.ToJapanese()}{damage.amount * 100}% + {damage.fixedAmount}{damage.amountAttribute.ToJapanese()}");
            }
            sb.AppendLine($"  - 基礎会心率 :{GetCriticalRate() * 100}%");
            sb.AppendLine($"  - 基礎会心ダメージ :{GetCriticalDamage() * 100}%");
            sb.AppendLine($"コスト:");
            float hp = 0;
            float mp = 0;
            foreach (var cost in data.CostDatas)
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

        public override bool IsNeedReplace(SkillData newItem, out List<SkillData> oldItems)
        {
            oldItems = new();
            if (newItem is AddonData && _addons.TryGetOverItem(_skill.InstallableAddon, out var res))
            {
                oldItems.AddRange(res);
                return true;
            }
            else if (newItem is ModifierData modifier && _skill != null)
            {
                oldItems.Add(SkillData);
                var needRemove = _addons.Count - modifier.InstallableAddon;
                if (needRemove > 0)
                {
                    for (int i = 0; i < needRemove; i++)
                    {
                        oldItems.Add(_addons[i]);
                    }
                }
                return true;
            }
            return false;
        }
    }
}