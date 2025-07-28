using Cysharp.Threading.Tasks;
using RPG_003.Battle.Factions;
using System.Threading;
using UnityEngine;

namespace RPG_003.Battle.Behaviour
{
    public class AIBehavior : ICharacterBehaviour
    {
        private ISkillBehaviour _skill;
        private TargetSelectHelper _TargetSelectHelper;

        public ICharacterBehaviour Initialize(BattleManager battleManager)
        {
            _TargetSelectHelper = new TargetSelectHelper(battleManager);
            return this;
        }
        public AIBehavior SetSkill(ISkillBehaviour behaviour)
        {
            _skill = behaviour;
            return this;
        }
        public async UniTask TurnBehaviour(Unit parent, CancellationToken token, bool instant = false)
        {
            // --- スキル選択 ---
            var chosenSkill = _skill.GetSkill(parent);

            // --- ターゲット選択 ---
            var targets = _TargetSelectHelper.SelectRandomTargets(chosenSkill, true);
            string debug = "targets";
            foreach (var target in targets)
            {
                debug += "\n" + target.name;
            }
            Debug.Log(debug);
            if (targets.Count <= 0) return;
            // --- ダメージ計算＆適用 ---
            await chosenSkill.Convert(parent).Execute(targets.AsTargetInfo(), true);
            await UniTask.Delay(1000);
        }
        public virtual void OnDeath(Unit unit)
        {
            Debug.Log(unit.Data.Name + "をゲームから削除");
            unit.BattleManager.RemoveCharacter(unit);
        }
    }
}
