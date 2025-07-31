using UnityEngine;
namespace RPG_003.DataManagements.Datas.Helper
{
    public static class ElementHelper
    {
        public static Elements GetElementFromStatus(this StatusAttribute status)
        {
            return status switch
            {
                StatusAttribute.T_Fire | StatusAttribute.A_Fire => Elements.Fire,
                StatusAttribute.T_Water | StatusAttribute.A_Water => Elements.Water,
                StatusAttribute.T_Electric | StatusAttribute.A_Electric => Elements.Electric,
                StatusAttribute.T_Wind | StatusAttribute.A_Wind => Elements.Air,
                StatusAttribute.T_Earth | StatusAttribute.A_Earth => Elements.Earth,
                StatusAttribute.T_Light | StatusAttribute.A_Light => Elements.Light,
                StatusAttribute.T_Dark | StatusAttribute.A_Dark => Elements.Dark,
                _ => Elements.Physics
            };
        }
        public static StatusAttribute GetStatusFromElement(this Elements element, bool isAttack)
        {
            return element switch
            {
                Elements.Fire => isAttack ? StatusAttribute.A_Fire : StatusAttribute.T_Fire,
                Elements.Water => isAttack ? StatusAttribute.A_Water : StatusAttribute.T_Water,
                Elements.Electric => isAttack ? StatusAttribute.A_Electric : StatusAttribute.T_Electric,
                Elements.Air => isAttack ? StatusAttribute.A_Wind : StatusAttribute.T_Wind,
                Elements.Earth => isAttack ? StatusAttribute.A_Earth : StatusAttribute.T_Earth,
                Elements.Light => isAttack ? StatusAttribute.A_Light : StatusAttribute.T_Light,
                Elements.Dark => isAttack ? StatusAttribute.A_Dark : StatusAttribute.T_Dark,
                _ => isAttack ? StatusAttribute.A_Pysical : StatusAttribute.T_Pysical,
            };
        }
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
