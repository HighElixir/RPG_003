using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RPG_003.Skills
{
    public class BasicUI : SkillUITemp
    {
        [SerializeField] private GridLayoutGroup _skillMath;
        [SerializeField] private GridLayoutGroup _skillUsed;

        public override void UpdateUI(List<SkillBuilder.DataContainer> containers)
        {
            foreach (var item in containers)
            {
                var parent = item.IsUsed ? _skillUsed.transform : _skillMath.transform;
                item.Button.transform.SetParent(parent, false);
            }
        }
    }
}
