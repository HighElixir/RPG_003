using System;
using System.Collections.Generic;
using System.Linq;

namespace HighElixir
{
    public static class EnumWrapper
    {
        // おそらく重いので乱用は控える
        /// <typeparam name="T">enum</typeparam>
        /// <returns>特定の列挙型の全ての値を格納したリスト</returns>
        public static List<T> GetEnumList<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }
    }

    public static class OverItem
    {
        /// <summary>
        /// 特定の大きさを超過したアイテムをすべて返す.
        /// </summary>
        /// <param name="allowSize">許可されるリストの大きさ</param>
        /// <param name="res"><ass cref="allowSize">allowSize</cref>を超過したアイテム</param>
        /// <returns>超過していたかどうか</returns>
        public static bool TryGetOverItem<T>(this List<T> list, int allowSize, out List<T> res)
        {
            res = new List<T>();
            var temp = list.Count - allowSize;
            if (temp > 0)
            {
                var index = list.Count - 1;
                for (int i = 0; i < temp; i++)
                {
                    res.Add(list[index - i]);
                }
                return true;
            }
            return false;
        }
    }
}