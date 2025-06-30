using RPG_003.Battle;
using RPG_003.Effect;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RPG_003.Skills
{
    [Serializable]
    public abstract class SkillDataHolder
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
        public SoundVFXData SoundVFXData => _soundVFXData;
        public abstract bool IsValid(out string errorMessage);

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