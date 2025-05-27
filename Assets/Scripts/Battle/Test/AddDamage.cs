using RPG_001.Characters;
using UnityEngine;

namespace RPG_001.Battle
{
    public class AddDamage : MonoBehaviour
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
                damageAmount,
                isCritical,
                isMissed,
                isDodged,
                isBlocked,
                IsResisted
            );
        }
        public void Execute()
        {
            target?.TakeDamage(damageInfo);
        }
    }
}