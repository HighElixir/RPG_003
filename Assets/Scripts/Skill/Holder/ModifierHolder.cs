using RPG_003.Battle.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Skills
{
    [Serializable]
    public class ModifierHolder : SkillDataHolder
    {
        [SerializeField] private ModifierData _skill;
        [SerializeField] private List<AddonData> _data;
        public override SkillData SkillData => _skill;
        public List<AddonData> Data => _data;
        public override Sprite Icon => _custonIcon ? _custonIcon : _skill.DefaultIcon;

        public override string Name => string.IsNullOrEmpty(_custonName) ? _skill.Name : _custonName;

        public override string Desc => string.IsNullOrEmpty(_custonDesc) ? _skill.Description : _custonDesc;

        public override SkillDataInBattle ConvartData()
        {
            // 元のスキルデータをコピーしてバトル用データを生成
            var s = new SkillDataInBattle(
                _skill.Name,
                _skill.Description,
                new List<DamageData>(_skill.DamageDatas),
                new List<CostData>(_skill.CostDatas),
                _skill.Target
            );

            foreach (var addon in _data)
            {
                // --- ダメージ修正 ---
                for (int i = 0; i < s.DamageData.Count; i++)
                {
                    var dmg = s.DamageData[i];
                    foreach (var eff in addon.ForDamages)
                    {
                        bool attrMatch = dmg.amountAttribute == eff.amountAttribute;
                        bool elemMatch =
                            eff.amountAttribute == AmountAttribute.Heal ||
                            eff.amountAttribute == AmountAttribute.Consume ||
                            eff.elements == Elements.None ||
                            dmg.element == eff.elements;

                        if (attrMatch && elemMatch)
                        {
                            // 【ダメージの係数】に scale をかけ、
                            dmg.amount = dmg.amount * eff.scale;
                            // 【ダメージの固定値】にも scale をかけたうえで fixedAmount を加算
                            dmg.fixedAmount = dmg.fixedAmount * eff.scale + eff.fixedAmount;
                        }
                    }
                    s.DamageData[i] = dmg;
                }

                // --- コスト修正 ---
                for (int i = 0; i < s.CostDatas.Count; i++)
                {
                    var cost = s.CostDatas[i];
                    foreach (var eff in addon.Costs)
                    {
                        if (cost.isHP == eff.isHP)
                        {
                            // コストも同様に scale をかけたあと fixedAmount を加算
                            cost.amount = cost.amount * eff.scale + eff.fixedAmount;
                        }
                    }
                    s.CostDatas[i] = cost;
                }

                if (addon.IsOverrideTarget) s.TargetData = addon.OverrideTarget;
                if (addon.IsOverrideTargetCount)
                    s.TargetData = new TargetData(s.TargetData.IsSelf, s.TargetData.Faction, addon.OverrideTargetCount);

                foreach (var mapping in addon.OverrideElement)
                {
                    for (int j = 0; j < s.DamageData.Count; j++)
                    {
                        var dd = s.DamageData[j];
                        if (dd.element == mapping.from)
                        {
                            dd.element = mapping.to;
                            s.DamageData[j] = dd;
                        }
                    }
                }
            }

            return s;
        }

    }
}