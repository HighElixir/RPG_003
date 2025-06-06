using System;
using System.Collections.Generic;
using System.Linq;
using RPG_003.Battle.Characters;
using UnityEngine;

namespace RPG_003.Battle
{
    /// <summary>
    /// ターン進行ロジック（インターバル計算・ターンキューの管理・次のアクターの決定）をまとめたクラス
    /// </summary>
    public class TurnManager
    {
        private Dictionary<CharacterPosition, CharacterBase> _characterPositions;
        private List<CharacterBase> _turnActors = new List<CharacterBase>();
        private int _turnCount = 0;
        private MonoBehaviour _coroutineHost; // StartCoroutine を持ってる BattleManager を渡す想定

        public Action<CharacterBase> OnExecuteTurn;

        public TurnManager(
            Dictionary<CharacterPosition, CharacterBase> characterPositions,
            MonoBehaviour coroutineHost)
        {
            _characterPositions = characterPositions;
            _coroutineHost = coroutineHost;
        }

        /// <summary>
        /// BattleManager.ProcessTurn で呼ばれる。
        /// </summary>
        public void ProcessTurn()
        {
            if (_turnCount >= 100) return;

            var all = _characterPositions.Values.ToList();
            if (all.Count == 0) return;

            int delta = all.Min(c => c.BehaviorIntervalCount.CurrentAmount);
            Debug.Log($"[TurnManager] Processing turn: {_turnCount}, advancing all by {delta}");

            if (delta > 0)
            {
                foreach (var c in all)
                {
                    c.BehaviorIntervalCount.Process(delta);
                    Debug.Log($"[TurnManager] Character {c.Data.Name} advanced by {delta}, new speed: {c.BehaviorIntervalCount.CurrentAmount}");
                }
            }

            var ready = all.Where(c => c.BehaviorIntervalCount.IsReady)
                           .OrderByDescending(c => c.BehaviorIntervalCount.CurrentAmount);
            _turnActors.AddRange(ready);

            if (_turnActors.Count > 0)
            {
                var next = _turnActors.First();
                _turnActors.RemoveAt(0);
                ExecuteTurn(next);
            }
        }

        /// <summary>
        /// BattleManager から Turn 終了通知が来たら呼ぶ
        /// </summary>
        public void FinishTurn()
        {
            if (_turnActors.Count > 0)
            {
                var nextActor = _turnActors.First();
                _turnActors.RemoveAt(0);
                ExecuteTurn(nextActor);
            }
            else
            {
                _turnCount++;
                ProcessTurn();
            }
        }

        private void ExecuteTurn(CharacterBase actor, bool instantStart = false)
        {
            actor.BehaviorIntervalCount.Reset();
            // BattleManager の StartCoroutine を呼びたいので、OnExecuteTurn を通して BattleManager 側に委譲
            OnExecuteTurn?.Invoke(actor);
        }
    }
}
