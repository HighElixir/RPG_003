using Sirenix.OdinInspector;
using UnityEngine;

namespace RPG_003.Battle
{
    public class SummonEnemy : MonoBehaviour
    {
        [SerializeField] private SpawningTable _spawningTable; // Prefab for the enemy characters
        [SerializeField] private BattleManager _battleManager; // Reference to the BattleManager

        [Button("Summon Enemy"), DisableInEditorMode]
        public void Execute()
        {
            if (_battleManager.TryGetUsablePosition(out var position, Factions.Faction.Enemy))
                _battleManager.SummonEnemy(_spawningTable.GetSpawnData(0.5f).GetEnemyData(), position);
        }
    }
}