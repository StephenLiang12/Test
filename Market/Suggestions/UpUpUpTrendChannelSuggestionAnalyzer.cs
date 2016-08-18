using System;
using System.Collections.Generic;
using Market.Analyzer.Channels;

namespace Market.Suggestions
{
    public class UpUpUpTrendChannelSuggestionAnalyzer : TrendChannelSuggestionAnalyzer
    {
        public override string Name { get { return "Up-Up-Up Trend Channel"; } }

        public override double CalculateForecaseCertainty(IList<TransactionData> orderedTransactions, Channel shortChannel, Tuple<double, double> shortPrice, Channel interChannel, Tuple<double, double> interPrice, Channel longChannel, Tuple<double, double> longPrice)
        {
            int count = orderedTransactions.Count;
            //Down to 20 support line - short buy
            if (orderedTransactions[count - 1].Low <= shortPrice.Item1 && orderedTransactions[count - 1].Low > interPrice.Item1 && orderedTransactions[count - 1].Low > longPrice.Item1)
            {
                Term = Term.Short;
                Action = Action.Buy;
                Price = shortPrice.Item1 - shortChannel.SupportChannelRatio;
                return 1;
            }
            //Down to 50 support line - intermediate buy
            if (orderedTransactions[count - 1].Low <= interPrice.Item1 && orderedTransactions[count - 1].Low > longPrice.Item1)
            {
                Term = Term.Intermediate;
                Action = Action.Buy;
                Price = interPrice.Item1 - interChannel.SupportChannelRatio;
                return 1;
            }
            //Down to 100 support line - long buy
            if (orderedTransactions[count - 1].Low <= longPrice.Item1)
            {
                Term = Term.Long;
                Action = Action.Buy;
                Price = longPrice.Item1 - longChannel.SupportChannelRatio;
                return 1;
            }
            //Up to 100 resistance line - short sell
            if (orderedTransactions[count - 1].High >= longPrice.Item2)
            {
                Term = Term.Short;
                Action = Action.Sell;
                Price = longPrice.Item2 + longChannel.ResistanceChannelRatio;
                return 1;
            }
            return 0;
        }
    }
}