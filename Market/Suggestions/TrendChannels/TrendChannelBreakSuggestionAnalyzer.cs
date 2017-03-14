using System;
using System.Collections.Generic;
using Market.Analyzer.Channels;
using Market.Tasks;

namespace Market.Suggestions.TrendChannels
{
    public class TrendChannelBreakSuggestionAnalyzer : ISuggestionAnalyzer
    {
        public string Name { get { return "TrendChannelBreakSuggestionAnalyzer"; } }
        public Term Term { get; private set; }
        public Action Action { get; private set; }
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
            int breakIndex;
            if (longTrendChannel.SupportChannelRatio >= 0)
            {
                if (longTrendChannel.BreakSupportLine(orderedTransactions, out breakIndex) && breakIndex == orderedTransactions.Count - 1)
                {
                    Action = Action.Sell;
                    Term = Term.Long;
                    return 1;
                }
                if (interTrendChannel.BreakSupportLine(orderedTransactions, out breakIndex) && breakIndex == orderedTransactions.Count - 1)
                {
                    Action = Action.Sell;
                    Term = Term.Intermediate;
                    return 1;
                }
                if (shortTrendChannel.BreakSupportLine(orderedTransactions, out breakIndex) && breakIndex == orderedTransactions.Count - 1)
                {
                    Action = Action.Sell;
                    Term = Term.Short;
                    return 1;
                }
                //if (longTrendChannel.BreakResistanceLine(orderedTransactions, out breakIndex) && breakIndex == orderedTransactions.Count - 1)
                //{
                //    Action = Action.Buy;
                //    Term = Term.Long;
                //    return 1;
                //}
                //if (interTrendChannel.BreakResistanceLine(orderedTransactions, out breakIndex) && breakIndex == orderedTransactions.Count - 1)
                //{
                //    Action = Action.Buy;
                //    Term = Term.Intermediate;
                //    return 1;
                //}
                //if (shortTrendChannel.BreakResistanceLine(orderedTransactions, out breakIndex) && breakIndex == orderedTransactions.Count - 1)
                //{
                //    Action = Action.Buy;
                //    Term = Term.Short;
                //    return 1;
                //}
            }
            if (longTrendChannel.SupportChannelRatio < 0)
            {
                if (longTrendChannel.BreakSupportLine(orderedTransactions, out breakIndex) && breakIndex == orderedTransactions.Count - 1)
                {
                    Action = Action.Sell;
                    Term = Term.Unlimited;
                    return 1;
                }
                if (interTrendChannel.BreakSupportLine(orderedTransactions, out breakIndex) && breakIndex == orderedTransactions.Count - 1)
                {
                    Action = Action.Sell;
                    Term = Term.Long;
                    return 1;
                }
                if (shortTrendChannel.BreakSupportLine(orderedTransactions, out breakIndex) && breakIndex == orderedTransactions.Count - 1)
                {
                    Action = Action.Sell;
                    Term = Term.Intermediate;
                    return 1;
                }
                //if (longTrendChannel.BreakResistanceLine(orderedTransactions, out breakIndex) && breakIndex == orderedTransactions.Count - 1)
                //{
                //    Action = Action.Buy;
                //    Term = Term.Long;
                //    return 1;
                //}
                //if (interTrendChannel.BreakResistanceLine(orderedTransactions, out breakIndex) && breakIndex == orderedTransactions.Count - 1)
                //{
                //    Action = Action.Buy;
                //    Term = Term.Intermediate;
                //    return 1;
                //}
                //if (shortTrendChannel.BreakResistanceLine(orderedTransactions, out breakIndex) && breakIndex == orderedTransactions.Count - 1)
                //{
                //    Action = Action.Buy;
                //    Term = Term.Short;
                //    return 1;
                //}
            }
            return 0;
        }
    }
}