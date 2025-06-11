using RPG_003.Battle.Behaviour;
using RPG_003.Battle.Characters;
using RPG_003.Battle.Characters.Enemy;
using RPG_003.Battle.Characters.Player;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG_003.Battle
{
    [DefaultExecutionOrder(-1)]
    public class BattleManager : MonoBehaviour
    {
        //=== Reference ===
        [BoxGroup("Reference"), SerializeField] private Camera _camera;
        [BoxGroup("Reference"), SerializeField] private SkillSelector _skillButtonManager;
        [BoxGroup("Reference"), SerializeField] private CharacterBase _characterBase;
        [BoxGroup("Reference"), SerializeField] private SelectTarget _selectTarget;
        [BoxGroup("Reference"), SerializeField] private Player _player;
        [BoxGroup("Reference"), SerializeField] private CharacterTransformHalper _characterTransformHalper;
        [BoxGroup("Reference"), SerializeField] private IndicatorFactory _indicatorFactory;
        [BoxGroup("Reference"), SerializeField] private BattleDataToUI _toUI;

        private PositionManager _posManager;
        private CharacterInitializer _charInitializer;
        private TurnManager _turnManager;

        // === Debug ===
        [SerializeField, ReadOnly] private IReadOnlyDictionary<CharacterPosition, CharacterBase> _characterPositions;

        //=== Non-Serialized Fields ===
        private Transform _charactersContainer;

#if UNITY_EDITOR
#pragma warning disable 0219
        private bool _allowNextTurn = false;
        [SerializeField, Tooltip("True => バトルマネージャにボタンが表示され、それを押すことでターンを進行するようになる")]
        private bool _enebleUseTurnManage = true;
#pragma warning restore 0219
#endif

        //=== Properties ===
        public Action<CharacterBase> OnCharacterRemoved { get; set; }
        public SelectTarget SelectTarget => _selectTarget;
        public SkillSelector SkillSelector => _skillButtonManager;

        //=== Public Methods ===
        public void StartBattle(List<PlayerData> players, SpawningTable table)
        {
            _posManager.Clear();
            // ターンキューもクリア（内部的に新規 TurnManager を替えるか、リセット処理を追加しても OK）
            _turnManager.Reset();
            _turnManager.OnExecuteTurn = (character) =>
            {
                StartCoroutine(character.TurnBehaviour());
            };

            for (int i = 0; i < players.Count && i < 4; i++)
            {
                var c = Instantiate(_player, Vector3.zero, Quaternion.identity);
                _charInitializer.InitPlayer(c, players[i], players[i].CharacterData, new PlayerBehaviour());
                RegisterCharacter((CharacterPosition)i, c);
            }

            for (int i = 0; i < 5; i++)
                SummonEnemy(table.GetSpawnData(0f).GetEnemyData(), (CharacterPosition)i + 4);

            ProcessTurn(); // BattleManager のラッパー
        }

        public void EndBattle()
        {
            Debug.Log("Battle ended.");
        }

        public void FinishTurn(CharacterBase actor)
        {
            // BattleManager が呼ばれたら、TurnManager に任せる
#if UNITY_EDITOR
            _allowNextTurn = true;
#else
            _turnManager.FinishTurn();
#endif
        }

        // デフォルトの場合、空いてる場所にスポーンさせる
        public void SummonEnemy(EnemyData enemyData, CharacterPosition characterPosition = CharacterPosition.None)
        {
            var c = Instantiate(_characterBase, Vector3.zero, Quaternion.identity);
            _charInitializer.InitCharacter(c, enemyData.characterData, enemyData.enemyBehaviorData.GetCharacterBehaviour());
            if (characterPosition == CharacterPosition.None && !_posManager.TryGetUsablePosition(out characterPosition))
            {
                Debug.Log("モンスターをスポーンさせるために必要なスペースがありません");
                return;
            }
            if (enemyData.icon != null)
                c.SetIcon(enemyData.icon);
            RegisterCharacter(characterPosition, c);
        }
        public void RegisterCharacter(CharacterPosition position, CharacterBase character)
        {
            var indicator = _indicatorFactory.Create(_characterTransformHalper.GetPosition(position));
            character.BehaviorIntervalCount.SetIndicator(indicator);
            _posManager.RegisterCharacter(position, character);
            character.transform.SetParent(_charactersContainer);
            _characterTransformHalper.SetPosition(character, position);
            Debug.Log($"Starting battle with player: {character.Data.Name} at position: {position}");
        }

        public void RemoveCharacter(CharacterBase character)
        {
            // PositionManager で位置削除
            _posManager.RemoveCharacter(character);
            _turnManager.RemoveCharacter(character);
            OnCharacterRemoved?.Invoke(character);
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

        public IReadOnlyDictionary<CharacterPosition, CharacterBase> GetCharacterMap()
        {
            return _posManager.GetCharacterMap();
        }

        public float GetDepth()
        {
            return 0.5f;
        }

        public void ApplyDamage(DamageInfo info)
        {
            Debug.Log($"{info.Target} is Taking damage: {info.Damage} from {info.Source.Data.Name ?? "Unknown"}");
            _toUI.CreateDamageText((info.Target as CharacterBase).transform, info.Damage);
            info.Target.TakeDamage(info);
        }
#if UNITY_EDITOR
        [ShowIf("_allowNextTurn", "_enebleUseTurnManage"), Button("TurnBehavior")]
        public void Next()
        {
            _allowNextTurn = false;
            Debug.Log("Next turn triggered in editor.");
            _turnManager.ProcessTurn();
        }
#endif

        //=== Private Methods ===
        public void ProcessTurn()
        {
#if UNITY_EDITOR
            _allowNextTurn = true;
            if (!_enebleUseTurnManage)
                _turnManager.ProcessTurn();
#else
            _turnManager.ProcessTurn();
#endif
        }

        //=== Unity Lifecycle ===
        private void Awake()
        {
            _charactersContainer = new GameObject("CharactersContainer").transform;

            // 分割クラスの初期化
            _posManager = new PositionManager(out _characterPositions);
            _charInitializer = new CharacterInitializer(this);
            _turnManager = new TurnManager(this);
            _turnManager.OnExecuteTurn = (character) =>
            {
                // コルーチン実行を登録
                StartCoroutine(character.TurnBehaviour());
            };
        }
    }
}
