using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Skills
{
    public class SkillUITemp : MonoBehaviour
    {
        [SerializeField] private Transform _confirm;
        [SerializeField] private Transform _cancel;
        [SerializeField] private Transform _namePlaceholder;
        [SerializeField] private Transform _descPlaceholder;
        [SerializeField] private Transform _stats;
        
        // 実装必須
        public virtual void UpdateUI(List<SkillBuilder.DataContainer> dataContainers)
        {
        }
        public virtual (Vector2 confirmPos, Vector2 cancelPos) GetButtonPositions()
        {
            return (_confirm.position, _cancel.position);
        }
        public virtual (Vector2 namePos, Vector2 descPos) GetTextPositions()
        {
            return (_namePlaceholder.position, _descPlaceholder.position);
        }
        public virtual Vector2 GetStatsPosition()
        {
            return _stats.position;
        }
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
