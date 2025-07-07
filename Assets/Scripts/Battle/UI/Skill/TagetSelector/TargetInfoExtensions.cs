using System.Collections.Generic;

namespace RPG_003.Battle
{
    public static class TargetInfoExtensions
    {
        /// <summary>
        /// targersの0番目をMainTargetに、それ以外をAdditionalTargetに割り当てる.
        /// </summary>
        public static TargetInfo AsTargetInfo(this List<Unit> targets)
        {
            var clone = new List<Unit>(targets);
            var temp = clone[0];
            clone.Remove(temp);
            return new TargetInfo(temp, clone);
        }
    }
}