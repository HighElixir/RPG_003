using HighElixir.Pool;
using UnityEngine;

namespace RPG_003.Battle
{
    public class ShowDamage : MonoBehaviour
    {
        [SerializeField] private PopText _popText;

        public void Show(Transform target, float damage, bool isCrit = false)
        {
            _popText.CreateText(target, $"<color=red>{damage}</color>");
        }
    }
}