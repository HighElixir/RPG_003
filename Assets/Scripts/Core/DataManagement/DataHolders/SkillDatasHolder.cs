using RPG_003.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace RPG_003.Core
{
    [Serializable]
    public class SkillDataHolder
    {
        [SerializeField]
        private Dictionary<SkillData, int> _skillDict = new();

        public float GetCount(SkillData data) => _skillDict[data];

        public void AddCount(SkillData equip, int count)
        {
            if (_skillDict.ContainsKey(equip))
                _skillDict[equip] += count;
            else
                _skillDict.Add(equip, count);
        }
        public void RemoveCount(SkillData equip, int count)
        {
            if (_skillDict.ContainsKey(equip))
                _skillDict[equip] -= count;
        }
        public void Save(Dictionary<SkillData, int> dict)
        {
            _skillDict.Clear();
            foreach (var kvp in dict)
            {
                if (kvp.Value > 0)
                    _skillDict[kvp.Key] = kvp.Value;
            }
        }
        public Dictionary<SkillData, int> GetAllSkills()
        {
            return new Dictionary<SkillData, int>(_skillDict);
        }
    }
}
