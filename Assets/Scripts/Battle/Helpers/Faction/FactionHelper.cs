using RPG_003.Battle.Characters;

namespace RPG_003.Battle.Factions
{
    /// <summary>
    /// 陣営の判定に用いるヘルパークラス
    /// </summary>
    public static class FactionHelper
    {
        public static bool IsAlly(this ICharacter character)
        {
            return IsAlly(character.Position);
        }

        public static bool IsAlly(this CharacterPosition position)
        {
            return
                position == CharacterPosition.Player_1 ||
                position == CharacterPosition.Player_2 ||
                position == CharacterPosition.Player_3 ||
                position == CharacterPosition.Player_4;
        }
        public static bool IsEnemy(this ICharacter character)
        {
            return IsEnemy(character.Position);
        }

        public static bool IsEnemy(this CharacterPosition position)
        {
            return
                position == CharacterPosition.Enemy_1 ||
                position == CharacterPosition.Enemy_2 ||
                position == CharacterPosition.Enemy_3 ||
                position == CharacterPosition.Enemy_4 ||
                position == CharacterPosition.Enemy_5;
        }
        public static bool IsSameFaction(this ICharacter c1, ICharacter c2)
        {
            return IsSameFaction(c1.Position, c2.Position);
        }
        public static bool IsSameFaction(this CharacterPosition p1, CharacterPosition p2)
        {
            return
                IsAlly(p1) && IsAlly(p2) ||
                IsEnemy(p1) && IsEnemy(p2);
        }
        public static bool IsSameFaction(this ICharacter c, Faction t)
        {
            return IsSameFaction(c.Position, t);
        }
        public static bool IsSameFaction(this CharacterPosition p, Faction t)
        {
            switch (t)
            {
                case Faction.All:
                    return true;
                case Faction.Enemy:
                    return IsEnemy(p);
                case Faction.Ally:
                    return IsAlly(p);
                default:
                    return false;
            }
        }
    }
}