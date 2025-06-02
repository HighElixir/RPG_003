using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using RPG_001.Battle.Behaviour;
using RPG_001.Battle.Characters;
using RPG_001.Battle.Characters.Enemy;
using Sirenix.Utilities.Editor;

namespace RPG_001.Battle
{
    public class BattleManager : SerializedMonoBehaviour, IBattleManager
    {
        //=== Serialized Fields ===
        [BoxGroup("Battle Manager Settings")]
        [SerializeField] private CharacterBase _characterBase;
        [SerializeField] private Positioning _positioning;
        [SerializeField, ReadOnly] private Dictionary<CharacterPosition, CharacterBase> _characterPositions = new Dictionary<CharacterPosition, CharacterBase>();

        //=== Non-Serialized Fields ===
        private SummonEnemies _summonEnemies;
        private Transform _charactersContainer;
        private List<CharacterBase> _turnActors = new List<CharacterBase>();
        private int _turnCount = 0;

#if UNITY_EDITOR
        private bool _allowNextTurn = false;
#endif

        //=== Properties ===
        private IStatusManager Gene => new StatusManager();
        public Action<CharacterBase> OnCharacterRemoved { get; set; }

        //=== Unity Lifecycle ===
        private void Awake()
        {
            _charactersContainer = new GameObject("CharactersContainer").transform;
            _summonEnemies = new SummonEnemies(_characterBase, this);
        }

        //=== Public Methods ===
        public void StartBattle(List<CharacterData> players, SpawningTable table)
        {
            _characterPositions.Clear();
            _turnActors.Clear();

            for (int i = 0; i < players.Count && i < 4; i++)
            {
                var c = Instantiate(_characterBase, Vector3.zero, Quaternion.identity);
                InitCharacter(c, Gene, players[i], GetPosition(i), new PlayerBehaviour());
                Debug.Log($"Starting battle with player: {players[i].Name} at position: {GetPosition(i)}");
            }

            for (int i = 0; i < 5; i++)
                SummonEnemy(table.GetSpawnData(0f).GetEnemyData(), GetPosition(i + 4));

            ProcessTurn();
        }

        public void EndBattle()
        {
            // 必要に応じて実装してね
        }

        public void FinishTurn(CharacterBase actor)
        {
            if (_turnActors.Count > 0)
            {
                var nextActor = _turnActors.First();
                _turnActors.RemoveAt(0);
                ExecuteTurn(nextActor);
            }
            else
            {
                _turnCount++;
#if UNITY_EDITOR
                _allowNextTurn = true;
#else
                ProcessTurn();
#endif
            }
        }

        public void SummonEnemy(EnemyData enemyData, CharacterPosition position)
        {
            var enemy = _summonEnemies.Summon(enemyData);
            InitCharacter(enemy, Gene, enemyData.characterData, position, new EnemyBehaviour(enemyData.EnemyBehaviorData));
        }

        public void RemoveCharacter(CharacterBase character)
        {
            if (character == null || !_characterPositions.ContainsValue(character)) return;

            var pos = _characterPositions.First(kv => kv.Value == character).Key;
            _characterPositions.Remove(pos);
            OnCharacterRemoved?.Invoke(character);
            _turnActors.Remove(character);
            StopCoroutine(character.TurnBehaviour());
            Destroy(character.gameObject);
        }

        public CharacterPosition GetUsablePosition(int option = 0)
        {
            if (TryGetUsablePosition(out var pos, option))
                return pos;
            return CharacterPosition.None;
        }

        public bool TryGetUsablePosition(out CharacterPosition position, int option = 0)
        {
            int start = option == 2 ? 4 : 0;
            int end = option == 1 ? 3 : 8;
            for (int i = start; i <= end; i++)
            {
                var p = GetPosition(i);
                if (!_characterPositions.ContainsKey(p))
                {
                    position = p;
                    return true;
                }
            }
            position = CharacterPosition.None;
            return false;
        }

        public List<CharacterBase> GetCharacters() => _characterPositions.Values.ToList();
        public Dictionary<CharacterPosition, CharacterBase> GetCharacterMap() => _characterPositions;
        public float GetDepth() => 0.5f;

#if UNITY_EDITOR
        [SerializeField, ShowIf("_allowNextTurn"), Button("TurnBehavior")]
        public void Next()
        {
            _allowNextTurn = false;
            Debug.Log("Next turn triggered in editor.");
            ProcessTurn();
        }
#endif

        //=== Private Methods ===
        private void ProcessTurn()
        {
            if (_turnCount >= 100) return;

            var all = _characterPositions.Values.ToList();
            int delta = all.Min(c => c.SpeedController.CurrentAmount);
            Debug.Log($"Processing turn: {_turnCount}, advancing all by {delta}");

            if (delta > 0)
            {
                foreach (var c in all)
                {
                    c.SpeedController.Process(delta);
                    Debug.Log($"Character {c.Data.Name} advanced by {delta}, new speed: {c.SpeedController.CurrentAmount}");
                }
            }

            var ready = all.Where(c => c.SpeedController.IsReady)
                           .OrderByDescending(c => c.SpeedController.CurrentAmount);
            _turnActors.AddRange(ready);

            if (_turnActors.Count > 0)
            {
                var next = _turnActors.First();
                _turnActors.RemoveAt(0);
                ExecuteTurn(next);
            }
        }

        private void ExecuteTurn(CharacterBase actor, bool instantStart = false)
        {
            actor.SpeedController.Reset();
            StartCoroutine(actor.TurnBehaviour(instantStart));
        }

        private void InitCharacter(CharacterBase c, IStatusManager statusMgr, CharacterData data, CharacterPosition pos, ICharacterBehaviour behaviour)
        {
            c.gameObject.name = data.Name;
            c.Position = pos;
            statusMgr.Initialize(c, data);
            c.Initialize(data, statusMgr, behaviour, this);

            if (_characterPositions.ContainsKey(pos))
            {
                Debug.LogWarning($"Position {pos} occupied by {_characterPositions[pos].gameObject.name}. Replacing.");
                Destroy(_characterPositions[pos].gameObject);
            }
            _characterPositions[pos] = c;
            c.transform.SetParent(_charactersContainer);
            _positioning.SetPosition(c, pos);
        }

        private CharacterPosition GetPosition(int i)
        {
            return i switch
            {
                0 => CharacterPosition.Player_1,
                1 => CharacterPosition.Player_2,
                2 => CharacterPosition.Player_3,
                3 => CharacterPosition.Player_4,
                4 => CharacterPosition.Enemy_1,
                5 => CharacterPosition.Enemy_2,
                6 => CharacterPosition.Enemy_3,
                7 => CharacterPosition.Enemy_4,
                8 => CharacterPosition.Enemy_5,
                _ => CharacterPosition.None,
            };
        }
    }
}
