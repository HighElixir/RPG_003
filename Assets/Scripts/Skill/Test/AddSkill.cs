using Sirenix.OdinInspector;
using UnityEngine;

namespace RPG_003.Skills
{
    [RequireComponent(typeof(BattleExecute))]
    public abstract class AddSkills : MonoBehaviour
    {
        [SerializeField] private BattleExecute _execute;

        [Button("AddSkill")]
        private void AddSkill()
        {
            _execute.AddSkill(Inner());
        }

        protected abstract SkillDataHolder Inner();
    }
}