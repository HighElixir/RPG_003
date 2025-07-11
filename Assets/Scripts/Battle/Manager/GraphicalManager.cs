using Cysharp.Threading.Tasks;
using HighElixir;
using HighElixir.UI;
using RPG_003.Effect;
using RPG_003.Status;
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
            await _effectPlayer.Play(data, vFXposition);
        }

        public async UniTask EffectPlay(SoundVFXData data, List<Vector2> vFXpositions, bool parallel = true)
        {
            if (vFXpositions == null || vFXpositions.Count == 0) return;
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
        }

        // === ThorwText ===
        public void ThrowDamageInfo(DamageInfo info)
        {
            Color c = info.Elements == Elements.None ? Color.red : info.Elements.GetColorElement();
            var t = _thrower.Create(info.Target.gameObject, info.Damage.ToString(), c);
            if (info.IsCritical)
                t.rectTransform.localScale = t.rectTransform.localScale * 1.5f;
        }
    }
}