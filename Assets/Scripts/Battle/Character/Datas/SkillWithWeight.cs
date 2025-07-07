using RPG_003.Battle.Behaviour;
using RPG_003.Effect;
using RPG_003.Skills;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Battle
{
    [CreateAssetMenu(fileName = "SkillWithWeightData", menuName = "RPG_003/Enemy/Behavior/SkillWithWeightData", order = 1)]
    public class SkillWithWeightData : EnemyBehaviorData
    {
        public List<(float weight, BasicData skill, SoundVFXData soundVFXData)> skills = new();

        private float _totalWeight = 0f;
        public override List<Skill> Skills
        {
            get
            {
                return Skill.CreateSkills(ConvertAll());
            }
        }
        public override Skill GetSkill(Unit parent)
        {
            if (skills.Count == 0)
                return null;
            // 重み付きランダム選択
            float randomValue = Random.Range(0f, _totalWeight);
            float cumulativeWeight = 0f;
            foreach (var skill in skills)
            {
                cumulativeWeight += skill.weight;
                if (randomValue <= cumulativeWeight)
                {
                    var skillHolder = new BasicHolder();
                    skillHolder.SetSkillData(skill.skill);
                    skillHolder.SetSoundVFXData(skill.soundVFXData);
                    return new Skill(skillHolder.ConvartData(), parent);
                }
            }
            return null; // ここに到達することはないはず
        }

        public override AIBehavior GetCharacterBehaviour()
        {
            return new AIBehavior(this);
        }

        private List<SkillDataInBattle> ConvertAll()
        {
            var res = new List<SkillDataInBattle>();
            foreach (var skill in skills)
            {
                var tmp = new BasicHolder(skill.skill).ConvartData();
                if (skill.soundVFXData != null) tmp.SetVFX(skill.soundVFXData);
                res.Add(tmp);
            }
            return res;
        }
        private void Awake()
        {
            // 重みの合計を計算
            _totalWeight = 0f;
            foreach (var data in skills)
            {
                _totalWeight += data.weight;
            }
        }
    }
}