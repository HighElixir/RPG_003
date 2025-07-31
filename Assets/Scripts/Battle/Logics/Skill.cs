using Cysharp.Threading.Tasks;
using RPG_003.Battle.Factions;
using RPG_003.DataManagements.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    if (c.isHP) consume_HP += c.amount;
                    else consume_MP += c.amount;
                if ((consume_HP > 0 && parent.StatusManager.HP < consume_HP) ||
                    (consume_MP > 0 && parent.StatusManager.MP < consume_MP))
                    return false;
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
            parent.GetComponent<UnitUI>().Action();
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

            // エフェクト追加
            AddEffects(info);

            // ダメージ追加
            foreach (var target in info)
            {
                Debug.Log($"Executing skill on {target.Data.Name}");
                foreach (var d in skillDataInBattle.DamageDatas)
                {
                    var dI = d.MakeDamageInfo(target, parent);
                    dI.Skill = this;
                    if (d.amountAttribute.HasFlag(AmountAttribute.Heal))
                        parent.BattleManager.ApplyHeal(dI);
                    else
                        parent.BattleManager.ApplyDamage(dI);
                }
            }
            await task;
        }

        private void AddEffects(TargetInfo info)
        {
            // ==== ここから追加：エフェクト適用 ==== //

            // 1. 自分へのエフェクト
            foreach (var ef in skillDataInBattle.SelfEffect)
            {
                if (UnityEngine.Random.value <= ef.chance)
                    parent.BattleManager.ApplyEffect(ef.effect, parent, parent);
            }

            // 2. 指定ターゲットへのエフェクト
            foreach (var target in info)
            {
                foreach (var ef in skillDataInBattle.TargetEffect)
                {
                    if (UnityEngine.Random.value <= ef.chance)
                        parent.BattleManager.ApplyEffect(ef.effect, target, parent);
                }
            }

            // 3. ランダムなターゲットへのエフェクト
            if (skillDataInBattle.RandomTargetEffect.Count > 0 && info.Any())
            {
                var pick = info.ToArray()[UnityEngine.Random.Range(0, info.TargetCount)];
                foreach (var ef in skillDataInBattle.RandomTargetEffect)
                {
                    if (UnityEngine.Random.value <= ef.chance)
                        parent.BattleManager.ApplyEffect(ef.effect, pick, parent);
                }
            }

            // 4. 味方全員へのエフェクト
            if (skillDataInBattle.AllAllyEffect.Count > 0)
            {
                var allies = parent.BattleManager
                                   .GetCharacterMap()
                                   .Values
                                   .Where(u => u.IsAlly());
                foreach (var u in allies)
                    foreach (var ef in skillDataInBattle.AllAllyEffect)
                        if (UnityEngine.Random.value <= ef.chance)
                            u.EffectController.AddEffect(ef.effect);
            }

            // 5. 敵全員へのエフェクト
            if (skillDataInBattle.AllEnemyEffect.Count > 0)
            {
                var enemies = parent.BattleManager
                                    .GetCharacterMap()
                                    .Values
                                    .Where(u => u.IsEnemy());
                foreach (var u in enemies)
                    foreach (var ef in skillDataInBattle.AllEnemyEffect)
                        if (UnityEngine.Random.value <= ef.chance)
                            parent.BattleManager.ApplyEffect(ef.effect, u, parent);
            }

            // 6. 全対象へのエフェクト
            foreach (var u in info)
            {
                foreach (var ef in skillDataInBattle.AllEffect)
                {
                    if (UnityEngine.Random.value <= ef.chance)
                        parent.BattleManager.ApplyEffect(ef.effect, u, parent);
                }
            }
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