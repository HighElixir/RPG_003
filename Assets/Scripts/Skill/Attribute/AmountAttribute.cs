using System;

namespace RPG_003.Skills
{
    [Flags]
    public enum AmountAttribute
    {
        None = 0,       // No attributes set
        Physic = 1 << 0,  // Bitwise flag for physical attributes
        Magic = 1 << 1,  // Bitwise flag for magical attributes
        Heal = 1 << 2,  // Bitwise flag for healing attributes
        Consume = 1 << 3,  // Bitwise flag for consumable attributes
    }

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
// unicode