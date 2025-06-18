using HighElixir.Pool;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RPG_003.Battle
{
    public class IndicatorFactory : MonoBehaviour
    {
        // === Reference ===
        [BoxGroup("Reference"), SerializeField] private Camera _camera;
        [BoxGroup("Reference"), SerializeField] private IntervalIndicator _prefab;
        [BoxGroup("Reference"), SerializeField] private CharacterTransformHelper _characterTransformHalper;
        [BoxGroup("Reference"), SerializeField] private Transform _container;
        private Pool<IntervalIndicator> _pool;

        public IntervalIndicator Create(Vector2 position)
        {
            // インターバルインジケータを生成して Canvas に配置
            var screenPoint = RectTransformUtility.WorldToScreenPoint(_camera, position);
            var ind = _pool.Get();
            ind.release = (item) => { _pool.Release(item); };
            ind.transform.position = screenPoint;
            return ind;
        }

        public void Release(IntervalIndicator indicator)
        {
            _pool.Release(indicator);
        }
        private void Awake()
        {
            _pool = new(_prefab, 10, _container, true);
        }
    }
}