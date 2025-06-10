using RPG_003.Battle.Behaviour;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Battle.Characters.Enemy
{
    [CreateAssetMenu(fileName = "SkillWithWeightData", menuName = "RPG_003/Enemy/Behavior/SkillWithWeightData", order = 1)]
    public class SkillWithWeightData : EnemyBehaviorData
    {
        public float weight = 1f; // Default weight for the action
        public List<(float weight, EnemySkill skill)> skills = new();

        private float _totalWeight = 0f;
        public override EnemySkill GetSkill()
        {
            if (skills.Count == 0)
                return null;
            // 重み付きランダム選択
            float randomValue = UnityEngine.Random.Range(0f, _totalWeight);
            float cumulativeWeight = 0f;
            foreach (var skill in skills)
            {
                cumulativeWeight += skill.weight;
                if (randomValue <= cumulativeWeight)
                {
                    return skill.skill;
                }
            }
            return null; // ここに到達することはないはず
        }

        public override AIBehavior GetCharacterBehaviour()
        {
            return new AIBehavior(this);
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