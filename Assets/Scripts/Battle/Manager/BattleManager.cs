using RPG_001.Battle.Behaviour;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using RPG_001.Battle.Characters.Enemy;
using RPG_001.Battle.Characters;

namespace RPG_001.Battle
{
    public class BattleManager : MonoBehaviour, IBattleManager
    {
        [Header("Battle Manager Settings")]
        [SerializeField] private CharacterBase _characterBase;
        [SerializeField] private Positioning _positioning;
        private SummonEnemies _summonEnemies;

        // Container for characterObjects in the battle
        private Transform _charactersContainer;
        private Dictionary<CharacterPosition, CharacterBase> _characterPositions = new Dictionary<CharacterPosition, CharacterBase>();
        private List<CharacterBase> _turnActors = new List<CharacterBase>();

        private IStatusManager Gene => new StatusManager();
        private void Awake()
        {
            _charactersContainer = new GameObject("CharactersContainer").transform;
            _summonEnemies = new SummonEnemies(_characterBase);
        }
        public void StartBattle(List<CharacterData> players)
        {
            int i = 0;
            foreach (var player in players)
            {
                // Initialize each player character for the battle
                var c = GameObject.Instantiate(_characterBase, Vector3.zero, Quaternion.identity);
                InitCharacter(c, Gene, player, GetPosition(i), new PlayerBehaviour());
                Debug.Log($"Starting battle with player: {player.Name}");
                i++;
            }
        }
        public void EndBattle()
        {
            // Implementation for ending the battle
        }

        public void ProcessTurn()
        {
            // 1. 全キャラのゲージを一律に進める
            var allChars = _characterPositions.Values.ToList();
            int delta = allChars.Min(c => c.SpeedController.CurrentAmount);
            foreach (var c in allChars)
                c.SpeedController.Process(delta);

            // 2. 行動可能（ゲージMAX）になったキャラをピックアップ
            _turnActors.AddRange(allChars
                .Where(c => c.SpeedController.IsReady)                   // IsReady==true のやつだけ
                .OrderByDescending(c => c.SpeedController.CurrentAmount) // ゲージ多い順（タイブレーク）
                .ToList());


            ExecuteTurn(_turnActors[0]);
        }

        // キャラの行動を実際に呼び出すヘルパー
        public void ExecuteTurn(CharacterBase actor, bool instantStart = false)
        {
            _turnActors.Remove(actor); // Remove the actor from the turn list
            // Todo : 
        }


        public void SummonEnemy(EnemyData enemyData, CharacterPosition position)
        {
            InitCharacter(_summonEnemies.Summon(enemyData), Gene, enemyData.characterData, position, new EnemyBehaviour(enemyData.actionMaps));
        }
        private void InitCharacter(CharacterBase c, IStatusManager s, CharacterData data, CharacterPosition position, ICharacterBehaviour behaviour)
        {
            c.gameObject.name = data.Name; // Set the character's name
            s.Initialize(c, data); // Initialize the status manager with the character and player data
            c.Initialize(data, s, behaviour); // Assuming null for IStatusManager for simplicity

            if (_characterPositions.ContainsKey(position))
            {
                Debug.LogWarning($"Position {position} is already occupied by {_characterPositions[position].gameObject.name}. Replacing with {c.gameObject.name}.");
                Destroy(_characterPositions[position].gameObject); // Destroy the existing character at this position
            }
            _characterPositions[position] = c; // Store the character in the appropriate position
            c.transform.SetParent(_charactersContainer); // Set the parent of the character's transform
            SetPosition(c, position); // Set the character's position in the battle
        }
        private void SetPosition(CharacterBase c, CharacterPosition p)
        {
            _positioning.SetPosition(c, p);
        }
        private CharacterPosition GetPosition(int i)
        {
            switch (i)
            {
                case 0: return CharacterPosition.Player_1;
                case 1: return CharacterPosition.Player_2;
                case 2: return CharacterPosition.Player_3;
                case 3: return CharacterPosition.Player_4;
                case 4: return CharacterPosition.Enemy_1;
                case 5: return CharacterPosition.Enemy_2;
                case 6: return CharacterPosition.Enemy_3;
                case 7: return CharacterPosition.Enemy_4;
                default: return CharacterPosition.None; // Default case
            }
        }
    }
}