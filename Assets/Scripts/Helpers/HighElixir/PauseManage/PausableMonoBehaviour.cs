using System;
using UnityEngine;

namespace HighElixir.PauseManage
{
    public abstract class PausableMonoBehaviour : MonoBehaviour, IObserver<PauseManage.PauseState>
    {
        private IDisposable _subscription;
        protected PauseManage.PauseState CurrentState { get; private set; }

        protected virtual void Awake()
        {
            // 起動時に自動で購読
            if (PauseManage.instance != null)
                _subscription = PauseManage.instance.Subscribe(this);
        }

        protected virtual void OnDestroy()
        {
            // 解放処理
            _subscription?.Dispose();
        }

        // IObserver 実装
        public void OnNext(PauseManage.PauseState state)
        {
            // 状態が変わったら、Pause/Resume の抽象メソッド呼び出し
            CurrentState = state;
            if (CurrentState == PauseManage.PauseState.Pause)
                OnPaused();
            else
                OnResumed();

        }

        public void OnCompleted()
        {
            // PauseManage が消えるタイミング（ほぼないけど）に呼ばれる
            _subscription?.Dispose();
        }

        public void OnError(Exception error)
        {
            Debug.LogError($"[PausableMonoBehaviour] Error from PauseManage: {error}");
        }

        // 継承クラスで実装すべきメソッド
        protected abstract void OnPaused();
        protected abstract void OnResumed();
    }
}
