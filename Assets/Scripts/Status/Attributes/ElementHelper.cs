namespace RPG_003.Status
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
                _ => Elements.None
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
    }
}
