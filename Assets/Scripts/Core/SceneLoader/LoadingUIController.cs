// LoadingUIController.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace RPG_003.Core
{
    public class LoadingUIController : MonoBehaviour
    {
        [SerializeField] private Slider progressBar;
        [SerializeField] private TMP_Text messageText;

        private CancellationTokenSource _cts;

        public void OnStart()
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            ShowLoadingMessage(_cts.Token).Forget();
        }

        public void UpdateProgress(float value)
        {
            progressBar.value = value;
        }

        public void OnComplete()
        {
            _cts.Cancel();
            messageText.maxVisibleCharacters = messageText.text.Length;
        }

        private async UniTask ShowLoadingMessage(CancellationToken token)
        {
            float delay = 0.25f;
            int totalChars = messageText.text.Length;

            while (!token.IsCancellationRequested)
            {
                for (int i = 0; i <= totalChars; i++)
                {
                    messageText.maxVisibleCharacters = i;
                    await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: token);
                }
            }
        }
    }
}