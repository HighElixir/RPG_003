using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;

namespace RPG_003.Battle
{
    public partial class BattleManager
    {
        private BoolReactiveProperty _isPause = new BoolReactiveProperty(false);
        public IObservable<bool> IsPause => _isPause.AsObservable();
        
        // === Pause ===
        protected void OnPaused()
        {
            Time.timeScale = 0;
            _isPause.Value = true;
        }

        protected void OnResumed()
        {
            Time.timeScale = 1.0f;
            _isPause.Value = false;
        }
        public async UniTask WaitForPause()
        {
            await UniTask.WaitWhile(() => _isPause.Value);
        }
    }
}