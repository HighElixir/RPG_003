using HighElixir.FollowObject;
using RPG_003.Battle.Characters;
using RPG_003.Battle.Factions;
using RPG_003.Battle.Skills;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

namespace RPG_003.Battle
{
    /// <summary>
    /// プレイヤー向けにスキルの効果対象を選ぶためのUIを提供するクラス
    /// </summary>
    [RequireComponent(typeof(FollowObjectPool))]
    public class SelectTarget : MonoBehaviour
    {
        // === Reference ===
        [BoxGroup("Reference"), SerializeField] private FollowObjectPool _pool;
        [BoxGroup("Reference"), SerializeField] private BattleManager _battleManager;
        [BoxGroup("Reference"), SerializeField] private Camera _camera;
        [BoxGroup("Buttons"), SerializeField] private Button _confirmButton;
        [BoxGroup("Buttons"), SerializeField] private Button _cancelButton;
        [BoxGroup("Options"), SerializeField] private Vector2 _followObjectOffset;
        private TargetSelectHelper _targetSelectHelper;
        private List<PointTarget> _targetPoints = new List<PointTarget>();

        // === inputAction ===
        [BoxGroup("InputActionAsset"), SerializeField] private InputActionAsset _actionAsset;
        [BoxGroup("InputActionAsset"), SerializeField] private string _mapName = "UI";
        [BoxGroup("InputActionAsset"), SerializeField] private string _navigateName = "Navigate";
        [BoxGroup("InputActionAsset"), SerializeField] private string _submitName = "Submit";
        [BoxGroup("InputActionAsset"), SerializeField] private string _cancelName = "Cancel";
        [BoxGroup("InputActionAsset"), SerializeField] private string _clickName = "Click";
        private InputActionMap _actionMap;
        private InputAction _navigate;
        private InputAction _submit;
        private InputAction _cancel;
        private InputAction _click;

        // === Data ===
        private Skill _skill;
        private Action<List<CharacterBase>, Skill> _onTargetSelected;
        private Action _onCanceled;
        private CharacterBase _beforeTarget; // 前回に選択されたメインターゲット
        [SerializeField, ReadOnly] private TargetInfo _targetInfo; // ターゲット情報

        // === Public Methode ===
        public void ShowTargets(Skill skill, Action<List<CharacterBase>, Skill> onTargetSelected, Action onCanceled)
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
            Debug.Log($"[SelectTarget] skill={skill.skillName}, caster={skill.parent.Data.Name}");

            CreatePoint(_beforeTarget.transform, _followObjectOffset);

            SetVisibleButtons(true);

            UpdateUI();
            EnableAction();
        }

       
        // === Private Methode ===
        private bool NeedsNewTarget(ICharacter before, Faction targetFaction) =>
    before == null || !before.Position.IsSameFaction(targetFaction) || !before.IsAlive;

        private void CreatePoint(Transform target, Vector2 offset)
        {
            _targetPoints.Add(_pool.CreateObject(_beforeTarget.transform, _followObjectOffset) as PointTarget);
        }
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
            foreach (var p in _targetPoints)
            {
                _pool.ReleaseEffect(p);
            }
            _targetPoints.Clear();
            SetVisibleButtons(false);
            DisableAction();
        }

        private void Release(PointTarget release)
        {
            _targetPoints.Remove(release);
            _pool.ReleaseEffect(release);
        }
        private void UpdateUI()
        {
            // 過不足分のUIを処理
            var t = _targetPoints.Count - _targetInfo.TargetCount;
            if (t > 0)
            {
                for (var i = 0; i < t; i++)
                {
                    Release(_targetPoints[i]);
                    _targetPoints.Remove(_targetPoints[i]);
                }
            }
            else if (t < 0)
            {
                t *= -1;
                for (var i = 0; i < t; i++)
                {
                    _targetPoints.Add(_pool.CreateObject(Vector2.zero) as PointTarget);
                }
            }
            // UIを移動
            for (int i = 0; i < _targetPoints.Count; i++)
            {
                CharacterBase target;
                if (i == 0)
                    target = _targetInfo.MainTarget;
                else
                    target = _targetInfo.AdditionalTargets[i - 1];
                _targetPoints[i].Enable = true;
                _targetPoints[i].SetTarget(target.transform, _followObjectOffset);
            }
        }

        // === InputAction ===
        public void EnableAction()
        {
            _confirmButton?.onClick.AddListener(SubmitSelect);
            _cancelButton?.onClick.AddListener(CancelSelect);
            if (!_actionMap.enabled) _actionMap.Enable();
            _submit.performed += OnSubmit;
            _cancel.performed += OnCancel;
            _navigate.performed += OnNavigate;
            _click.performed += OnClick;
        }
        public void DisableAction()
        {
            _confirmButton.onClick.RemoveListener(SubmitSelect);
            _cancelButton.onClick.RemoveListener(CancelSelect);
            _submit.performed -= OnSubmit;
            _cancel.performed -= OnCancel;
            _navigate.performed -= OnNavigate;
            _click.performed -= OnClick;
        }
        private void OnSubmit(CallbackContext context)
        {
            if (_targetInfo.IsValid)
                SubmitSelect();
        }

        private void OnCancel(CallbackContext context)
        {
            CancelSelect();
        }

        private void OnClick(CallbackContext context)
        {

            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10.0f))
            {
                Debug.Log(hit.point);
            }
        }
        // 矢印キーでメインターゲットを上下に移動させる
        private void OnNavigate(CallbackContext context)
        {
            var v = context.ReadValue<Vector2>();
            if (v.y == 0) return;

            var isup = v == Vector2.up;
            var p = (int)_targetSelectHelper.GetPosition(_targetInfo.MainTarget);
            var p2 = CharacterPosition.None;
            bool isAlly = _targetInfo.MainTarget.IsAlly();
            CharacterBase res;
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
            if (!_pool) _pool = GetComponent<FollowObjectPool>();
            if (!_camera) _camera = Camera.main;
            if (!_battleManager) _battleManager = GetComponent<BattleManager>();
            _targetSelectHelper = new TargetSelectHelper(_battleManager);

            _actionMap = _actionAsset.FindActionMap(_mapName);
            _submit = _actionMap.FindAction(_submitName);
            _cancel = _actionMap.FindAction(_cancelName);
            _navigate = _actionMap.FindAction(_navigateName);
            _click = _actionMap.FindAction(_clickName);
        }
        private void OnDisable()
        {
            DisableAction();
        }
    }
}