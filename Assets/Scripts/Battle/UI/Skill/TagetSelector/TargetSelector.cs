using RPG_003.Battle.Factions;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UniRx;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;
using System.Linq;

namespace RPG_003.Battle
{
    /// <summary>
    /// プレイヤー向けにスキルの効果対象を選ぶためのUIを提供するクラス
    /// </summary>
    [RequireComponent(typeof(TargetSelectorUI), typeof(PlayerInput))]
    public class TargetSelector : MonoBehaviour
    {
        // === Reference ===
        [BoxGroup("Reference"), SerializeField] private BattleManager _battleManager;
        [BoxGroup("Reference"), SerializeField] private Camera _camera;
        [BoxGroup("Reference"), SerializeField] private TargetSelectorUI _ui;
        [BoxGroup("Buttons"), SerializeField] private Button _confirmButton;
        [BoxGroup("Buttons"), SerializeField] private Button _cancelButton;
        private TargetSelectHelper _targetSelectHelper;

        // === inputAction ===
        private Vector2 _pos = Vector2.zero;

        // === Data ===
        private Skill _skill;
        private Action<List<CharacterObject>, Skill> _onTargetSelected;
        private Action _onCanceled;
        /// <summary>
        /// 前回選択されたメインターゲット
        /// </summary> 
        CharacterObject _beforeTarget;
        [SerializeField, ReadOnly] private TargetInfo _targetInfo; // ターゲット情報

        public BoolReactiveProperty IsSelecting { get; private set; } = new(false);
        // === Public Methode ===
        public void ShowTargets(Skill skill, Action<List<CharacterObject>, Skill> onTargetSelected, Action onCanceled)
        {
            if (skill == null) throw new ArgumentNullException(nameof(skill));
            if (onTargetSelected == null) throw new ArgumentNullException(nameof(onTargetSelected));
            if (onCanceled == null) throw new ArgumentNullException(nameof(onCanceled));

            ReleaseAll(); // コールバック累積防止

            _skill = skill;
            _onTargetSelected = onTargetSelected;
            _onCanceled = onCanceled;

            // 前回ターゲットがない or スキルターゲットになる勢力が違う場合はランダム選択
            if (NeedsNewTarget(_beforeTarget, skill.skillDataInBattle.Target))
                _beforeTarget = _targetSelectHelper.SelectRandomTarget(skill.skillDataInBattle.Target);
            _targetInfo = new(_beforeTarget);
            Debug.Log($"[TargetSelector] skill={skill.skillDataInBattle.Name}, caster={skill.parent.Data.Name}");

            IsSelecting.Value = true;
            UpdateUI();
        }


        // === Private Methode ===
        private bool NeedsNewTarget(CharacterObject before, Faction targetFaction) =>
    before == null || !before.Position.IsSameFaction(targetFaction) || !before.IsAlive;

        private void SetVisibleButtons(bool isVisible)
        {
            _confirmButton?.gameObject.SetActive(isVisible);
            _cancelButton?.gameObject.SetActive(isVisible);
        }
        private void SubmitSelect()
        {
            ReleaseAll();
            _onTargetSelected?.Invoke(_targetInfo.ToList(), _skill);
        }
        private void CancelSelect()
        {
            ReleaseAll();
            _onCanceled?.Invoke();
        }

        private void ReleaseAll()
        {
            _ui.RemoveAll();
            IsSelecting.Value = false;
        }
        private void UpdateUI()
        {
            _ui.RemoveAll();
            foreach (var item in _targetInfo)
                _ui.CreatePoint(item.transform);
        }

        // === InputAction ===
        public void OnSubmit()
        {
            if (!IsSelecting.Value) return;
            if (_targetInfo.IsValid)
                SubmitSelect();
        }
        public void OnClick()
        {
            if (!IsSelecting.Value) return;

            // ① まずスクリーン座標をゲット
            Debug.Log($"[スクリーン座標] {_pos}");

            // ② その座標からレイを飛ばす
            Ray ray = _camera.ScreenPointToRay(_pos);
            var hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.transform != null &&
                hit.transform.TryGetComponent<CharacterObject>(out var character) &&
                character.IsSameFaction(_skill.skillDataInBattle.Target))
            {
                Debug.Log("Hitted. Name : " + character.Data.Name);
                if (_targetInfo.Contains(character)) _targetInfo.RemoveTarget(character);
                else if (_skill.skillDataInBattle.TargetCount < _targetInfo.TargetCount) _targetInfo.AddTarget(character);
                else _targetInfo.MainTarget = character;
                
                UpdateUI();
                return;
            }
            Debug.Log("Don't Hit");
        }
        public void OnPoint(InputValue context)
        {
            if (!IsSelecting.Value) return;
            _pos = context.Get<Vector2>();
        }
        // 矢印キーでメインターゲットを上下に移動させる
        public void OnNavigate(InputValue context)
        {
            if (!IsSelecting.Value) return;
            var v = context.Get<Vector2>();
            if (v.y == 0) return;

            var isup = v == Vector2.up;
            var p = (int)_targetSelectHelper.GetPosition(_targetInfo.MainTarget);
            var p2 = CharacterPosition.None;
            bool isAlly = _targetInfo.MainTarget.IsAlly();
            CharacterObject res;
            int min = isAlly ? 0 : 4;
            int max = isAlly ? 3 : 8;
            while (true)
            {
                p += isup ? -1 : 1;
                p2 = (CharacterPosition)(Mathf.Clamp(p, min, max));
                if (_targetSelectHelper.TryGetCharacter(p2, out res)) break;
                if (p < min || p > max) return;
            }
            _targetInfo.MainTarget = res;
            UpdateUI();
        }

        // === Unity LifeCycle ===
        private void Awake()
        {
            if (!_camera) _camera = Camera.main;
            if (!_battleManager) _battleManager = GetComponent<BattleManager>();
            _targetSelectHelper = new TargetSelectHelper(_battleManager);

            _confirmButton?.onClick.AddListener(() =>
            {
                if (_targetInfo.IsValid)
                    SubmitSelect();
            });
            _cancelButton?.onClick.AddListener(CancelSelect);
            IsSelecting.Subscribe(value =>
            {
                SetVisibleButtons(value);
            }).AddTo(this);
        }
    }
}