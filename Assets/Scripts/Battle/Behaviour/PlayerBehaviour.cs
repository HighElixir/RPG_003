using RPG_003.Battle.Characters;
using RPG_003.Battle.Characters.Player;
using RPG_003.Battle.Skills;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NUnit.Framework;

namespace RPG_003.Battle.Behaviour
{
    public class PlayerBehaviour : ICharacterBehaviour
    {
        private Player _parent;
        private SelectTarget _target;
        private SkillSelector _SkillSelector;

        private Skill _chosen;
        private List<CharacterBase> _targets = new List<CharacterBase>();
        private bool _isfinishedChosen = false;
        private bool _isfinishedTurn = false;
        public void Initialize(ICharacter parent, BattleManager battleManager)
        {
            _parent = parent as Player;
            var bm = battleManager;
            _target = bm.SelectTarget;
            _SkillSelector = bm.SkillSelector;
            parent.OnDeath += OnDeath;
        }

        public IEnumerator TurnBehaviour(bool instant = false)
        {
            _isfinishedTurn = false;
            _isfinishedChosen = false;
            _chosen = null;
            _targets.Clear();

            Debug.Log($"Turn Start : actor is {_parent.Data.Name}");
            yield return null;
            _SkillSelector.CreateButtons(_parent.Skills, OnSkillSelected);
            yield return new WaitUntil(() => _isfinishedChosen == true);
            UseSkill(_chosen, _targets);
            if (_chosen.skillDataInBattle.VFXData != null)
                yield return GraphicalManager.instance.EffectPlay(_chosen.skillDataInBattle.VFXData, _targets.ConvertAll<Vector2>((c) => { return c.transform.position; }));

            yield return new WaitUntil(() => _isfinishedTurn == true); // Simulate a delay for the turn behaviour
        }

        public void OnDeath(ICharacter character)
        {
            // Additional logic can be added here, such as updating UI or triggering events
        }

        private void OnSkillSelected(Skill skill)
        {
            _target.ShowTargets(skill, OnTargetSelected, OnCanceledSelectingTarget);
        }

        private void OnCanceledSelectingTarget()
        {
            _SkillSelector.CreateButtons(_parent.Skills, OnSkillSelected);
        }
        private void OnTargetSelected(List<CharacterBase> targets, Skill skill)
        {
            _chosen = skill;
            _targets = targets;
            _isfinishedChosen = true;
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
            _isfinishedTurn = true;
        }
    }
}