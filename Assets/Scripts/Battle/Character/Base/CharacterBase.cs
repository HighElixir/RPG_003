using HighElixir.StateMachine;
using RPG_001.Battle.Behaviour;
using UnityEngine;

namespace RPG_001.Battle.Characters
{
    public class CharacterBase : MonoBehaviour, ICharacter
    {
        private CharacterData _characterData; // Reference to the character's data
        private IStatusManager _statusManager; // Reference to the character's status manager
        private ICharacterBehaviour _characterBehaviour; // Reference to the character's behaviour
        private SpeedController _speedController; // Reference to the speed controller

        public IStatusManager StatusManager => _statusManager; // Property to access the status manager
        public CharacterData Data => _characterData; // Property to access character data
        public ICharacterBehaviour Behaviour => _characterBehaviour;
        public SpeedController SpeedController => _speedController; // Property to access the speed controller
        public bool IsAlive { get; private set; } = true; // Property to check if the character is alive
        // Method to initialize the character
        public void Initialize(CharacterData data, IStatusManager manager, ICharacterBehaviour characterBehaviour)
        {
            _statusManager = manager; // Set the status manager
            _characterData = data; // Store the character data
            _characterBehaviour = characterBehaviour; // Set the character's behaviour
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
            {
                _statusManager.TakeDamage(damage); // Delegate damage handling to the status manager
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (!IsAlive)
            {
                enabled = false; // Disable this MonoBehaviour to stop further updates
            }
        }

        public void TurnBehaviour()
        {
            if (IsAlive)
            {
                Debug.Log($"{gameObject.name} is taking its turn.");
                _characterBehaviour.TurnBehaviour(); // Call the character's behaviour for its turn
            }
            else
            {
                Debug.Log($"{gameObject.name} is dead and cannot take a turn.");
            }
        }
        public void NotifyDeath()
        {
            IsAlive = false; // Set the character as dead
            // Additional logic for notifying other systems about the character's death can be added here
            Debug.Log($"{gameObject.name} has been notified of death.");
        }
    }
}