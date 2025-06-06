using UnityEngine;
using RPG_003.Battle.Skills;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;
using HighElixir.Pool;

namespace RPG_003.Battle
{
    public class MakeSkillButton : MonoBehaviour
    {
        [BoxGroup("Settings")]
        [SerializeField] private SkillButton skillButtonPrefab;
        [BoxGroup("Settings"), PropertyTooltip("シーンビュー上にて配置されている想定")]
        [SerializeField] private Button _confirm;
        [BoxGroup("Settings"), PropertyTooltip("シーンビュー上にて配置されている想定")]
        [SerializeField] private Button _cancel;
        [BoxGroup("Settings")]
        [SerializeField] private Transform _skillButtonContainer;
        private Skill _chosen;
        private Action<Skill> _onConfirmCallback;
        private Pool<SkillButton> _skillButtonPool;
        public void CreateButtons(List<Skill> skills, Action<Skill> onConfirmCallback)
        {
            _onConfirmCallback = onConfirmCallback;
            ResetSkill();
            ReleaseButtons();
            foreach (var skill in skills)
            {
                CreateSkillButton(skill);
            }
            CreateConfirm();
            CreateCancel();
        }
        public void CreateSkillButton(Skill skillData)
        {
            var skillButton = _skillButtonPool?.Get();
            skillButton.Setup(skillData, SetSkill);
        }
        private void CreateConfirm()
        {
            var b = Instantiate(_confirm);
            b.onClick.AddListener(OnConfirm);
        }

        private void CreateCancel()
        {
            var b = Instantiate(_cancel);
            b.onClick.AddListener(OnCancel);
        }
        public void ReleaseButtons()
        {
            foreach (Transform child in _skillButtonContainer)
            {
                _skillButtonPool?.Release(child.GetComponent<SkillButton>());
            }
            _confirm?.gameObject.SetActive(false);
            _cancel?.gameObject.SetActive(false);

        }
        private void SetSkill(Skill skill)
        {
            _chosen = skill;
        }
        private void ResetSkill()
        {
            _chosen = null;
        }
        private void OnConfirm()
        {
            if (_chosen == null) return;
            _onConfirmCallback?.Invoke(_chosen);
            ResetSkill();
            ReleaseButtons();
        }
        private void OnCancel()
        {
            _chosen = null;
        }
        // Unity lifecycle methods
        private void OnEnable()
        {
            ReleaseButtons();
        }
        private void OnDisable()
        {
            ReleaseButtons();
        }
        private void Start()
        {
            if (_confirm?.isActiveAndEnabled == true)
            {
                _confirm?.gameObject.SetActive(false);
            }
            if (_cancel?.isActiveAndEnabled == true)
            {
                _cancel?.gameObject.SetActive(false);
            }
            _chosen = null;
            if (!_skillButtonContainer.TryGetComponent<HorizontalLayoutGroup>(out var _))
            {
                // If the HorizontalLayoutGroup component is not present, add it
                _skillButtonContainer.gameObject.AddComponent<HorizontalLayoutGroup>();
            }
            _skillButtonPool = new Pool<SkillButton>(skillButtonPrefab, 3, _skillButtonContainer, true);

        }
    }
}