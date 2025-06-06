using UnityEngine;
using RPG_003.Battle.Skills;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;
using HighElixir.Pool;
using UnityEngine.InputSystem;

namespace RPG_003.Battle
{
    public class MakeSkillButton : MonoBehaviour
    {
        // Settings And References
        [BoxGroup("Settings"), SerializeField] private SkillButton skillButtonPrefab;
        [PropertyTooltip("シーンビュー上にて配置されている想定")]
        [BoxGroup("Settings"), SerializeField] private Button _confirm;
        [PropertyTooltip("シーンビュー上にて配置されている想定")]
        [BoxGroup("Settings"), SerializeField] private Button _cancel;
        [BoxGroup("Settings"), SerializeField] private Transform _canvas;
        [BoxGroup("Settings"), SerializeField] private Transform _container;

        // InputAction
        [BoxGroup("InputActionAsset"), SerializeField] private InputActionAsset _asset;
        [BoxGroup("InputActionAsset"), SerializeField] private string _mapName;
        private InputActionMap _onUI;
        private InputAction _navigate;
        //
        private SkillButton _chosen;
        private Action<Skill> _onConfirmCallback;
        private Pool<SkillButton> _skillButtonPool;
        private List<SkillButton> _skillButtons = new List<SkillButton>();
        private int _idx = 0;
        public void CreateButtons(List<Skill> skills, Action<Skill> onConfirmCallback)
        {
            _onConfirmCallback = onConfirmCallback;
            ResetSkill();
            ReleaseButtons();
            foreach (var skill in skills)
            {
                CreateSkillButton(skill);
            }
            SetSkill(_skillButtons[0]);
            _idx = 0;
            ShowButtons();
        }
        public void CreateSkillButton(Skill skillData)
        {
            var skillButton = _skillButtonPool?.Get();
            _skillButtons.Add(skillButton.Setup(skillData, SetSkill));
        }

        public void ReleaseButtons()
        {
            if (_container.childCount == 0) return;
            foreach (Transform child in _container)
            {
                _skillButtonPool?.Release(child.GetComponent<SkillButton>());
            }
            HideButtons();
            _skillButtons.Clear();
        }
        private void ShowButtons()
        {
            _confirm.onClick.AddListener(OnConfirm);
            _cancel.onClick.AddListener(OnCancel);
            SetButtonsVisible(true);
        }
        private void HideButtons()
        {
            _confirm.onClick.RemoveListener(OnConfirm);
            _cancel.onClick.RemoveListener(OnCancel);
            SetButtonsVisible(false);
        }
        private void SetButtonsVisible(bool isShow)
        {
            _confirm?.gameObject.SetActive(isShow);
            _cancel?.gameObject.SetActive(isShow);
        }
        private void SetSkill(SkillButton skill)
        {
            _chosen?.SetDefaultColor();
            _chosen = skill;
            _chosen.SetSelectedColor();
        }
        private void ResetSkill()
        {
            _chosen?.SetDefaultColor();
            _chosen = null;
        }
        private void OnConfirm()
        {
            if (_chosen == null) return;
            ReleaseButtons();
            _onConfirmCallback?.Invoke(_chosen.GetSkill());
            ResetSkill();
        }
        private void OnCancel()
        {
            ResetSkill();
        }
        // InputAction
        private void OnNavigate(InputAction.CallbackContext callbackContext)
        {
            var v = callbackContext.ReadValue<Vector2>();
            if (v == Vector2.right)
            {
                _idx--;
                if (_idx < 0)
                    _idx = _skillButtons.Count - 1;
                SetSkill(_skillButtons[_idx]);
            }
            else if (v == Vector2.left)
            {
                _idx++;
                if (_idx >= _skillButtons.Count)
                    _idx = 0;
                SetSkill(_skillButtons[_idx]);
            }
        }
        // lifecycle
        private void Awake()
        {
            if (_asset)
            {
                _onUI = _asset.FindActionMap(_mapName);
                _navigate = _onUI.FindAction("Navigate");
            }
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

            _skillButtonPool = new Pool<SkillButton>(skillButtonPrefab, 3, _container, _canvas, true);
        }
        private void OnEnable()
        {
            if (!_onUI.enabled) _onUI.Enable();
            _navigate.performed += OnNavigate;
        }
        private void OnDisable()
        {
            _navigate.performed -= OnNavigate;
        }
    }
}