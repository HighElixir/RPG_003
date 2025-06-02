using RPG_001.Battle.Characters;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RPG_001.Battle
{
    public class AddDamage : SerializedMonoBehaviour
    {
        [SerializeField] private CharacterBase target;
        [SerializeField] private int damageAmount = 10;
        [SerializeField] private bool isCritical = false;
        [SerializeField] private bool isMissed = false;
        [SerializeField] private bool isDodged = false;
        [SerializeField] private bool isBlocked = false;
        [SerializeField] private bool IsResisted = false;
        private DamageInfo damageInfo;
        private void Start()
        {
            // Initialize damageInfo with default values
            damageInfo = new DamageInfo(
                null,
                null,
                damageAmount,
                isCritical,
                isMissed,
                isDodged,
                isBlocked,
                IsResisted
            );
        }

        [Button("Execute Damage"), DisableInEditorMode]
        public void Execute()
        {
            target?.TakeDamage(damageInfo);
        }

        private void OnValidate()
        {
            // Update damageInfo when values change in the inspector
            damageInfo = new DamageInfo(
                null,
                target,
                damageAmount,
                isCritical,
                isMissed,
                isDodged,
                isBlocked,
                IsResisted
            );
        }
    }
}