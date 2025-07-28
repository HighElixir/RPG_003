using System;
using System.Collections.Generic;
using System.Text;

namespace RPG_003.Skills
{
    public static partial class ExcelDataHelper
    {
        // === CostData ===
        public static string ToExcelFormat(List<CostData> costs)
        {
            var sb = new StringBuilder();
            foreach (var cost in costs)
            {
                sb.Append($"{cost.amount}:{cost.isHP},");
            }

            if (sb.Length > 0)
                sb.Length--;

            return sb.ToString();
        }

        public static List<CostData> FromExcelFormat_Cost(string cell)
        {
            var result = new List<CostData>();
            if (string.IsNullOrWhiteSpace(cell))
                return result;

            var entries = cell.Split(',');
            foreach (var entry in entries)
            {
                var parts = entry.Split(':');
                if (parts.Length != 2) continue;

                if (!float.TryParse(parts[0], out var amount)) continue;
                if (!bool.TryParse(parts[1], out var isHP)) continue;

                result.Add(new CostData
                {
                    amount = amount,
                    isHP = isHP
                });
            }

            return result;
        }
    }
}
