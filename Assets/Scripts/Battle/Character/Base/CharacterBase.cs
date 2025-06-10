using RPG_003.Battle.Behaviour;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;

namespace RPG_003.Battle.Characters
{
    public class CharacterBase : MonoBehaviour, ICharacter
    {
        // === Reference ===
        private IStatusManager _statusManager;
        private ICharacterBehaviour _characterBehaviour;
        private BehaviorIntervalCount _BehaviorIntervalCount;
        private BattleManager _battleManager;

        // === Data ===
        private CharacterData _characterData;

        // === Action & Callback ===
        public Action<ICharacter> OnDeath { get; set; }

        // === Property ===
        public IStatusManager StatusManager => _statusManager;
        public BehaviorIntervalCount BehaviorIntervalCount => _BehaviorIntervalCount;
        public BattleManager BattleManager => _battleManager;
        public virtual CharacterData Data => _characterData;
        public CharacterPosition Position { get; set; }
        public bool IsAlive { get; private set; } = true;

        // === Public Methode ===
        public void Initialize(CharacterData data, IStatusManager statusManager, ICharacterBehaviour characterBehaviour, BattleManager battleManager)
        {
            _statusManager = statusManager;
            _characterData = data;
            _characterBehaviour = characterBehaviour;
            _battleManager = battleManager;
            _BehaviorIntervalCount = new BehaviorIntervalCount();
            InitializeClass();
            // Initialization logic here
            IsAlive = true;
        }

        public void TakeDamage(DamageInfo damage)
        {
            if (IsAlive)
                _statusManager.TakeDamage(damage);
        }

        public void SetIcon(Sprite icon)
        {
            GetComponent<SpriteRenderer>().sprite = icon;
        }
        // コルーチンで管理
        public IEnumerator TurnBehaviour(bool instant = false)
        {
            yield return _characterBehaviour.TurnBehaviour(instant); // Call the character's behaviour for its turn
            NotifyTurnEnd();
        }

        // === 各状況で呼ばれるメソッド ===
        public virtual void NotifyDeath()
        {
            Debug.Log($"{gameObject.name} has been notified of death.");
            IsAlive = false;
            OnDeath?.Invoke(this);
        }

        public virtual void NotifyTurnEnd()
        {
            Debug.Log($"{gameObject.name} has ended its turn.");
            BattleManager.FinishTurn(this);
        }

        // === Protected Methode ===
        protected virtual void InitializeClass()
        {
            // CharacterBaseが持つクラスの初期化を行う
            _characterBehaviour.Initialize(this, _battleManager);
            _statusManager.Initialize(this, _characterData);
            _BehaviorIntervalCount.Initialize(_statusManager.GetStatusAmount(StatusAttribute.SPD));
        }

        protected virtual void Update() { }
    }
}