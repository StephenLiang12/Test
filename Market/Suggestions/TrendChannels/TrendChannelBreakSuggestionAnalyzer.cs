using System;
using System.Collections.Generic;
using System.Linq;
using Market.Analyzer.Channels;
using Market.Analyzer.MACD;
using Market.Tasks;

namespace Market.Suggestions.TrendChannels
{
    public class TrendChannelBreakSuggestionAnalyzer : ISuggestionAnalyzer
    {
        public string Name { get { return "TrendChannelBreakSuggestionAnalyzer"; } }
        public Term Term { get; private set; }
        public Action Action { get; private set; }
        public string Pattern { get; private set; }
        public double Price { get; private set; }

        private readonly int shortTerm;
        private readonly int interTerm;
        private readonly int longTerm;
        private readonly IStockTask stockTask;

        public TrendChannelBreakSuggestionAnalyzer(): this(50, 100, 200)
        {}

        public TrendChannelBreakSuggestionAnalyzer(int shortTerm, int interTerm, int longTerm)
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
            var longTrendChannel = stockTask.GetPreviousChannel(stockKey, longTerm, endTime);
            var interTrendChannel = stockTask.GetPreviousChannel(stockKey, interTerm, endTime);
            var shortTrendChannel = stockTask.GetPreviousChannel(stockKey, shortTerm, endTime);
            if (longTrendChannel == null || interTrendChannel == null || shortTrendChannel == null)
                return 0;
            StockContext context = new StockContext();
            if (context.MovingAverageConvergenceDivergences.Any(m =>
                    m.StockKey == stockKey && m.TimeStamp == endTime) == false)
                return 0;
            var macd = context.MovingAverageConvergenceDivergences.First(m =>
                m.StockKey == stockKey && m.TimeStamp == endTime);
            int breakIndex;
            if (longTrendChannel.BreakSupportLine(orderedTransactions, out breakIndex) &&
                breakIndex == orderedTransactions.Count - 1 && MovingAverageConvergenceDivergenceLessThanZero(macd))
            {
                Action = Action.Sell;
                Term = Term.Long;
                Pattern = "Break Long Support Line";
                return 1;
            }

            if (interTrendChannel.BreakSupportLine(orderedTransactions, out breakIndex) &&
                breakIndex == orderedTransactions.Count - 1 && MovingAverageConvergenceDivergenceLessThanZero(macd))
            {
                Action = Action.Sell;
                Term = Term.Intermediate;
                Pattern = "Break Intermediate Support Line";
                return 1;
            }

            if (shortTrendChannel.BreakSupportLine(orderedTransactions, out breakIndex) &&
                breakIndex == orderedTransactions.Count - 1 && MovingAverageConvergenceDivergenceLessThanZero(macd))
            {
                Action = Action.Sell;
                Term = Term.Short;
                Pattern = "Break Short Support Line";
                return 1;
            }

            if (longTrendChannel.ResistanceChannelRatio <= 0 && longTrendChannel.BreakResistanceLine(orderedTransactions, out breakIndex) &&
                breakIndex == orderedTransactions.Count - 1 && MovingAverageConvergenceDivergenceGreaterThanZero(macd))
            {
                Action = Action.Buy;
                Term = Term.Long;
                Pattern = "Break Long Resistance Line";
                return 1;
            }

            if (longTrendChannel.ResistanceChannelRatio >= 0 && interTrendChannel.ResistanceChannelRatio <= 0 && interTrendChannel.BreakResistanceLine(orderedTransactions, out breakIndex) &&
                breakIndex == orderedTransactions.Count - 1 && MovingAverageConvergenceDivergenceGreaterThanZero(macd))
            {
                Action = Action.Buy;
                Term = Term.Intermediate;
                Pattern = "Break Intermediate Resistance Line";
                return 1;
            }

            if (longTrendChannel.ResistanceChannelRatio >= 0 && interTrendChannel.ResistanceChannelRatio >= 0 && shortTrendChannel.ResistanceChannelRatio <= 0 && 
                 shortTrendChannel.BreakResistanceLine(orderedTransactions, out breakIndex) &&breakIndex == orderedTransactions.Count - 1 && MovingAverageConvergenceDivergenceGreaterThanZero(macd))
            {
                Action = Action.Buy;
                Term = Term.Short;
                Pattern = "Break Short Resistance Line";
                return 1;
            }

            return 0;
        }

        private bool MovingAverageConvergenceDivergenceLessThanZero(MovingAverageConvergenceDivergence macd)
        {
            return macd.MACD < 0 && macd.Histogram < 0 && macd.Signal < 0;
        }

        private bool MovingAverageConvergenceDivergenceGreaterThanZero(MovingAverageConvergenceDivergence macd)
        {
            return macd.MACD > 0 && macd.Histogram > 0 && macd.Signal > 0;
        }
    }
}