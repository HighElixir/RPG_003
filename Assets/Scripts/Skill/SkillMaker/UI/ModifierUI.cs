using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RPG_003.Skills
{
    public class ModifierUI : SkillUITemp
    {
        [SerializeField] private GridLayoutGroup _modifiers;
        [SerializeField] private GridLayoutGroup _addons;
        [SerializeField] private GridLayoutGroup _usedModifiers;
        [SerializeField] private GridLayoutGroup _usedAddons;
        public override void UpdateUI(List<SkillBuilder.DataContainer> containers)
        {
            foreach (var item in containers)
            {
                if (item.Data.IsAddon())
                    item.Button.transform.SetParent(item.IsUsed ? _usedAddons.transform : _addons.transform, false);
                if (item.Data.IsModifier())
                    item.Button.transform.SetParent(item.IsUsed ? _usedModifiers.transform : _modifiers.transform, false);
            }
        }
    }
}