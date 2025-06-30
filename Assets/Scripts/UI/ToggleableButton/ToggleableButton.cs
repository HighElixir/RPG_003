using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace HighElixir.UI
{
    public class ToggleableButton : Button
    {
        [SerializeField] private Sprite _toggled;
        [SerializeField] private Sprite _untoggled;

        private BoolReactiveProperty _isToggled = new BoolReactiveProperty(false);

        public IObservable<bool> IsToggled => _isToggled;
        public void Toggle()
        {
            _isToggled.Value = !_isToggled.Value;
        }
        private void UpdateVisualState(bool isToggled)
        {
            // Update the button's visual state based on IsToggled
            if (isToggled) image.sprite = _toggled;
            else image.sprite = _untoggled;
        }

        protected override void Start()
        {
            IsToggled.Subscribe(UpdateVisualState).AddTo(this);
        }
    }
}