using System;
using System.Collections.Generic;
using System.Linq;
using Market.Analyzer;
using Market.Analyzer.Channels;
using Market.Tasks;

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
        private readonly IStockTask stockTask;

        public MACDSuggestionAnalyzer() : this(50, 100, 200)
        {}

        public MACDSuggestionAnalyzer(int shortTerm, int interTerm, int longTerm)
        {
            this.shortTerm = shortTerm;
            this.interTerm = interTerm;
            this.longTerm = longTerm;
            stockContext = new StockContext();
            trendChannelAnalyzer = new TrendChannelAnalyzer();
            stockTask = new StockTask();
        }

        public double CalculateForecaseCertainty(IList<TransactionData> orderedTransactions)
        {
            var count = orderedTransactions.Count;
            if (count < 200)
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
            var longTrendChannel = stockTask.GetChannel(stockKey, longTerm, startTime, endTime);
            var interTrendChannel = stockTask.GetChannel(stockKey, interTerm, startTime, endTime);
            var shortTrendChannel = stockTask.GetChannel(stockKey, shortTerm, startTime, endTime);
            //Long resistance line is positive
            if (longTrendChannel.GetResistanceSign() > 0)
            {
                //Price is spiking, sell it if it shows exhausted
                if (longTrendChannel.IsSpiking())
                {
                    Term = Term.Long;
                    return 1;
                }
                //Price is close to long resistance line
                if (longTrendChannel.DoesCloseNearResistanceLine(orderedTransactions))
                {
                    Term = Term.Short;
                    return 1;
                }
                //inter resisance line is positive
                if (interTrendChannel.GetResistanceSign() > 0)
                {
                    //Price is spiking, sell it if it shows exhausted
                    if (interTrendChannel.IsSpiking())
                    {
                        Term = Term.Intermediate;
                        return 1;
                    }
                    //inter resistance line is strong and price is close to inter resistance line
                    if (interTrendChannel.IsResistanceStrong(orderedTransactions) &&
                        interTrendChannel.DoesCloseNearResistanceLine(orderedTransactions))
                    {
                        Term = Term.Intermediate;
                        return 1;
                    }
                    //short resistance line is positive
                    if (shortTrendChannel.GetResistanceSign() > 0)
                    {
                        //Price is spiking, sell it if it shows exhausted
                        if (shortTrendChannel.IsSpiking())
                        {
                            Term = Term.Short;
                            return 1;
                        }
                        //short resistance line is strong and price is close to short resistance line
                        if (shortTrendChannel.IsResistanceStrong(orderedTransactions) &&
                            shortTrendChannel.DoesCloseNearResistanceLine(orderedTransactions))
                        {
                            Term = Term.Short;
                            return 1;
                        }
                    }
                    return 0;
                }
                Term = Term.Short;
                return 1;
            }
            //Long resistance line is not positive, but inter resistance is positive
            if (interTrendChannel.GetResistanceSign() > 0)
            {
                //inter resistance line is strong and price is close to inter resistance line
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
            var longTrendChannel = stockTask.GetChannel(stockKey, longTerm, startTime, endTime);
            var interTrendChannel = stockTask.GetChannel(stockKey, interTerm, startTime, endTime);
            var shortTrendChannel = stockTask.GetChannel(stockKey, shortTerm, startTime, endTime);
            Action = Action.Buy;
            //Long support line is positive
            //if (longTrendChannel.GetSupportSign() > 0)
            {
                //Has a new low support line in long term, no buy signal
                var previousLongTrendChannel = stockTask.GetPreviousChannel(stockKey, longTerm, endTime);
                if (previousLongTrendChannel == null)
                    return 0;
                if (longTrendChannel.SupportChannelRatio < previousLongTrendChannel.SupportChannelRatio)
                    return 0;
                //Long support line is strong and price is rising from long support line
                if (longTrendChannel.IsSupportStrong(orderedTransactions) &&
                    longTrendChannel.DoesCloseNearSupportLine(orderedTransactions))
                {
                    Term = Term.Long;
                    return 1;
                }
                //Price is rising, but is not from long support line
                //Inter support line is positive
                if (interTrendChannel.GetSupportSign() > 0)
                {
                    //Has a new low support line in inter term, no buy signal
                    var previousInterTrendChannel = stockTask.GetPreviousChannel(stockKey, interTerm, endTime);
                    if (previousInterTrendChannel != null &&
                        interTrendChannel.SupportChannelRatio < previousInterTrendChannel.SupportChannelRatio)
                        return 0;
                    //Inter support line is strong and price is rising from iner support line
                    if (interTrendChannel.IsSupportStrong(orderedTransactions) &&
                        interTrendChannel.DoesCloseNearSupportLine(orderedTransactions))
                    {
                        Term = Term.Intermediate;
                        return 1;
                    }
                }
                //Price is rising, but is not from inter support line
                //Short support line is positive
                if (shortTrendChannel.GetSupportSign() > 0)
                {
                    //Has a new low support line in short term, no but signal
                    var previousShortTrendChannel = stockTask.GetPreviousChannel(stockKey, shortTerm, endTime);
                    if (previousShortTrendChannel != null &&
                        shortTrendChannel.SupportChannelRatio < previousShortTrendChannel.SupportChannelRatio)
                        return 0;
                    //Short support line is strong and price is rising from short support line
                    if (shortTrendChannel.IsSupportStrong(orderedTransactions) &&
                        shortTrendChannel.DoesCloseNearSupportLine(orderedTransactions))
                    {
                        Term = Term.Short;
                        return 1;
                    }
                }
                //Price is close to long resistance line, no buy signal
                if (longTrendChannel.DoesCloseNearResistanceLine(orderedTransactions))
                    return 0;
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