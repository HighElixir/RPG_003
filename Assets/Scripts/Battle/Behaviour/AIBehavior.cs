using RPG_003.Battle.Factions;
using System.Collections;
using UnityEngine;

namespace RPG_003.Battle.Behaviour
{
    public class AIBehavior : ICharacterBehaviour
    {
        private readonly EnemyBehaviorData _EnemyBehaviorData;
        private CharacterObject _parent;
        private TargetSelectHelper _TargetSelectHelper;

        public AIBehavior(EnemyBehaviorData EnemyBehaviorData)
        {
            _EnemyBehaviorData = EnemyBehaviorData;
        }

        public void Initialize(CharacterObject parent, BattleManager battleManager)
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
            var chosenSkill = _EnemyBehaviorData.GetSkill(_parent);

            // --- 3. ターゲット選択 ---
            var faction = chosenSkill.skillDataInBattle.Target.GetReverse();
            var count = chosenSkill.skillDataInBattle.TargetCount;
            var canSame = chosenSkill.skillDataInBattle.CanSelectSameTarget;
            var targets = _TargetSelectHelper.SelectRandomTargets(faction, count, canSame);
            yield return new WaitForSeconds(1.5f);
            foreach (var target in targets)
            {
                Debug.Log($"Target : {target.Data.Name}");
            }
            if (targets.Count <= 0) Debug.Log("Can't find Targets.");
            // --- 4. ダメージ計算＆適用 ---
            yield return chosenSkill.Execute(targets, true);
        }
        public virtual void OnDeath(CharacterObject dead)
        {
            Debug.Log(dead.Data.Name + "をゲームから削除");
            _parent.BattleManager.RemoveCharacter(dead as CharacterObject);
        }
    }
}
