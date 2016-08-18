using System;
using System.Collections.Generic;
using Market.Analyzer.Channels;

namespace Market.Suggestions
{
    public class DownEvenEvenTrendChannelSuggestionAnalyzer : TrendChannelSuggestionAnalyzer
    {
        public override string Name { get { return "Down-Even-Even Trend Channel"; } }

        public override double CalculateForecaseCertainty(IList<TransactionData> orderedTransactions, Channel shortChannel, Tuple<double, double> shortPrice, Channel interChannel, Tuple<double, double> interPrice, Channel longChannel, Tuple<double, double> longPrice)
        {
            int count = orderedTransactions.Count;
            //Up to 50 resistance line - short sell
            if (orderedTransactions[count - 1].High >= interPrice.Item2 && orderedTransactions[count - 1].High < longPrice.Item2)
            {
                Term = Term.Short;
                Action = Action.Sell;
                Price = interPrice.Item2 + interChannel.ResistanceChannelRatio;
                return 1;
            }
            //Up to 100 resistance line - intermediate sell
            if (orderedTransactions[count - 1].High >= longPrice.Item2)
            {
                Term = Term.Intermediate;
                Action = Action.Sell;
                Price = longPrice.Item2 + longChannel.ResistanceChannelRatio;
                return 1;
            }
            return 0;
        }
    }
}