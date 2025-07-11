using Cysharp.Threading.Tasks;
using RPG_003.Battle.Factions;
using System.Threading;
using UnityEngine;

namespace RPG_003.Battle.Behaviour
{
    public class AIBehavior : ICharacterBehaviour
    {
        private ISkillBehaviour _skill;
        private Unit _parent;
        private TargetSelectHelper _TargetSelectHelper;

        public ICharacterBehaviour Initialize(Unit parent, BattleManager battleManager)
        {
            _parent = parent;
            _TargetSelectHelper = new TargetSelectHelper(battleManager);
            parent.OnDeath += OnDeath;
            return this;
        }
        public AIBehavior SetSkill(ISkillBehaviour behaviour)
        {
            _skill = behaviour;
            return this;
        }
        public async UniTask TurnBehaviour(CancellationToken token, bool instant = false)
        {
            // --- スキル選択 ---
            var chosenSkill = _skill.GetSkill(_parent);

            // --- ターゲット選択 ---
            var faction = chosenSkill.skillDataInBattle.Target.GetReverse();
            var count = chosenSkill.skillDataInBattle.TargetCount;
            var canSame = chosenSkill.skillDataInBattle.CanSelectSameTarget;
            var targets = _TargetSelectHelper.SelectRandomTargets(faction, count, canSame);
            foreach (var target in targets)
            {
                Debug.Log($"Target : {target.Data.Name}");
            }
            if (targets.Count <= 0) Debug.Log("Can't find Targets.");
            // --- ダメージ計算＆適用 ---
            await chosenSkill.Execute(targets.AsTargetInfo(), true);
            await UniTask.Delay(1000);
        }
        public virtual void OnDeath(Unit dead)
        {
            Debug.Log(dead.Data.Name + "をゲームから削除");
            _parent.BattleManager.RemoveCharacter(dead as Unit);
        }
    }
}
