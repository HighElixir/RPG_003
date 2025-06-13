using HighElixir.Pool;
using HighElixir.Utilities;
using RPG_003.Effect;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Battle
{
    public class GraphicalManager : SingletonBehavior<GraphicalManager>
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private SpriteRenderer _background;
        [SerializeField] private EffectPlayer _effectPlayer;
        [SerializeField] private PopText _popText;

        // === BackGround ===
        public void SetBackgroundSprite(Sprite sprite)
        {
            _background.sprite = sprite;
        }
        public void SetBackground(SpriteRenderer background) => _background = background;

        // === EffectPlayer ===
        public IEnumerator EffectPlay(SoundVFXData data, Vector2 vFXposition)
        {
            yield return _effectPlayer.Play(data, vFXposition);
        }

        public IEnumerator EffectPlay(SoundVFXData data, List<Vector2> vFXpositions, bool parallel = true)
        {
            if (vFXpositions == null || vFXpositions.Count == 0)
                yield break;
            if (parallel)
            {


                int remaining = vFXpositions.Count;

                // 各位置でコルーチンをスタート
                foreach (var pos in vFXpositions)
                {
                    // StartCoroutineはMonoBehaviourメソッドとして呼び出すこと
                    StartCoroutine(PlayAndNotify(data, pos, () => remaining--));
                }

                // 全部のremainingが0以下になるまで待機
                yield return new WaitUntil(() => remaining <= 0);
            }
            else
            {
                // 直列再生
                foreach (var pos in vFXpositions)
                {
                    yield return EffectPlay(data, pos);
                }
            }
        }

        // === ThorwText ===
        public void ThrowText(Vector2 position, string text, Color col)
        {
            var c = ColorUtility.ToHtmlStringRGB(col);
            _popText.CreateText(position, $"<color=#{c}>{text}</color>");
        }

        public Vector2 ScreenPointToWorld(Vector3 position)
        {
            return _camera.ScreenToWorldPoint(position);
        }
        private IEnumerator PlayAndNotify(SoundVFXData data, Vector2 pos, Action onComplete)
        {
            yield return _effectPlayer.Play(data, pos);
            onComplete?.Invoke();
        }

        protected override void Awake()
        {
            base.Awake();
        }
    }
}