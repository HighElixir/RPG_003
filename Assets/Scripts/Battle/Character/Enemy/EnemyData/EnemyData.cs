using Sirenix.OdinInspector;
using UnityEngine;

namespace RPG_001.Battle.Characters.Enemy
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "RPG_001/Enemy/EnemyData", order = 1)]
    public class EnemyData : SerializedScriptableObject
    {
        public string enemyName;
        public string description;
        public Sprite icon;
        public CharacterData characterData;
        public EnemyBehaviorData EnemyBehaviorData;

        private void Awake()
        {
            characterData.Name = enemyName;
        }
    }
}