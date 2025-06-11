using RPG_003.Battle.Factions;
using RPG_003.Effect;
using RPG_003.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Battle.Characters.Player
{
    /// <summary>
    /// バトルマネージャーに引き渡すためのデータクラス
    /// </summary>
    [Serializable]
    public class SkillData
    {

        [SerializeField] private string _name;
        [SerializeField] private string _description;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private List<DamageData> _damageData;
        [SerializeField] private List<CostData> _costDatas;
        [SerializeField] private Faction _target;
        [SerializeField, Min(1)] private int _targetCount;
        [SerializeField] private SoundVFXData _vFXData;
        public string Name => _name;
        public string Description => _description;
        public Sprite Sprite => _sprite;
        public List<DamageData> DamageData => _damageData;
        public List<CostData> CostDatas => _costDatas;
        public Faction Target => _target;
        public int TargetCount => _targetCount;
        public SoundVFXData VFXData => _vFXData;

        public SkillData(string name, string desc, List<DamageData> damageDatas, List<CostData> costDatas, Faction target, int targetCount, SoundVFXData vFXData)
        {
            _name = name;
            _description = desc;
            _damageData = damageDatas;
            _costDatas = costDatas;
            _target = target;
            _vFXData = vFXData;
            switch (target)
            {
                case Faction.All:
                    _targetCount = Math.Min(targetCount, 9);
                    break;
                case Faction.Enemy:
                    _targetCount = Math.Min(targetCount, 5);
                    break;
                case Faction.Ally:
                    _targetCount = Math.Min(targetCount, 4);
                    break;
                default:
                    _targetCount = 0;
                    break;
            }
        }
    }
    [Serializable]
    public struct DamageData
    {
        public float amount; // ステータスにかかる係数
        public float fixedAmount; // 固定値
        public StatusAttribute type;
        public Elements element;
        public AmountAttribute amountAttribute;
    }
    [Serializable]
    public struct CostData
    {
        public float amount;
        public StatusAttribute type;
        public Symbol symbol;

        public enum Symbol
        {
            GreaterThen,
            More,
            Below,
            Less,
        }
    }
}