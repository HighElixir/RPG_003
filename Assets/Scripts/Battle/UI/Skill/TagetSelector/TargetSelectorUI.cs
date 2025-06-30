using DG.Tweening;
using HighElixir.Pool;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace RPG_003.Battle
{
    public class TargetSelectorUI : MonoBehaviour
    {
        private class ObjectContainer
        {
            public Transform target;    // 追従対象
            public Transform obj;       // プールから取ってきたオブジェクト
            public Vector2 offset;      // 各ポインタ専用オフセット
            public Tween tween;         // DOTween の Tween
        }

        [BoxGroup("Pool"), SerializeField] private Transform _prefab;
        [BoxGroup("Pool"), SerializeField] private Transform _container;
        [BoxGroup("Pool"), SerializeField] private int _poolSize;
        [BoxGroup("Option"), SerializeField] private Vector2 _defaultOffset;
        [BoxGroup("Option"), SerializeField] private Vector2 _range = new Vector2(-0.25f, 0.25f);
        [BoxGroup("Option"), SerializeField, Min(0.1f)] private float _bobbingDuration = 1f;
        [BoxGroup("Option"), SerializeField] private float _xOffsetStep = 0.1f; // 同一ターゲット時のX軸ずらし量

        private Pool<Transform> _pool;
        private List<ObjectContainer> _pointers = new();

        /// <summary>
        /// ポインタ生成。offset を指定しない場合はデフォルトを使い、同一ターゲットならX軸をずらす
        /// </summary>
        public void CreatePoint(Transform target, Vector2? offset = null)
        {
            // ベースオフセット
            Vector2 baseOffset = offset ?? _defaultOffset;
            // 同じターゲットに既存のポインタ数をカウント
            int sameCount = _pointers.Count(pc => pc.target == target);
            // X軸をずらして最終オフセットを決定
            Vector2 finalOffset = new Vector2(baseOffset.x + sameCount * _xOffsetStep, baseOffset.y);

            var pc = new ObjectContainer
            {
                target = target,
                offset = finalOffset,
                obj = _pool.Get()
            };

            // 最初の配置
            var startPos = target.position + (Vector3)pc.offset;
            pc.obj.position = startPos;

            // DOVirtual.Float に直接コールバックを渡す
            pc.tween = DOVirtual.Float(
                    _range.x,
                    _range.y,
                    _bobbingDuration,
                    y =>
                    {
                        var p = pc.target.position + (Vector3)pc.offset;
                        p.y += y;
                        pc.obj.position = p;
                    }
                )
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);

            _pointers.Add(pc);
        }

        /// <summary>
        /// 必要なくなったら呼んでね
        /// </summary>
        public void RemovePoint(Transform obj)
        {
            var pc = _pointers.Find(x => x.obj == obj);
            if (pc != null) RemoveDirect(pc);
        }

        public void RemovePoint(GameObject target)
        {
            var pc = _pointers.Find(x => x.target == target.transform);
            if (pc != null) RemoveDirect(pc);
        }

        private void RemoveDirect(ObjectContainer container)
        {
            container.tween.Kill();
            _pointers.Remove(container);
            _pool.Release(container.obj);
        }

        public void RemoveAll()
        {
            foreach (var pc in _pointers)
            {
                pc.tween.Kill();
                _pool.Release(pc.obj);
            }
            _pointers.Clear();
        }

        private void Awake()
        {
            _pool = new Pool<Transform>(_prefab, _poolSize, _container);
        }

        private void OnDestroy()
        {
            RemoveAll(); // 念のため全リソース解放
        }
    }
}
