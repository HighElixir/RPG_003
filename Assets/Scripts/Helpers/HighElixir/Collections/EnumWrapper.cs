using System;
using System.Collections.Generic;
using System.Linq;

namespace HighElixir
{
    public static class EnumWrapper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">enum</typeparam>
        /// <returns>全ての値を格納したリスト</returns>
        public static List<T> GetEnumList<T>() where T : Enum
    => Enum.GetValues(typeof(T)).Cast<T>().ToList();

    }
}