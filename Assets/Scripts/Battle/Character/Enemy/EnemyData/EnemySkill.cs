using RPG_001.Skills;
using UnityEngine;

namespace RPG_001.Battle.Characters.Enemy
{
    [CreateAssetMenu(fileName = "EnemySkills", menuName = "RPG_001/Enemy/EnemySkills", order = 1)]
    public class EnemySkills : ScriptableObject
    {
        public string skillName;
        public float weight = 1f; // Default weight for the skill
        public Elements elements;
        public AmountAttribute skillType;
        [Min(0)] public float damage_with_str; // 1f = 100%
        [Min(0)] public float damage_with_int; // 1f = 100%
        [Min(0)] public float heal_with_int; // 1f = 100%
        [Min(0)] public float heal_with_str; // 1f = 100%
        // Note : 今後追加デバフなどを追加する
    }
}
// unicode