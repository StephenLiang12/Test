using System;
using System.Collections.Generic;
using Market.Analyzer.Channels;

namespace Market.Suggestions
{
    public class DownUpDownTrendChannelSuggestionAnalyzer : TrendChannelSuggestionAnalyzer
    {
        public override string Name { get { return "Down-Up-Down Trend Channel"; } }

        public override double CalculateForecaseCertainty(IList<TransactionData> orderedTransactions, Channel shortChannel, Tuple<double, double> shortPrice, Channel interChannel, Tuple<double, double> interPrice, Channel longChannel, Tuple<double, double> longPrice)
        {
            int count = orderedTransactions.Count;
            //Up to 20 resistance line - short sell
            if (orderedTransactions[count - 1].High >= shortPrice.Item2)
            {
                Term = Term.Short;
                Action = Action.Sell;
                Price = shortPrice.Item2 + shortChannel.ResistanceChannelRatio;
                return 1;
            }
            return 0;
        }
    }
}