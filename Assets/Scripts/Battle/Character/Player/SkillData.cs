using System;
using System.Collections.Generic;
using RPG_003.Skills;
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
        public string Name => _name;
        public string Description => _description;
        public Sprite Sprite => _sprite;
        public List<DamageData> DamageData => _damageData;
        public List<CostData> CostDatas => _costDatas;

        public SkillData(string name, string desc, List<DamageData> damageDatas, List<CostData> costDatas)
        {
            _name = name;
            _description = desc;
            _damageData = damageDatas;
            _costDatas = costDatas;
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
    }
}