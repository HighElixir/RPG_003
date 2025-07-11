using RPG_003.Core;
using System.Text;
using TMPro;
using UnityEngine;

namespace RPG_003.Equipments
{
    public class StatsMaker : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        private void Update()
        {
            var players = GameDataHolder.instance.Players.Data;
            var sb = new StringBuilder();
            foreach (var p in players)
            {
                sb.AppendLine(p.Name);
                sb.AppendLine("Used Points : " + p.UsedPoints());
                sb.AppendLine();
            }
            if (players.Count == 0)
                sb.AppendLine("No Players");
            _text.text = sb.ToString();
        }
    }
}