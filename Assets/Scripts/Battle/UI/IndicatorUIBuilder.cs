using HighElixir.Pool;
using RPG_003.Status;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG_003.Battle
{
    public class IndicatorUIBuilder : MonoBehaviour
    {
        public class Container
        {
            public Image image;
            public TMP_Text text;
            public CharacterObject character;
            public StatusAmount status;
            public float interval;
            public Container(Image image, CharacterObject character, TMP_Text text, float interval = 999)
            {
                this.image = image;
                this.character = character;
                this.interval = interval;
                this.text = text;
            }
        }
        // === Reference ===
        [BoxGroup("Pool"), SerializeField] private Image _prefab; // 子オブジェクトにTMP_Textつけて
        [BoxGroup("Pool"), SerializeField] private Transform _container;
        [BoxGroup("UI"), SerializeField] private HorizontalLayoutGroup _layoutGroup;
        private Pool<Image> _pool;
        private List<Container> _containers = new List<Container>();

        public void Release(Container c)
        {
            _pool.Release(c.image);
            _containers.Remove(c);
        }
        public void ReleaseAll()
        {
            foreach (Container c in _containers)
            {
                _pool.Release(c.image);
            }
            _containers.Clear();
        }
        public void UpdateUI(List<CharacterObject> characters)
        {
            ReleaseAll();
            foreach (CharacterObject character in characters)
            {
                var interval = character.BehaviorIntervalCount.CurrentAmount;
                var image = _pool.Get();
                var t = image.GetComponentInChildren<TMP_Text>();
                image.sprite = character.Icon;
                image.transform.SetParent(_layoutGroup.transform);
                t.SetText(interval.ToString());
                _containers.Add(new(image, character, t, interval));
            }

            var l = _containers.OrderBy(c => c.interval).ToArray();
            for (int i = 0; i < l.Length; i++)
            {
                l[i].image.transform.SetSiblingIndex(i);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutGroup.GetComponent<RectTransform>());
        }
        // Unity
        private void Awake()
        {
            _pool = new(_prefab, 10, _container, true);
        }
    }
}