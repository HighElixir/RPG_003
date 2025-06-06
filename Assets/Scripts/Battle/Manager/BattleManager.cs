using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using RPG_003.Battle.Behaviour;
using RPG_003.Battle.Characters;
using RPG_003.Battle.Characters.Enemy;
using RPG_003.Battle.Characters.Player;

namespace RPG_003.Battle
{
    public class BattleManager : SerializedMonoBehaviour, IBattleManager
    {
        //=== Serialized Fields ===
        [BoxGroup("Battle Manager Settings")]
        [SerializeField] private Camera _camera;
        [BoxGroup("Battle Manager Settings")]
        [SerializeField] private IntervalIndicator _IntervalIndicatorPrefab;
        [BoxGroup("Battle Manager Settings")]
        [SerializeField] private CharacterBase _characterBase;
        [BoxGroup("Battle Manager Settings")]
        [SerializeField] private SelectTarget _selectTarget;
        [BoxGroup("Battle Manager Settings")]
        [SerializeField] private Player _player;
        [BoxGroup("Battle Manager Settings")]
        [SerializeField] private Positioning _positioning;
        [BoxGroup("Battle Manager Settings")]
        [SerializeField] private Transform _canvas;
        [BoxGroup("Battle Manager Settings")]
        [SerializeField, ReadOnly] private Dictionary<CharacterPosition, CharacterBase> _characterPositions = new Dictionary<CharacterPosition, CharacterBase>();

        //=== Non-Serialized Fields ===
        private Transform _charactersContainer;

        //=== 分割したクラスの参照 ===
        private PositionManager _posManager;
        private CharacterInitializer _charInitializer;
        private TurnManager _turnManager;

#if UNITY_EDITOR
        private bool _allowNextTurn = false;
        [SerializeField, PropertyTooltip("True => バトルマネージャにボタンが表示され、それを押すことでターンを進行するようになる")] private bool _enebleUseTurnManage = true;
#endif

        //=== Properties ===
        private IStatusManager Gene => new StatusManager();
        public Action<CharacterBase> OnCharacterRemoved { get; set; }
        public SelectTarget SelectTarget => _selectTarget;

        //=== Unity Lifecycle ===
        private void Awake()
        {
            _charactersContainer = new GameObject("CharactersContainer").transform;

            // 分割クラスの初期化
            _posManager = new PositionManager(_characterPositions);
            _charInitializer = new CharacterInitializer(
                _camera,
                _IntervalIndicatorPrefab,
                _posManager,
                _positioning,
                _canvas,
                _charactersContainer,
                this
            );
            _turnManager = new TurnManager(_characterPositions, this);
            _turnManager.OnExecuteTurn += (character) =>
            {
                // コルーチン実行を登録
                StartCoroutine(character.TurnBehaviour());
            };
        }

        //=== Public Methods ===
        public void StartBattle(List<PlayerData> players, SpawningTable table)
        {
            _characterPositions.Clear();
            // ターンキューもクリア（内部的に新規 TurnManager を替えるか、リセット処理を追加しても OK）
            _turnManager = new TurnManager(_characterPositions, this);
            _turnManager.OnExecuteTurn += (character) =>
            {
                StartCoroutine(character.TurnBehaviour());
            };

            for (int i = 0; i < players.Count && i < 4; i++)
            {
                var c = Instantiate(_player, Vector3.zero, Quaternion.identity);
                c.SetPlayerData(players[i]);

                var pos = _posManager.ConvertIndexToPosition(i);
                _charInitializer.InitCharacter(c, Gene, players[i].CharacterData, pos, new PlayerBehaviour());
                // BattleManager をセットし直し
                c.Initialize(
                    players[i].CharacterData,
                    Gene,
                    new PlayerBehaviour(),
                    this,
                    _IntervalIndicatorPrefab
                );

                Debug.Log($"Starting battle with player: {players[i].CharacterData.Name} at position: {pos}");
                _characterPositions[pos] = c;
                _positioning.SetPosition(c, pos);
            }

            for (int i = 0; i < 5; i++)
            {
                var enemyData = table.GetSpawnData(0f).GetEnemyData();
                var pos = _posManager.ConvertIndexToPosition(i + 4);
                SummonEnemy(enemyData, pos);
            }

            ProcessTurn(); // BattleManager のラッパー
        }

        public void EndBattle()
        {
            Debug.Log("Battle ended.");
        }

        public void FinishTurn(CharacterBase actor)
        {
            // BattleManager が呼ばれたら、TurnManager に任せる
            _turnManager.FinishTurn();
        }

        public void SummonEnemy(EnemyData enemyData, CharacterPosition position)
        {
            var enemyGO = Instantiate(_characterBase, Vector3.zero, Quaternion.identity);
            var enemy = enemyGO.GetComponent<CharacterBase>();

            _charInitializer.InitCharacter(enemy, Gene, enemyData.characterData, position, enemyData.enemyBehaviorData.GetCharacterBehaviour());
            // BattleManager を渡す
            enemy.Initialize(
                enemyData.characterData,
                Gene,
                enemyData.enemyBehaviorData.GetCharacterBehaviour(),
                this,
                _IntervalIndicatorPrefab
            );

            _characterPositions[position] = enemy;
            enemy.transform.SetParent(_charactersContainer);
            _positioning.SetPosition(enemy, position);
        }

        public void RemoveCharacter(CharacterBase character)
        {
            if (character == null || !_characterPositions.ContainsValue(character)) return;

            // PositionManager で位置削除
            _posManager.RemovePosition(character);

            OnCharacterRemoved?.Invoke(character);
            // ターンキューにも残っていたら削除
            // TurnManager 内部の _turnActors に直接アクセスできないから、FinishTurn の後処理を入れるか、
            // もしくは TurnManager に専用メソッドを作っても OK。ここでは簡易的に FinishTurn 呼んでおく。
            // → ちゃんと消したいなら TurnManager.RemoveFromQueue(character) を実装してもいい。
            _positioning.SetPosition(character, CharacterPosition.None);
            StopCoroutine(character.TurnBehaviour());
            Destroy(character.gameObject);
        }

        public CharacterPosition GetUsablePosition(int option = 0)
        {
            return _posManager.GetUsablePosition(option);
        }

        public bool TryGetUsablePosition(out CharacterPosition position, int option = 0)
        {
            return _posManager.TryGetUsablePosition(out position, option);
        }

        public List<CharacterBase> GetCharacters()
        {
            return _posManager.GetCharacters();
        }

        public Dictionary<CharacterPosition, CharacterBase> GetCharacterMap()
        {
            return _posManager.GetCharacterMap();
        }

        public float GetDepth()
        {
            return _posManager.GetDepth();
        }

#if UNITY_EDITOR
        [SerializeField, HideIf("_allowNextTurn"), ShowIf("_enebleUseTurnManage"), Button("TurnBehavior")]
        public void Next()
        {
            _allowNextTurn = true;
            Debug.Log("Next turn triggered in editor.");
            ProcessTurn();
        }
#endif

        //=== Private Methods ===
        private void ProcessTurn()
        {
#if UNITY_EDITOR
            if (_enebleUseTurnManage && !_allowNextTurn)
                return;
            _allowNextTurn = false;
#endif
            _turnManager.ProcessTurn();
        }
    }
}
