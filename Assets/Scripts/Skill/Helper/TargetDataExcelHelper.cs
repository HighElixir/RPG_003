using System;
using System.Collections.Generic;
using System.Text;
using RPG_003.Battle.Factions;

namespace RPG_003.Skills
{
    public static partial class ExcelDataHelper
    {
        // === TargetData ===
        public static string ToExcelFormat(TargetData target)
        {
            return $"{target.IsSelf}:{target.Faction}:{target.Count}:{target.IsRandom}:{target.CanSelectSameTarget}";
        }

        public static TargetData FromExcelFormat_Target(string cell)
        {
            var parts = cell.Split(':');
            if (parts.Length != 5) return default;

            bool.TryParse(parts[0], out var isSelf);
            Enum.TryParse(parts[1], out Faction faction);
            int.TryParse(parts[2], out var count);
            bool.TryParse(parts[3], out var isRandom);
            bool.TryParse(parts[4], out var canSelectSame);

            return new TargetData(isSelf, faction, count, isRandom, canSelectSame);
        }
    }
}
