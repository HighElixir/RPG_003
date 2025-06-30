using RPG_003.Status;
using UnityEngine;

namespace RPG_003.Battle
{
    // StatusManagerより後に初期化(StatusAmountが必要なため)
    public class BehaviorIntervalCount
    {
        private readonly float maxSpeed = 1000f;
        private readonly float scale = 400f;
        private readonly float minInterval = 10f;
        private readonly float intervalBase = 140f;

        private StatusAmount _speedAmount;
        private int _currentAmount = 0;

        public bool IsReady => _currentAmount == 0;
        public int CurrentAmount => _currentAmount;
        public int Max
        {
            get
            {
                var speed = _speedAmount.ChangedMax;
                if (speed >= maxSpeed) speed = maxSpeed;

                float t = (maxSpeed - speed) / scale;
                // ベキ乗してスケールをかける
                return (int)(minInterval + intervalBase * Mathf.Pow(t, 2.3f));
            }
        }

        public float Speed => _speedAmount.ChangedMax;

        public void Process(int amount)
        {
            _currentAmount = Mathf.Max(0, _currentAmount - amount);
        }

        public void Reset()
        {
            _currentAmount = Max;
        }
        public void Initialize(StatusAmount speedAmount)
        {
            _speedAmount = speedAmount;
            _currentAmount = Max;
        }
    }
}