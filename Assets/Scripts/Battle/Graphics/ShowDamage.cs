using HighElixir.Pool;
using UnityEngine;

namespace RPG_003.Battle
{
    public class ShowDamage : MonoBehaviour
    {
        [SerializeField] private PopText _popText;

        public void Show(Transform target, float damage, Color col, bool isCrit = false)
        {
            var c = ColorUtility.ToHtmlStringRGB(col);
            _popText.CreateText(target, $"<color=#{c}>{damage}</color>");
        }
    }
}