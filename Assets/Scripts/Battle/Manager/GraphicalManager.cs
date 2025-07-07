using Cysharp.Threading.Tasks;
using HighElixir.UI;
using HighElixir.Utilities;
using RPG_003.Effect;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Battle
{
    public class GraphicalManager : SingletonBehavior<GraphicalManager>
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private SpriteRenderer _background;
        [SerializeField] private EffectPlayer _effectPlayer;
        [SerializeField] private TextThrower _thrower;
        [SerializeField] private BattleLog _battleLog;
        [SerializeField, ReadOnly] private bool _isPlaying = false;

        public TextThrower Text => _thrower;
        public BattleLog BattleLog => _battleLog;
        // === BackGround ===
        public void SetBackgroundSprite(Sprite sprite)
        {
            _background.sprite = sprite;
        }
        public void SetBackground(SpriteRenderer background) => _background = background;

        // === EffectPlayer ===
        public async UniTask EffectPlay(SoundVFXData data, Vector2 vFXposition)
        {
            _isPlaying = true;
            await _effectPlayer.Play(data, vFXposition);
            _isPlaying = false;
        }

        public async UniTask EffectPlay(SoundVFXData data, List<Vector2> vFXpositions, bool parallel = true)
        {
            if (vFXpositions == null || vFXpositions.Count == 0) return;
            _isPlaying = true;
            if (parallel)
            {
                List<UniTask> tasks = new List<UniTask>();
                // 各位置でコルーチンをスタート
                foreach (var pos in vFXpositions)
                {
                    tasks.Add(_effectPlayer.Play(data, pos));
                }

                // 全部のremainingが0以下になるまで待機
                await UniTask.WhenAll(tasks);
            }
            else
            {
                // 直列再生
                foreach (var pos in vFXpositions)
                {
                    await _effectPlayer.Play(data, pos);
                }
            }
            _isPlaying = false;
        }

        // === ThorwText ===
        public void ThrowText(Vector2 position, string text, Color col)
        {
            var c = ColorUtility.ToHtmlStringRGB(col);
            _thrower.Create(position, text, col);
        }
        protected override void Awake()
        {
            base.Awake();
        }
    }
}