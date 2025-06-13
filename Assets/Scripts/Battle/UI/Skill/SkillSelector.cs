using HighElixir.Pool;
using RPG_003.Battle.Skills;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace RPG_003.Battle
{
    public class SkillSelector : MonoBehaviour
    {
        // === Settings And References ===
        [BoxGroup("Reference"), SerializeField] private SkillButton skillButtonPrefab;
        [PropertyTooltip("シーンビュー上にて配置されている想定")]
        [BoxGroup("Reference"), SerializeField] private Button _confirm;
        [PropertyTooltip("シーンビュー上にて配置されている想定")]
        [BoxGroup("Reference"), SerializeField] private Button _cancel;
        [BoxGroup("Reference"), SerializeField] private Transform _container;

        // === InputAction ===
        [BoxGroup("InputActionAsset"), SerializeField] private InputActionAsset _asset;
        [BoxGroup("InputActionAsset"), SerializeField] private string _mapName = "UI";
        [BoxGroup("InputActionAsset"), SerializeField] private string _actionName = "Navigate";
        private InputActionMap _onUI;
        private InputAction _navigate;

        // === Date ===
        private SkillButton _chosen;
        private Action<Skill> _onConfirmCallback;
        private Pool<SkillButton> _skillButtonPool;
        private List<SkillButton> _skillButtons = new List<SkillButton>();
        private int _idx = 0;

        // === Public Methodes ===
        public void CreateButtons(List<Skill> skills, Action<Skill> onConfirmCallback)
        {
            _onConfirmCallback = onConfirmCallback;
            ResetSkill();
            ReleaseButtons();
            foreach (var skill in skills)
            {
                CreateSkillButton(skill);
            }
            Debug.Log(_skillButtons.Count);
            SetSkill(_skillButtons[0]);
            _idx = 0;
            ShowButtons();
            EnableAction();
        }
        public void CreateSkillButton(Skill SkillDataInBattle)
        {
            var skillButton = _skillButtonPool?.Get();
            if (skillButton == null)
            {
                Debug.LogError("SkillDataInBattle is null");
            }
            _skillButtons.Add(skillButton.Setup(SkillDataInBattle, SetSkill) as SkillButton);
        }

        public void ReleaseButtons()
        {
            if (_container.childCount == 0) return;
            foreach (Transform child in _container)
            {
                if (child.TryGetComponent<SkillButton>(out var b))
                    _skillButtonPool?.Release(b);
            }
            HideButtons();
            DisableAction();
            _skillButtons.Clear();
        }

        public void EnableAction()
        {
            if (!_onUI.enabled) _onUI.Enable();
            _navigate.performed += OnNavigate;
        }
        public void DisableAction()
        {
            _navigate.performed -= OnNavigate;
        }

        // === Private Methodes ===
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
        private void SetSkill(ISkillSelectorComponent skill)
        {
            var before = _chosen;
            _chosen = skill as SkillButton;
            if (_chosen == null) Debug.LogError("!!!!!");
            _chosen.Selected = true;
            if (before != null)
                before.Selected = false;
        }
        private void ResetSkill()
        {
            if (_chosen != null)
                _chosen.Selected = false;
            _chosen = null;
        }
        private void OnConfirm()
        {
            if (_chosen == null) return;
            ReleaseButtons();
            _onConfirmCallback?.Invoke(_chosen.Skill);
            ResetSkill();
            DisableAction();
        }
        private void OnCancel()
        {
            ResetSkill();
        }
        // InputAction
        private void OnNavigate(InputAction.CallbackContext callbackContext)
        {
            var v = callbackContext.ReadValue<Vector2>();
            if (v.x == 0) return;
            if (v == Vector2.right)
            {
                _idx--;
                if (_idx < 0)
                    _idx = _skillButtons.Count - 1;

            }
            else if (v == Vector2.left)
            {
                _idx++;
                if (_idx >= _skillButtons.Count)
                    _idx = 0;
            }
            _skillButtons[_idx].OnClick();
        }
        // === Unity Lifecycle ===
        private void Awake()
        {
            if (_asset)
            {
                _onUI = _asset.FindActionMap(_mapName);
                _navigate = _onUI.FindAction(_actionName);
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

            _skillButtonPool = new Pool<SkillButton>(skillButtonPrefab, 5, _container, true);
        }

        private void OnDisable()
        {
            DisableAction();
        }
    }
}