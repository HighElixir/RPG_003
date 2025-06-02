using RPG_001.Battle.Behaviour;
using RPG_001.Battle.Characters;
using RPG_001.Battle.Characters.Enemy;
using UnityEngine;

namespace RPG_001.Battle
{
    public class SummonEnemies
    {
        private readonly CharacterBase _enemyPrefab; // Prefab for the enemy characters
        private readonly IBattleManager _battleManager; // Reference to the battle manager

        /// <summary>
        /// Summons an enemy into the battle.
        /// </summary>
        /// <param name="enemyData">The data of the enemy to summon.</param>
        public CharacterBase Summon(EnemyData enemyData)
        {
            Debug.Log($"Summoning enemy: {enemyData.enemyName}");
            var e = GameObject.Instantiate(_enemyPrefab, Vector3.zero, Quaternion.identity);
            e.Initialize(enemyData.characterData, new StatusManager(), new EnemyBehaviour(enemyData.EnemyBehaviorData), _battleManager);
            if (enemyData.icon != null)
                e.GetComponent<SpriteRenderer>().sprite = enemyData.icon; // Set the enemy's icon
            return e; // Return the instantiated enemy character
        }

        public SummonEnemies(CharacterBase enemyPrefab, IBattleManager battleManager)
        {
            _enemyPrefab = enemyPrefab;
            _battleManager = battleManager;
        }
    }
}
// unicode