using RPG_003.Battle.Skills;
using UnityEngine;

namespace RPG_003.Skills
{
    [CreateAssetMenu(fileName = "Modifier", menuName = "RPG_003/Skills/Modifier/Skill")]
    public class ModifierData : BasicData
    {
        private int _installableAddon = 1;
        public int InstallableAddon => _installableAddon;
    }
}