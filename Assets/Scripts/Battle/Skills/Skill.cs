using RPG_003.Battle.Characters;
using RPG_003.Status;
using RPG_003.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq.Expressions;

namespace RPG_003.Battle.Skills
{
    /// <summary>
    /// スキルの細かい実装が決まっていないため簡素な実装
    /// </summary>
    [Serializable]
    public class Skill
    {
        public CharacterBase parent;
        public DamageInfo damageInfo;
        public string skillName;
        public SkillDataInBattle skillDataInBattle;
        private Func<StatusManager, bool> _activationPredicate;
        private void BuildActivationPredicate(IEnumerable<CostData> costs)
        {
            // StatusManager 型のパラメータ
            var smParam = Expression.Parameter(typeof(StatusManager), "sm");
            Expression body = null;

            // GetAmounts メソッド情報
            var getAmt = typeof(StatusManager).GetMethod(nameof(StatusManager.GetAmounts), new[] { typeof(StatusAttribute) });

            foreach (var cost in costs)
            {
                // sm.GetAmounts(cost.type)
                var callGet = Expression.Call(
                    smParam,
                    getAmt,
                    Expression.Constant(cost.isHP ? StatusAttribute.HP : StatusAttribute.MP)
                );
                var constAmt = Expression.Constant(cost.amount, typeof(float));

                // cost.symbol に応じた比較式
                Expression cmp = cost.symbol switch
                {
                    CostData.Symbol.GreaterThen => Expression.GreaterThan(callGet, constAmt),
                    CostData.Symbol.More => Expression.GreaterThanOrEqual(callGet, constAmt),
                    CostData.Symbol.Below => Expression.LessThanOrEqual(callGet, constAmt),
                    CostData.Symbol.Less => Expression.LessThan(callGet, constAmt),
                    _ => throw new NotSupportedException($"Unknown symbol: {cost.symbol}")
                };

                // AND で繋ぐ
                body = body == null ? cmp : Expression.AndAlso(body, cmp);
            }

            // ラムダにしてコンパイル
            var lambda = Expression.Lambda<Func<StatusManager, bool>>(body!, smParam);
            _activationPredicate = lambda.Compile();
        }

        public bool IsActive
        {
            get
            {
#if UNITY_EDITOR
                if (DebugSwitch.instance.isThought_cost) return true;
#endif
                // ここは一行でＯＫ！
                return _activationPredicate(parent.StatusManager);
            }
        }
        public void PaymentCost()
        {
            Vector2 pos = parent.gameObject.transform.position;
            foreach (var c in skillDataInBattle.CostDatas)
            {
                if (c.amount == 0) continue;
                string head = c.amount < 0 ? "+" : "-";
                if (c.isHP)
                {
                    parent.BattleManager.ApplyDamage(new DamageInfo(
                        parent,
                        parent,
                        c.amount,
                        AmountAttribute.Consume
                        ));
                }
                else
                {
                    parent.StatusManager.MP -= c.amount;
                    GraphicalManager.instance.ThrowText(pos, head + c.amount, new Color(30, 144, 255));
                }
                
            }
        }
        public void Execute(List<CharacterBase> targets)
        {
            PaymentCost();
            foreach (var target in targets)
            {
                Debug.Log($"Executing skill on {target.Data.Name}");
                foreach (var d in skillDataInBattle.DamageData)
                {
                    var dI = MakeDamageInfo(d, target, d.amountAttribute.HasFlag(AmountAttribute.Magic), d.element);
                    if (d.amountAttribute.HasFlag(AmountAttribute.Heal))
                        parent.BattleManager.ApplyHeal(dI);
                    else
                        parent.BattleManager.ApplyDamage(dI);
                }
            }
        }


        public DamageInfo MakeDamageInfo(DamageData data, CharacterBase target, bool isMagic, Elements element)
        {
            var d = new DamageInfo(parent, target, 0);
            float damage = data.fixedAmount;
            float amount;
            switch (data.type)
            {
                case StatusAttribute.HP:
                    amount = parent.StatusManager.HP;
                    break;
                case StatusAttribute.MaxHP:
                    amount = parent.StatusManager.MaxHP;
                    break;
                case StatusAttribute.MP:
                    amount = parent.StatusManager.GetStatusAmount(StatusAttribute.MP).currentAmount;
                    break;
                default:
                    amount = parent.StatusManager.GetStatusAmount(data.type).ChangedMax;
                    break;
            }
            damage += amount * data.amount;
            d.Damage = damage;
            d.Elements = data.element;
            d.AmountAttribute = data.amountAttribute;
            return d;
        }
        public Skill(SkillDataInBattle data, CharacterBase parent)
        {
            skillDataInBattle = data;
            skillName = data.Name;
            this.parent = parent;
            BuildActivationPredicate(data.CostDatas);
        }
    }
}