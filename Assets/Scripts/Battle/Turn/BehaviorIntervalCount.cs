using RPG_003.Battle.Characters;
using UnityEngine;

namespace RPG_003.Battle
{
    // StatusManagerより後に初期化
    public class BehaviorIntervalCount
    {
        private readonly float maxSpeed = 500f;
        private readonly float scale = 400f;
        private readonly float minInterval = 10f;
        private readonly float intervalBase = 140f;

        private StatusAmount _speedAmount;
        private IntervalIndicator _indicator;
        private int _currentAmount = 0;

        public bool IsReady { get; private set; } = false;
        public int CurrentAmount => _currentAmount;
        public int Max
        {
            get
            {
                var speed = _speedAmount.ChangedMax;
                if (speed >= maxSpeed)speed = maxSpeed;

                float t = (maxSpeed - speed) / scale;
                // ベキ乗してスケールをかける
                return (int)(minInterval + intervalBase * Mathf.Pow(t, 2.3f));
            }
        }

        public void Process(int amount)
        {
            _currentAmount = Mathf.Max(0, _currentAmount - amount);
            _indicator?.SetIndicator(Max - _currentAmount, Max);
            if (_currentAmount <= 0)IsReady = true;
            else IsReady = false;
        }

        public void Reset()
        {
            _currentAmount = Max;
            IsReady = false;
            _indicator?.SetIndicator(0, Max);
        }
        public void Initialize(StatusAmount speedAmount, IntervalIndicator indicator)
        {
            _speedAmount = speedAmount;
            _indicator = indicator;
            _currentAmount = Max;
            _indicator.SetIndicator(0, Max);
        }
    }
}