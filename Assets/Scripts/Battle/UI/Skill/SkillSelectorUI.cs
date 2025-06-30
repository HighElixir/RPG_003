using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RPG_003.Battle
{
    public class SkillSelectorUI : MonoBehaviour
    {
        [BoxGroup("Button"), SerializeField] private Color _usable;
        [BoxGroup("Button"), SerializeField] private Color _unusable;
        [BoxGroup("Button"), SerializeField] private Color _selected;

        public void UpdateUI(List<SkillSelector.ButtonContainer> containers)
        {
            foreach (var item in containers)
            {
                var button = item.Button;
                var data = item.Skill.skillDataInBattle;
                if (item.IsSelected)
                    button.image.color = _selected;
                else if (item.Skill.IsActive)
                    button.image.color = _usable;
                else
                    button.image.color = _unusable;
                if (data.Sprite == null) Debug.Log("asasaa");
                button.image.sprite = data.Sprite;
                button.GetComponentInChildren<TMP_Text>().SetText(data.Name);
            }
        }
    }
}