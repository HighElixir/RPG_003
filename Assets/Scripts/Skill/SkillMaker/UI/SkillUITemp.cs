using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Skills
{
    public abstract class SkillUITemp : MonoBehaviour
    {
        [SerializeField] private Transform _confirm;
        [SerializeField] private Transform _cancel;
        [SerializeField] private Transform _namePlaceholder;
        [SerializeField] private Transform _descPlaceholder;
        [SerializeField] private Transform _stats;
        [SerializeField] private Transform _chipStats;

        public Vector2 ChipStatsPosition => _chipStats.position;
        public Vector2 StatsPosition => _stats.position;
        public (Vector2 confirmPos, Vector2 cancelPos) ButtonPositions => (_confirm.position, _cancel.position);
        public (Vector2 namePos, Vector2 descPos) TextPositions => (_namePlaceholder.position, _descPlaceholder.position);
        
        // 実装必須
        public abstract void UpdateUI(List<SkillBuilder.DataContainer> dataContainers);
        
        
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
        public virtual void Show()
        {
            gameObject.SetActive(true);
        }
    }
}
