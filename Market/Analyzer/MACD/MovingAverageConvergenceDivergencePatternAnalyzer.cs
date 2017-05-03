using System;
using System.Collections.Generic;
using System.Linq;

namespace Market.Analyzer.MACD
{
    public class MovingAverageConvergenceDivergencePatternAnalyzer
    {
        private const int LongTerm = 100;
        private const int ShortTerm = 20;

        public MovingAverageConvergenceDivergenceFeature Analyze(
            IList<MovingAverageConvergenceDivergence> list)
        {
            double histogram;
            int peakIndex;
            int count = list.Count;
            MovingAverageConvergenceDivergence[] array = list.ToArray();
            double longTermAvg = array.GetRearPartial(LongTerm).Average(a => Math.Abs(a.Histogram));
            double shortTermAvg = array.GetRearPartial(ShortTerm).Average(a => Math.Abs(a.Histogram));
            var lastMovingAverageConvergenceDivergence = array[array.Length - 1];
            StockContext context = new StockContext();
            var previous = context.MovingAverageConvergenceDivergenceAnalyses.Where(m => m.StockKey == lastMovingAverageConvergenceDivergence.StockKey && m.TimeStamp < lastMovingAverageConvergenceDivergence.TimeStamp).OrderByDescending(m => m.TimeStamp).FirstOrDefault();
            if (previous != null)
            {
                if (previous.Feature == MovingAverageConvergenceDivergenceFeature.FluctuateAroundZero)
                {
                    var dateTime = previous.TimeStamp.AddDays(-40);
                    var previousList =
                        context.MovingAverageConvergenceDivergences.Where(
                            m => m.StockKey == previous.StockKey && m.TimeStamp <= previous.TimeStamp &&
                                       m.TimeStamp > dateTime).OrderBy(m => m.TimeStamp).ToList();
                    double previousShortTermAvg = previousList.GetRearPartial(ShortTerm).Average(a => Math.Abs(a.Histogram));
                    if (Math.Abs(lastMovingAverageConvergenceDivergence.Histogram) > 3*previousShortTermAvg)
                    {
                        if (lastMovingAverageConvergenceDivergence.Histogram > 0)
                            return MovingAverageConvergenceDivergenceFeature.RiseFromZeroAfterFluctuation;
                        return MovingAverageConvergenceDivergenceFeature.DropBelowZeroAfterFluctuation;
                    }
                }
            }
            int sign = GetHistogramSignChangeAndAbsPeak(array, out histogram, out peakIndex);
            if (sign > 0)
            {
                //Histogram changes from negative to positive
                if (peakIndex == count - 1 && Math.Sign(array[count - 2].Histogram) != sign)
                {
                    if (shortTermAvg < 0.2 * longTermAvg)
                        return MovingAverageConvergenceDivergenceFeature.FluctuateAroundZero;
                    if (array[peakIndex].MACD > 0 && array[peakIndex].Signal > 0)
                        return MovingAverageConvergenceDivergenceFeature.AllAboveZero;
                    if (HadSharpDrop(array, longTermAvg))
                        return MovingAverageConvergenceDivergenceFeature.RiseAboveZeroAfterSharpDrop;
                    return MovingAverageConvergenceDivergenceFeature.RiseAboveZero;
                }
                //Histogram just peaked
                if (peakIndex == count - 2)
                {
                    if (shortTermAvg < 0.2 * longTermAvg)
                        return MovingAverageConvergenceDivergenceFeature.FluctuateAroundZero;
                    return MovingAverageConvergenceDivergenceFeature.Peak;
                }
            }
            else
            {
                //Histogram changes from positive to negative
                if (peakIndex == count - 1 && Math.Sign(array[count - 2].Histogram) != sign)
                {
                    if (shortTermAvg < 0.2 * longTermAvg)
                        return MovingAverageConvergenceDivergenceFeature.FluctuateAroundZero;
                    if (array[peakIndex].MACD < 0 && array[peakIndex].Signal < 0)
                        return MovingAverageConvergenceDivergenceFeature.AllBelowZero;
                    if (HadSharpRise(array, longTermAvg))
                        return MovingAverageConvergenceDivergenceFeature.DropBelowZeroAfterSharpRise;
                    return MovingAverageConvergenceDivergenceFeature.DropBelowZero;
                }
                //Histogram just bottomed
                if (peakIndex == count - 2)
                {
                    if (shortTermAvg < 0.2 * longTermAvg)
                        return MovingAverageConvergenceDivergenceFeature.FluctuateAroundZero;
                    return MovingAverageConvergenceDivergenceFeature.Bottom;
                }
            }
            int bottomIndex;
            sign = GetHistogramAbsBottom(array, peakIndex, out bottomIndex);
            if (sign > 0)
            {
                //Histogram just bottomed above zero and the bottom histogram is less than the half of peak histogram
                if (bottomIndex == count - 2 && Math.Sign(array[bottomIndex - 1].Histogram) == sign && Math.Sign(array[bottomIndex + 1].Histogram) == sign && array[bottomIndex].Histogram < array[peakIndex].Histogram/2)
                {
                    return MovingAverageConvergenceDivergenceFeature.RiseFromZero;
                }
            }
            else
            {
                //Histogram just peaked below zero and the abs bottom histogram is less than the half of abs peak histogram
                if (bottomIndex == count - 2 && Math.Sign(array[bottomIndex - 1].Histogram) == sign && Math.Sign(array[bottomIndex + 1].Histogram) == sign && Math.Abs(array[bottomIndex].Histogram) < Math.Abs(array[peakIndex].Histogram / 2))
                {
                    return MovingAverageConvergenceDivergenceFeature.RetrieveFromZero;
                }
            }
            return MovingAverageConvergenceDivergenceFeature.Unkown;
        }

        private bool HadSharpDrop(MovingAverageConvergenceDivergence[] array, double longTermAvg)
        {
            for (int i = array.Length - 2; i >= 0; i--)
            {
                if (array[i].Histogram > 0)
                    return false;
                if (array[i].Histogram < -2*longTermAvg)
                    return true;
            }
            return false;
        }

        private bool HadSharpRise(MovingAverageConvergenceDivergence[] array, double longTermAvg)
        {
            for (int i = array.Length - 2; i >= 0; i--)
            {
                if (array[i].Histogram < 0)
                    return false;
                if (array[i].Histogram > 2*longTermAvg)
                    return true;
            }
            return false;
        }

        private int GetHistogramSignChangeAndAbsPeak(MovingAverageConvergenceDivergence[] array, out double histogram, out int index)
        {
            int count = array.Length;
            int sign = Math.Sign(array[count - 1].Histogram);
            histogram = 0;
            index = count - 1;
            for (int i = count - 1; i > 0; i--)
            {
                if (sign != Math.Sign(array[i].Histogram))
                    break;
                if (histogram < Math.Abs(array[i].Histogram))
                {
                    histogram = Math.Abs(array[i].Histogram);
                    index = i;
                }
            }
            return sign;
        }

        private int GetHistogramAbsBottom(MovingAverageConvergenceDivergence[] array, int peakInex, out int bottomIndex)
        {
            int count = array.Length;
            int sign = Math.Sign(array[count - 1].Histogram);
            var histogram = Double.MaxValue;
            bottomIndex = count - 1;
            for (int i = count - 1; i > peakInex; i--)
            {
                if (sign != Math.Sign(array[i].Histogram))
                    break;
                if (histogram > Math.Abs(array[i].Histogram))
                {
                    histogram = Math.Abs(array[i].Histogram);
                    bottomIndex = i;
                }
            }
            return sign;
        }
    }
}