using Lean.Gui;
using UniRx;
using UnityEngine;

namespace RPG_003.Core
{
    public class Toggle : MonoBehaviour
    {
        [SerializeField] UnityEngine.UI.Toggle _toggle;
        [SerializeField] LeanWindow _window;

        private void Awake()
        {
            _toggle.onValueChanged.AsObservable().Subscribe(flag =>
            {
                _window.Set(flag);
            }).AddTo(this);
        }
    }
}