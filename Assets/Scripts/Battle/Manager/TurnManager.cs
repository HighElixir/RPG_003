using RPG_003.Battle.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<CharacterBase> _turnActors = new List<CharacterBase>();
        private int _turnCount = 0;

        // === Action & Callback ===
        public Action<CharacterBase> OnExecuteTurn;

        // === Constructor ===
        public TurnManager(BattleManager parent)
        {
            _parent = parent;
        }

        // === Public Methode ===

        /// <summary>
        /// BattleManager.ProcessTurn で呼ばれる。
        /// </summary>
        public void ProcessTurn()
        {
            if (_turnCount >= 100) return;

            var all = _parent.GetCharacters();
            if (all.Count == 0) return;

            int delta = all.Min(c => c.BehaviorIntervalCount.CurrentAmount);
            Debug.Log($"[TurnManager] Processing turn: {_turnCount}, advancing all by {delta}");

            if (delta > 0)
            {
                foreach (var c in all)
                {
                    c.BehaviorIntervalCount.Process(delta);
                    Debug.Log($"[TurnManager] Character {c.Data.Name}'s new intervalCount is: {c.BehaviorIntervalCount.CurrentAmount}.");
                }
            }

            var ready = all.Where(c => c.BehaviorIntervalCount.IsReady)
                           .OrderByDescending(c => c.BehaviorIntervalCount.Speed);
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
#if !UNITY_EDITOR
                ProcessTurn();
#endif
            }
        }
        public void RemoveCharacter(CharacterBase character)
        {
            _parent.StopCoroutine(character.TurnBehaviour());
            _turnActors.Remove(character);
        }

        public void Reset()
        {
            _turnActors.Clear();
            _turnCount = 0;
        }
        // === PrivateMethode ===
        private void ExecuteTurn(CharacterBase actor, bool instantStart = false)
        {
            actor.BehaviorIntervalCount.Reset();
            // BattleManager の StartCoroutine を呼びたいので、OnExecuteTurn を通して BattleManager 側に委譲
            OnExecuteTurn?.Invoke(actor);
        }
    }
}
