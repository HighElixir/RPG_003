using RPG_003.Skills;
using System.Collections.Generic;
using System.Linq;

namespace RPG_003.Core
{
    public class SkillDatasHolder
    {
        public class DataSet
        {
            public List<(SkillData data, int count)> values = new();
            public int AllCount
            {
                get
                {
                    var res = 0;
                    foreach (var item in values)
                    {
                        res += item.count;
                    }
                    return res;
                }
            }
        }
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

        public DataSet GetSkillsByType(SkillType type)
        {
            var res = new DataSet();
            foreach(var skill in _skillDict)
            {
                if (skill.Key.IsMatch(type))
                    res.values.Add((skill.Key, skill.Value));
            }
            return res;
        }
    }
}
