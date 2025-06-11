using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG_003.Battle
{
    public class IntervalIndicator : MonoBehaviour
    {
        [SerializeField] private Image image;
        public Action<IntervalIndicator> release;
        public void SetAmount(float current, float max, float duration = 0.2f)
        {
            if (max <= 0f || image == null) return;

            float t = Mathf.Clamp01(current / max);
            Debug.Log("Current : " + t);
            image.DOFillAmount(t, duration);
        }

        public void Release()
        {
            release?.Invoke(this);
        }
    }
}