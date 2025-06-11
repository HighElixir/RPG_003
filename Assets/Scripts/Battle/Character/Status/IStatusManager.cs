using System;
using System.Collections.Generic;

namespace RPG_003.Battle.Characters
{
    public interface IStatusManager
    {
        ICharacter Parent { get; set; }
        float HP { get; set; }
        float MaxHP { get; }
        void Initialize(ICharacter parent, CharacterData data);
        void TakeDamage(DamageInfo damage);
        void TakeHeal(DamageInfo info);
        StatusAmount GetStatusAmount(StatusAttribute status);
        bool TryGetStatus(StatusAttribute status, out StatusAmount amount);
        List<StatusAttribute> GetStatusList();
        bool IsRegistered(StatusAttribute status);
        /// <summary>
        /// 既に存在する場合は上書きされる
        /// </summary>
        /// <param name="status">追加したいステータス</param>
        /// <param name="amount">値</param>
        void AddStatus(StatusAttribute status, float amount);

        /// <summary>
        /// ステータスを削除できるが、HP等の必須ステータスは残すこと
        /// Note:無敵にしたい場合はTakeDamageIncreaseを0にすればいい
        /// </summary>
        /// <param name="status"></param>
        void RemoveStatus(StatusAttribute status);

        /// <summary>
        /// 値を更新することが明示的なメソッド
        /// 存在しないステータスを参照するとエラーログを返すが何もしない
        /// </summary>
        void UpdateStatus(StatusAttribute status, float amount);

        /// <summary>
        /// 指定したステータスを増減させる倍率を足せる
        /// 0未満にはならない
        /// </summary>
        void AddRatio(StatusAttribute status, float ratio);

        /// <summary>
        /// 指定したステータスのratioをリセット
        /// </summary>
        /// <param name="status"></param>
        void ResetRatio(StatusAttribute status);

        /// <summary>
        /// 0未満にはならない
        /// </summary>
        void AddChanged(StatusAttribute status, float amount);

        /// <summary>
        /// 指定したステータスの最大値の加減算をリセット
        /// </summary>
        /// <param name="status"></param>
        void ResetChanged(StatusAttribute status);
    }

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