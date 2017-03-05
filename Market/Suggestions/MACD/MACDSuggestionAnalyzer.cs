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
            var result = stockContext.MovingAverageConvergenceDivergences.Where(m => m.StockKey == stockKey && m.TimeStamp >= startTime && m.TimeStamp <= endTime).OrderBy(m => m.TimeStamp);
            var array = result.ToArray();
            double peakHistogram;
            int peakHistogramIndex;
            int sign = GetPeakHistogram(array, out peakHistogram, out peakHistogramIndex);
            if (sign > 0)
            {
                if (peakHistogramIndex == count - 1 && Math.Sign(array[count - 2].Histogram) != sign)
                {
                    return CalculateBuyCertainty(orderedTransactions, stockKey, startTime, endTime);
                }
                if (peakHistogramIndex == count - 2)
                {
                    return CalculateSellCertainty(orderedTransactions, stockKey, startTime, endTime);
                }
            }
            else
            {
                if (peakHistogramIndex == count - 1 && Math.Sign(array[count - 2].Histogram) != sign)
                {
                    return CalculateSellCertainty(orderedTransactions, stockKey, startTime, endTime);
                }
                if (peakHistogramIndex == count - 2)
                {
                    return CalculateBuyCertainty(orderedTransactions, stockKey, startTime, endTime);
                }
            }
            return 0;
        }

        private double CalculateSellCertainty(IList<TransactionData> orderedTransactions, int stockKey, DateTime startTime, DateTime endTime)
        {
            Action = Action.Sell;
            var longTrendChannel = GetChannel(stockKey, longTerm, startTime, endTime);
            var interTrendChannel = GetChannel(stockKey, interTerm, startTime, endTime);
            if (longTrendChannel.ResistanceChannelRatio > 0)
            {
                if (longTrendChannel.DoesCloseNearResistanceLine(orderedTransactions))
                {
                    Term = Term.Short;
                    return 1;
                }
                if (interTrendChannel.ResistanceChannelRatio > 0)
                {
                    if (interTrendChannel.IsResistanceStrong(orderedTransactions) &&
                        interTrendChannel.DoesCloseNearResistanceLine(orderedTransactions))
                    {
                        Term = Term.Intermediate;
                        return 1;
                    }
                    return 0;
                }
                Term = Term.Short;
                return 1;
            }
            if (interTrendChannel.ResistanceChannelRatio > 0)
            {
                if (interTrendChannel.IsResistanceStrong(orderedTransactions) &&
                    interTrendChannel.DoesCloseNearResistanceLine(orderedTransactions))
                {
                    Term = Term.Long;
                    return 1;
                }
                Term = Term.Intermediate;
                return 1;
            }
            Term = Term.Long;
            return 1;
        }

        private double CalculateBuyCertainty(IList<TransactionData> orderedTransactions, int stockKey, DateTime startTime, DateTime endTime)
        {
            var longTrendChannel = GetChannel(stockKey, longTerm, startTime, endTime);
            var interTrendChannel = GetChannel(stockKey, interTerm, startTime, endTime);
            Action = Action.Buy;
            if (longTrendChannel.SupportChannelRatio > 0)
            {
                var previousLongTrendChannel = GetPreviousChannel(stockKey, longTerm, endTime);
                if (previousLongTrendChannel != null &&
                    longTrendChannel.SupportChannelRatio < previousLongTrendChannel.SupportChannelRatio)
                    return 0;
                if (longTrendChannel.IsSupportStrong(orderedTransactions) &&
                    longTrendChannel.DoesCloseNearSupportLine(orderedTransactions))
                {
                    Term = Term.Long;
                    return 1;
                }
                if (interTrendChannel.SupportChannelRatio > 0)
                {
                    var previousInterTrendChannel = GetPreviousChannel(stockKey, interTerm, endTime);
                    if (previousInterTrendChannel != null &&
                        interTrendChannel.SupportChannelRatio < previousInterTrendChannel.SupportChannelRatio)
                        return 0;
                    if (interTrendChannel.IsSupportStrong(orderedTransactions) &&
                        interTrendChannel.DoesCloseNearSupportLine(orderedTransactions))
                    {
                        Term = Term.Intermediate;
                        return 1;
                    }
                }
                if (longTrendChannel.DoesCloseNearResistanceLine(orderedTransactions))
                    return 0;
                Term = Term.Short;
                return 1;
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

        private Channel GetChannel(int stockKey, int length, DateTime startTime, DateTime endTime)
        {
            var channel =
                stockContext.Channels.FirstOrDefault(c => c.StockKey == stockKey && c.Length == length && c.EndDate == endTime);
            if (channel != null)
                return channel;
            IList<TransactionData> orderedList =
                stockContext.TransactionData.Where(t => t.StockKey == stockKey && t.TimeStamp >= startTime && t.TimeStamp <= endTime)
                    .OrderBy(t => t.TimeStamp)
                    .ToList();
            var partialList = orderedList.GetRearPartial(length);
            channel = trendChannelAnalyzer.AnalyzeTrendChannel(partialList);
            stockContext.Channels.Add(channel);
            stockContext.SaveChanges();
            return channel;
        }

        private Channel GetPreviousChannel(int stockKey, int length, DateTime endTime)
        {
            var channels = stockContext.Channels.Where(c => c.StockKey == stockKey && c.Length == length && c.EndDate < endTime).ToList();
            if (channels.Count > 0)
            {
                var maxDate = channels.Max(c => c.EndDate);
                return channels.First(c => c.EndDate == maxDate);
            }
            return null;
        }
    }
}