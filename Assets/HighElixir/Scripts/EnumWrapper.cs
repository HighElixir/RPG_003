using System;
using System.Collections.Generic;
using System.Linq;

namespace HighElixir
{
    public static class EnumWrapper
    {
        /// <typeparam name="T">enum</typeparam>
        /// <returns>全ての値を格納したリスト</returns>
        public static List<T> GetEnumList<T>() where T : Enum
    => Enum.GetValues(typeof(T)).Cast<T>().ToList();
    }

    public static class OverItem
    {
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