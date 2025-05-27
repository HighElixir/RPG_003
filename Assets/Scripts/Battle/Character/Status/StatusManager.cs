using System.Collections.Generic;
using UnityEngine;

namespace RPG_001.Battle.Characters
{
    public class StatusManager : IStatusManager
    {
        private readonly Dictionary<StatusAttribute, StatusAmount> _statusAmounts = new();
        public Dictionary<StatusAttribute, StatusAmount> StatusPair => _statusAmounts;
        public ICharacter Parent { get; set; }

        private StatusAmount _HPAmount;
        public float HP
        {
            get => _HPAmount.currentAmount;
            set => _HPAmount.currentAmount = Mathf.Clamp(value, 0, MaxHP);
        }
        public float MaxHP => _HPAmount.ChangedMax;

        public void Initialize(ICharacter parent, CharacterData data)
        {
            Parent = parent;
            _statusAmounts.Clear();

            // ��b�X�e�[�^�X���ꊇ�o�^
            AddStatus(StatusAttribute.HP, data.HP);
            AddStatus(StatusAttribute.MP, data.MP);
            AddStatus(StatusAttribute.STR, data.STR);
            AddStatus(StatusAttribute.INT, data.INT);
            AddStatus(StatusAttribute.SPD, data.SPD);
            AddStatus(StatusAttribute.DEF, data.DEF);
            AddStatus(StatusAttribute.MDEF, data.MDEF);
            AddStatus(StatusAttribute.LUK, data.LUK);

            // ����HP���ő�l�ɐݒ�
            _HPAmount = _statusAmounts[StatusAttribute.HP];
            _HPAmount.currentAmount = _HPAmount.ChangedMax;
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

        public void UpdateStatus(StatusAttribute status, float amount)
        {
            if (!IsRegistered(status))
            {
                Debug.LogError($"Cannot update unregistered status: {status}");
                return;
            }
            var stat = _statusAmounts[status];
            // ��b�l��ύX�������ɁAtemporaryChanged �𒲐����ĐV�����l������
            stat.SetChanged(amount - stat.defaultAmount);
        }

        public void AddChanged(StatusAttribute status, float amount)
        {
            if (!IsRegistered(status))
            {
                Debug.LogError($"Cannot add change to unregistered status: {status}");
                return;
            }
            _statusAmounts[status].AddChanged(amount);
        }

        public void ResetChanged(StatusAttribute status)
        {
            if (!IsRegistered(status))
            {
                Debug.LogError($"Cannot reset change for unregistered status: {status}");
                return;
            }
            _statusAmounts[status].ResetChanged();
        }

        public void AddRatio(StatusAttribute status, float ratio)
        {
            if (!IsRegistered(status))
            {
                Debug.LogError($"Cannot add ratio to unregistered status: {status}");
                return;
            }
            _statusAmounts[status].AddRatio(ratio);
        }

        public void ResetRatio(StatusAttribute status)
        {
            if (!IsRegistered(status))
            {
                Debug.LogError($"Cannot reset ratio for unregistered status: {status}");
                return;
            }
            _statusAmounts[status].ResetRatio();
        }

        public void Heal(float amount)
        {
            HP = Mathf.Min(MaxHP, HP + amount);
        }

        public void TakeDamage(DamageInfo info)
        {
            if (info.Source != null)
                Debug.Log($"Taking damage: {info.Damage} from {info.Source.Data.Name}");
            else
                Debug.Log($"Taking damage: {info.Damage} from Unknown.");

            HP = Mathf.Max(0, HP - info.Damage);
            Debug.Log($"Current HP: {HP}, Max HP: {MaxHP}");
            if (HP <= 0)
                Parent.NotifyDeath();
        }
    }
}
