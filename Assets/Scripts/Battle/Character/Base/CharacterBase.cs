using RPG_003.Battle.Behaviour;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using System;

namespace RPG_003.Battle.Characters
{
    public class CharacterBase : SerializedMonoBehaviour, ICharacter
    {
        private CharacterData _characterData;
        private IStatusManager _statusManager;
        private ICharacterBehaviour _characterBehaviour;
        private BehaviorIntervalCount _BehaviorIntervalCount;
        private IBattleManager _battleManager;

        public Action<ICharacter> OnDeath { get; set; }

        public IStatusManager StatusManager => _statusManager;
        public CharacterData Data => _characterData;
        public BehaviorIntervalCount BehaviorIntervalCount => _BehaviorIntervalCount;
        public IBattleManager BattleManager => _battleManager;
        public bool IsAlive { get; private set; } = true;
        public CharacterPosition Position { get; set; }
        // Method to initialize the character
        public void Initialize(CharacterData data, IStatusManager statusManager, ICharacterBehaviour characterBehaviour, IBattleManager battleManager, IntervalIndicator IntervalIndicator)
        {
            _statusManager = statusManager;
            _characterData = data;
            _characterBehaviour = characterBehaviour;
            _battleManager = battleManager;
            _BehaviorIntervalCount = new BehaviorIntervalCount();
            InitializeClass(IntervalIndicator);
            // Initialization logic here
            IsAlive = true;
        }

        protected virtual void InitializeClass(IntervalIndicator indicator)
        {
            _characterBehaviour.Initialize(this); 
            _statusManager.Initialize(this, _characterData);
            _BehaviorIntervalCount.Initialize(_statusManager.GetStatusAmount(StatusAttribute.SPD), indicator); 
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