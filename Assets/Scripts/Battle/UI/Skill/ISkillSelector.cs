using RPG_003.Battle.Skills;
using System;
using UnityEngine;

namespace RPG_003.Battle
{
    public interface ISkillSelectorComponent
    {
        Skill Skill { get; }
        ISkillSelectorComponent Setup(Skill item, Action<ISkillSelectorComponent> onClick);
        void SetDecorationData(SkillSelectorData data);
        void SetSelectingState(bool selected);
    }

    [Serializable]
    public struct SkillSelectorData
    {
        public Color selectedColor;
        public Color defaultColor;
    }
}