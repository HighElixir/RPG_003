using RPG_001.Battle.Enemy;
using UnityEngine;

namespace RPG_001.Battle
{
    public class SummonEnemy : MonoBehaviour
    {
        [SerializeField] private EnemyData _enemyPrefab; // Prefab for the enemy characters
        [SerializeField] private BattleManager _battleManager; // Reference to the BattleManager
        
        public void Execute()
        {
            _battleManager.SummonEnemy(_enemyPrefab, CharacterPosition.Enemy_1);
        }
    }
}