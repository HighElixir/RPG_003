using RPG_003.Battle.Characters;
using RPG_003.Battle.Characters.Enemy;
using System;
using System.Collections.Generic;

namespace RPG_003.Battle
{
    /// <summary>
    /// Interface for managing battles in the RPG game.
    /// </summary>
    public interface IBattleManager
    {
        Action<CharacterBase> OnCharacterRemoved { get; set; }
        /// <summary>
        /// Starts the battle with the specified characters.
        /// </summary>
        /// <param name="characters">Array of characters participating in the battle.</param>
        void StartBattle(List<CharacterData> players, SpawningTable table);
        /// <summary>
        /// Ends the current battle.
        /// </summary>
        void EndBattle();
        void FinishTurn(CharacterBase actor);

        void SummonEnemy(EnemyData enemyData, CharacterPosition position);
        void RemoveCharacter(CharacterBase character);
        bool TryGetUsablePosition(out CharacterPosition position, int option = 0);
        CharacterPosition GetUsablePosition(int option = 0);
        List<CharacterBase> GetCharacters();
        Dictionary<CharacterPosition, CharacterBase> GetCharacterMap();
        /// <summary>
        /// ウェーブ進行度を取得
        /// </summary>
        /// <returns></returns>
        float GetDepth();
    }
}