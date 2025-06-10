using RPG_003.Battle.Characters;
using RPG_003.Battle.Characters.Player;
using RPG_003.Battle.Skills;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Battle.Behaviour
{
    public class PlayerBehaviour : ICharacterBehaviour
    {
        private Player _parent;
        private SelectTarget _target;
        private SkillButtonManager _SkillButtonManager;
        private bool _finishedTurn = false;
        public void Initialize(ICharacter parent, BattleManager battleManager)
        {
            _parent = parent as Player;
            var bm = battleManager as BattleManager;
            _target = bm.SelectTarget;
            _SkillButtonManager = bm.SkillButtonManager;
            parent.OnDeath += OnDeath;
        }

        public IEnumerator TurnBehaviour(bool instant = false)
        {
            _finishedTurn = false;

            _SkillButtonManager.CreateButtons(_parent.Skills, OnSkillSelected);
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
            _SkillButtonManager.ReleaseButtons();
            _target.ShowTargets(skill, OnTargetSelected, OnCanceledSelectingTarget);
        }

        private void OnCanceledSelectingTarget()
        {
            _SkillButtonManager.CreateButtons(_parent.Skills, OnSkillSelected);
        }
        private void OnTargetSelected(List<CharacterBase> targets, Skill skill)
        {
            UseSkill(skill, targets);
        }

        private void UseSkill(Skill skill, List<CharacterBase> targets)
        {
            if (targets.Count > 0)
            {
                skill.Execute(targets);
            }
            else
            {
                Debug.LogWarning("No valid targets selected for the skill.");
            }
            _finishedTurn = true;
        }
    }
}