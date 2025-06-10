using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RPG_003.Battle
{
    public class IntervalIndicator : MonoBehaviour
    {
        [SerializeField] private Image image;

        public void SetAmount(float current, float max, float duration = 0.2f)
        {
            if (max <= 0f || image == null) return;

            float t = Mathf.Clamp01(current / max);
            Debug.Log("Current : " + t);
            image.DOFillAmount(t, duration);
        }
    }
}