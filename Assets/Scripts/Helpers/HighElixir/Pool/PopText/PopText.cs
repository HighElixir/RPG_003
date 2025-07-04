﻿using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HighElixir.Pool
{
    public class PopText : MonoBehaviour
    {
        private class PopEntry
        {
            public TextMeshProUGUI text;
            public RectTransform rect;
            public Color startColor;
            public float elapsed;
        }

        [Header("Reference")]
        [SerializeField] private TextMeshProUGUI _uGUI;
        [SerializeField] private RectTransform _container;
        [SerializeField] private Camera _camera;

        [Header("Data")]
        [SerializeField] private float _duration = 1f;
        [SerializeField] private Vector2 _moveDir = Vector2.up;
        [SerializeField] private float _speed = 50f;
        [SerializeField] private int _maxEntries = 10;
        [SerializeField] private Easing.Ease _ease;

        private Pool<TextMeshProUGUI> _pool;
        private List<PopEntry> _entries = new();
        private Func<float, float> _easeMethod;

        private void Awake()
        {
            // Nullチェック
            if (!_uGUI || !_container || !_camera)
            {
                Debug.LogError("Inspector設定不足！", this);
                enabled = false;
                return;
            }
            _pool = new Pool<TextMeshProUGUI>(_uGUI, 5, _container, true);
            _easeMethod += Easing.GetEasingMethod(_ease);
        }

        public void CreateText(Transform target, string text, TMP_FontAsset font = null, Color textColor = default)
        {
            CreateText(target.position, text, font, textColor);
        }
        public void CreateText(Vector3 position, string text, TMP_FontAsset font = null, Color textColor = default)
        {
            // 上限越えたら一番古いやつを強制リリース
            if (_entries.Count >= _maxEntries)
            {
                var old = _entries[0];
                _entries.RemoveAt(0);
                _pool.Release(old.text);
            }

            var txt = _pool.Get();
            if (textColor != default)
                txt.color = textColor;
            if (font != null)
                txt.font = font;
            txt.text = text;
            var entry = new PopEntry
            {
                text = txt,
                rect = txt.rectTransform,
                startColor = txt.color,
                elapsed = 0f
            };
            // スクリーン→ローカル座標でanchoredPositionセット
            Vector2 screen = RectTransformUtility.WorldToScreenPoint(_camera, position);
            entry.rect.position = screen;
            txt.gameObject.SetActive(true);

            _entries.Add(entry);

        }
        private void Update()
        {
            float dt = Time.deltaTime;
            for (int i = _entries.Count - 1; i >= 0; i--)
            {
                var e = _entries[i];
                e.elapsed += dt;
                float t = e.elapsed / _duration;

                // 移動
                e.rect.anchoredPosition += _moveDir * _speed * dt;
                // フェードアウト
                e.text.color = Color.Lerp(e.startColor, new Color(e.startColor.r, e.startColor.g, e.startColor.b, 0), _easeMethod(t));

                if (e.elapsed >= _duration)
                {
                    e.text.color = Color.white;
                    _pool.Release(e.text);
                    _entries.RemoveAt(i);
                }
            }
        }
    }
}
// unicode