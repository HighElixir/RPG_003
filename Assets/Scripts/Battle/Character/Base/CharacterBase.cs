using RPG_001.Battle.Behaviour;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using System;

namespace RPG_001.Battle.Characters
{
    public class CharacterBase : SerializedMonoBehaviour, ICharacter
    {
        private CharacterData _characterData; // Reference to the character's data
        private IStatusManager _statusManager; // Reference to the character's status manager
        private ICharacterBehaviour _characterBehaviour; // Reference to the character's behaviour
        private SpeedController _speedController; // Reference to the speed controller
        private IBattleManager _battleManager;

        public Action<ICharacter> OnDeath { get; set; } // Action to be called when the character dies
        public IStatusManager StatusManager => _statusManager; // Property to access the status manager
        public CharacterData Data => _characterData; // Property to access character data
        public SpeedController SpeedController => _speedController; // Property to access the speed controller
        public IBattleManager BattleManager => _battleManager; // Property to access the battle manager, assuming status manager implements IBattleManager
        public bool IsAlive { get; private set; } = true; // Property to check if the character is alive
        public CharacterPosition Position { get; set; } // Property to get the character's position
        // Method to initialize the character
        public void Initialize(CharacterData data, IStatusManager statusManager, ICharacterBehaviour characterBehaviour, IBattleManager battleManager)
        {
            _statusManager = statusManager; // Set the status manager
            _characterData = data; // Store the character data
            _characterBehaviour = characterBehaviour; // Set the character's behaviour
            _battleManager = battleManager; // Set the battle manager
            _speedController = new SpeedController(); // Initialize the speed controller
            InitializeClass(); // Call the method to initialize the class
            // Initialization logic here
            IsAlive = true;
        }

        protected virtual void InitializeClass()
        {
            _characterBehaviour.Initialize(this); // Initialize the character's behaviour with this character instance
            _statusManager.Initialize(this, _characterData);
            _speedController.Initialize(_statusManager.GetStatusAmount(StatusAttribute.SPD)); // Initialize the speed controller with the speed status
        }
        // Method to apply damage to the character
        public void TakeDamage(DamageInfo damage)
        {
            if (IsAlive)
                _statusManager.TakeDamage(damage);
        }

        public IEnumerator TurnBehaviour(bool instant = false)
        {
            yield return _characterBehaviour.TurnBehaviour(instant); // Call the character's behaviour for its turn
            NotifyTurnEnd();
        }
        public virtual void NotifyDeath()
        {
            // Additional logic for notifying other systems about the character's death can be added here
            Debug.Log($"{gameObject.name} has been notified of death.");
            IsAlive = false; // Set the character as dead
            OnDeath?.Invoke(this); // Invoke the death action if any subscribers are present
        }

        public virtual void NotifyTurnEnd()
        {
            Debug.Log($"{gameObject.name} has ended its turn.");
            BattleManager.FinishTurn(this); // Notify the battle manager that the character's turn has ended
        }
    }
}