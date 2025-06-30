using HighElixir.Pool;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace RPG_003.Battle
{
    [RequireComponent(typeof(PlayerInput))]
    public class SkillSelector : MonoBehaviour
    {
        public class ButtonContainer
        {
            public Button Button { get; set; }
            public Skill Skill { get; set; }
            public bool IsSelected { get; set; }

            public ButtonContainer() { }
            public ButtonContainer(Button button, Skill skill, bool isSelected)
            {
                Button = button;
                Skill = skill;
                IsSelected = isSelected;
            }
        }
        // === References ===
        [BoxGroup("Reference"), SerializeField] private SkillSelectorUI _ui;
        [BoxGroup("Pool"), SerializeField] private Button _skillButtonPrefab;
        [BoxGroup("Pool"), SerializeField] private Transform _container;

        [PropertyTooltip("シーンビュー上にて配置されている想定")]
        [BoxGroup("UI"), SerializeField] private Button _confirm;
        [PropertyTooltip("シーンビュー上にて配置されている想定")]
        [BoxGroup("UI"), SerializeField] private Button _cancel;

        // === Date ===
        private ButtonContainer _chosen;
        private Action<Skill> _onConfirmCallback;
        private Pool<Button> _skillButtonPool;
        private List<ButtonContainer> _skillButtons = new();
        private int _idx = 0;

        // === Public Methodes ===
        public void InvokeSelector(List<Skill> skills, Action<Skill> onConfirmCallback)
        {
            _onConfirmCallback = onConfirmCallback;
            ResetSkill();
            CreateButtons(skills);
            _confirm.gameObject.SetActive(true);
            _cancel.gameObject.SetActive(true);
        }
        public void Exit()
        {
            ReleaseButtons();
            ResetSkill();
            _confirm.gameObject.SetActive(false);
            _cancel.gameObject.SetActive(false);
        }
        public void CreateButtons(List<Skill> skills)
        {
            ReleaseButtons();
            foreach (var skill in skills)
            {
                var button = _skillButtonPool?.Get();
                ButtonContainer container = new(button, skill, false);
                _skillButtons.Add(container);
                button.OnClickAsObservable().Subscribe(_ =>
                {
                    if (_chosen != null)
                    {
                        var before = _chosen;
                        before.IsSelected = false;
                    }
                    _chosen = container;
                    _chosen.IsSelected = true;
                    _ui.UpdateUI(_skillButtons);
                }).AddTo(this);
                Debug.Log(skill.ToString());
            }
            _idx = 0;
            SetButtonsVisible(true);
            _ui.UpdateUI(_skillButtons);
            foreach (var item in _skillButtons)
            {
                if (item.Skill.IsActive) return;
            }
            Notify();
        }

        public void ReleaseButtons()
        {
            foreach (var button in _skillButtons)
            {
                _skillButtonPool.Release(button.Button);
            }
            _skillButtons.Clear();
        }



        // === Private Methodes ===
        private void Notify()
        {
            _onConfirmCallback?.Invoke(_chosen.Skill);
            Exit();
        }
        private void SetButtonsVisible(bool isShow)
        {
            _confirm?.gameObject.SetActive(isShow);
            _cancel?.gameObject.SetActive(isShow);
        }

        private void ResetSkill()
        {
            if (_chosen == null) return;
            _chosen.IsSelected = false;
            _chosen = null;
            _ui.UpdateUI(_skillButtons);
        }

        // InputAction
        private void OnNavigate(InputValue value)
        {
            var v = value.Get<Vector2>();
            if (v.x == 0) return;
            if (v == Vector2.right)
            {
                _idx++;
                if (_idx >= _skillButtons.Count)
                    _idx = 0;

            }
            else if (v == Vector2.left)
            {
                _idx--;
                if (_idx < 0)
                    _idx = _skillButtons.Count - 1;
            }
            _skillButtons[_idx].Button.onClick.Invoke();
        }

        // === Unity Lifecycle ===
        private void Awake()
        {
            if (_confirm)
            {
                _confirm.gameObject.SetActive(false);
                _confirm.OnClickAsObservable().Subscribe(_ =>
                {
                    if (_chosen?.Skill == null) return;
                    Notify();
                }).AddTo(this);
            }
            if (_cancel)
            {
                _cancel.gameObject.SetActive(false);
                _cancel.OnClickAsObservable().Subscribe(_ =>
                {
                    ResetSkill();
                }).AddTo(this);
            }
        }
        private void Start()
        {
            ResetSkill();

            _skillButtonPool = new Pool<Button>(_skillButtonPrefab, 5, _container, true);
        }
    }
}