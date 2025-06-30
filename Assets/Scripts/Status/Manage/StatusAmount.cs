using System;

namespace RPG_003.Status
{
    public class StatusAmount
    {
        private float _defaultAmount;
        /// <summary>
        /// HPやMPなどの頻繁に変動するステータスで使用
        /// </summary>
        public float currentAmount;
        private float temporaryChanged;
        private float temporaryRatio = 1f; // 足し算・引き算で管理

        private bool dirty = true; // 値が変更されたかどうか
        private float _changedMax;
        public float ChangedMax
        {
            get
            {
                if (dirty)
                {
                    dirty = false;
                    _changedMax = (_defaultAmount + temporaryChanged) * Math.Max(0, temporaryRatio); // デフォルト値に一時的な変更を加え、倍率を掛ける
                }
                return _changedMax;
            }
        }
        public float DefaultAmount => _defaultAmount;
        public StatusAmount(float defaultAmount)
        {
            this._defaultAmount = defaultAmount;
            currentAmount = defaultAmount;
        }

        public void AddChanged(float amount)
        {
            temporaryChanged += amount;
            dirty = true;
        }
        public void SetChanged(float amount)
        {
            temporaryChanged = amount;
            dirty = true;
        }
        public void ResetChanged()
        {
            temporaryChanged = 0f;
            dirty = true;
        }
        public void AddRatio(float ratio)
        {
            temporaryRatio += ratio;
            dirty = true;
        }
        public void SetRatio(float ratio)
        {
            temporaryRatio = ratio;
            dirty = true;
        }
        public void ResetRatio()
        {
            temporaryRatio = 1f;
            dirty = true;
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
// unicode形式で保存済み