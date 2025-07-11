using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace RPG_003.Battle.Behaviour
{
    public class PlayerBehaviour : ICharacterBehaviour
    {
        private Unit _parent;
        private TargetSelector _target;
        private SkillSelector _SkillSelector;

        private Skill _chosen;
        public ICharacterBehaviour Initialize(Unit parent, BattleManager battleManager)
        {
            _parent = parent;
            var bm = battleManager;
            _target = bm.TargetSelector;
            _SkillSelector = bm.SkillSelector;
            parent.OnDeath += OnDeath;
            return this;
        }

        public async UniTask TurnBehaviour(CancellationToken token, bool instant = false)
        {
            _chosen = null;
            //Debug.Log($"Turn Start : actor is {_parent.Data.Name}");
            bool selecting = true;
            TargetInfo target = null;
            while (selecting)
            {
#if UNITY_EDITOR
                GraphicalManager.instance.BattleLog.Add("[TurnBehaviour] : Choose Skill", BattleLog.IconType.Normal);
#endif
                // キャンセルされたらここで早期リターン
                //if (token.IsCancellationRequested) return;
                var selected = await _SkillSelector.InvokeSelector(_parent.Skills);
                if (selected == null)
                {
                    Debug.Log("選べるスキルがないよ！");
                    return;
                }
                var res = await _target.ShowTargets(selected);
                _chosen = selected;
                target = res.info;
                //Debug.Log(res.info.ToString() + ", " + res.hasCanceled.ToString());
                selecting = res.hasCanceled;
            }
            await _chosen.Execute(target);
        }

        public void OnDeath(Unit character)
        {
        }
    }
}