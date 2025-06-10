using RPG_003.Battle.Skills;
using System;
using UnityEngine;

namespace RPG_003.Battle
{
    public interface ISkillSelecter
    {
        Skill Skill { get; }
        ISkillSelecter Setup(Skill item, Action<ISkillSelecter> onClick);
        void SetDecorationData(SkillSelecterData data);
        void SetSelectingState(bool selected);
    }

    [Serializable]
    public struct SkillSelecterData
    {
        public Color selectedColor;
        public Color defaultColor;
    }
}