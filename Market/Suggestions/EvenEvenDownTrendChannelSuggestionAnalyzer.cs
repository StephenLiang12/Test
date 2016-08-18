using System;
using System.Collections.Generic;
using Market.Analyzer.Channels;

namespace Market.Suggestions
{
    public class EvenEvenDownTrendChannelSuggestionAnalyzer : TrendChannelSuggestionAnalyzer
    {
        public override string Name { get { return "Even-Even-Down Trend Channel"; } }

        public override double CalculateForecaseCertainty(IList<TransactionData> orderedTransactions, Channel shortChannel, Tuple<double, double> shortPrice, Channel interChannel, Tuple<double, double> interPrice, Channel longChannel, Tuple<double, double> longPrice)
        {
            int count = orderedTransactions.Count;
            //Down to 100 support line - short buy
            if (orderedTransactions[count - 1].Low <= longPrice.Item1)
            {
                Term = Term.Short;
                Action = Action.Buy;
                Price = longPrice.Item1 - longChannel.SupportChannelRatio;
                return 1;
            }
            //Up to 20 resistance line - short sell
            if (orderedTransactions[count - 1].High >= shortPrice.Item2 && orderedTransactions[count - 1].High < interPrice.Item2)
            {
                Term = Term.Short;
                Action = Action.Sell;
                Price = shortPrice.Item2 + shortChannel.ResistanceChannelRatio;
                return 1;
            }
            //Up to 50 resistance line - intermediate sell
            if (orderedTransactions[count - 1].High >= interPrice.Item2)
            {
                Term = Term.Intermediate;
                Action = Action.Sell;
                Price = interPrice.Item2 + interChannel.ResistanceChannelRatio;
                return 1;
            }
            return 0;
        }
    }
}