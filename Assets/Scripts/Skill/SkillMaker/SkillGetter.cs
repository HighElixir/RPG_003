using RPG_003.Core;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Skills
{
    [DefaultExecutionOrder(-5)]
    public class SkillGetter : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private List<BasicData> _basics = new();
        [SerializeField] private List<ModifierData> _modifiers = new();
        [SerializeField] private List<AddonData> _addons = new();
        [SerializeField] private List<SkillSlotData> _slots = new();
        [SerializeField] private List<EffectChip> _effects = new();
        [SerializeField] private List<CostChip> _costs = new();
        [SerializeField] private List<TargetChip> _targets = new();
        [SerializeField] private bool _loadOnAwake = true;
#endif
        private Dictionary<SkillData, int> _datas = new();
        private bool _isLoaded = false;

        public Dictionary<SkillData, int> Datas => _datas;
        public void Save()
        {
            GameDataHolder.instance.SkillDatas.Save(_datas);
        }
        public void Load()
        {
            Debug.Log("Loading Skill Data...");
            _datas = GameDataHolder.instance.SkillDatas.GetAllSkills();
            _isLoaded = true;
        }

        public void Add(SkillData data, int count)
        {
            if (_datas.ContainsKey(data))
                _datas[data] += count;
            else
                _datas.Add(data, count);
        }

        public void Remove(SkillData data, int count)
        {
            if (_datas.ContainsKey(data))
            {
                _datas[data] -= count;
                if (_datas[data] <= 0)
                    _datas.Remove(data);
            }
        }

        public void Set(SkillData data, int count, bool ignoreToNotFound = true)
        {
            if (_datas.ContainsKey(data))
            {
                if (count <= 0)
                    _datas.Remove(data);
                else
                    _datas[data] = count;
            }
            else if (ignoreToNotFound && count > 0)
            {
                _datas.Add(data, count);
            }
        }

        public Dictionary<SkillData, int> GetAll()
        {
            if (!_isLoaded)
            {
                Load(); // Load if the dictionary is empty
            }
            return new Dictionary<SkillData, int>(_datas);
        }

        public Dictionary<SkillData, int> GetByType(SkillMaker.SkillType type)
        {
            if (!_isLoaded)
            {
                Load(); // Load if the dictionary is empty
            }
            var res = new Dictionary<SkillData, int>();
            foreach (var skill in _datas)
            {
                if (skill.Key.IsMatch(type))
                    res[skill.Key] = skill.Value;
            }
            return res;
        }
#if UNITY_EDITOR
        private void Awake()
        {
            if (!_loadOnAwake) return;
            SetSkillsByType(_basics);
            SetSkillsByType(_modifiers);
            SetSkillsByType(_addons);
            SetSkillsByType(_slots);
            SetSkillsByType(_effects);
            SetSkillsByType(_costs);
            SetSkillsByType(_targets);
            _isLoaded = true;
        }
        private void SetSkillsByType<T>(List<T> target) where T : SkillData
        {
            foreach (var skill in target)
            {
                if (_datas.ContainsKey(skill))
                    _datas[skill]++;
                else
                    _datas.Add(skill, 1);
            }
        }
#endif
    }
}