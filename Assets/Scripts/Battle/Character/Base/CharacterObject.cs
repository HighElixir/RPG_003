using RPG_003.Battle.Behaviour;
using RPG_003.Status;
using System;
using System.Collections;
using UnityEngine;

namespace RPG_003.Battle
{
    public class CharacterObject : MonoBehaviour
    {
        // === Reference ===
        private StatusManager _statusManager;
        private ICharacterBehaviour _characterBehaviour;
        private BehaviorIntervalCount _BehaviorIntervalCount;
        private BattleManager _battleManager;

        // === Data ===
        private CharacterData _characterData;

        // === Action & Callback ===
        public Action<CharacterObject> OnDeath { get; set; }

        // === Property ===
        public StatusManager StatusManager => _statusManager;
        public BehaviorIntervalCount BehaviorIntervalCount => _BehaviorIntervalCount;
        public BattleManager BattleManager => _battleManager;
        public virtual CharacterData Data => _characterData;
        public CharacterPosition Position { get; set; }
        public bool IsAlive { get; private set; } = true;
        public Sprite Icon { get; set; }
        public Sprite FullBody { get; set; }

        // === Public Methode ===
        public void Initialize(CharacterData data, StatusManager statusManager, ICharacterBehaviour characterBehaviour, BattleManager battleManager)
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
            else
                Debug.LogWarning($"{gameObject.name} is already dead and cannot take damage.");
        }
        public void TakeHeal(DamageInfo info)
        {
            if (IsAlive)
                _statusManager.TakeHeal(info);
            else
                Debug.LogWarning($"{gameObject.name} is already dead and cannot be healed.");
        }
        public void Revive(DamageInfo info)
        {
            if (!IsAlive)
            {
                _statusManager.TakeHeal(info);
                IsAlive = true;
                Debug.Log($"{gameObject.name} has been revived.");
            }
            else
            {
                Debug.LogWarning($"{gameObject.name} is already alive and cannot be revived.");
            }
        }
        public void SetIcon(Sprite icon)
        {
            Icon = icon;
            GetComponent<SpriteRenderer>().sprite = icon;
        }
        public void SetFullBody(Sprite body)
        {
            FullBody = body;
        }
        public void Release()
        {
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
            _battleManager.OnDeath(this);
        }

        public virtual void NotifyTurnEnd()
        {
            Debug.Log($"{gameObject.name} has ended its turn.");
            BattleManager.FinishTurn(this);
        }
        // === Protected Methode ===
        protected virtual void InitializeClass()
        {
            // CharacterObjectが持つクラスの初期化を行う
            _characterBehaviour.Initialize(this, _battleManager);
            _statusManager.Initialize(this, _characterData);
            OnDeath += (chara) =>
            {
                IsAlive = false;
            };
            _BehaviorIntervalCount.Initialize(_statusManager.GetStatusAmount(StatusAttribute.SPD));
        }

        // === Unity Lifecycle ===
    }
}