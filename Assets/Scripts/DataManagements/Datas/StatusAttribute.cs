namespace RPG_003.DataManagements.Datas
{
    public enum StatusAttribute
    {
        None,
        HP,
        MP,
        STR,
        INT,
        SPD,
        DEF,
        MDEF,
        LUK,
        HATE,
        MaxHP,
        MaxMP,
        CriticalRate,
        CriticalDamage,
        TakeDamageScale,

        T_Fire,     // 受ける炎ダメージ倍率
        T_Water,      // 受ける水ダメージ倍率
        T_Electric,// 受ける雷ダメージ倍率
        T_Wind,     // 受ける風ダメージ倍率
        T_Earth,    // 受ける地ダメージ倍率
        T_Light,    // 受ける光ダメージ倍率
        T_Dark,     // 受ける闇ダメージ倍率
        T_Pysical,  // 受ける物理ダメージ倍率

        A_Fire,     // 与える炎ダメージ倍率
        A_Water,      // 与える水ダメージ倍率
        A_Electric,// 与える雷ダメージ倍率
        A_Wind,     // 与える風ダメージ倍率
        A_Earth,    // 与える地ダメージ倍率
        A_Light,    // 与える光ダメージ倍率
        A_Dark,     // 与える闇ダメージ倍率
        A_Pysical,  // 与える物理ダメージ倍率
    }

    public static class StatusAttributeExtensions
    {
        public static string ToJapanese(this StatusAttribute attribute)
        {
            return attribute switch
            {
                StatusAttribute.HP => "HP",
                StatusAttribute.MP => "MP",
                StatusAttribute.STR => "STR",
                StatusAttribute.INT => "INT",
                StatusAttribute.SPD => "SPD",
                StatusAttribute.DEF => "DEF",
                StatusAttribute.MDEF => "MDEF",
                StatusAttribute.LUK => "LUK",
                StatusAttribute.HATE => "HATE",
                StatusAttribute.MaxHP => "MaxHP",
                StatusAttribute.MaxMP => "MaxMP",
                StatusAttribute.CriticalRate => "会心率",
                StatusAttribute.CriticalDamage => "会心ダメージ",
                StatusAttribute.TakeDamageScale => "ダメージ軽減",
                // 受けるダメージ倍率
                StatusAttribute.T_Fire => "受ける炎ダメージ倍率",
                StatusAttribute.T_Water => "受ける水ダメージ倍率",
                StatusAttribute.T_Electric => "受ける雷ダメージ倍率",
                StatusAttribute.T_Wind => "受ける風ダメージ倍率",
                StatusAttribute.T_Earth => "受ける地ダメージ倍率",
                StatusAttribute.T_Light => "受ける光ダメージ倍率",
                StatusAttribute.T_Dark => "受ける闇ダメージ倍率",
                StatusAttribute.T_Pysical => "受ける物理ダメージ倍率",
                // 与えるダメージ倍率
                StatusAttribute.A_Fire => "与える炎ダメージ倍率",
                StatusAttribute.A_Water => "与える水ダメージ倍率",
                StatusAttribute.A_Electric => "与える雷ダメージ倍率",
                StatusAttribute.A_Wind => "与える風ダメージ倍率",
                StatusAttribute.A_Earth => "与える地ダメージ倍率",
                StatusAttribute.A_Light => "与える光ダメージ倍率",
                StatusAttribute.A_Dark => "与える闇ダメージ倍率",
                StatusAttribute.A_Pysical => "与える物理ダメージ倍率",
                _ => attribute.ToString(),
            };
        }
    }
}