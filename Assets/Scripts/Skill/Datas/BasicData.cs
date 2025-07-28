using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RPG_003.Skills
{
    [CreateAssetMenu(fileName = "Basic", menuName = "RPG_003/Skills/Basic")]
    public class BasicData : SkillData
    {
        [SerializeField] private List<DamageData> _damage;
        [SerializeField] private List<CostData> _costDatas;
        [SerializeReference] private List<EffectData> _effectDatas;
        [SerializeField] private TargetData _target;

        public List<DamageData> DamageDatas => _damage;
        public List<CostData> CostDatas => _costDatas;
        public List<EffectData> EffectDatas => _effectDatas;
        public TargetData Target => _target;

#if UNITY_EDITOR
        public string output = "";
        [Button("Output Damage Data")]
        private void OutPutDamageDatas() =>
            output = ExcelDataHelper.ToExcelFormat(DamageDatas);
        [Button("Output Target Data")]
        private void OutPutTargetData() =>
            output = ExcelDataHelper.ToExcelFormat(Target);
        [Button("Output Cost Data")]
        private void OutPutCostData() =>
            output = ExcelDataHelper.ToExcelFormat(CostDatas);

    }
#endif
}