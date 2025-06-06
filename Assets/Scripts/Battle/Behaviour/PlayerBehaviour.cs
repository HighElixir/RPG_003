using RPG_003.Battle.Characters;
using RPG_003.Battle.Characters.Player;
using RPG_003.Battle.Skills;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace RPG_003.Battle.Behaviour
{
    public class PlayerBehaviour : ICharacterBehaviour
    {
        private Player _parent;
        private SelectTarget _target;
        private MakeSkillButton _makeSkillButton;
        private bool _finishedTurn = false;
        public void Initialize(ICharacter parent, IBattleManager battleManager)
        {
            _parent = parent as Player;
            var bm = battleManager as BattleManager;
            _target = bm.SelectTarget;
            _makeSkillButton = bm.GetComponent<MakeSkillButton>();
            parent.OnDeath += OnDeath;
        }

        public IEnumerator TurnBehaviour(bool instant = false)
        {
            _finishedTurn = false;

            _makeSkillButton.CreateButtons(_parent.Skills, OnSkillSelected);
            yield return new WaitUntil(() => _finishedTurn == true); // Simulate a delay for the turn behaviour
        }

        public void OnDeath(ICharacter character)
        {
            // Handle player-specific death logic here
            Debug.Log($"{character.Data.Name} has died in battle.");
            // Additional logic can be added here, such as updating UI or triggering events
        }

        private void OnSkillSelected(Skill skill)
        {
            _makeSkillButton.ReleaseButtons();
            _target.ShowTargets(skill, OnTargetSelected, OnCanceledSelectingTarget);
        }

        private void OnCanceledSelectingTarget()
        {
            _makeSkillButton.CreateButtons(_parent.Skills, OnSkillSelected);
        }
        private void OnTargetSelected(List<ICharacter> targets, Skill skill)
        {
            UseSkill(skill, targets);
        }

        private void UseSkill(Skill skill, List<ICharacter> targets)
        {
            if (targets.Count > 0)
            {
                skill.Execute(targets);
            }
            else
            {
                Debug.LogWarning("No valid targets selected for the skill.");
            }
        }
    }
}