using System;
using UnityEngine;
using UnityEngine.Events;
using RPG_003.Battle.Skills;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace RPG_003.Battle
{
    public class SkillButton : MonoBehaviour
    {
        [BoxGroup("Button"), SerializeField] private UnityEngine.UI.Button _button;
        [BoxGroup("Button"), SerializeField] private TMPro.TextMeshProUGUI _buttonText;
        [BoxGroup("Image"), SerializeField] private Color _selectedColor;
        [BoxGroup("Image"), SerializeField] private Image _image;
        private Action<SkillButton> _onClickAction;
        [SerializeField, ReadOnly] private Skill _skill;
        private Color _default;
        public SkillButton Setup(Skill hold, Action<SkillButton> onClick)
        {
            _skill = hold;
            _buttonText.text = _skill.skillName;
            _onClickAction = onClick;
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(OnClick);
            _image.sprite = hold.skillData.Sprite;
            return this;
        }
        private void OnClick()
        {
            _onClickAction?.Invoke(this);
        }
        public Skill GetSkill()
        {
            return _skill;
        }

        public void SetSelectedColor()
        {
            _image.color = _selectedColor;
        }
        public void SetDefaultColor()
        {
            _image.color = _default;
        }

        private void Awake()
        {
            _default = _image.color;
        }
    }
}