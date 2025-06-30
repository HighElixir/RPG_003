using RPG_003.Battle.Factions;
using RPG_003.Status;
using System;
using UnityEngine;

namespace RPG_003.Skills
{

    [Serializable]
    public struct DamageData
    {
        [Tooltip("ステータスにかかる係数")]
        public float amount;
        [Tooltip("固定値")]
        public float fixedAmount;
        [Tooltip("amountが参照するステータス")]
        public StatusAttribute type;
        [Tooltip("ダメージの属性")]
        public Elements element;
        [Tooltip("このスキルのダメージの値。現状はHealのみ特殊な挙動あり")]
        public AmountAttribute amountAttribute;
        [Tooltip("クリティカル確率")]
        public float criticalRate;
        [Tooltip("クリティカルダメージ倍率")]
        public float criticalRateBonus;
        [Tooltip("ダメージのブレ")]
        public float variance;
    }
    [Serializable]
    public struct CostData
    {
        public float amount;
        public bool isHP;
    }

    [Serializable]
    public struct TargetData
    {
        [SerializeField] private bool _isSelf;
        [SerializeField] private Faction _faction;
        [SerializeField] private int _count;
        [SerializeField] private bool _isRandom;
        [SerializeField] private bool _canSelectSameTarget;
        public bool IsSelf => _isSelf;
        public Faction Faction => _faction;
        public int Count => _count;
        public bool IsRandom => _isRandom;
        public bool CanSelectSameTarget => _canSelectSameTarget;

        public TargetData(bool isSelf, Faction faction, int count, bool isRandom, bool canSelectSameTarget)
        {
            _isSelf = isSelf;
            _faction = faction;
            _count = Math.Max(count, 1);
            _isRandom = isRandom; // Fix for CS0171: Assigning _isRandom
            _canSelectSameTarget = canSelectSameTarget; // Fix for CS0171: Assigning _canSelectSameTarget
        }
    }
}