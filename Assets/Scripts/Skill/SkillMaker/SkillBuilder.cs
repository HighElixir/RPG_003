using HighElixir.Pool;
using RPG_003.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Skills
{
    public class SkillBuilder : MonoBehaviour, IObserver<SkillType>
    {
        [SerializeField] private Pool<SkillButton> _buttonPool;
        private SkillDataHolder _temp;
        private SkillMaker _director;
        private SkillButtonUIBuilder _ui;
        private IDisposable _unSubscriber;
        private List<SkillButton> _buttons;
        public Action<SkillDataHolder> OnComplete { get; private set; }
        public SkillDataHolder Temp => _temp;
        public void Init()
        {
            if (_unSubscriber != null)
                _unSubscriber.Dispose();
            _director = GetComponent<SkillMaker>();
            _ui = GetComponent<SkillButtonUIBuilder>();
            _unSubscriber = _director.Subscribe(this);
        }
        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(SkillType value)
        {
            switch (value)
            {
                case SkillType.Basic:
                    _temp = new BasicHolder();
                    break;
                case SkillType.Modifies:
                    _temp = new ModifierHolder();
                    break;
                case SkillType.Smith:
                    _temp = new SmithHolder();
                    break;
                default:
                    break;
            }
            ChangeMode(value);
        }

        public void ChangeMode(SkillType type)
        {
            foreach (var button in _buttons)
            {
                _buttons.Remove(button);
                _buttonPool.Release(button);
            }
            var s = GameDataHolder.instance.SkillDatas.GetSkillsByType(type);
            var count = s.AllCount;
            for (var i = 0; i < count; i++)
            {
                var b = _buttonPool.Get();
                b.SetParent(this);
                b.UpdateButton();
                _buttons.Add(b);
            }
        }
        private void Awake()
        {
            Init();
        }
    }
}
