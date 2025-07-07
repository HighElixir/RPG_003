using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG_003.Core
{
    public class LoadingUIController : MonoBehaviour, ILoadSceneReceiver
    {
        [SerializeField] private Slider _progressBar;
        [SerializeField] private TMP_Text _messageText;
        [SerializeField] private TipsController _tipsController;

        private CancellationTokenSource _cts;

        public void OnStart()
        {
            _progressBar.gameObject.SetActive(true);
            _messageText.gameObject.SetActive(true);
            _tipsController.gameObject.SetActive(true);
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            ShowLoadingMessage(_cts.Token).Forget();
        }

        public void OnProcess(float progress)
        {
            _progressBar.value = progress;
        }

        public void OnCompleted()
        {
            _cts.Cancel();
            _messageText.maxVisibleCharacters = _messageText.text.Length;

            _progressBar.gameObject.SetActive(false);
            _messageText.gameObject.SetActive(false);
            _tipsController.gameObject.SetActive(true);
        }

        private async UniTask ShowLoadingMessage(CancellationToken token)
        {
            float delay = 0.25f;
            int totalChars = _messageText.text.Length;

            while (!token.IsCancellationRequested)
            {
                for (int i = 0; i <= totalChars; i++)
                {
                    _messageText.maxVisibleCharacters = i;
                    await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: token);
                }
            }
        }
    }
}