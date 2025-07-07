using RPG_003.Status;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace RPG_003.Skills
{
    [CreateAssetMenu(fileName = "Addon", menuName = "RPG_003/Skills/Modifier/Addon")]
    public class AddonData : SkillData
    {
        // このアドオンをつけられるスキルをIDで制限
        [SerializeField] private List<string> _limitIds;
        [SerializeField] private List<AddonEffectForDamage> _forDamage;
        [SerializeField] private List<AddonEffectForCost> _forCost;
        [SerializeField] private bool _isOverrideTarget = false;
        [SerializeField] private TargetData _overrideTarget;
        [SerializeField] private bool _isOverrideTargetCount = false;
        [SerializeField] private int _overrideTargetCount;
        [SerializeField] private List<(Elements from, Elements to)> _overrideElement = new();

        public List<string> LimitIds => _limitIds;
        public List<AddonEffectForDamage> ForDamages => _forDamage;
        public List<AddonEffectForCost> Costs => _forCost;
        public bool IsOverrideTarget => _isOverrideTarget;
        public TargetData OverrideTarget => _overrideTarget;
        public bool IsOverrideTargetCount => _isOverrideTargetCount;
        public int OverrideTargetCount => _overrideTargetCount;
        public List<(Elements from, Elements to)> OverrideElement => _overrideElement;
    }

    [Serializable]
    public struct AddonEffectForDamage
    {
        // 条件は両方真で成立。片方しか指定していない場合、それだけを条件にする
        // HealやConsumeの場合、Elementは無視される
        public AmountAttribute amountAttribute;
        public Elements elements;
        public StatusAttribute statusAttribute;
        // scaleをかけた後にfixedAmountを加算する
        public float scale; // 元の数値にかかる倍率
        public float fixedAmount; // 固定の増減

        public bool Usable(DamageData data)
        {
            bool flag1 = amountAttribute == AmountAttribute.None || data.amountAttribute == amountAttribute;
            bool flag2 = amountAttribute == AmountAttribute.Heal || amountAttribute == AmountAttribute.Consume;
            bool flag3 = elements == Elements.None || data.element == elements;
            bool flag4 = statusAttribute == StatusAttribute.None || statusAttribute == data.type;
            return flag1 && (flag2 || flag3) && flag4;
        }
    }
    [Serializable]
    public struct AddonEffectForCost
    {
        public enum Patarn
        {
            None,
            HP,
            MP,
            All
        }
        public Patarn conditions; // 条件
        // scaleをかけた後にfixedAmountを加算する
        public float scale; // 元の数値にかかる倍率
        public float fixedAmount; // 固定の増減

        public bool Usable(CostData data)
        {
            bool flag1 = conditions == Patarn.All;
            bool flag2 = conditions == Patarn.HP && data.isHP;
            bool flag3 = conditions == Patarn.MP && !data.isHP;
            return flag1 || flag2 || flag3;
        }
    }
}