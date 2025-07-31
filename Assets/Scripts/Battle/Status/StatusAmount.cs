using System;

namespace RPG_003.Battle
{
    public class StatusAmount
    {
        private float _defaultAmount;
        /// <summary>
        /// HPやMPなどの頻繁に変動するステータスで使用
        /// </summary>
        public float currentAmount;

        // 足し算・引き算で管理
        // 計算式: ChangedMax = (DefaultAmount + TemporaryChanged) * TemporaryRatio
        private float _temporaryChanged;
        private float _temporaryRatio = 1f; 
        

        private bool dirty = true; // 値が変更されたかどうか
        private float _changedMax;
        public float ChangedMax
        {
            get
            {
                if (dirty)
                {
                    dirty = false;
                    _changedMax = (_defaultAmount + _temporaryChanged) * Math.Max(0, _temporaryRatio); // デフォルト値に一時的な変更を加え、倍率を掛ける
                }
                return _changedMax;
            }
        }
        public float DefaultAmount => _defaultAmount;
        public float TemporaryChanged
        {
            get => _temporaryChanged;
            set
            {
                if (_temporaryChanged == value) return;
                _temporaryChanged = value;
                dirty = true; // 値が変更されたので、ChangedMaxを再計算する必要がある
            }
        }
        public float TemporaryRatio
        {
            get => _temporaryRatio;
            set
            {
                if (value < 0f) throw new ArgumentOutOfRangeException(nameof(value), "TemporaryRatio must be non-negative.");
                if (_temporaryRatio == value) return;
                _temporaryRatio = value;
                dirty = true; // 値が変更されたので、ChangedMaxを再計算する必要がある
            }
        }
        public StatusAmount(float defaultAmount)
        {
            _defaultAmount = defaultAmount;
            currentAmount = defaultAmount;
        }

#if UNITY_EDITOR
        public void Debug(float @new, bool isCurrent)
        {
            if (!isCurrent)
                _defaultAmount = @new;
            else
                currentAmount = @new;
        }
#endif
    }
}