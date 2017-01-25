using System;
using System.Collections.Generic;
using System.Linq;
using Market.Analyzer;
using Market.Analyzer.Channels;

namespace Market.Suggestions.MACD
{
    public class MACDSuggestionAnalyzer : ISuggestionAnalyzer
    {
        public string Name { get { return "MovingAverageConvergenceDivergence"; } }
        public Term Term { get; private set; }
        public Action Action { get; private set; }
        public double Price { get; private set; }

        private readonly int shortTerm;
        private readonly int interTerm;
        private readonly int longTerm;

        private readonly StockContext stockContext;
        private readonly TrendChannelAnalyzer trendChannelAnalyzer;

        public MACDSuggestionAnalyzer() : this(20, 50, 100)
        {}

        public MACDSuggestionAnalyzer(int shortTerm, int interTerm, int longTerm)
        {
            this.shortTerm = shortTerm;
            this.interTerm = interTerm;
            this.longTerm = longTerm;
            stockContext = new StockContext();
            trendChannelAnalyzer = new TrendChannelAnalyzer();
        }

        public double CalculateForecaseCertainty(IList<TransactionData> orderedTransactions)
        {
            var count = orderedTransactions.Count;
            if (count < 100)
                return 0;

            int stockKey = orderedTransactions[0].StockKey;
            DateTime startTime = orderedTransactions[0].TimeStamp;
            DateTime endTime = orderedTransactions[count - 1].TimeStamp;
            var shortTrendChannel = stockContext.Channels.FirstOrDefault(c => c.StockKey == stockKey && c.Length == shortTerm && c.EndDate == endTime);
            var interTrendChannel = stockContext.Channels.FirstOrDefault(c => c.StockKey == stockKey && c.Length == interTerm && c.EndDate == endTime);
            var longTrendChannel = stockContext.Channels.FirstOrDefault(c => c.StockKey == stockKey && c.Length == longTerm && c.EndDate == endTime);
            var result = stockContext.MovingAverageConvergenceDivergences.Where(m => m.StockKey == stockKey && m.TimeStamp >= startTime && m.TimeStamp <= endTime).OrderBy(m => m.TimeStamp);
            var array = result.ToArray();
            double peakHistogram;
            int peakHistogramIndex;
            int sign = GetPeakHistogram(array, out peakHistogram, out peakHistogramIndex);
            if (sign > 0)
            {
                if (peakHistogramIndex == count - 1 && Math.Sign(array[count - 2].Histogram) != sign)
                {
                    Action = Action.Buy;
                    if (longTrendChannel.ChannelTrend.GetSign() <= 0)
                    {
                        if (longTrendChannel.IsSupportStrong())
                            Term = Term.Long;
                        else
                            return 0;
                    }
                    else if (interTrendChannel.ChannelTrend.GetSign() <= 0)
                    {
                        if (interTrendChannel.IsSupportStrong())
                            Term = Term.Intermediate;
                        else
                            return 0;
                    }
                    else
                    {
                        Term = Term.Short;
                    }
                    return 1;
                }
                if (peakHistogramIndex == count - 2)
                {
                    Action = Action.Sell;
                    if (longTrendChannel.ChannelTrend.GetSign() >= 0)
                    {
                        if (interTrendChannel.ChannelTrend.GetSign() >= 0)
                        {
                            if (interTrendChannel.IsResistanceStrong())
                                Term = Term.Short;
                            else
                                return 0;
                        }
                        else
                        {
                            if (longTrendChannel.IsResistanceStrong())
                                Term = Term.Intermediate;
                            else
                                Term = Term.Short;
                        }
                    }
                    else
                    {
                        Term = Term.Long;
                    }
                    return 1;
                }
            }
            else
            {
                if (peakHistogramIndex == count - 1 && Math.Sign(array[count - 2].Histogram) != sign)
                {
                    Action = Action.Sell;
                    if (longTrendChannel.ChannelTrend.GetSign() >= 0)
                    {
                        if (interTrendChannel.ChannelTrend.GetSign() >= 0)
                        {
                            if (interTrendChannel.IsResistanceStrong())
                                Term = Term.Intermediate;
                            else
                                return 0;
                        }
                        else
                        {
                            if (longTrendChannel.IsResistanceStrong())
                                Term = Term.Long;
                            else
                                Term = Term.Intermediate;
                        }
                    }
                    else
                    {
                        Term = Term.Long;
                    }
                    return 1;
                }
                if (peakHistogramIndex == count - 2)
                {
                    Action = Action.Buy;
                    if (longTrendChannel.ChannelTrend.GetSign() <= 0)
                    {
                        return 0;
                    }
                    if (interTrendChannel.ChannelTrend.GetSign() <= 0)
                    {
                        if (interTrendChannel.IsSupportStrong())
                            Term = Term.Short;
                        else
                            return 0;
                    }
                    else
                    {
                        Term = Term.Short;
                    }
                    return 1;
                }
            }
            return 0;
        }

        private int GetPeakHistogram(MovingAverageConvergenceDivergence[] array, out double peakHistogram,
            out int peakHistogramIndex)
        {
            int count = array.Length;
            int sign = Math.Sign(array[count - 1].Histogram);
            peakHistogram = 0;
            peakHistogramIndex = count - 1;
            for (int i = count - 1; i > 0; i--)
            {
                if (sign != Math.Sign(array[i].Histogram))
                    break;
                if (peakHistogram < Math.Abs(array[i].Histogram))
                {
                    peakHistogram = Math.Abs(array[i].Histogram);
                    peakHistogramIndex = i;
                }
            }
            return sign;
        }
    }
}