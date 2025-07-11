﻿using System; // require keep for Windows Universal App
using UnityEngine;

namespace UniRx.Triggers
{
    [DisallowMultipleComponent]
    public class ObservableAnimatorTrigger : ObservableTriggerBase
    {
        private Subject<int> onAnimatorIK;

        /// <summary>Callback for setting up animation IK (inverse kinematics).</summary>
        private void OnAnimatorIK(int layerIndex)
        {
            if (onAnimatorIK != null) onAnimatorIK.OnNext(layerIndex);
        }

        /// <summary>Callback for setting up animation IK (inverse kinematics).</summary>
        public IObservable<int> OnAnimatorIKAsObservable()
        {
            return onAnimatorIK ?? (onAnimatorIK = new Subject<int>());
        }

        private Subject<Unit> onAnimatorMove;

        /// <summary>Callback for processing animation movements for modifying root motion.</summary>
        private void OnAnimatorMove()
        {
            if (onAnimatorMove != null) onAnimatorMove.OnNext(Unit.Default);
        }

        /// <summary>Callback for processing animation movements for modifying root motion.</summary>
        public IObservable<Unit> OnAnimatorMoveAsObservable()
        {
            return onAnimatorMove ?? (onAnimatorMove = new Subject<Unit>());
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
            if (onAnimatorIK != null)
            {
                onAnimatorIK.OnCompleted();
            }
            if (onAnimatorMove != null)
            {
                onAnimatorMove.OnCompleted();
            }
        }
    }
}