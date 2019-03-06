using System;
using System.Collections.Generic;
using System.Linq;
using Market.Analyzer.Channels;
using Market.Analyzer.MACD;
using Market.Tasks;

namespace Market.Suggestions.TrendChannels
{
    public class TrendChannelTriangleBreakSuggestionAnalyzer : ISuggestionAnalyzer
    {
        public string Name { get { return "TrendChannelTriangleBreakSuggestionAnalyzer"; } }
        public Term Term { get; private set; }
        public Action Action { get; private set; }
        public string Pattern { get; private set; }
        public double Price { get; private set; }

        private readonly int shortTerm;
        private readonly int interTerm;
        private readonly int longTerm;
        private readonly IStockTask stockTask;

        public TrendChannelTriangleBreakSuggestionAnalyzer() : this(50, 100, 200)
        { }

        public TrendChannelTriangleBreakSuggestionAnalyzer(int shortTerm, int interTerm, int longTerm)
        {
            this.shortTerm = shortTerm;
            this.interTerm = interTerm;
            this.longTerm = longTerm;
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
            var longTrendChannel = stockTask.GetChannel(stockKey, longTerm, endTime);
            var interTrendChannel = stockTask.GetChannel(stockKey, interTerm, endTime);
            var shortTrendChannel = stockTask.GetChannel(stockKey, shortTerm, endTime);
            if (longTrendChannel == null || interTrendChannel == null || shortTrendChannel == null)
                return 0;
            StockContext context = new StockContext();
            if (context.MovingAverageConvergenceDivergences.Any(m =>
                    m.StockKey == stockKey && m.TimeStamp == endTime) == false)
                return 0;
            var macd = context.MovingAverageConvergenceDivergences.First(m =>
                m.StockKey == stockKey && m.TimeStamp == endTime);
            var longTrendChannels = stockTask.GetChannels(stockKey, longTerm, interTrendChannel.StartDate, interTrendChannel.EndDate).ToList();
            var interTrendChannels = stockTask.GetChannels(stockKey, interTerm, shortTrendChannel.StartDate, shortTrendChannel.EndDate).ToList();
            int breakIndex;
            if (interTrendChannel.GetResistanceSign() <= 0)
            {
                foreach (var channel in longTrendChannels)
                {
                    if (channel.BreakSupportLine(orderedTransactions, out breakIndex))
                    {
                        Action = Action.Sell;
                        Term = Term.Long;
                        Pattern = "Break Long Support and Intermediate Resistance Triangle";
                        return 1;
                    }
                }
            }

            if (shortTrendChannel.GetResistanceSign() <= 0)
            {
                foreach (var channel in interTrendChannels)
                {
                    if (channel.BreakSupportLine(orderedTransactions, out breakIndex))
                    {
                        Action = Action.Sell;
                        Term = Term.Intermediate;
                        Pattern = "Break Intermediate Support and Short Resistance Triangle";
                        return 1;
                    }
                }
            }

            if (interTrendChannel.GetSupportSign() >= 0)
            {
                foreach (var channel in longTrendChannels)
                {
                    if (channel.BreakResistanceLine(orderedTransactions, out breakIndex) && channel.GetResistanceSign() <= 0 && MovingAverageConvergenceDivergenceGreaterThanZero(macd))
                    {
                        Action = Action.Buy;
                        Term = Term.Long;
                        Pattern = "Break Long Resistance and Intermediate Support Triangle";
                        return 1;
                    }
                }
            }

            if (shortTrendChannel.GetSupportSign() >= 0)
            {
                foreach (var channel in interTrendChannels)
                {
                    if (channel.BreakResistanceLine(orderedTransactions, out breakIndex) && channel.GetResistanceSign() <= 0 && MovingAverageConvergenceDivergenceGreaterThanZero(macd))
                    {
                        Action = Action.Buy;
                        Term = Term.Intermediate;
                        Pattern = "Break Intermediate Resistance and Short Support Triangle";
                        return 1;
                    }
                }
            }

            return 0;
        }

        private bool MovingAverageConvergenceDivergenceLessThanZero(MovingAverageConvergenceDivergence macd)
        {
            return macd.Histogram < 0 || (macd.Signal < 0 && macd.MACD < 0);
        }

        private bool MovingAverageConvergenceDivergenceGreaterThanZero(MovingAverageConvergenceDivergence macd)
        {
            return macd.Histogram > 0 || (macd.Signal > 0 && macd.MACD > 0);
        }
    }
}