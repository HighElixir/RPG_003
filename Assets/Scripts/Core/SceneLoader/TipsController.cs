using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;

namespace RPG_003.Core
{
    public class TipsController : MonoBehaviour
    {
        [Serializable]
        public struct Tip
        {
            public string title;
            public string description;
        }

        [SerializeField] private TMP_Text tipText;
        [SerializeField] private Transform startViewPos;
        [SerializeField] private Transform _goalPos;
        [SerializeField] private List<Tip> tips;
        private CancellationTokenSource _cts;
        private StringBuilder _sb = new StringBuilder();

        private void OnEnable()
        {
            StartTips();
        }

        private void OnDisable()
        {
            _cts.Cancel();
        }

        public void StartTips()
        {
            tipText.color = new Color(tipText.color.r, tipText.color.g, tipText.color.b, 0);
            tipText.transform.position = startViewPos.position;
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            ShowTipsLoop(_cts.Token).Forget();
        }

        private async UniTaskVoid ShowTipsLoop(CancellationToken token)
        {
            TimeSpan interval = TimeSpan.FromSeconds(4f);
            float animTime = 1.3f;

            while (!token.IsCancellationRequested)
            {
                _sb.Clear();
                var randomTip = tips[UnityEngine.Random.Range(0, tips.Count)];
                _sb.AppendLine(randomTip.title);
                _sb.AppendLine(randomTip.description);
                tipText.text = _sb.ToString();

                // DOTween Sequence
                var seq = DOTween.Sequence();

                seq.Append(tipText.transform.DOMove(_goalPos.position, animTime))
                   .Join(tipText.DOFade(1f, animTime))
                   .AppendInterval(2f)
                   .Append(tipText.DOFade(0f, animTime));

                await UniTask.Delay(interval, cancellationToken: token);
            }
        }
    }

}