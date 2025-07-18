﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace HighElixir.Pool
{
    public class Pool<T> where T : Component
    {
        private readonly T _original;
        private readonly int _maxPoolSize;
        private readonly Stack<T> _available = new();
        private readonly Transform _container;
        private readonly HashSet<T> _inUse = new();

        public Transform Container => _container;
        public List<T> InUse => new List<T>(_inUse);
        public Pool(T original, int maxPoolSize, Transform container = null, bool isRect = false)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));
            if (maxPoolSize <= 0) throw new ArgumentOutOfRangeException(nameof(maxPoolSize));

            _original = original;
            _maxPoolSize = maxPoolSize;
            _container = container;
            if (isRect && !_container.TryGetComponent<RectTransform>(out var _))
            {
                var r = _container.gameObject.AddComponent<RectTransform>();
                r.anchoredPosition = Vector3.zero;
            }
            for (int i = 0; i < maxPoolSize; i++)
            {
                var obj = GameObject.Instantiate(_original, _container.transform);
                obj.gameObject.SetActive(false);
                _available.Push(obj);
            }
        }

        public T Get()
        {
            T obj = _available.Count > 0
                ? _available.Pop()
                : GameObject.Instantiate(_original, _container.transform);

            if (!_inUse.Add(obj))
                Debug.LogWarning($"{obj.name} がすでにプール中にあります", obj);

            obj.gameObject.SetActive(true);
            return obj;
        }

        public void Release(T obj)
        {
            if (obj == null) return;
            if (_available.Contains(obj)) return;

            if (_inUse.Remove(obj))
            {
                obj.gameObject.SetActive(false);
                obj.transform.SetParent(_container, false);
                if (_available.Count < _maxPoolSize)
                    _available.Push(obj);
                else
                    GameObject.Destroy(obj.gameObject);
            }
            else
            {
                Debug.LogWarning($"{obj.name} はプール外のオブジェクトです", obj);
            }
        }
        public void Dispose()
        {
            foreach (var obj in _available)
            {
                UnityEngine.Object.Destroy(obj);
            }
            foreach (var obj in _inUse)
            {
                UnityEngine.Object.Destroy(obj);
            }
        }
    }

}
// unicode