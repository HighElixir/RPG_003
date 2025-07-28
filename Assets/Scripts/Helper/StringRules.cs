using RPG_003.Battle;
using System.Collections.Generic;
using static Unity.VisualScripting.Member;
using static UnityEngine.GraphicsBuffer;

namespace RPG_003.Helper
{
    /// <summary>
    /// 文字列変換に関するルールとユーティリティを提供するクラス。
    /// </summary>
    /// <remarks>
    /// <see cref="StringRules"/> クラスは、文字列中のプレースホルダー（例：@Target や @Source）を
    /// 特定の値（ユニット名など）に変換するためのルールを定義しています。
    /// 利用可能なルールは <see cref="TransRules"/> 列挙体で表されます。
    /// </remarks>
    public static class StringRules
    {
        public enum TransRules
        {
            Target, // ターゲット名に変換
            Source, // ソース名に変換
            Element, // 属性名に変換
        }
        private static readonly Dictionary<string, TransRules> _rules = new Dictionary<string, TransRules>()
        {
            { "@Target", TransRules.Target },
            { "@Source", TransRules.Source },
            { "@Element", TransRules.Element },
        };

        public static string TransTextSourceToTarget(string input, Unit source, Unit target)
        {
            if (string.IsNullOrEmpty(input)) return input;
            foreach (var rule in _rules)
            {
                if (input.Contains(rule.Key))
                {
                    switch (rule.Value)
                    {
                        case TransRules.Target:
                            input = input.Replace(rule.Key, target.Data.Name ?? "???");
                            break;
                        case TransRules.Source:
                            input = input.Replace(rule.Key, source.Data.Name ?? "???");
                            break;
                    }
                }
            }
            return input;
        }
    }
}