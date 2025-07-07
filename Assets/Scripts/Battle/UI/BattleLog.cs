using HighElixir;
using HighElixir.Pool;
using RPG_003.Battle.Factions;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG_003.Battle
{
    public class BattleLog : SerializedMonoBehaviour
    {
        public enum IconType
        {
            Normal,
            Positive,
            Negative,
            Dead,
        }
        [BoxGroup("Icon"), SerializeField] private Dictionary<IconType, Sprite> _icons = new();
        [BoxGroup("Pool"), SerializeField] private RectTransform _prefab;
        [BoxGroup("Pool"), SerializeField] private RectTransform _container;
        [BoxGroup("View"), SerializeField] private RectTransform _viewPoint;
        [BoxGroup("View"), SerializeField] private Scrollbar _bar;
        [BoxGroup("View"), SerializeField] private int _max = 30;
        [BoxGroup("View"), SerializeField] private bool _reverse = true;
        private Pool<Transform> _pool;
        private float _height = float.NaN;
        private float _defaulHeight = float.NaN;
        public void Add(string text, IconType icon)
        {
            var g = _pool.Get();
            g.SetParent(_viewPoint);
            foreach (Transform item in g)
            {
                if (item.TryGetComponent<Image>(out var image))
                    image.sprite = _icons[icon];
                if (item.TryGetComponent<TMP_Text>(out var tmp))
                    tmp.SetText(text);
            }
            if (_viewPoint.childCount > _max) ReleaceOld();
            if (float.IsNaN(_defaulHeight))
                _defaulHeight = _viewPoint.rect.height;
            if (float.IsNaN(_height))
                _height = _prefab.GetComponentInChildren<TMP_Text>().GetComponent<RectTransform>().rect.height;
            var temp1 = _height * _viewPoint.childCount;
            var temp2 = _viewPoint.rect;
            if (temp1 >= temp2.height || (temp1 < temp2.height && temp2.height > _defaulHeight))
            {
                temp2.height = temp1;
                _viewPoint.rect.Set(temp2.x, temp2.y, temp2.width, temp2.height);
            }
            else if (temp1 < _defaulHeight)
            {
                _viewPoint.rect.Set(temp2.x, temp2.y, temp2.width, _defaulHeight);
            }
            var n = temp1 / temp2.height;
            if (_reverse) n = 1 - n;
            _bar.value = n;
        }

        public void ReleaceOld()
        {
            _pool.Release(_viewPoint.GetChild(0));
        }

        public static string UseSkill(Skill skill, TargetInfo info)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"<color=#{ColorUtility.ToHtmlStringRGB(skill.parent.Faction.FactionToColor())}>{skill.parent.Data.Name}</color>は");
            sb.Append($"{skill.skillDataInBattle.Name}を放った！");
            return sb.ToString();
        }
        public static string NameWithColor(Unit unit)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(unit.Faction.FactionToColor())}>{unit.Data.Name}</color>";
        }
        [Button("Create")]
        private void Set()
        {
            _icons.Clear();
            foreach (var item in EnumWrapper.GetEnumList<IconType>())
            {
                _icons[item] = null;
            }
        }

        [SerializeField] private string _text;
        [SerializeField] private IconType _icon;
        [Button("Test")]
        private void SetText()
        {
            Awake();
            Add(_text, _icon);
        }
        [Button("Clear")]
        private void Clear()
        {
            foreach (Transform t in _viewPoint)
            {
                _pool.Release(t);
            }
        }
        private void Awake()
        {
            _pool = new Pool<Transform>(_prefab, _max, _container, true);
        }
    }
}