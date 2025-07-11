using RPG_003.Effect;
using RPG_003.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RPG_003.Battle
{
    [Serializable]
    public class SkillWithWeightData : ISkillBehaviour
    {
        [Serializable]
        public class SkillWeight
        {
            public float weight = 1;
            public BasicData skill;
            public SoundVFXData sound;
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
        public Skill GetSkill(Unit parent)
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
                    var skillHolder = new BasicHolder();
                    skillHolder.SetSkillData(skill.skill);
                    skillHolder.SetSoundVFXData(skill.sound);
                    return new Skill(skillHolder.ConvartData(), parent);
                }
            }
            return null; // ここに到達することはないはず
        }

        private List<SkillDataInBattle> ConvertAll()
        {
            var res = new List<SkillDataInBattle>();
            foreach (var skill in _skills)
            {
                var tmp = new BasicHolder(skill.skill).ConvartData();
                if (skill.sound != null) tmp.SetVFX(skill.sound);
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