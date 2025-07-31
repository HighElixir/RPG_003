using System;

namespace RPG_003.DataManagements.Datas
{
    [Serializable]
    public struct DamageData
    {
        //ステータスにかかる係数
        public float amount;
        //固定値
        public float fixedAmount;
        //amountが参照するステータス
        public StatusAttribute type;
        //ダメージの属性
        public Elements element;
        //このスキルのダメージの値。現状はHealのみ特殊な挙動あり
        public AmountAttribute amountAttribute;
        //クリティカル確率
        public float criticalRate;
        //クリティカルダメージ倍率
        public float criticalRateBonus;
        //ダメージのブレ
        public float variance;
    }
}