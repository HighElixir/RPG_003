using Sirenix.OdinInspector;
using UnityEngine;
using RPG_003.Status;

namespace RPG_003.Battle
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "RPG_003/Enemy/EnemyData", order = 1)]
    public class EnemyData : SerializedScriptableObject
    {
        public string enemyName;
        public string description;
        public Sprite icon;
        public StatusData statusData;
        [SerializeReference] public ISkillBehaviour skill;

        private void Awake()
        {
            statusData.Name = enemyName;
        }
    }
}