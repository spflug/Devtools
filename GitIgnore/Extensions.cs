using System;
using System.Collections.Generic;
using System.Linq;

namespace GitIgnore
{
    internal static class Extensions
    {
        internal static bool In<T>(this T value, params T[] values) => values.Contains(value);

        internal static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            var localAction = action ?? (t => { });
            var localSource = source ?? Enumerable.Empty<T>();

            foreach (var item in localSource)
                localAction(item);
        }
    }
}