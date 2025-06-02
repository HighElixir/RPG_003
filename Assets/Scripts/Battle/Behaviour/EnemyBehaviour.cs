using System.Collections;
using System.Linq;
using UnityEngine;
using RPG_001.Battle.Characters;
using RPG_001.Battle.Characters.Enemy;

namespace RPG_001.Battle.Behaviour
{
    public class EnemyBehaviour : ICharacterBehaviour
    {
        private readonly EnemyBehaviorData _EnemyBehaviorData;
        private ICharacter _parent;
        private TargetSelecter _targetSelecter;

        public EnemyBehaviour(EnemyBehaviorData EnemyBehaviorData)
        {
            _EnemyBehaviorData = EnemyBehaviorData; // EnemyBehaviorData: 重み付きマップ定義 :contentReference[oaicite:2]{index=2}
        }

        public void Initialize(ICharacter parent)
        {
            _parent = parent;
            _targetSelecter = new TargetSelecter(_parent.BattleManager);
            parent.OnDeath += OnDeath;
        }

        public IEnumerator TurnBehaviour(bool instant = false)
        {
            if (!instant)
                yield return new WaitForSeconds(3.5f);

            // --- 2. スキル選択 ---
            var chosenSkill = _EnemyBehaviorData.GetSkill();

            // --- 3. ターゲット選択 ---
            var target = _targetSelecter.SelectRandomTarget(1);
            Debug.Log($"{_parent.Data.Name} のターン: {chosenSkill.skillName} を使用。ターゲット: {target.Data.Name}");
            yield return new WaitForSeconds(2.5f);

            // --- 4. ダメージ計算＆適用 ---
            var str = _parent.StatusManager.GetStatusAmount(StatusAttribute.STR).ChangedMax;
            var intel = _parent.StatusManager.GetStatusAmount(StatusAttribute.INT).ChangedMax;
            float dmg = str * chosenSkill.damage_with_str + intel * chosenSkill.damage_with_int;
            target.TakeDamage(new DamageInfo(_parent, target,dmg));
        }

        public void OnDeath(ICharacter dead)
        {
            Debug.Log($"{dead.Data.Name} が戦闘不能に！");
            _parent.BattleManager.RemoveCharacter(dead as CharacterBase);
        }
    }
}
