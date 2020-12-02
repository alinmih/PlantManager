using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlantManagerAPI.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Times<T>(this int count, Func<int, T> func)
        {
            for (int i = 1; i < count; i++)
            {
                yield return func.Invoke(i);
            }
        }
    }
}
