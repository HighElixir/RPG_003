using UnityEngine;

namespace RPG_003.Skills
{
    [CreateAssetMenu(fileName = "Modifier", menuName = "RPG_003/Skills/Modifier/Skill")]
    public class ModifierData : BasicData
    {
        [SerializeField] private int _installableAddon = 1;
        public int InstallableAddon => _installableAddon;
    }
}