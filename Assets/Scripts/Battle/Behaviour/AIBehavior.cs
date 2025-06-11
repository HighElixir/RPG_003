using RPG_003.Battle.Characters;
using RPG_003.Battle.Characters.Enemy;
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
            if (!instant)
                yield return new WaitForSeconds(0.5f);

            // --- 2. スキル選択 ---
            var chosenSkill = _EnemyBehaviorData.GetSkill();

            // --- 3. ターゲット選択 ---
            var target = _TargetSelectHelper.SelectRandomTarget(chosenSkill.GetTargetFaction());
            Debug.Log($"{_parent.Data.Name} のターン: {chosenSkill.skillName} を使用。ターゲット: {target.Data.Name}");
            yield return new WaitForSeconds(1.5f);

            // --- 4. ダメージ計算＆適用 ---
            var str = _parent.StatusManager.GetStatusAmount(StatusAttribute.STR).ChangedMax;
            var intel = _parent.StatusManager.GetStatusAmount(StatusAttribute.INT).ChangedMax;
            float dmg = str * chosenSkill.damage_with_str + intel * chosenSkill.damage_with_int;
            _parent.BattleManager.ApplyDamage(new DamageInfo(_parent, target, dmg));
        }

        public void OnDeath(ICharacter dead)
        {
            _parent.BattleManager.RemoveCharacter(dead as CharacterBase);
        }
    }
}
