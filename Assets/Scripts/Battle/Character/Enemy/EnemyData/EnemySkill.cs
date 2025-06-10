using RPG_003.Battle.Factions;
using RPG_003.Skills;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RPG_003.Battle.Characters.Enemy
{
    [CreateAssetMenu(fileName = "EnemySkill", menuName = "RPG_003/Enemy/EnemySkill", order = 1)]
    public class EnemySkill : SerializedScriptableObject
    {
        public string skillName;
        public Elements elements;
        public AmountAttribute skillType;
        public Faction target;
        [Min(0)] public float damage_with_str; // 1f = 100%
        [Min(0)] public float damage_with_int; // 1f = 100%
        [Min(0)] public float heal_with_int; // 1f = 100%
        [Min(0)] public float heal_with_str; // 1f = 100%
        // Note : 今後追加デバフなどを追加する
    }
}
// unicode