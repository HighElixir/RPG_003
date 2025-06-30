namespace RPG_003.Status
{
    public enum Elements
    {
        None = 0,          // 無属性
        Fire,     // 火属性
        Water,    // 水属性
        Earth,    // 地属性
        Electric, // 電気属性
        Air,      // 風属性
        Light,    // 光属性
        Dark,     // 闇属性
    }

    public static class ElementsExtensions
    {
        public static string ToJapanese(this Elements element)
        {
            return element switch
            {
                Elements.None => "無属性",
                Elements.Fire => "火属性",
                Elements.Water => "水属性",
                Elements.Earth => "地属性",
                Elements.Electric => "電気属性",
                Elements.Air => "風属性",
                Elements.Light => "光属性",
                Elements.Dark => "闇属性",
                _ => element.ToString()
            };
        }
    }
}
// unicode 