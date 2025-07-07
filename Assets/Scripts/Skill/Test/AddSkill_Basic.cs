using RPG_003.Effect;
using Sirenix.OdinInspector;
using UnityEngine;
namespace RPG_003.Skills
{
    public class AddSkill_Basic : MonoBehaviour
    {
        [SerializeField] private BattleExecute _execute;
        [SerializeReference, SubclassSelector] private SkillHolder _holder;

        [Button("AddSkill")]
        private void AddSkill()
        {
            _execute.AddSkill(_holder);
        }
    }
}