using RPG_003.Core;
using System.Text;
using TMPro;
using UnityEngine;

namespace RPG_003.Equipments
{
    public class SkillStatsMaker : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        private void Update()
        {
            var skills = GameDataHolder.instance.Skills;
            var sb = new StringBuilder();
            foreach (var skill in skills)
            {
                sb.AppendLine(skill.Name);
                sb.AppendLine(skill.Desc);
                sb.AppendLine();
            }
            if (skills.Count == 0)
                sb.AppendLine("No Players");
            _text.text = sb.ToString();
        }
    }
}