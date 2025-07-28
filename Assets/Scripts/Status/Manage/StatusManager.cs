using RPG_003.Battle;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Status
{
    public class StatusManager
    {
        private readonly Dictionary<StatusAttribute, StatusAmount> _statusAmounts = new();
        public IReadOnlyDictionary<StatusAttribute, StatusAmount> StatusPair => _statusAmounts;
        public Unit Parent { get; set; }

        private StatusAmount _HPAmount;
        private StatusAmount _MPAmount;
        public float HP
        {
            get => _HPAmount.currentAmount;
            set => _HPAmount.currentAmount = Mathf.Clamp(value, 0, MaxHP);
        }
        public float MaxHP => _HPAmount.ChangedMax;

        public float MP
        {
            get => _MPAmount.currentAmount;
            set => _MPAmount.currentAmount = Mathf.Clamp(value, 0, MaxMP);
        }
        public float MaxMP => _MPAmount.ChangedMax;

        public StatusManager Initialize(Unit parent, StatusData data)
        {
            Parent = parent;
            _statusAmounts.Clear();

            // 基礎ステータスを一括登録
            AddStatus(StatusAttribute.HP, data.HP);
            AddStatus(StatusAttribute.MP, data.MP);
            AddStatus(StatusAttribute.STR, data.STR);
            AddStatus(StatusAttribute.INT, data.INT);
            AddStatus(StatusAttribute.SPD, data.SPD);
            AddStatus(StatusAttribute.DEF, data.DEF);
            AddStatus(StatusAttribute.MDEF, data.MDEF);
            AddStatus(StatusAttribute.LUK, data.LUK);
            AddStatus(StatusAttribute.CriticalRate, data.CR + data.LUKToCR);
            AddStatus(StatusAttribute.CriticalDamage, data.CRDamage + data.LUKToCRDamage);
            AddStatus(StatusAttribute.TakeDamageScale, data.TakeDamageScale);

            // 現在HPを最大値に設定
            _HPAmount = _statusAmounts[StatusAttribute.HP];
            _HPAmount.currentAmount = _HPAmount.ChangedMax;
            _MPAmount = _statusAmounts[StatusAttribute.MP];
            _MPAmount.currentAmount = _HPAmount.ChangedMax;
            foreach (var d in data.TakeElementDamageScale)
                AddStatus(d.element.GetStatusFromElement(false), d.scale);
            foreach (var d in data.GiveElementDamageScale)
                AddStatus(d.element.GetStatusFromElement(true), d.scale);
            return this;
        }

        public void AddStatus(StatusAttribute status, float amount)
        {
            var stat = new StatusAmount(amount);
            _statusAmounts[status] = stat;
            if (status == StatusAttribute.HP)
            {
                _HPAmount = stat;
            }
        }

        public void RemoveStatus(StatusAttribute status)
        {
            if (status == StatusAttribute.HP || status == StatusAttribute.MP)
            {
                Debug.LogWarning($"Cannot remove required status: {status}");
                return;
            }
            _statusAmounts.Remove(status);
        }

        public bool IsRegistered(StatusAttribute status) => _statusAmounts.ContainsKey(status);

        public StatusAmount GetStatusAmount(StatusAttribute status)
        {
            if (IsRegistered(status))
                return _statusAmounts[status];
            Debug.LogError($"Status {status} is not registered.");
            return null;
        }

        public bool TryGetStatus(StatusAttribute status, out StatusAmount amount)
        {
            return _statusAmounts.TryGetValue(status, out amount);
        }

        public List<StatusAttribute> GetStatusList() => new List<StatusAttribute>(_statusAmounts.Keys);

        public void TakeHeal(DamageInfo info)
        {
            HP = Mathf.Min(MaxHP, HP + info.Damage);
        }

        /// <returns>死亡したかどうか</returns>
        public bool TakeDamage(DamageInfo info)
        {
            HP = Mathf.Max(0, HP - info.Damage);
            return HP <= 0;
        }
    }
}
