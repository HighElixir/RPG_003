using RPG_003.Battle.Skills;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG_003.Battle
{
    public class SkillButton : MonoBehaviour, ISkillSelecter
    {
        [BoxGroup("Button"), SerializeField] private UnityEngine.UI.Button _button;
        [BoxGroup("Button"), SerializeField] private TMPro.TextMeshProUGUI _buttonText;
        [BoxGroup("Image"), SerializeField] protected SkillSelecterData _data;
        [BoxGroup("Image"), SerializeField] protected Image _image;
        private Action<SkillButton> _onClickAction;
        [SerializeField, ReadOnly] private Skill _skill;

        public Skill Skill => _skill;
        public virtual ISkillSelecter Setup(Skill hold, Action<ISkillSelecter> onClick)
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

        public virtual void SetDecorationData(SkillSelecterData data)
        {
            _data = data;
            if (_data.defaultColor == default)
            {
                _data.defaultColor = Color.white;
            }
        }
        public virtual void SetSelectingState(bool selected)
        {
            if (selected)
            {
                _image.color = _data.selectedColor;
            }
            else
            {
                _image.color = _data.defaultColor;
            }
        }
    }
}