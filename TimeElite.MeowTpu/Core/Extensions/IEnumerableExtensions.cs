using System;
using System.Collections.Generic;
using Core.Enums;

namespace Core.Extensions
{
    /// <summary>
    ///     Предоставляет методы для типа <see cref="DateTime" />.
    /// </summary>
    public static class EnumerableExtensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}