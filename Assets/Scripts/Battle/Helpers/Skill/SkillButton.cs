using System;
using UnityEngine;
using UnityEngine.Events;
using RPG_003.Battle.Skills;

namespace RPG_003.Battle
{
    public class SkillButton : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Button button;
        [SerializeField] private TMPro.TextMeshProUGUI skillNameText;
        private Action<Skill> onClickAction;
        private Skill skill;
        public void Setup(Skill hold, Action<Skill> onClick)
        {
            skill = hold;
            skillNameText.text = skill.skillName;
            onClickAction = onClick;
            button.onClick.AddListener(OnClick);
        }
        private void OnClick()
        {
            onClickAction?.Invoke(skill);
        }
    }
}