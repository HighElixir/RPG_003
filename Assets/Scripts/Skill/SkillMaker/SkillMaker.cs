using RPG_003.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Skills
{
    public class SkillMaker : MonoBehaviour, IObservable<SkillType>
    {
        // Observer
        #region
        private class UnSubscribe : IDisposable
        {
            private List<IObserver<SkillType>> _obs;
            private IObserver<SkillType> _observer;
            public UnSubscribe(List<IObserver<SkillType>> obs, IObserver<SkillType> observer)
            {
                _obs = obs;
                _observer = observer;
            }

            public void Dispose()
            {
                _obs.Remove(_observer);
            }
        }
        private List<IObserver<SkillType>> _observers = new();
        public IDisposable Subscribe(IObserver<SkillType> observer)
        {
            _observers.Add(observer);
            observer.OnNext(_current);
            return new UnSubscribe(_observers, observer);
        }
        private void Notify()
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(_current);
            }
        }
        #endregion

        private SkillType _current;
        public void Confirm(SkillDataHolder production)
        {
            foreach (var obs in _observers)
            {
                obs.OnCompleted();
            }
            GameDataHolder.instance.AddSkill(production);
        }

        public void ResetMaker(SkillDataHolder holder)
        {
            Notify();
        }
    }
    public enum SkillType
    {
        Basic,
        Modifies,
        Smith,
    }
}