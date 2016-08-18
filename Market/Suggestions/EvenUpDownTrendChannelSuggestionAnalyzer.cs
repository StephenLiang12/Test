using System;
using System.Collections.Generic;
using Market.Analyzer.Channels;

namespace Market.Suggestions
{
    public class EvenUpDownTrendChannelSuggestionAnalyzer : TrendChannelSuggestionAnalyzer
    {
        public override string Name { get { return "Even-Up-Down Trend Channel"; } }

        public override double CalculateForecaseCertainty(IList<TransactionData> orderedTransactions, Channel shortChannel, Tuple<double, double> shortPrice, Channel interChannel, Tuple<double, double> interPrice, Channel longChannel, Tuple<double, double> longPrice)
        {
            int count = orderedTransactions.Count;
            //Down to 50 support line - short buy
            if (orderedTransactions[count - 1].Low <= interPrice.Item1 && orderedTransactions[count - 1].Low > longPrice.Item1)
            {
                Term = Term.Short;
                Action = Action.Buy;
                Price = interPrice.Item1 - interChannel.SupportChannelRatio;
                return 1;
            }
            //Down to 100 support line - intermediate buy
            if (orderedTransactions[count - 1].Low <= longPrice.Item1)
            {
                Term = Term.Intermediate;
                Action = Action.Buy;
                Price = longPrice.Item1 - longChannel.SupportChannelRatio;
                return 1;
            }
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