using RPG_003.Battle.Characters;
using RPG_003.Battle.Characters.Enemy;
using RPG_003.Status;
using RPG_003.Skills;
using System.Collections;
using UnityEngine;

namespace RPG_003.Battle.Behaviour
{
    public class AIBehavior : ICharacterBehaviour
    {
        private readonly EnemyBehaviorData _EnemyBehaviorData;
        private ICharacter _parent;
        private TargetSelectHelper _TargetSelectHelper;

        public AIBehavior(EnemyBehaviorData EnemyBehaviorData)
        {
            _EnemyBehaviorData = EnemyBehaviorData;
        }

        public void Initialize(ICharacter parent, BattleManager battleManager)
        {
            _parent = parent;
            _TargetSelectHelper = new TargetSelectHelper(battleManager);
            parent.OnDeath += OnDeath;
        }

        public IEnumerator TurnBehaviour(bool instant = false)
        {
            Debug.Log("!");
            if (!instant)
                yield return new WaitForSeconds(0.5f);

            // --- 2. スキル選択 ---
            var chosenSkill = _EnemyBehaviorData.GetSkill();

            // --- 3. ターゲット選択 ---
            var targets = _TargetSelectHelper.SelectRandomTargets(chosenSkill.GetTargetFaction(), chosenSkill.targetData.Count, chosenSkill.canSecanSelectSameTargetlect);
            yield return new WaitForSeconds(1.5f);

            // --- 4. ダメージ計算＆適用 ---
            foreach (var target in targets)
            {
                Debug.Log($"Executing skill on {target.Data.Name}");
                foreach (var d in chosenSkill.damageDatas)
                {
                    var dI = MakeDamageInfo(d, target, d.amountAttribute.HasFlag(AmountAttribute.Magic), d.element);
                    if (d.amountAttribute.HasFlag(AmountAttribute.Heal))
                        _parent.BattleManager.ApplyHeal(dI);
                    else
                        _parent.BattleManager.ApplyDamage(dI);
                }
            }
        }

        public DamageInfo MakeDamageInfo(DamageData data, CharacterBase target, bool isMagic, Elements element)
        {
            var d = new DamageInfo(_parent, target, 0);
            float damage = data.fixedAmount;
            float amount;
            switch (data.type)
            {
                case StatusAttribute.HP:
                    amount = _parent.StatusManager.HP;
                    break;
                case StatusAttribute.MaxHP:
                    amount = _parent.StatusManager.MaxHP;
                    break;
                case StatusAttribute.MP:
                    amount = _parent.StatusManager.GetStatusAmount(StatusAttribute.MP).currentAmount;
                    break;
                default:
                    amount = _parent.StatusManager.GetStatusAmount(data.type).ChangedMax;
                    break;
            }
            damage += amount * data.amount;
            d.Damage = damage;
            d.Elements = data.element;
            d.AmountAttribute = data.amountAttribute;
            return d;
        }
        public virtual void OnDeath(ICharacter dead)
        {
            Debug.Log(dead.Data.Name + "をゲームから削除");
            _parent.BattleManager.RemoveCharacter(dead as CharacterBase);
        }
    }
}
