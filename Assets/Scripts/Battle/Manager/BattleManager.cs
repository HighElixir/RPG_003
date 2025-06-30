using HighElixir.PauseManage;
using RPG_003.Battle.Behaviour;
using RPG_003.Battle.Factions;
using RPG_003.Core;
using RPG_003.Status;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG_003.Battle
{
    [DefaultExecutionOrder(-1)]
    public class BattleManager : PausableMonoBehaviour
    {
        //=== Reference ===
        [BoxGroup("Reference"), SerializeField] private Camera _camera;
        [BoxGroup("Reference"), SerializeField] private SkillSelector _skillButtonManager;
        [BoxGroup("Reference"), SerializeField] private CharacterObject _characterObject;
        [BoxGroup("Reference"), SerializeField] private TargetSelector _targetSelector;
        [BoxGroup("Reference"), SerializeField] private IndicatorUIBuilder _indicatorUI;
        [BoxGroup("Reference"), SerializeField] private Player _player;
        [BoxGroup("Reference"), SerializeField] private CharacterTransformHelper _characterTransformHelper;
        [BoxGroup("Reference"), SerializeField] private SceneLoaderAsync _sceneLoaderAsync;

        private PositionManager _posManager;
        private TurnManager _turnManager;

        // === Debug ===
        [SerializeField, ReadOnly] private IReadOnlyDictionary<CharacterPosition, CharacterObject> _characterPositions;

        //=== Non-Serialized Fields ===
        private Transform _charactersContainer;
        private bool _isBattleContinue = false;

#if UNITY_EDITOR
#pragma warning disable 0414
        private bool _allowNextTurn = false;
        [SerializeField, Tooltip("True => バトルマネージャにボタンが表示され、それを押すことでターンを進行するようになる")]
        private bool _enebleUseTurnManage = true;
#pragma warning restore 0414
#endif

        //=== Properties ===
        public Action<CharacterObject> OnCharacterRemoved { get; set; }
        public TargetSelector TargetSelector => _targetSelector;
        public SkillSelector SkillSelector => _skillButtonManager;
        public bool IsBattleContinue => _isBattleContinue;
        public IndicatorUIBuilder IndicatorUIBuilder => _indicatorUI;

        //=== Public Methods ===
        public void StartBattle(List<PlayerData> players, SpawningTable table)
        {
            _isBattleContinue = true;
            Initialize();

            for (int i = 0; i < players.Count && i < 4; i++)
            {
                var c = Instantiate(_player, Vector3.zero, Quaternion.identity);
                this.InitPlayer(c, players[i], players[i].CharacterData, new PlayerBehaviour());
                RegisterCharacter((CharacterPosition)i, c);
            }

            for (int i = 0; i < 5; i++)
                SummonEnemy(table.GetSpawnData(0f).GetEnemyData(), (CharacterPosition)i + 4);

            _indicatorUI.UpdateUI(_posManager.GetCharacters());
            ProcessTurn(); // BattleManager のラッパー
        }
        public void StartBattle(List<Player> players, SpawningTable table)
        {
            _posManager.Clear();
            _turnManager.Reset();

            for (int i = 0; i < players.Count && i < 4; i++)
            {
                RegisterCharacter((CharacterPosition)i, players[i]);
            }

            for (int i = 0; i < 5; i++)
                SummonEnemy(table.GetSpawnData(0f).GetEnemyData(), (CharacterPosition)i + 4);

            _isBattleContinue = true;
            ProcessTurn(); // BattleManager のラッパー
        }
        public void EndBattle()
        {
            Debug.Log("Battle ended.");
            _isBattleContinue = false;
            foreach (var c in _posManager.GetCharacters()) c.Release();
        }
        public void EndBattle_Won()
        {
            Debug.Log("Won");
            EndBattle();
        }
        public void EndBattle_Lost()
        {
            Debug.Log("Lost");
            EndBattle();
        }

        public void FinishTurn(CharacterObject actor)
        {
            if (_isBattleContinue)
            {
                // BattleManager が呼ばれたら、TurnManager に任せる
#if UNITY_EDITOR
                if (_enebleUseTurnManage)
                    _allowNextTurn = true;
                else
                    _turnManager.FinishTurn();
#else
                _turnManager.FinishTurn();
#endif
            }
        }

        // デフォルトの場合、空いてる場所にスポーンさせる
        public void SummonEnemy(EnemyData enemyData, CharacterPosition characterPosition = CharacterPosition.None)
        {
            var c = Instantiate(_characterObject, Vector3.zero, Quaternion.identity);
            this.InitCharacter(c, enemyData.characterData, enemyData.enemyBehaviorData.GetCharacterBehaviour());
            if (characterPosition == CharacterPosition.None && !_posManager.TryGetUsablePosition(out characterPosition, Faction.Enemy))
            {
                Debug.Log("モンスターをスポーンさせるために必要なスペースがありません");
                return;
            }
            if (enemyData.icon != null)
                c.SetIcon(enemyData.icon);
            RegisterCharacter(characterPosition, c);
        }
        // 各派生クラスにキャラクターを登録する
        public void RegisterCharacter(CharacterPosition position, CharacterObject character)
        {
            //var indicator = _indicatorFactory.Create(_characterTransformHelper.GetPosition(position));
            _posManager.RegisterCharacter(position, character);
            character.transform.SetParent(_charactersContainer);
            _characterTransformHelper.SetPosition(character, position);
            Debug.Log($"Starting battle with player: {character.Data.Name} at position: {position}");
        }

        public void RemoveCharacter(CharacterObject character)
        {
            // PositionManager で位置削除
            _posManager.RemoveCharacter(character);
            _turnManager.RemoveCharacter(character);
            OnCharacterRemoved?.Invoke(character);
            CheckBattleEnd();
            Destroy(character.gameObject);
        }

        public CharacterPosition GetUsablePosition(Faction faction = Faction.All)
        {
            return _posManager.GetUsablePosition(faction);
        }

        public bool TryGetUsablePosition(out CharacterPosition position, Faction faction = Faction.All)
        {
            return _posManager.TryGetUsablePosition(out position, faction);
        }

        public List<CharacterObject> GetCharacters()
        {
            return _posManager.GetCharacters();
        }

        public IReadOnlyDictionary<CharacterPosition, CharacterObject> GetCharacterMap()
        {
            return _posManager.GetCharacterMap();
        }

        public float GetDepth()
        {
            return 0.5f;
        }

        public void ApplyDamage(DamageInfo info)
        {
            Debug.Log(info.ToString());
            Debug.Log($"{info.Target} is Taking damage: {info.Damage} from {info.Source.Data.Name ?? "Unknown"}");
            Color c = info.Elements == Elements.None ? Color.red : info.Elements.GetColorElement();
            GraphicalManager.instance.ThrowText(RandomPos((info.Target as CharacterObject).transform.position, 1.5f), $"{info.Damage}", c);
            info.Target.TakeDamage(info);
        }

        public void ApplyHeal(DamageInfo info)
        {
            Debug.Log($"{info.Target} is Taking heal: {info.Damage} from {info.Source.Data.Name ?? "Unknown"}");
            GraphicalManager.instance.ThrowText(RandomPos((info.Target as CharacterObject).transform.position, 1.5f), $"{info.Damage}", Color.green);
            info.Target.TakeHeal(info);
        }

        // === Pause ===

        protected override void OnPaused()
        {
            Time.timeScale = 0;
        }

        protected override void OnResumed()
        {
            Time.timeScale = 1.0f;
        }

        // === Notify ===
        public void OnDeath(CharacterObject character)
        {
            Debug.Log(character.Data.Name + "が死亡した！");
            character.OnDeath?.Invoke(character);
            CheckBattleEnd();
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

        //=== Private ===
        public void ProcessTurn()
        {
            if (!_isBattleContinue) return;
#if UNITY_EDITOR
            _allowNextTurn = true;
            if (!_enebleUseTurnManage)
                _turnManager.ProcessTurn();
#else
            _turnManager.ProcessTurn();
#endif
        }

        private void CheckBattleEnd()
        {
            var c = _posManager.AllFactionCount();
            if (c.allies <= 0) EndBattle_Lost();
            if (c.enemies <= 0) EndBattle_Won();
        }
        private Vector2 RandomPos(Vector2 pos, float radius)
        {
            // Random.insideUnitCircle は原点を中心とした半径1の円内のランダムな点を返す
            Vector2 offset = UnityEngine.Random.insideUnitCircle * radius;
            return pos + offset;
        }
        private void Initialize()
        {
            if (!_charactersContainer)
                _charactersContainer = new GameObject("CharactersContainer").transform;
            BattleSceneManager.instance.SetBattleManageer(this);
            // 分割クラスの初期化
            _posManager = new PositionManager(out _characterPositions);
            _turnManager = new TurnManager(this, this);
        }
        //=== Unity Lifecycle ===
        protected override void Awake()
        {
            Initialize();
        }
    }
}
