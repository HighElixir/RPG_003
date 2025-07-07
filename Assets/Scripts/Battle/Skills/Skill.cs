using Cysharp.Threading.Tasks;
using RPG_003.Skills;
using RPG_003.Battle.Factions;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RPG_003.Battle
{
    [Serializable]
    public class Skill
    {
        public Unit parent;
        public SkillDataInBattle skillDataInBattle;

        public bool IsActive
        {
            get
            {
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
        public Skill(SkillDataInBattle data, Unit parent)
        {
            skillDataInBattle = data;
            this.parent = parent;
        }
        public Skill() { }
        public void PaymentCost()
        {
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
                    GraphicalManager.instance.Text.Create(parent.gameObject, c.amount < 0 ? "+" : "-" + c.amount, new Color(30, 144, 255, 1));
                }

            }
        }
        public async UniTask Execute(TargetInfo info, bool isPaymentSkip = false)
        {
            if (!isPaymentSkip)
                PaymentCost();

            // ログ追加
            string text = BattleLog.UseSkill(this, info);
            BattleLog.IconType icon = parent.IsAlly() ? BattleLog.IconType.Positive : BattleLog.IconType.Negative;
            GraphicalManager.instance.BattleLog.Add(text, icon);

            // エフェクト再生
            UniTask task;
            if (skillDataInBattle.VFXData != null)
            {
                List<Vector2> pos = new List<Vector2>();
                foreach (var item in info.ToList())
                {
                    if (item != null)
                        pos.Add(item.transform.position);
                }
                task = GraphicalManager.instance.EffectPlay(skillDataInBattle.VFXData, pos);
            }
            else
                task = UniTask.WaitForEndOfFrame();

            // ダメージ追加
            foreach (var target in info)
            {
                //Debug.Log($"Executing skill on {target.Data.Name}");
                foreach (var d in skillDataInBattle.DamageDatas)
                {
                    var dI = d.MakeDamageInfo(target, parent);
                    dI.Skill = this;
                    if (d.amountAttribute.HasFlag(AmountAttribute.Heal))
                        parent.BattleManager.ApplyHeal(dI);
                    else
                        parent.BattleManager.ApplyDamage(dI);
                    await UniTask.WaitForSeconds(0.2f);
                }
            }
            await task;
        }
        public async UniTask Execute(Unit target)
        {
            await Execute(new TargetInfo(target));
        }


        public static List<Skill> CreateSkills(List<SkillDataInBattle> datas)
        {
            var res = new List<Skill>();
            foreach (var data in datas)
            {
                res.Add(new Skill().SetData(data));
            }
            return res;
        }
        // メソッドチェーン
        public Skill SetData(SkillDataInBattle data)
        {
            skillDataInBattle = data;
            return this;
        }
        public Skill SetParent(Unit parent)
        {
            this.parent = parent;
            return this;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Name : " + skillDataInBattle.Name);
            sb.AppendLine("Desc : " + skillDataInBattle.Description);
            return sb.ToString();
        }
    }
    public static class SkillExtensions
    {
        public static List<Skill> SetParent(this List<Skill> skills, Unit parent)
        {
            foreach (var skill in skills)
            {
                skill.SetParent(parent);
            }
            return skills;
        }
    }
}