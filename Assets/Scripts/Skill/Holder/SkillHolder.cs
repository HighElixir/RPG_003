using RPG_003.Battle;
using RPG_003.Effect;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RPG_003.Skills
{
    [Serializable]
    public abstract class SkillHolder
    {
        [SerializeField] protected Sprite _custonIcon;
        [SerializeField] protected SoundVFXData _soundVFXData;
        [SerializeField] protected string _custonName;
        [SerializeField] protected string _custonDesc;

        // === Properties ===
        public abstract SkillData SkillData { get; }
        public abstract Sprite Icon { get; }
        public abstract string Name { get; }
        public abstract string Desc { get; }
        public SoundVFXData SoundVFXData => SkillData.Sfx ?? _soundVFXData;

        /// <param name="errorCode">正常値＝０</param>
        public abstract bool IsValid(out int errorCode);

        // === Public ===
        public void SetIcon(Sprite icon)
        {
            _custonIcon = icon;
        }
        public void SetSoundVFXData(SoundVFXData soundVFXData)
        {
            _soundVFXData = soundVFXData;
        }
        public void SetCustomName(string name)
        {
            _custonName = name;
        }
        public void SetCustomDesc(string desc)
        {
            _custonDesc = desc;
        }
        public abstract SkillDataInBattle ConvartData();

        /// <summary>
        /// 新しいスキルデータのセットに伴い、入れ替えが必要かどうか
        /// </summary>
        /// <param name="newItem">セット予定のデータ</param>
        /// <param name="oldItems">取り除きが必要なデータ</param>
        /// <returns></returns>
        public abstract bool IsNeedReplace(SkillData newItem, out List<SkillData> oldItems);
        public abstract bool CanSetSkillData(SkillData data);
        public abstract void SetSkillData(SkillData data);

        /// <summary>
        /// 全てのダメージデータに設定されたクリティカル率の平均から算出
        /// </summary>
        public abstract float GetCriticalRate();
        /// <summary>
        /// 全てのダメージデータに設定されたクリティカル率の平均から算出
        /// </summary>
        public abstract float GetCriticalDamage();
        public abstract bool RemoveSkillData(SkillData data);
        /// <summary>
        /// スキルを外すときに他のものも連鎖的に外れる場合に使う
        /// </summary>
        public abstract bool RemoveSkillData(SkillData data, out List<SkillData> list);
        /// <summary>
        /// 保持している全てのスキルデータを取得するメソッド
        /// </summary>
        public abstract IReadOnlyList<SkillData> GetSkillDatas();
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Name: {Name}");
            sb.AppendLine($"Description: {Desc}");
            sb.AppendLine($"Icon: {Icon?.name ?? "No Icon"}");
            sb.AppendLine($"SoundVFXData: {SoundVFXData?.name ?? "No SoundVFXData"}");
            return sb.ToString();
        }
    }
}