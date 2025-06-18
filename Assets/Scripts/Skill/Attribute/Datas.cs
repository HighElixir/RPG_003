using RPG_003.Status;
using RPG_003.Battle.Factions;
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
    }
    [Serializable]
    public struct CostData
    {
        public float amount;
        public bool isHP;
        public Symbol symbol;

        public enum Symbol
        {
            [Tooltip("以上")]
            GreaterThen,
            [Tooltip("より大きい")]
            More,
            [Tooltip("以下")]
            Below,
            [Tooltip("未満")]
            Less,
        }
    }

    [Serializable]
    public struct TargetData
    {
        [SerializeField] private bool _isSelf;
        [SerializeField] private Faction _faction;
        [SerializeField] private int _count;

        public bool IsSelf => _isSelf;
        public Faction Faction => _faction;
        public int Count => _count;

        public TargetData(bool isSelf, Faction faction, int count)
        {
            _isSelf = isSelf;
            _faction = faction;
            _count = Math.Max(count, 1);
        }
    }
}