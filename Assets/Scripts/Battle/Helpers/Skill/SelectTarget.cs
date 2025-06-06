using RPG_003.Battle.Skills;
using UnityEngine;
using System;
using System.Collections.Generic;
using RPG_003.Battle.Characters;
using Sirenix.OdinInspector;
using HighElixir.FollowObject;
using UnityEngine.UI;

namespace RPG_003.Battle
{
    /// <summary>
    /// プレイヤー向けにスキルの効果対象を選ぶためのUIを提供するクラス
    /// </summary>
    [RequireComponent(typeof(FollowObjectPool), typeof(BattleManager))]
    public class SelectTarget : MonoBehaviour
    {
        [BoxGroup("Reference"), SerializeField] private FollowObjectPool _pool;
        [BoxGroup("Reference"), SerializeField] private BattleManager _battleManager;
        [BoxGroup("Buttons"), SerializeField] private Button _confirm;
        [BoxGroup("Buttons"), SerializeField] private Button _cancel;
        [BoxGroup("Options"), SerializeField] private Vector2 _followObjectOffset;
        private TargetSelecter _targetSelecter;
        private Skill _skill;
        private Action<List<ICharacter>, Skill> _onTargetSelected;
        private Action _onCanceled;
        private List<PointTarget> _targetPoints = new List<PointTarget>();

        private CharacterBase _beforeTarget; // 前回に選択されたメインターゲット
        private TargetInfo _targetInfo; // ターゲット情報
        public void ShowTargets(Skill skill, Action<List<ICharacter>, Skill> onTargetSelected, Action onCanceled)
        {
            Release();
            _skill = skill;
            _onTargetSelected += onTargetSelected;
            _onCanceled += onCanceled;

            _targetInfo = new TargetInfo(_beforeTarget);
            // Display target selection UI here
            if (_beforeTarget == null)
            {
                _beforeTarget = _targetSelecter.SelectRandomTarget(2);
            }
            Debug.Log($"Selecting targets for skill: {skill.skillName} by {skill.parent.Data.Name}");
            CreatePoint(_beforeTarget.transform, _followObjectOffset);

            SetVisibleButtons(true);
            _confirm.onClick.AddListener(OnConfirm);
            _cancel.onClick.AddListener(OnCancel);
            // Simulate target selection
        }
        private void CreatePoint(Transform target, Vector2 offset)
        {
            _targetPoints.Add(_pool.CreateObject(_beforeTarget.transform, _followObjectOffset) as PointTarget);
        }
        private void SetVisibleButtons(bool isVisible)
        {
            _confirm?.gameObject.SetActive(isVisible);
            _cancel?.gameObject.SetActive(isVisible);
        }
        private void OnConfirm()
        {
            Release();
            _onTargetSelected?.Invoke(_targetInfo.CloneAndConvert(), _skill);
        }
        private void OnCancel()
        {
            Release();
            _onCanceled?.Invoke();
        }

        private void Release()
        {
            foreach (var p in _targetPoints)
            {
                _pool.ReleaseEffect(p);
            }
            _targetPoints.Clear();
            SetVisibleButtons(false);
            _confirm.onClick.RemoveListener(OnConfirm);
            _cancel.onClick.RemoveListener(OnCancel);
        }
        private void Awake()
        {
            _pool = GetComponent<FollowObjectPool>();
            _battleManager = GetComponent<BattleManager>();
            _targetSelecter = new TargetSelecter(_battleManager);
        }
    }
}