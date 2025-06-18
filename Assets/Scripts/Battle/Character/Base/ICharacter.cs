using RPG_003.Battle.Behaviour;
using RPG_003.Status;
using System;
using System.Collections;
using UnityEngine;

namespace RPG_003.Battle.Characters
{
    public interface ICharacter
    {
        BattleManager BattleManager { get; }
        StatusManager StatusManager { get; }
        BehaviorIntervalCount BehaviorIntervalCount { get; }
        CharacterData Data { get; }
        CharacterPosition Position { get; }
        bool IsAlive { get; }
        Action<ICharacter> OnDeath { get; set; }
        void Initialize(CharacterData data, StatusManager statusManager, ICharacterBehaviour characterBehaviour, BattleManager battleManager);
        void TakeDamage(DamageInfo damage);
        void TakeHeal(DamageInfo damage);
        void SetIcon(Sprite sprite);
        IEnumerator TurnBehaviour(bool instant = false);

        // Notifies
        void NotifyDeath();
        void NotifyTurnEnd();
    }
}