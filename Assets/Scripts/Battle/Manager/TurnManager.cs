using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RPG_003.Battle
{
    /// <summary>
    /// ターン進行ロジック（インターバル計算・ターンキューの管理・次のアクターの決定）をまとめたクラス
    /// </summary>
    public class TurnManager
    {
        // === Reference ===
        private BattleManager _parent;

        // === Data ===
        private List<CharacterObject> _turnActors = new List<CharacterObject>();

        // === Action & Callback ===
        public Action<CharacterObject> OnExecuteTurn;

        private Dictionary<CharacterObject, Coroutine> _coroutines = new();
        private MonoBehaviour _coroutineRunner;

        // === Constructor ===
        public TurnManager(BattleManager parent, MonoBehaviour coroutineRunner)
        {
            _parent = parent;
            _coroutineRunner = coroutineRunner;
        }

        // === Public ===

        /// <summary>
        /// BattleManager.ProcessTurn で呼ばれる。
        /// </summary>
        public void ProcessTurn()
        {
            if (_turnActors.Count > 0)
            {
                var next = _turnActors[0];
                ExecuteTurn(next);
                return;
            }

            var all = _parent.GetCharacters();
            if (all.Count == 0) return;

            int delta = all.Min(c => c.BehaviorIntervalCount.CurrentAmount);
            Debug.Log($"[TurnManager] Processing turn: advancing all by {delta}");

            if (delta > 0)
            {
                foreach (var c in all)
                {
                    c.BehaviorIntervalCount.Process(delta);
                    Debug.Log($"[TurnManager] Character {c.Data.Name}'s new intervalCount is: {c.BehaviorIntervalCount.CurrentAmount}.");
                }
            }
            _parent.IndicatorUIBuilder.UpdateUI(_parent.GetCharacters());
            var ready = all.Where(c => c.BehaviorIntervalCount.IsReady).OrderByDescending(c => c.BehaviorIntervalCount.Speed);
            _turnActors.AddRange(ready);
#if UNITY_EDITOR       
            if (_turnActors.Count != 0)
            {
                var sb = new StringBuilder("[TurnManager] _turnActors:");
                foreach (var turnActor in _turnActors)
                {
                    sb.Append($"\n {turnActor.Data.Name},");
                }
                Debug.Log(sb.ToString());
            }
#endif
            _parent.ProcessTurn();
        }

        /// <summary>
        /// BattleManager から Turn 終了通知が来たら呼ぶ
        /// </summary>
        public void FinishTurn()
        {
            if (_turnActors.Count > 0)
            {
                var nextActor = _turnActors[0];
                ExecuteTurn(nextActor);
            }
            else
            {
                ProcessTurn();
            }
        }
        public void RemoveCharacter(CharacterObject character)
        {
            _turnActors.Remove(character);
            if (_coroutines.ContainsKey(character))
                _coroutineRunner.StopCoroutine(_coroutines[character]);

        }

        public void Reset()
        {
            foreach (var c in _coroutines.Values)
            {
                _coroutineRunner.StopCoroutine(c);
            }
            _turnActors.Clear();
        }
        // === Private ===
        private void ExecuteTurn(CharacterObject actor, bool instantStart = false)
        {
            if (actor == null) return;
            if (!instantStart) Remove(actor);
            OnExecuteTurn?.Invoke(actor);
            _coroutines[actor] = _coroutineRunner.StartCoroutine(actor.TurnBehaviour());
        }
        private void Remove(CharacterObject character)
        {
            _turnActors.Remove(character);
            character.BehaviorIntervalCount.Reset();
            _parent.IndicatorUIBuilder.UpdateUI(_parent.GetCharacters());
        }
    }
}
