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
            var shortTransactions = orderedTransactions.GetRearPartial(shortTerm);
            var interTransactions = orderedTransactions.GetRearPartial(interTerm);
            var longTransactions = orderedTransactions.GetRearPartial(longTerm);
            var shortTrendChannel = trendChannelAnalyzer.AnalyzeTrendChannel(shortTransactions);
            var interTrendChannel = trendChannelAnalyzer.AnalyzeTrendChannel(interTransactions);
            var longTrendChannel = trendChannelAnalyzer.AnalyzeTrendChannel(longTransactions);

            int stockKey = orderedTransactions[0].StockKey;
            DateTime startTime = orderedTransactions[0].TimeStamp;
            DateTime endTime = orderedTransactions[count - 1].TimeStamp;
            var result = stockContext.MovingAverageConvergenceDivergences.Where(m => m.StockKey == stockKey && m.TimeStamp >= startTime && m.TimeStamp <= endTime).OrderBy(m => m.TimeStamp);
            var array = result.ToArray();
            if (Math.Sign(array[count - 1].Histogram) != Math.Sign(array[count - 2].Histogram))
            {
                if (Math.Sign(array[count - 1].Histogram) ==1)
                {
                    Action = Action.Buy;
                    if (longTrendChannel.ChannelTrend > 0)
                        Term = Term.Long;
                    else if (longTrendChannel.ChannelTrend == 0 && interTrendChannel.ChannelTrend > 0)
                        Term = Term.Intermediate;
                    else if (longTrendChannel.ChannelTrend == 0 && interTrendChannel.ChannelTrend == 0)
                        Term = Term.Short;
                    else
                        return 0;
                    return 1;
                }
                if (Math.Sign(array[count - 1].Histogram) == -1)
                {
                    Action = Action.Sell;
                    if (longTrendChannel.ChannelTrend > 0)
                        Term = Term.Short;
                    else if (longTrendChannel.ChannelTrend == 0 && interTrendChannel.ChannelTrend > 0)
                        Term = Term.Intermediate;
                    else if (longTrendChannel.ChannelTrend == 0 && interTrendChannel.ChannelTrend == 0)
                        Term = Term.Long;
                    return 1;
                }
            }
            return 0;
        }
    }
}