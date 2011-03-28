using System;
using System.Collections.Generic;

namespace NuTools.Common
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Each<T>(this IEnumerable<T> source, Action<T> action)
        {
            if(action != null)
                foreach (var item in source) action(item);
            return source;
        }
    }
}
