using System.Text;
using UnityEngine;

namespace RPG_003.DataManagements.Datas
{
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "Modifier", menuName = "RPG_003/Skills/Modifier/Skill")]
#endif
    public class ModifierData : BasicData
    {
        [SerializeField] private int _installableAddon = 1;
        public int InstallableAddon => _installableAddon;

        public override string AdditionalDescriptions(string desc)
        {
            StringBuilder sb = new StringBuilder(desc);
            sb.AppendLine($"[アドオンの数] :{InstallableAddon}");
            return sb.ToString();
        }
    }
}