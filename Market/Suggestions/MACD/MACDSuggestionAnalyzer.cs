using System;
using System.Collections.Generic;
using System.Linq;
using Market.Analyzer;

namespace Market.Suggestions.MACD
{
    public class MACDSuggestionAnalyzer : ISuggestionAnalyzer
    {
        public string Name { get { return "MovingAverageConvergenceDivergence"; } }
        public Term Term { get; private set; }
        public Action Action { get; private set; }
        public double Price { get; private set; }

        //private MovingAverageConvergenceDivergenceCalculator calculator = new MovingAverageConvergenceDivergenceCalculator();
        private MovingAverageAnalyzer analyzer = new MovingAverageAnalyzer();
        private StockContext stockContext = new StockContext();

        public double CalculateForecaseCertainty(IList<TransactionData> orderedTransactions)
        {
            var count = orderedTransactions.Count;
            if (count < 60)
                return 0;
            int stockKey = orderedTransactions[0].StockKey;
            DateTime startTime = orderedTransactions[0].TimeStamp;
            DateTime endTime = orderedTransactions[count - 1].TimeStamp;
            var result = stockContext.MovingAverageConvergenceDivergences.Where(m => m.StockKey == stockKey && m.TimeStamp >= startTime && m.TimeStamp <= endTime).OrderBy(m => m.TimeStamp);
            var array = result.ToArray();
            if (Math.Sign(array[count - 1].Histogram) != Math.Sign(array[count - 2].Histogram))
            {
                if (array[count-1].Histogram >= 0)
                {
                    Term = Term.Short;
                    Action = Action.Buy;
                    return 1;
                }
                else
                {
                    Term = Term.Short;
                    Action = Action.Sell;
                    return 1;
                }
            }
            return 0;
        }
    }
}