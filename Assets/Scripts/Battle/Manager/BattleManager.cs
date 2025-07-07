using Cysharp.Threading.Tasks;
using RPG_003.Battle.Behaviour;
using RPG_003.Battle.Factions;
using RPG_003.Core;
using RPG_003.Status;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace RPG_003.Battle
{
    [DefaultExecutionOrder(-1)]
    public class BattleManager : MonoBehaviour
    {
        //=== Reference ===
        [BoxGroup("Reference"), SerializeField] private Camera _camera;
        [BoxGroup("Reference"), SerializeField] private SkillSelector _skillButtonManager;
        [BoxGroup("Reference"), SerializeField] private Unit _unitPrefab;
        [BoxGroup("Reference"), SerializeField] private TargetSelector _targetSelector;
        [BoxGroup("Reference"), SerializeField] private IndicatorUIBuilder _indicatorUI;
        [BoxGroup("Reference"), SerializeField] private PlaySounds _sounds;
        [BoxGroup("Reference"), SerializeField] private CharacterTransformHelper _characterTransformHelper;
        [BoxGroup("Reference"), SerializeField] private SceneLoaderAsync _sceneLoaderAsync;
        [BoxGroup("Container"), SerializeField] private Transform _charactersContainer;
        private PositionManager _posManager;
        private TurnManager _turnManager;



        //=== Non-Serialized Fields ===
        private bool _isBattleContinue = false;
        private BoolReactiveProperty _isPause = new BoolReactiveProperty(false);

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField, ReadOnly] private IReadOnlyDictionary<CharacterPosition, Unit> _characterPositions;
#pragma warning restore 0414
#endif

        //=== Properties ===
        public Action<Unit> OnCharacterRemoved { get; set; }
        public TargetSelector TargetSelector => _targetSelector;
        public SkillSelector SkillSelector => _skillButtonManager;
        public bool IsBattleContinue => _isBattleContinue;
        public IObservable<bool> IsPause => _isPause.AsObservable();
        public IndicatorUIBuilder IndicatorUIBuilder => _indicatorUI;

        //=== Public Methods ===
        public void StartBattle(List<PlayerData> players, SpawningTable table)
        {
            Initialize();
            foreach (var item in _charactersContainer)
                Destroy(item as GameObject);

            var p = new List<Unit>();
            for (int i = 0; i < players.Count && i < 4; i++)
            {
                var c = Instantiate(_unitPrefab, Vector3.zero, Quaternion.identity);
                c.InitPlayer(this, players[i]).SetBehaivior(new PlayerBehaviour());
                p.Add(c);
            }
            StartBattle(p, table);
        }
        public void StartBattle(List<Unit> players, SpawningTable table)
        {
            _posManager.Clear();
            _turnManager.Reset();

            for (int i = 0; i < players.Count && i < 4; i++)
            {
                RegisterCharacter((CharacterPosition)i, players[i]);
            }

            for (int i = 0; i < 5; i++)
                SummonEnemy(table.GetSpawnData(0f).GetEnemyData(), (CharacterPosition)i + 4);

            _sounds.Play(PlaySounds.PlaySound.BGM);
            _isBattleContinue = true;
            _indicatorUI.UpdateUI(_posManager.GetCharacters());
            _ = _turnManager.ProcessTurn();
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
            _sounds.Play(PlaySounds.PlaySound.Win);
            EndBattle();
        }
        public void EndBattle_Lost()
        {
            Debug.Log("Lost");
            _sounds.Play(PlaySounds.PlaySound.Lose);
            EndBattle();
        }

        // デフォルトの場合、空いてる場所にスポーンさせる
        public void SummonEnemy(EnemyData enemyData, CharacterPosition characterPosition = CharacterPosition.None)
        {
            if (characterPosition == CharacterPosition.None && !_posManager.TryGetUsablePosition(out characterPosition, Faction.Enemy))
            {
                Debug.Log("モンスターをスポーンさせるために必要なスペースがありません");
                return;
            }
            var c = Instantiate(_unitPrefab, Vector3.zero, Quaternion.identity);
            c.InitEnemy(this, enemyData);

            GraphicalManager.instance.BattleLog.Add($"<color=purple>{enemyData.enemyName}</color>が現れた！", BattleLog.IconType.Normal);
            RegisterCharacter(characterPosition, c);
        }
        // 各派生クラスにキャラクターを登録する
        public void RegisterCharacter(CharacterPosition position, Unit character)
        {
            //var indicator = _indicatorFactory.Create(_characterTransformHelper.GetPosition(position));
            _posManager.RegisterCharacter(position, character);
            character.transform.SetParent(_charactersContainer);
            _characterTransformHelper.SetPosition(character, position);
            Debug.Log($"Starting battle with player: {character.Data.Name} at position: {position}");
        }

        public void RemoveCharacter(Unit character)
        {
            // PositionManager で位置削除
            _posManager.RemoveCharacter(character);
            _turnManager.RemoveCharacter(character);
            OnCharacterRemoved?.Invoke(character);
            CheckBattleEnd();
            Destroy(character.gameObject);
        }

        public bool TryGetUsablePosition(out CharacterPosition position, Faction faction = Faction.All)
        {
            return _posManager.TryGetUsablePosition(out position, faction);
        }

        public List<Unit> GetCharacters()
        {
            return _posManager.GetCharacters();
        }

        public IReadOnlyDictionary<CharacterPosition, Unit> GetCharacterMap()
        {
            return _posManager.GetCharacterMap();
        }

        public float GetDepth()
        {
            return 0.5f;
        }

        public void ApplyDamage(DamageInfo info)
        {
            if (!info.Target.IsAlive) return;
            var resist = info.ResistDamage();
            // ThrowText
            Color c = resist.Elements == Elements.None ? Color.red : resist.Elements.GetColorElement();
            GraphicalManager.instance.Text.Create(resist.Target.gameObject, resist.Damage.ToString(), c);

            // ダメージ適用
            resist.Target.TakeDamage(resist);
        }

        public void ApplyHeal(DamageInfo info)
        {
            Debug.Log($"{info.Target} is Taking heal: {info.Damage} from {info.Source.Data.Name ?? "Unknown"}");
            GraphicalManager.instance.Text.Create(info.Target.gameObject, $"{info.Damage}", Color.green);
            info.Target.TakeHeal(info);
        }

        // === Pause ===

        protected void OnPaused()
        {
            Time.timeScale = 0;
            _isPause.Value = true;
        }

        protected void OnResumed()
        {
            Time.timeScale = 1.0f;
            _isPause.Value = false;
        }
        public async UniTask WaitForPause()
        {
            await UniTask.WaitWhile(() => _isPause.Value);
        } 
        // === Notify ===
        public void OnDeath(Unit character)
        {
            GraphicalManager.instance.BattleLog.Add($"{BattleLog.NameWithColor(character)}が<color=red>死亡</color>した！", BattleLog.IconType.Dead);
            Debug.Log(character.Data.Name + "が死亡した！");
            character.OnDeath?.Invoke(character);
            CheckBattleEnd();
        }

        //=== Private ===

        private void CheckBattleEnd()
        {
            var c = _posManager.AllFactionCount();
            if (c.allies <= 0) EndBattle_Lost();
            if (c.enemies <= 0) EndBattle_Won();
        }

        private void Initialize()
        {
            if (!_charactersContainer)
                _charactersContainer = new GameObject("CharactersContainer").transform;
            // 分割クラスの初期化
            _posManager = new PositionManager(out _characterPositions);
            _turnManager = new TurnManager(this);
        }
        //=== Unity Lifecycle ===
        protected void Awake()
        {
            Initialize();
        }
    }
}
