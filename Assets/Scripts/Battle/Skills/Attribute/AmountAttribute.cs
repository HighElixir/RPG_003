using System;

namespace RPG_001.Skills
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
}
// unicode