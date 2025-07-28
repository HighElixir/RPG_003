using System;
using System.Collections.Generic;
using System.Linq;

namespace RPG_003.Battle
{
    public static class TargetInfoExtensions
    {
        /// <summary>
        /// targersの0番目をMainTargetに、それ以外をAdditionalTargetに割り当てる.
        /// </summary>
        public static TargetInfo AsTargetInfo(this List<Unit> targets)
        {
            if (targets == null || targets.Count == 0)
                throw new ArgumentException("targets が空だよ");

            // 先頭要素をそのまま取得
            var first = targets[0];
            // 先頭以外を別リストに複製
            var rest = targets.Skip(1).ToList();

            return new TargetInfo(first, rest);
        }

    }
}