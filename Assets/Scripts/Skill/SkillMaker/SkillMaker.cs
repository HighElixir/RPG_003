using RPG_003.Core;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Lean.Gui;

namespace RPG_003.Skills
{
    [DefaultExecutionOrder(-1)]
    public class SkillMaker : MonoBehaviour
    {
        public enum SkillType
        {
            None,
            Basic,
            Modifies,
            Smith,
        }
        [SerializeField] private Button _createSkillButton;
        [SerializeField] private LeanWindow _skillPanel;
        [SerializeField] private Button _basicButton;
        [SerializeField] private Button _modifiesButton;
        [SerializeField] private Button _smithButton;
        [SerializeField] private Button _exit;
        private ReactiveProperty<SkillType> _current = new(SkillType.None);
        public IObservable<SkillType> Current => _current;
        public void Confirm(SkillDataHolder production)
        {
            Debug.Log($"SkillDataHolder: {production.Name} is completed.");
            GameDataHolder.instance.AddSkill(production);
            _current.Value = SkillType.None;
        }

        public void ResetMaker(SkillDataHolder holder)
        {
        }
        private void Awake()
        {
            _createSkillButton.OnClickAsObservable().Subscribe(_ => { _skillPanel.Set(true); }).AddTo(this);
            _basicButton.OnClickAsObservable().Subscribe(_ =>
            {
                _current.Value = SkillType.Basic;
                _skillPanel.Set(false);
            }).AddTo(this);
            _modifiesButton.OnClickAsObservable().Subscribe(_ =>
            {
                _current.Value = SkillType.Modifies;
                _skillPanel.Set(false);
            }).AddTo(this);
            _smithButton.OnClickAsObservable().Subscribe(_ =>
            {
                _current.Value = SkillType.Smith;
                _skillPanel.Set(false);
            }).AddTo(this);
            GetComponent<SkillBuilder>().OnComplete += Confirm;
            _current.Value = SkillType.None;
        }
    }
}