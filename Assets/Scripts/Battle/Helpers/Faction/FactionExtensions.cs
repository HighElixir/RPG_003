using UnityEngine;

namespace RPG_003.Battle.Factions
{
    public static class FactionExtensions
    {
        public static Color FactionToColor(this Faction faction)
        {
            return faction switch
            {
                Faction.Ally => Color.blue,
                Faction.Enemy => Color.red,
                _ => Color.cyan
            };
        }

        // 陣営を日本語に変換する拡張メソッド
        public static string ToJapanese(this Faction faction)
        {
            return faction switch
            {
                Faction.None => "なし",
                Faction.All => "全体",
                Faction.Ally => "味方",
                Faction.Enemy => "敵",
                _ => faction.ToString()
            };
        }
    }
}