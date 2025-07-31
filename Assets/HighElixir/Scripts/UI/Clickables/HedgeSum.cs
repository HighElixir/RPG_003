using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HighElixir.UI
{
    public class HedgeSum
    {
        private List<CountableSwitch> _targets = new();
        private (int min, int max) _hedge;
        private bool _disallowedNegative = true;

        public int SumAll => _targets.Sum(c => c.CurrentAmount);
        public void Add(CountableSwitch target)
        {
            _targets.Add(target);
            if (_disallowedNegative)
                target.SetMin(0);
            target.AllowChange += AllowChange;
        }
        private bool AllowChange(int before, int delta)
        {
            //Debug.Log($"before: {before}, delta: {delta}");
            return SumAll + delta <= _hedge.max && SumAll + delta >= _hedge.min;
        }

        // 内部Builder的な
        public HedgeSum Hedge(int min, int max)
        {
            _hedge = (min, max);
            return this;
        }
        public HedgeSum DisallowedNegative(bool disallowedNegative)
        {
            _disallowedNegative = disallowedNegative;
            foreach (var target in _targets)
            {
                if (_disallowedNegative)
                    target.SetMin(0);
                else
                    target.SetMin(int.MinValue);
            }
            return this;
        }

        public static implicit operator int(HedgeSum sum) => sum.SumAll;
    }
}