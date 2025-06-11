namespace RPG_003.Battle.Skills
{
    using RPG_003.Battle.Characters;
    using RPG_003.Battle.Characters.Player;
    using RPG_003.Skills;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// スキルの細かい実装が決まっていないため簡素な実装
    /// </summary>
    [Serializable]
    public class Skill
    {
        public CharacterBase parent;
        public DamageInfo damageInfo;
        public string skillName;
        public SkillData skillData;

        public void Execute(List<CharacterBase> targets)
        {
            foreach (var target in targets)
            {
                Debug.Log($"Executing skill on {target.Data.Name}");
                foreach (var d in skillData.DamageData)
                {
                    var dI = MakeDamageInfo(d, target, d.amountAttribute.HasFlag(AmountAttribute.Magic), d.element);
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
            return d;
        }
        public Skill(SkillData data, CharacterBase parent)
        {
            skillData = data;
            skillName = data.Name;
            this.parent = parent;
        }
    }
}