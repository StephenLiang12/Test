using System.Collections.Generic;
using System.Linq;

namespace Market.Analyzer
{
    public static class EnumerableExtension
    {
        public static double GetMedian(this IEnumerable<double> enumerable)
        {
            double median = 0;
            var orderedVariances = enumerable.OrderBy(d => d);
            var count = enumerable.Count();
            int halfIndex = count / 2;
            if (count % 2 == 0)
            {
                median = (orderedVariances.ElementAt(halfIndex) + orderedVariances.ElementAt(halfIndex - 1)) / 2;
            }
            else
            {
                median = orderedVariances.ElementAt(halfIndex);
            }
            return median;
        }
    }
}