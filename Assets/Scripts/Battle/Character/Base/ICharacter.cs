using RPG_003.Battle.Behaviour;
using System;
using System.Collections;

namespace RPG_003.Battle.Characters
{
    public interface ICharacter
    {
        CharacterData Data { get; } // Property to access character data
        IStatusManager StatusManager { get; } // Property to access status manager
        BehaviorIntervalCount BehaviorIntervalCount { get; } // Property to access speed controller
        bool IsAlive { get; } // Property to check if the character is alive
        CharacterPosition Position { get; } // Property to get the character's position

        Action<ICharacter> OnDeath { get; set; } // Action to be called when the character dies
        IBattleManager BattleManager { get; } // Property to access the battle manager
        void Initialize(CharacterData data, IStatusManager statusManager, ICharacterBehaviour characterBehaviour, IBattleManager battleManager, IntervalIndicator indicator); // Method to initialize the character
        void TakeDamage(DamageInfo damage); // Method to apply damage to the character
        IEnumerator TurnBehaviour(bool instant = false); // Method to handle character's turn behaviour

        // Notifies
        void NotifyDeath();
        void NotifyTurnEnd();
    }
}