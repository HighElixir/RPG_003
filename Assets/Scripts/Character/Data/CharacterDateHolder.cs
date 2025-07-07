using HighElixir;
using RPG_003.Battle;
using RPG_003.Skills;
using RPG_003.Status;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Character
{
    [Serializable]
    public class CharacterDataHolder
    {
        [SerializeField] private string _name;
        [SerializeField] private Sprite _icon;
        [SerializeField] private Dictionary<StatusAttribute, int> _statusPoints = new();
        [SerializeField] private Dictionary<Elements, int> _elementPoints = new();
        [SerializeReference, SubclassSelector] private List<SkillHolder> _skills = new();
        private int _usablePoints = 0;
        [SerializeField] private StatusData _customData;
        [SerializeField] private bool _useCustomData = false;
        public string Name => _name;
        public Sprite Icon => _icon;
        public Dictionary<StatusAttribute, int> StatusPoints => _statusPoints;
        public Dictionary<Elements, int> ElementPoints => _elementPoints;
        public List<SkillHolder> Skills => _skills;
        public int usablePoints => _usablePoints;
        public StatusData ConvertedData => _useCustomData ? _customData : this.CharacterConvart();
        public int IsValid
        {
            get
            {
                if (string.IsNullOrEmpty(_name)) return -1;
                if (_usablePoints >= UsedPoints()) return 0;
                return -2;
            }
        }
        public CharacterDataHolder()
        {
            foreach (var item in EnumWrapper.GetEnumList<StatusAttribute>())
            {
                if (item != StatusAttribute.None)
                    _statusPoints[item] = 0;
            }
            foreach (var item in EnumWrapper.GetEnumList<Elements>())
            {
                if (item != Elements.None)
                    _elementPoints[item] = 0;
            }
        }
        public CharacterDataHolder SetName(string name)
        {
            _name = name;
            return this;
        }

        public CharacterDataHolder SetIcon(Sprite icon)
        {
            _icon = icon;
            return this;
        }
        public CharacterDataHolder SetSkills(List<SkillHolder> skills)
        {
            _skills = skills;
            return this;
        }
        public CharacterDataHolder SetStatusPoint(StatusAttribute attribute, int point)
        {
            _statusPoints[attribute] = point;
            return this;
        }
        public CharacterDataHolder SetElememtPoint(Elements elements, int point)
        {
            _elementPoints[elements] = point;
            return this;
        }
        public CharacterDataHolder SetUsablePoints(int point)
        {
            _usablePoints = point;
            return this;
        }
        public int UsedPoints()
        {
            var res = 0;
            foreach (var element in _elementPoints.Values)
                res += element;
            foreach (var status in _statusPoints.Values)
                res += status;
            return res;
        }
        public PlayerData Convert()
        {
            var pd = new PlayerData(ConvertedData, Skills.ConvertList());
            if (Icon != null)
                pd.SetIcon(Icon);
            return pd;
        }

#if UNITY_EDITOR
        public CharacterDataHolder SetStatus(StatusData status)
        {
            _customData = status;
            return this;
        }
#endif
    }
}