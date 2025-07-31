using System;
using System.Collections.Generic;
using System.Text;

namespace RPG_003.DataManagements.Datas.Helper
{
    public static class StatusDataExtensions
    {
        /// <summary>
        /// CSV文字列からStatusDataを生成する（属性倍率含む）
        /// </summary>
        public static StatusData FromCsv(this string csvLine)
        {
            var values = csvLine.Split(',');
            if (values.Length < 12)
                throw new ArgumentException("CSV line does not contain enough values.");

            string name = values[0].Trim();
            float hp = float.Parse(values[1].Trim());
            float mp = float.Parse(values[2].Trim());
            float str = float.Parse(values[3].Trim());
            float intelligence = float.Parse(values[4].Trim());
            float spd = float.Parse(values[5].Trim());
            float def = float.Parse(values[6].Trim());
            float mdef = float.Parse(values[7].Trim());
            float luk = float.Parse(values[8].Trim());
            float criticalRate = float.Parse(values[9].Trim());
            float criticalDamage = float.Parse(values[10].Trim());
            float takeDamageScale = float.Parse(values[11].Trim());

            List<StatusData.ElementDamageScale> takeElement = new();
            List<StatusData.ElementDamageScale> giveElement = new();

            for (int i = 12; i < values.Length; i++)
            {
                var parts = values[i].Trim().Split(':');
                if (parts.Length >= 2 && System.Enum.TryParse(parts[0], out Elements element))
                {
                    float take = float.Parse(parts[1].Trim());
                    takeElement.Add(new StatusData.ElementDamageScale(element, take));

                    if (parts.Length >= 3)
                    {
                        float give = float.Parse(parts[2].Trim());
                        giveElement.Add(new StatusData.ElementDamageScale(element, give));
                    }
                }
            }

            return new StatusData(
                name, hp, mp, str, intelligence, spd, def, mdef, luk,
                criticalRate, criticalDamage, takeDamageScale,
                takeElement, giveElement
            );
        }

        /// <summary>
        /// StatusData を CSV文字列に変換する
        /// </summary>
        public static string ToCsv(this StatusData status)
        {
            var sb = new StringBuilder();
            sb.Append($"{status.Name},{status.HP},{status.MP},{status.STR},{status.INT},{status.SPD},");
            sb.Append($"{status.DEF},{status.MDEF},{status.LUK},{status.CR},{status.CRDamage},{status.TakeDamageScale}");

            var elementDict = new Dictionary<Elements, (float take, float give)>();

            foreach (var t in status.TakeElementDamageScale)
            {
                if (!elementDict.ContainsKey(t.element))
                    elementDict[t.element] = (t.scale, 1f);
                else
                    elementDict[t.element] = (t.scale, elementDict[t.element].give);
            }

            foreach (var g in status.GiveElementDamageScale)
            {
                if (!elementDict.ContainsKey(g.element))
                    elementDict[g.element] = (1f, g.scale);
                else
                    elementDict[g.element] = (elementDict[g.element].take, g.scale);
            }

            foreach (var pair in elementDict)
            {
                var (take, give) = pair.Value;
                sb.Append($",{pair.Key}:{take}:{give}");
            }

            return sb.ToString();
        }
    }
}
