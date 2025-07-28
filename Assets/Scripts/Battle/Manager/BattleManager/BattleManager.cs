using Cysharp.Threading.Tasks;
using RPG_003.Core;
using RPG_003.Helper;
using RPG_003.Skills;
using RPG_003.StatesEffect;
using RPG_003.Status;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace RPG_003.Battle
{
    [DefaultExecutionOrder(-1)]
    public partial class BattleManager : MonoBehaviour
    {
        //=== Reference ===
        [BoxGroup("Reference"), SerializeField] private Camera _camera;
        [BoxGroup("Reference"), SerializeField] private SkillSelector _skillButtonManager;
        [BoxGroup("Reference"), SerializeField] private Unit _unitPrefab;
        [BoxGroup("Reference"), SerializeField] private TargetSelector _targetSelector;
        [BoxGroup("Reference"), SerializeField] private IndicatorUIBuilder _indicatorUI;
        [BoxGroup("Reference"), SerializeField] private PlaySounds _sounds;
        [BoxGroup("Reference"), SerializeField] private CharacterTransformHelper _characterTransformHelper;
        [BoxGroup("Reference"), SerializeField] private SceneLoader _loader;
        
        private Transform _charactersContainer;
        private PositionManager _posManager;
        private TurnManager _turnManager;
        private bool _isBattleContinue = false;

        //=== Properties ===
        public Action<Unit> OnCharacterRemoved { get; set; }
        public TargetSelector TargetSelector => _targetSelector;
        public SkillSelector SkillSelector => _skillButtonManager;
        public bool IsBattleContinue => _isBattleContinue;
        public IndicatorUIBuilder IndicatorUIBuilder => _indicatorUI;
        public CharacterTransformHelper TransformHelper => _characterTransformHelper;

        //=== Public Methods ===
        public void StartBattle(List<PlayerData> players, SpawningTable table)
        {
            Initialize();
            foreach (var item in _charactersContainer)
                Destroy(item as GameObject);

            var p = SummonPlayers(players);
            StartBattle(p, table);
        }
        public void StartBattle(List<Unit> players, SpawningTable table)
        {
            _posManager.Clear();
            _turnManager.Reset();

            for (int i = 0; i < players.Count && i < 4; i++)
                RegisterCharacter((CharacterPosition)i, players[i]);

            for (int i = 0; i < 5; i++)
                SummonEnemy(table.GetSpawnData(0f).GetEnemyData());

            _sounds.Play(PlaySounds.PlaySound.BGM);
            _isBattleContinue = true;
            _indicatorUI.UpdateUI(_posManager.GetCharacters());
            _ = _turnManager.ProcessTurn();
        }
        private async UniTask BattleExit(int to = -1)
        {
            for(int i = 3; i >= 0; i--)
            {
                GraphicalManager.instance.Text.Create(new Vector2(0, 0), i.ToString(), Color.white);
                await UniTask.WaitForSeconds(1);
            }
            if (to == -1)
            {
                BattleSceneManager.instance.BackScene();
            }
            else
            {
                _loader.SceneLoad(to);
            }
        }

        // Wave進行度(未実装)
        public float GetDepth()
        {
            return 0.5f;
        }

        public void ApplyDamage(DamageInfo info)
        {
            if (!info.Target.IsAlive) return;
            var resist = info.ResistDamage();
            // ダメージを表示
            GraphicalManager.instance.ThrowDamageInfo(resist);

            // ダメージ適用
            resist.Target.TakeDamage(resist);
        }

        public void ApplyHeal(DamageInfo info)
        {
            Debug.Log($"{info.Target} is Taking heal: {info.Damage} from {info.Source.Data.Name ?? "Unknown"}");
            GraphicalManager.instance.Text.Create(info.Target.gameObject, $"{info.Damage}", Color.green);
            info.Target.TakeHeal(info);
        }

        public void ApplyEffect(IStatesEffect effect, Unit target, Unit source = null)
        {
            if (target == null || !target.IsAlive) return;
            // エフェクトの適用
            target.EffectController.AddEffect(effect);
            GraphicalManager.instance.BattleLog.Add(
                StringRules.TransTextSourceToTarget(effect.OnAddedMessage, source, target),
                effect.IsPositive ? BattleLog.IconType.Positive : BattleLog.IconType.Negative
                );
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
        private void Initialize()
        {
            if (!_charactersContainer)
                _charactersContainer = new GameObject("CharactersContainer").transform;
            // 分割クラスの初期化
            _posManager = new PositionManager();
            _turnManager = new TurnManager(this);
        }
        //=== Unity Lifecycle ===
        protected void Awake()
        {
            Initialize();
        }
    }
}