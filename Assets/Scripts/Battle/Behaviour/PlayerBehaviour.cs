using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Battle.Behaviour
{
    public class PlayerBehaviour : ICharacterBehaviour
    {
        private Player _parent;
        private TargetSelector _target;
        private SkillSelector _SkillSelector;

        private Skill _chosen;
        private List<CharacterObject> _targets = new List<CharacterObject>();
        private bool _isfinishedChosen = false;
        private bool _finished = false; // 選択できるスキルがなかった場合、trueにすることで早めにターンを終える
        public void Initialize(CharacterObject parent, BattleManager battleManager)
        {
            _parent = parent as Player;
            var bm = battleManager;
            _target = bm.TargetSelector;
            _SkillSelector = bm.SkillSelector;
            parent.OnDeath += OnDeath;
        }

        public IEnumerator TurnBehaviour(bool instant = false)
        {
            _isfinishedChosen = false;
            _chosen = null;
            _targets.Clear();

            Debug.Log($"Turn Start : actor is {_parent.Data.Name}");
            yield return null;
            _SkillSelector.InvokeSelector(_parent.Skills, OnSkillSelected);
            yield return new WaitUntil(() => _isfinishedChosen == true);
            if (_finished)yield break;
            yield return UseSkill(_chosen, _targets); // Simulate a delay for the turn behaviour
        }

        public void OnDeath(CharacterObject character)
        {
            // Additional logic can be added here, such as updating UI or triggering events
            Debug.Log(character.Data.Name + "は死亡した");
        }

        private void OnSkillSelected(Skill skill)
        {
            if (skill == null)
            {
                _isfinishedChosen = true;
                _finished = true;
            }
            _target.ShowTargets(skill,
                (targets, skill) =>
                {
                    _chosen = skill;
                    _targets = targets;
                    _isfinishedChosen = true;
                },
                () =>
                {
                    _SkillSelector.InvokeSelector(_parent.Skills, OnSkillSelected);
                });
        }

        private IEnumerator UseSkill(Skill skill, List<CharacterObject> targets)
        {
            if (skill.skillDataInBattle.IsSelf)
                yield return skill.Execute(_parent);
            else if (targets.Count > 0)
            {
                yield return skill.Execute(targets);
            }
        }
    }
}