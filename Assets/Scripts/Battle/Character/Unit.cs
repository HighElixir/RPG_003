using Cysharp.Threading.Tasks;
using RPG_003.Battle.Behaviour;
using RPG_003.Battle.Factions;
using RPG_003.Status;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace RPG_003.Battle
{
    public class Unit : MonoBehaviour
    {
        // === Reference ===
        private StatusManager _statusManager;
        private ICharacterBehaviour _characterBehaviour;
        private BehaviorIntervalCount _BehaviorIntervalCount;
        private BattleManager _battleManager;

        // === Data ===
        private StatusData _statusData;
        private List<Skill> _skills = new();

        // === Action & Callback ===
        public Action<Unit> OnDeath { get; set; }

        // === Property ===
        internal BattleManager BattleManager => _battleManager;
        public StatusManager StatusManager => _statusManager;
        public BehaviorIntervalCount BehaviorIntervalCount => _BehaviorIntervalCount;
        public ICharacterBehaviour Behaviour => _characterBehaviour;
        public virtual StatusData Data => _statusData;
        public virtual List<Skill> Skills => _skills;
        public CharacterPosition Position { get; set; }
        public Faction Faction => Position.PositionToFaction();
        public bool IsAlive { get; private set; } = true;
        public Sprite Icon { get; set; }
        public Sprite FullBody { get; set; }

        // === Public Methode ===
        public void TakeDamage(DamageInfo info)
        {
            if (IsAlive)
            {
                var isDead = _statusManager.TakeDamage(info);
                GraphicalManager.instance.BattleLog.Add(BattleLog.TakeDamage(info), BattleLog.IconType.Negative);
                GetComponent<UnitUI>().Shake();
                if (isDead)
                    NotifyDeath();
            }
        }
        public void TakeHeal(DamageInfo info)
        {
            if (IsAlive)
            {
                _statusManager.TakeHeal(info);
            }
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
        public void Release()
        {
        }
        #region
        internal Unit Initialize(BattleManager battleManager)
        {
            _battleManager = battleManager;
            _BehaviorIntervalCount = new BehaviorIntervalCount();
            OnDeath += (chara) =>
            {
                IsAlive = false;
            };
            IsAlive = true;
            return this;
        }
        public Unit SetStatus(StatusData data)
        {
            _statusData = data;
            SetStatusManager(new StatusManager(this, data));
            return this;
        }
        public Unit SetBehaivior(ICharacterBehaviour behaviour)
        {
            _characterBehaviour = behaviour;
            return this;
        }
        public Unit SetStatusManager(StatusManager manager)
        {
            _statusManager = manager;
            _statusManager.Initialize(this, _statusData);
            _BehaviorIntervalCount.Initialize(_statusManager.GetStatusAmount(StatusAttribute.SPD));
            return this;
        }
        public Unit SetIcon(Sprite icon)
        {
            Icon = icon;
            GetComponent<SpriteRenderer>().sprite = icon;
            return this;
        }
        public Unit SetFullBody(Sprite body)
        {
            FullBody = body;
            return this;
        }
        public Unit SetSkills(List<Skill> skills)
        {
            _skills = new(skills);
            return this;
        }
        #endregion

        // === 各状況で呼ばれるメソッド ===
        public virtual void NotifyDeath()
        {
            _battleManager.OnDeath(this);
        }

        // === Protected Methode ===

        // === Unity Lifecycle ===
    }
}