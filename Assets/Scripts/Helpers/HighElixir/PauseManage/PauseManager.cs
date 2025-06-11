using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HighElixir.PauseManage
{
    public class PauseManage : MonoBehaviour, IObservable<PauseManage>
    {
        // シングルトン化
        public static PauseManage Instance { get; private set; }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<PauseManage>> _obs;
            private IObserver<PauseManage> _observer;
            public Unsubscriber(List<IObserver<PauseManage>> obs, IObserver<PauseManage> observer)
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

        private List<IObserver<PauseManage>> _observers = new();
        private InputAction _togglePauseAction;

        private void Awake()
        {
            // シングルトン初期化
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // InputActionセットアップ（ESCキーでトグル）
            _togglePauseAction = new InputAction("TogglePause", binding: "<Keyboard>/escape");
            _togglePauseAction.performed += ctx => TogglePause();
            _togglePauseAction.Enable();
        }

        private void OnDestroy()
        {
            // 完了通知してオブザーバー解放
            Notify_OnCompleted();
            _togglePauseAction.Disable();
        }

        public IDisposable Subscribe(IObserver<PauseManage> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
                // 初回通知で現在状態を教える
                observer.OnNext(this);
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
                obs.OnNext(this);
        }

        private void Notify_OnCompleted()
        {
            foreach (var obs in _observers)
                obs.OnCompleted();
        }
    }
}
