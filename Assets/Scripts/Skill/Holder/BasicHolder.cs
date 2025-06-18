using RPG_003.Battle.Skills;
using UnityEngine;

namespace RPG_003.Skills
{
    public class BasicHolder : SkillDataHolder
    {
        [SerializeField] private BasicData _skill;

        public override SkillData SkillData => _skill;
        public override Sprite Icon => _custonIcon ? _custonIcon : _skill.DefaultIcon;

        public override string Name => string.IsNullOrEmpty(_custonName) ? _skill.Name : _custonName;

        public override string Desc => string.IsNullOrEmpty(_custonDesc) ? _skill.Description : _custonDesc;

        // ===- Constructor ===
        public BasicHolder(BasicData skill)
        {
            _skill = skill;
        }

        // === Public ===
        public void SetSkillData(BasicData data)
        {
            _skill = data;
        }
        public override SkillDataInBattle ConvartData()
        {
            var s = new SkillDataInBattle(_skill.Name, _skill.Description, _skill.DamageDatas, _skill.CostDatas, _skill.Target);
            return s;
        }
    }
}