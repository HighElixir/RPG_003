namespace RPG_003.DataManagements.Datas.Helper
{
    public static class AmountAttributeExtensions
    {
        public static string ToJapanese(this AmountAttribute attribute)
        {
            return attribute switch
            {
                AmountAttribute.None => "なし",
                AmountAttribute.Physic => "物理ダメージ",
                AmountAttribute.Magic => "魔法ダメージ",
                AmountAttribute.Heal => "回復",
                AmountAttribute.Consume => "消費",
                _ => attribute.ToString()
            };
        }
    }
}