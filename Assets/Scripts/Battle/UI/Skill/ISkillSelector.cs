using RPG_003.Battle.Skills;
using System;
using UnityEngine;

namespace RPG_003.Battle
{
    public interface ISkillSelectorComponent
    {
        public bool Selected { get; set; }
        Skill Skill { get; }
        ISkillSelectorComponent Setup(Skill item, Action<ISkillSelectorComponent> onClick);
        void SetDecorationData(SkillSelectorData data);
    }

    [Serializable]
    public struct SkillSelectorData
    {
        public Color selectedColor;
        public Color defaultColor;
        public Color inactibeColor;
    }
}