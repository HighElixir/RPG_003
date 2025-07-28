using UnityEngine;
namespace RPG_003.Status
{
    public enum Elements
    {
        None = 0,          // 無属性
        Physics,          // 無属性
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
                Elements.Physics => "無属性",
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


        public static Color GetColorElement(this Elements elements)
        {
            return elements switch
            {
                Elements.Physics => Color.white,
                Elements.Fire => Color.red,
                Elements.Water => new Color(65, 105, 255, 1),
                Elements.Earth => new Color(0, 100, 0, 1),
                Elements.Air => new Color(60, 179, 113, 1),
                Elements.Electric => new Color(225, 215, 0, 1),
                Elements.Light => new Color(255, 250, 205, 1),
                Elements.Dark => new Color(72, 61, 139, 1),
                _ => Color.magenta,
            };
        }
    }
}