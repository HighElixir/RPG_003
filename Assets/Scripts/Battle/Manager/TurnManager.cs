using Cysharp.Threading.Tasks;
using RPG_003.Battle.Factions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private List<Unit> _turnActors = new List<Unit>();

        // === Action & Callback ===
        public Action<Unit> OnExecuteTurn;
        private CancellationTokenSource _tokenSource = new();

        // === Constructor ===
        public TurnManager(BattleManager parent)
        {
            _parent = parent;
        }

        // === Public ===
        public async UniTask ProcessTurn()
        {
            if (!_parent.IsBattleContinue) return;
            await _parent.WaitForPause();
            if (_turnActors.Count > 0)
            {
                while (_turnActors.Count > 0)
                {
                    if (!_parent.IsBattleContinue) return;
                    await _parent.WaitForPause();
                    var next = _turnActors.First();
                    await ExecuteTurn(next);
                }
            }

            // 最小の行動値に合わせて全体を減少
            var all = _parent.GetCharacters();
            if (all.Count == 0) return;
            int delta = all.Min(c => c.BehaviorIntervalCount.CurrentAmount);
            //Debug.Log($"[TurnManager] Processing turn: advancing all by {delta}");

            if (delta > 0)
            {
                foreach (var c in all)
                {
                    c.BehaviorIntervalCount.Process(delta);
                    //Debug.Log($"[TurnManager] Character {c.Data.Name}'s new intervalCount is: {c.BehaviorIntervalCount.CurrentAmount}.");
                }
            }
            GraphicalManager.instance.IndicatorUI.UpdateUI(_parent.GetCharacters());

            // 行動値が0のunitを全て行動待ちリストに追加
            var ready = all.Where(c => c.BehaviorIntervalCount.IsReady).OrderByDescending(c => c.BehaviorIntervalCount.Speed);
            _turnActors.AddRange(ready);
            _ = ProcessTurn();
        }

        public void RemoveCharacter(Unit character)
        {
            _turnActors.Remove(character);
        }

        public void Reset()
        {
            _tokenSource.Cancel();
            _turnActors.Clear();
        }
        // === Private ===
        private async UniTask ExecuteTurn(Unit actor, bool instantStart = false)
        {
            if (actor == null) return;
            if (!instantStart) RemoveToTurnActors(actor);
            OnExecuteTurn?.Invoke(actor);
            GraphicalManager.instance.BattleLog.Add($"<color=#{ColorUtility.ToHtmlStringRGB(actor.Faction.FactionToColor())}>{actor.Data.Name}</color>のターン！", BattleLog.IconType.Normal);
            await actor.ExecuteTurn(instantStart, _tokenSource.Token);
        }
        private void RemoveToTurnActors(Unit character)
        {
            _turnActors.Remove(character);
            character.BehaviorIntervalCount.Reset();
            GraphicalManager.instance.IndicatorUI.UpdateUI(_parent.GetCharacters());
        }
    }
}
