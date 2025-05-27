using UnityEngine;

namespace RPG_001.Battle.Characters.Enemy
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "RPG_001/Enemy/EnemyData", order = 1)]
    public class EnemyData : ScriptableObject
    {
        public string enemyName;
        public string description;
        public Sprite icon;
        public CharacterData characterData;
        public ActionMaps actionMaps;

        private void Awake()
        {
            characterData.Name = enemyName;
        }
    }
}