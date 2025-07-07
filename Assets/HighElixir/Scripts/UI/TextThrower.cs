using DG.Tweening;
using HighElixir.Pool;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HighElixir.UI
{
    public class TextThrower : MonoBehaviour
    {
        [Header("Canvas")]
        [SerializeField] private Canvas uiCanvas;           // インスペクターでセット
        [Header("Camera")]
        [SerializeField] private Camera _camera;

        [Header("Pool")]
        [SerializeField] private TMP_Text _prefab;
        [SerializeField] private int _poolSize = 15;
        [SerializeField] private RectTransform _container;

        [Header("Options")]
        [SerializeField] private Vector3 _endPosDelta = new Vector3(0, 2.5f, 0);
        [SerializeField] private float _duration = 1f;

        private Pool<TMP_Text> _pool;
        private Dictionary<TMP_Text, Sequence> _sequences;

        private RectTransform UiParentRect => uiCanvas.transform as RectTransform;

        public void Create(GameObject go, string text, Color color)
        {
            Vector2 localPos;
            // UI 要素ならそのままアンカー位置を使う
            if (go.TryGetComponent<RectTransform>(out var rt))
            {
                localPos = rt.anchoredPosition;
            }
            else
            {
                // ワールド位置→スクリーン座標
                Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(_camera, go.transform.position);
                // スクリーン座標→UI ローカル座標
                bool ok = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    UiParentRect,
                    screenPos,
                    uiCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _camera,
                    out localPos
                );
                if (!ok)
                    Debug.LogWarning("スクリーン→ローカル変換に失敗したよ！");
            }

            // あとはこれまで通り
            Create(localPos, Quaternion.identity, text, color);
        }

        // Vector2 版
        public void Create(Vector2 pos, Quaternion rotation, string text, Color color)
        {
            // プールから取得＆初期化
            var t = _pool.Get();  // プールから Text コンポ使ってる想定
            var r = t.GetComponent<RectTransform>();
            r.SetParent(UiParentRect, false);
            r.anchoredPosition = pos;
            r.rotation = rotation;
            t.text = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{text}</color>";
            t.color = new Color(t.color.r, t.color.g, t.color.b, 1f);

            // 動かす
            var seq = DOTween.Sequence();
            seq.Append(r.DOAnchorPos3D((Vector3)pos + _endPosDelta, _duration).SetEase(Ease.Linear))
               .Join(t.DOFade(0f, _duration))
               .OnComplete(() => Release(t));

            _sequences[t] = seq;
        }

        public void Create(Vector2 pos, string text, Color color) => Create(pos, Quaternion.identity, text, color);
        public void Release(TMP_Text text)
        {
            // Tween を止めて辞書から削除、プールに返却
            if (_sequences.TryGetValue(text, out var seq))
            {
                seq.Kill();
                _sequences.Remove(text);
            }
            _pool.Release(text);
        }
        private void Awake()
        {
            if (_container == null) _container = transform.GetComponent<RectTransform>();
            _pool = new Pool<TMP_Text>(_prefab, _poolSize, _container, true);
            _sequences = new Dictionary<TMP_Text, Sequence>();
            //Debug.Log("aaaaaa");
        }
        private void OnDestroy()
        {
            // 破棄時に全 Sequence を Kill
            foreach (var seq in _sequences.Values) seq.Kill();
            _sequences.Clear();
        }
    }
}
