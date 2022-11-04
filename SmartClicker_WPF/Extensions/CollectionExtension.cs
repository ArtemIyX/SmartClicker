using System;
using System.Collections.Generic;

namespace SmartClicker_WPF.Extensions
{
    public static class CollectionExtension
    {
        private static readonly Random Rand = new Random();

        public static T RandomElement<T>(this IList<T> list)
        {
            return list[Rand.Next(list.Count)];
        }

        public static T RandomElement<T>(this T[] array)
        {
            return array[Rand.Next(array.Length)];
        }
    }
}
