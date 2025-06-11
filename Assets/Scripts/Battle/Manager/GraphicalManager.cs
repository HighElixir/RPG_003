using RPG_003.Effect;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Battle
{
    public class GraphicalManager : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _background;
        [SerializeField] private EffectPlayer _effectPlayer;

        public void SetBackground(Sprite sprite)
        {
            _background.sprite = sprite;
        }

        public IEnumerator EffectPlay(SoundVFXData data, Vector2 vFXposition)
        {
            yield return _effectPlayer.Play(data, vFXposition);
        }

        // オーバーロードのほう
        public IEnumerator EffectPlay(SoundVFXData data, List<Vector2> vFXpositions, bool parallel = true)
        {
            // 配列が空なら何もしない
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

        // ヘルパーコルーチン：再生後にコールバック
        private IEnumerator PlayAndNotify(SoundVFXData data, Vector2 pos, Action onComplete)
        {
            yield return _effectPlayer.Play(data, pos);
            onComplete?.Invoke();
        }

    }
}