using RPG_003.Battle.Behaviour;
using System;
using System.Collections;
using UnityEngine;

namespace RPG_003.Battle.Characters
{
    public interface ICharacter
    {
        BattleManager BattleManager { get; }
        IStatusManager StatusManager { get; }
        BehaviorIntervalCount BehaviorIntervalCount { get; }
        CharacterData Data { get; }
        CharacterPosition Position { get; }
        bool IsAlive { get; }
        Action<ICharacter> OnDeath { get; set; }
        void Initialize(CharacterData data, IStatusManager statusManager, ICharacterBehaviour characterBehaviour, BattleManager battleManager);
        void TakeDamage(DamageInfo damage);
        void SetIcon(Sprite sprite);
        IEnumerator TurnBehaviour(bool instant = false);

        // Notifies
        void NotifyDeath();
        void NotifyTurnEnd();
    }
}