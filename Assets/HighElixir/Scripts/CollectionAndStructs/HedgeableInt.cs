using System;

namespace HighElixir
{
    public class HedgeableInt : IHedgeable<int, HedgeableInt>
    {
        private int _value;
        private int _minValue = int.MinValue;
        private int _maxValue = int.MaxValue;
        // -1 ならば負方向、+1 ならば正方向、0 ならば変化なし
        private int _direction = 0;
        private Action<int> _onHedge;
        public int Value
        {
            get => _value;
            set
            {
                var oldValue = _value;
                if (value > _maxValue)
                {
                    _value = _maxValue;
                    _onHedge?.Invoke(_direction);
                }
                else if (value < _minValue)
                {
                    _value = _minValue;
                    _onHedge?.Invoke(_direction);
                }
                else
                {
                    _value = value;
                }
                // Direction の更新
                int diff = _value - oldValue;
                Direction = diff;
            }
        }
        public int Direction
        {
            get => _direction;
            private set => _direction = (value == 0) ? 0 : value / Math.Abs(value);
        }
        public int MinValue => _minValue;
        public int MaxValue => _maxValue;
        public HedgeableInt(int initialValue = 0)
        {
            Value = initialValue;
        }
        public HedgeableInt(int minValue, int maxValue, int initialValue = 0)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            Value = initialValue;
        }

        public HedgeableInt SetMax(int maxValue)
        {
            _maxValue = maxValue;
            if (_value > _maxValue)
            {
                Value = _maxValue;
            }
            return this;
        }
        public HedgeableInt SetMin(int minValue)
        {
            _minValue = minValue;
            if (_value < _minValue)
            {
                Value = _minValue;
            }
            return this;
        }
        public IDisposable Subscribe(Action<int> onHedge)
        {
            _onHedge = onHedge;
            return Disposable.Create(() => _onHedge = null);
        }
    }
}