namespace RPG_003.Battle.Factions
{
    // 陣営を示す列挙型
    public enum Faction
    {
        None = -1,
        All = 0,
        Ally = 1,
        Enemy = 2
    }

    public static class FactionExtensions
    {
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