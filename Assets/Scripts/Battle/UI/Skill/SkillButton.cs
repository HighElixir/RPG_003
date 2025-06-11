using RPG_003.Battle.Skills;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG_003.Battle
{
    public class SkillButton : MonoBehaviour, ISkillSelectorComponent
    {
        [BoxGroup("Button"), SerializeField] private UnityEngine.UI.Button _button;
        [BoxGroup("Button"), SerializeField] private TMPro.TextMeshProUGUI _buttonText;
        [BoxGroup("Image"), SerializeField] protected SkillSelectorData _data;
        [BoxGroup("Image"), SerializeField] protected Image _image;
        private Action<SkillButton> _onClickAction;
        [SerializeField, ReadOnly] private Skill _skill;

        public bool Selected { get; set; }
        public Skill Skill => _skill;
        public virtual ISkillSelectorComponent Setup(Skill hold, Action<ISkillSelectorComponent> onClick)
        {
            _skill = hold;
            _buttonText.text = _skill.skillName;
            _onClickAction = onClick;
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(OnClick);
            _image.sprite = hold.skillData.Sprite;
            return this;
        }
        public void OnClick()
        {
            if (_skill.IsActive)
                _onClickAction?.Invoke(this);
            else
            {
                var gM = _skill.parent.BattleManager.GraphicalManager;
                var pos = gM.ScreenPointToWorld(transform.position);
                gM.ThrowText(pos, "使用できません！", Color.red);
            }
        }

        public virtual void SetDecorationData(SkillSelectorData data)
        {
            _data = data;
            if (_data.defaultColor == default)
            {
                _data.defaultColor = Color.white;
            }
        }
        public virtual void ChangeColor()
        {
            if (Selected)
            {
                _image.color = _data.selectedColor;
            }
            else
            {
                _image.color = _data.defaultColor;
            }
        }

        private void FixedUpdate()
        {
            if (_skill == null) return;
            if (_skill.IsActive) ChangeColor();
            else
            {
                Selected = false;
                _image.color = _data.inactibeColor;
            }
        }
    }
}