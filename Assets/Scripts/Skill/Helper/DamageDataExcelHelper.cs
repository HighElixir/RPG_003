using RPG_003.Status;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPG_003.Skills
{
    public static partial class ExcelDataHelper
    {
        public static string ToExcelFormat(List<DamageData> datas)
        {
            if (datas == null || datas.Count == 0)
                return "";

            var sb = new StringBuilder();
            foreach (var d in datas)
            {
                // {type}:{amount}:{fixed}:{element}:{amountAttr}:{crit}:{critBonus}:{variance}
                sb.Append($"{d.type}:{d.amount}:{d.fixedAmount}:{d.element}:{d.amountAttribute}:{d.criticalRate}:{d.criticalRateBonus}:{d.variance},");
            }

            // 最後のカンマを削除
            if (sb.Length > 0)
                sb.Length--;

            return sb.ToString();
        }

        public static List<DamageData> FromExcelFormat(string cell)
        {
            var result = new List<DamageData>();
            if (string.IsNullOrWhiteSpace(cell))
                return result;

            var entries = cell.Split(',');
            foreach (var entry in entries)
            {
                var parts = entry.Split(':');
                if (parts.Length != 8)
                    continue;

                if (!float.TryParse(parts[1], out var amount)) continue;
                if (!float.TryParse(parts[2], out var fixedAmount)) continue;
                if (!float.TryParse(parts[5], out var crit)) continue;
                if (!float.TryParse(parts[6], out var critBonus)) continue;
                if (!float.TryParse(parts[7], out var variance)) continue;

                var data = new DamageData
                {
                    type = Enum.TryParse(parts[0], out StatusAttribute typeVal) ? typeVal : StatusAttribute.STR,
                    amount = amount,
                    fixedAmount = fixedAmount,
                    element = Enum.TryParse(parts[3], out Elements elemVal) ? elemVal : Elements.Physics,
                    amountAttribute = Enum.TryParse(parts[4], out AmountAttribute attrVal) ? attrVal : AmountAttribute.Physic,
                    criticalRate = crit,
                    criticalRateBonus = critBonus,
                    variance = variance,
                };

                result.Add(data);
            }

            return result;
        }
    }
}
