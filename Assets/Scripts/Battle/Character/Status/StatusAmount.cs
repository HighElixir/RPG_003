using System;
using System.Collections.Generic;

namespace RPG_003.Battle.Characters
{
    public class StatusAmount
    {
        public readonly float defaultAmount;
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
                    _changedMax = (defaultAmount + temporaryChanged) * Math.Max(0, temporaryRatio); // デフォルト値に一時的な変更を加え、倍率を掛ける
                }
                return _changedMax;
            }
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
        public StatusAmount(float defaultAmount)
        {
            this.defaultAmount = defaultAmount;
            currentAmount = defaultAmount;
        }
    }
}
// unicode形式で保存済み