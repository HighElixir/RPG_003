using RPG_003.Status;
using UnityEngine;

namespace RPG_003.Battle
{
    public static class ColorSet
    {
        public static Color GetColorElement(this Elements elements)
        {
            return elements switch
            {
                Elements.Physics => Color.gray,
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