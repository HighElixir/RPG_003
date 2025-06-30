using RPG_003.Skills;
using UnityEditor;

namespace RPG_003.Battle.Factions
{
    /// <summary>
    /// 陣営の判定に用いるヘルパークラス
    /// </summary>
    public static class FactionHelper
    {
        public static bool IsAlly(this CharacterObject character)
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
        public static bool IsEnemy(this CharacterObject character)
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
        public static bool IsSameFaction(this CharacterObject c1, CharacterObject c2)
        {
            return IsSameFaction(c1.Position, c2.Position);
        }
        public static bool IsSameFaction(this CharacterPosition p1, CharacterPosition p2)
        {
            return
                IsAlly(p1) && IsAlly(p2) ||
                IsEnemy(p1) && IsEnemy(p2);
        }
        /// <summary>
        /// キャラクターが任意の派閥かどうかを返す
        /// </summary>
        public static bool IsSameFaction(this CharacterObject c, Faction t)
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
        public static Faction GetReverse(this Faction faction)
        {
            return faction switch
            {
                Faction.Ally => Faction.Enemy,
                Faction.Enemy => Faction.Ally,
                _ => faction,
            };
        }
    }
}