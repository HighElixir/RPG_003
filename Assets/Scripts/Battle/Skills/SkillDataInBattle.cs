using RPG_003.Battle.Factions;
using RPG_003.Effect;
using RPG_003.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Battle.Skills
{
    /// <summary>
    /// バトルマネージャーに引き渡すためのデータクラス
    /// </summary>
    [Serializable]
    public class SkillDataInBattle
    {

        [SerializeField] private string _name;
        [SerializeField] private string _description;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private List<DamageData> _damageData;
        [SerializeField] private List<CostData> _costDatas;
        [SerializeField] private TargetData _target;
        [SerializeField] private SoundVFXData _vFXData;
        public string Name => _name;
        public string Description => _description;
        public Sprite Sprite => _sprite;
        public List<DamageData> DamageData => _damageData;
        public List<CostData> CostDatas {  get { return _costDatas; } set { _costDatas = value; } }
        public TargetData TargetData { get { return _target; } set { _target = value; } }
        public Faction Target => _target.Faction;
        public int TargetCount => _target.Count;
        public bool IsSelf => _target.IsSelf;
        public SoundVFXData VFXData => _vFXData;

        public SkillDataInBattle(string name, string desc, List<DamageData> damageDatas, List<CostData> costDatas, TargetData targetData)
        {
            _name = name;
            _description = desc;
            _damageData = damageDatas;
            _costDatas = costDatas;
            _target = targetData;
        }

        public void SetVFX(SoundVFXData vFXData)
        {
            _vFXData = vFXData;
        }

        public void SetIcon(Sprite icon)
        {
            _sprite = icon;
        }
    }
}