using RPG_003.Effect;
using RPG_003.Skills;
using System;
using UnityEngine;

namespace RPG_003.Battle
{
    [Serializable]
    public class AISkillSet
    {
        public BasicData skill;
        public SoundVFXData sound;
        public TargetSource brain = TargetSource.Random;
        [Range(0, 1)] public float pow; // brainをどの程度の割合で優先するかどうか 1 => 絶対 0 => 使わない（常時ランダム）

        public Skill Convert(Unit parent)
        {
            return new Skill(new BasicHolder(skill).SetSoundVFXData(sound).ConvartData(), parent);
        }
    }

    public enum TargetSource
    {
        Random,     // default
        MinHP,      // 実際のHPが少ないターゲットを狙う
        MaxHP,      // 実際のHPが多いターゲットを狙う
        MinHPRatio, // HPの割合が少ないターゲットを狙う
        MaxHPRatio, // HPの割合が多いターゲットを狙う
    }
}