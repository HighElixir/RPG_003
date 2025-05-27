using HighElixir.StateMachine;
using RPG_001.Battle.Behaviour;
using System;

namespace RPG_001.Battle.Characters
{
    public interface ICharacter
    {
        CharacterData Data { get; } // Property to access character data
        ICharacterBehaviour Behaviour { get; } // Property to access character behaviour
        IStatusManager StatusManager { get; } // Property to access status manager
        SpeedController SpeedController { get; } // Property to access speed controller
        bool IsAlive { get; } // Property to check if the character is alive
        void Initialize(CharacterData data, IStatusManager manager, ICharacterBehaviour characterBehaviour); // Method to initialize the character
        void TakeDamage(DamageInfo damage); // Method to apply damage to the character

        // Notifies
        void NotifyDeath();
    }
}