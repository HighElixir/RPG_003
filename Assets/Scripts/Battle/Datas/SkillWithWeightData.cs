using RPG_003.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RPG_003.Battle
{
    // Excelのフォーマット
    // スキルのID: 重み（float）,
    // E1: 1.0, E2: 0.7, E3: 1.2
    [Serializable]
    public class SkillWithWeightData : ISkillBehaviour
    {
        [Serializable]
        public class SkillWeight
        {
            public float weight = 1;
            public AISkillSet skillSet;
        }
        [SerializeField]
        private List<SkillWeight> _skills = new();

        private float _totalWeight = 0f;
        public List<Skill> Skills
        {
            get
            {
                return Skill.CreateSkills(ConvertAll());
            }
        }
        public AISkillSet GetSkill(Unit parent)
        {
            if (_skills.Count == 0)
                return null;
            // 重み付きランダム選択
            float randomValue = Random.Range(0f, _totalWeight);
            float cumulativeWeight = 0f;
            foreach (var skill in _skills)
            {
                cumulativeWeight += skill.weight;
                if (randomValue <= cumulativeWeight)
                {
                    return skill.skillSet;
                }
            }
            return null; // ここに到達することはないはず
        }

        private List<SkillDataInBattle> ConvertAll()
        {
            var res = new List<SkillDataInBattle>();
            foreach (var skill in _skills)
            {
                var tmp = new BasicHolder(skill.skillSet.skill).ConvartData();
                if (skill.skillSet.sound != null) tmp.SetVFX(skill.skillSet.sound);
                res.Add(tmp);
            }
            return res;
        }
        public void Initialize()
        {
            // 重みの合計を計算
            _totalWeight = 0f;
            foreach (var data in _skills)
            {
                _totalWeight += data.weight;
            }
        }
    }
}