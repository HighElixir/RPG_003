using RPG_003.Skills;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RPG_003.Battle
{
    [Serializable]
    public class Skill
    {
        public CharacterObject parent;
        public SkillDataInBattle skillDataInBattle;

        public bool IsActive
        {
            get
            {
#if UNITY_EDITOR
                if (DebugSwitch.instance.isThought_cost) return true;
#endif
                var consume_HP = 0f;
                var consume_MP = 0f;
                foreach (var c in skillDataInBattle.CostDatas)
                {
                    if (c.isHP) consume_HP += parent.StatusManager.HP;
                    else consume_MP = parent.StatusManager.MP;
                }
                if (consume_HP > 0 && parent.StatusManager.HP < consume_HP)
                {
                    return false;
                }
                if (consume_MP > 0 && parent.StatusManager.MP < consume_MP)
                {
                    return false;
                }
                return true;
            }
        }
        public Skill(SkillDataInBattle data, CharacterObject parent)
        {
            skillDataInBattle = data;
            this.parent = parent;
        }
        public void PaymentCost()
        {
            Vector2 pos = parent.gameObject.transform.position;
            foreach (var c in skillDataInBattle.CostDatas)
            {
                if (c.amount == 0) continue;
                if (c.isHP)
                {
                    parent.BattleManager.ApplyDamage(new DamageInfo(
                        null,
                        parent,
                        c.amount,
                        AmountAttribute.Consume
                        ));
                }
                else
                {
                    parent.StatusManager.MP -= c.amount;
                    GraphicalManager.instance.ThrowText(pos, c.amount < 0 ? "+" : "-" + c.amount, new Color(30, 144, 255));
                }

            }
        }
        public IEnumerator Execute(List<CharacterObject> targets, bool isPaymentSkip = false)
        {
#if UNITY_EDITOR
            Debug.Log("Execute : " + skillDataInBattle.Name);
            yield return new WaitForSeconds(1f);
#endif
            if (!isPaymentSkip)
                PaymentCost();
            foreach (var target in targets)
            {
                Debug.Log($"Executing skill on {target.Data.Name}");
                foreach (var d in skillDataInBattle.DamageDatas)
                {
                    var dI = d.MakeDamageInfo(target, parent);
                    if (d.amountAttribute.HasFlag(AmountAttribute.Heal))
                        parent.BattleManager.ApplyHeal(dI);
                    else
                        parent.BattleManager.ApplyDamage(dI);
                }
            }

            if (skillDataInBattle.VFXData != null)
                yield return GraphicalManager.instance.EffectPlay(skillDataInBattle.VFXData, targets.ConvertAll<Vector2>((c) => { return c.transform.position; }));
        }
        public IEnumerator Execute(CharacterObject target)
        {
            yield return Execute(new List<CharacterObject> { target });
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Name : " + skillDataInBattle.Name);
            sb.AppendLine("Desc : " + skillDataInBattle.Description);
            return sb.ToString();
        }
    }
}