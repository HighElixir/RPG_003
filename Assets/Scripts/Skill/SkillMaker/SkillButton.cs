using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG_003.Skills
{
    public class SkillButton : MonoBehaviour
    {
        [SerializeField, BoxGroup("Design")] private Image _image;
        [SerializeField, BoxGroup("Design")] private Color _defaultColor;
        [SerializeField, BoxGroup("Design")] private Color _cantClickableColor;
        private Button _button;
        private SkillBuilder _parent;
        private SkillData _skill;
        private bool _active = true;

        public Action<SkillData> OnClickAction { get; private set; }

        public void UpdateButton()
        {
            if (!_skill.IsMatch(_parent.Temp))
            {
                SetActive(false);
                return;
            }

            if (_parent.Temp is SmithHolder smith && _skill is SmithChip chip)
            {
                SetActive(smith.SlotData != null && smith.Load + chip.Load >= smith.SlotData.MaximumLoad);
            }
            if (_parent.Temp is ModifierHolder modifier && _skill is AddonData)
            {
                SetActive(modifier.SkillData != null && modifier.Data.Count > (modifier.SkillData as ModifierData).InstallableAddon);
            }
        }

        public void SetParent(SkillBuilder builder)
        {
            _parent = builder;
        }
        // Private
        private void SetActive(bool active)
        {
            _active = active;
            if (_active)
                _image.color = _defaultColor;
            else
                _image.color = _cantClickableColor;
        }
        private void OnClick()
        {
            if (!_active) return;
            OnClickAction?.Invoke(_skill);
        }
        // === Unity ===
        private void Awake()
        {
            _button = GetComponent<Button>();
            _image = GetComponent<Image>();
        }
        private void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
        }
        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClick);
        }
    }
}