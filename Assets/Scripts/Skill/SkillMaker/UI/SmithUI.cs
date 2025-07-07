using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG_003.Skills
{
    public class SmithUI : SkillUITemp
    {
        [SerializeField, BoxGroup("UnUse")] private GridLayoutGroup _slots;
        [SerializeField, BoxGroup("UnUse")] private GridLayoutGroup _effects;
        [SerializeField, BoxGroup("UnUse")] private GridLayoutGroup _costs;
        [SerializeField, BoxGroup("UnUse")] private GridLayoutGroup _targets;
        [SerializeField, BoxGroup("Used")] private GridLayoutGroup _usedSlots;
        [SerializeField, BoxGroup("Used")] private VerticalLayoutGroup _usedEffects;
        [SerializeField, BoxGroup("Used")] private VerticalLayoutGroup _usedCosts;
        [SerializeField, BoxGroup("Used")] private VerticalLayoutGroup _usedTargets;
        //[SerializeField] private RadioButtonGroup

        public override void UpdateUI(List<SkillBuilder.DataContainer> containers)
        {
            foreach (var item in containers)
            {
                if (item.Data.IsSlot())
                    item.Button.transform.SetParent(item.IsUsed ? _usedSlots.transform : _slots.transform, false);
                if (item.Data.IsEffect())
                    item.Button.transform.SetParent(item.IsUsed ? _usedEffects.transform : _effects.transform, false);
                if (item.Data.IsCost())
                    item.Button.transform.SetParent(item.IsUsed ? _usedCosts.transform : _costs.transform, false);
                if (item.Data.IsTarget())
                    item.Button.transform.SetParent(item.IsUsed ? _usedTargets.transform : _targets.transform, false);
            }
        }

    }
}