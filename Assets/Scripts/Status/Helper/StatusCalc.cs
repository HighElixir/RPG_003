using RPG_003.Battle;
using RPG_003.Skills;
using System.Collections.Generic;
using System.Linq;
namespace RPG_003.Status
{
    public static class StatusCalc
    {
        public static float CalcCritRateAverage(this List<DamageData> datas)
        {
            return datas.Average(d => d.criticalRate);
        }
        public static float CalcCritDamageAverage(this List<DamageData> datas)
        {
            return datas.Average(d => d.criticalRateBonus);
        }
    }
}