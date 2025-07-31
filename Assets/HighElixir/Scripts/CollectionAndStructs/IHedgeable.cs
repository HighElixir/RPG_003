using System;

namespace HighElixir
{
    /// <summary>
    /// ヘッジ可能な要素を表すインターフェース。
    /// </summary>
    public interface IHedgeable<T, TSelf>
        where TSelf : IHedgeable<T, TSelf>
        where T : IComparable<T>, IEquatable<T>
    {
        /// <summary>
        /// ヘッジ可能な値を取得する。
        /// </summary>
        T Value { get; set; }
        T MinValue { get; }
        T MaxValue { get; }

        /// <summary>
        /// 値の変化方向を取得する。
        /// </summary>
        T Direction { get; }

        TSelf SetMax(T maxValue);
        TSelf SetMin(T minValue);

        /// <summary>
        /// ヘッジ処理が行われたときに呼び出されるイベントを購読する。
        /// </summary>
        IDisposable Subscribe(Action<T> onHedge);
    }
}