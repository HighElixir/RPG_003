using RPG_003.Battle.Characters;
using Sirenix.OdinInspector;
using System.Text;
using UnityEngine;

namespace RPG_003.Battle
{
    public class CheckInterval : MonoBehaviour
    {
        [SerializeField] private BattleManager _battleManager;
        private bool IsValid => _battleManager;

        [Button("Output Interval"), HideInEditorMode, ShowIf("IsValid")]
        private void Output()
        {
            var items = _battleManager.GetCharacters();
            var sb = new StringBuilder();
            sb.Append("[CheckInterval] OutputCharacter's intervalCount.\n");
            foreach (var item in items)
            {
                sb.Append($"[{item.Data.Name}] : Speed {item.StatusManager.GetStatusAmount(StatusAttribute.SPD).ChangedMax},");
                sb.Append($"Interval {item.BehaviorIntervalCount.CurrentAmount} / {item.BehaviorIntervalCount.Max}, \n");
            }
            Debug.Log(sb.ToString());
        }
    }
}