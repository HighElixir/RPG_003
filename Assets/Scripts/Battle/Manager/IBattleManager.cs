using RPG_001.Battle.Characters.Enemy;
using RPG_001.Battle.Characters;
using System.Collections.Generic;

namespace RPG_001.Battle
{
    /// <summary>
    /// Interface for managing battles in the RPG game.
    /// </summary>
    public interface IBattleManager
    {
        /// <summary>
        /// Starts the battle with the specified characters.
        /// </summary>
        /// <param name="characters">Array of characters participating in the battle.</param>
        void StartBattle(List<CharacterData> players);
        /// <summary>
        /// Ends the current battle.
        /// </summary>
        void EndBattle();
        /// <summary>
        /// Processes a character's turn in the battle.
        /// </summary>
        /// <param name="character">The character whose turn is being processed.</param>
        void ProcessTurn();

        void SummonEnemy(EnemyData enemyData, CharacterPosition position);
        void ExecuteTurn(CharacterBase actor, bool instantStart = false);
    }
}