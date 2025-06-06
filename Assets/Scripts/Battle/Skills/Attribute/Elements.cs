using System;

namespace RPG_003.Skills
{
    [Flags]
    public enum Elements
    {
        None = 0,          // 無属性
        Fire = 1 << 0,     // 火属性
        Water = 1 << 1,    // 水属性
        Earth = 1 << 2,    // 地属性
        Air = 1 << 3,      // 風属性
        Light = 1 << 4,    // 光属性
        Dark = 1 << 5,     // 闇属性
        Electric = 1 << 6, // 電気属性
    }
}
// unicode 