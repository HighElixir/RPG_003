using HighElixir.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HighElixir.PauseManage
{
    public class PauseManage : SingletonBehavior<PauseManage>, IObservable<PauseManage.PauseState>
    {
        private class Unsubscriber : IDisposable
        {
            private List<IObserver<PauseState>> _obs;
            private IObserver<PauseState> _observer;
            public Unsubscriber(List<IObserver<PauseState>> obs, IObserver<PauseState> observer)
            {
                _obs = obs;
                _observer = observer;
            }
            public void Dispose()
            {
                if (_observer != null && _obs.Contains(_observer))
                    _obs.Remove(_observer);
            }
        }

        public enum PauseState { Pause, Play }
        public PauseState State { get; private set; } = PauseState.Play;

        private List<IObserver<PauseState>> _observers = new();
        private InputAction _togglePauseAction;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);

            // InputActionセットアップ（ESCキーでトグル）
            _togglePauseAction = new InputAction("TogglePause", binding: "<Keyboard>/escape");
            _togglePauseAction.performed += ctx => TogglePause();
            _togglePauseAction.Enable();
        }

        protected override void OnDestroy()
        {
            // 完了通知してオブザーバー解放
            Notify_OnCompleted();
            _togglePauseAction.Disable();
        }

        public IDisposable Subscribe(IObserver<PauseState> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
                // 初回通知で現在状態を教える
                observer.OnNext(State);
            }
            return new Unsubscriber(_observers, observer);
        }

        private void TogglePause()
        {
            State = State == PauseState.Play ? PauseState.Pause : PauseState.Play;
            // デバッグログでニヤリ
            Debug.Log($"[PauseManage] State changed to {State}");
            Notify_StateChanged();
        }

        private void Notify_StateChanged()
        {
            foreach (var obs in _observers)
                obs.OnNext(State);
        }

        private void Notify_OnCompleted()
        {
            foreach (var obs in _observers)
                obs.OnCompleted();
        }
    }
}
