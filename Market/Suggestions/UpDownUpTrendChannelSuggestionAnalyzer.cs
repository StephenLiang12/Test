using System;
using System.Collections.Generic;
using Market.Analyzer.Channels;

namespace Market.Suggestions
{
    public class UpDownUpTrendChannelSuggestionAnalyzer : TrendChannelSuggestionAnalyzer
    {
        public override string Name { get { return "Up-Down-Up Trend Channel"; } }

        public override double CalculateForecaseCertainty(IList<TransactionData> orderedTransactions, Channel shortChannel, Tuple<double, double> shortPrice, Channel interChannel, Tuple<double, double> interPrice, Channel longChannel, Tuple<double, double> longPrice)
        {
            int count = orderedTransactions.Count;
            //Up to 50 Resistance line - short sell
            if (orderedTransactions[count - 1].High >= interPrice.Item2)
            {
                Term = Term.Short;
                Action = Action.Sell;
                Price = interPrice.Item2 + interChannel.ResistanceChannelRatio;
                return 1;
            }
            //down to 50 suport line - short buy
            if (orderedTransactions[count - 1].Low <= interPrice.Item1 && orderedTransactions[count - 1].Low > longPrice.Item1)
            {
                Term = Term.Short;
                Action = Action.Buy;
                Price = interPrice.Item1 - interChannel.SupportChannelRatio;
                return 1;
            }
            //down to 100 support - intermediate buy
            if (orderedTransactions[count - 1].Low <= longPrice.Item1)
            {
                Term = Term.Intermediate;
                Action = Action.Buy;
                Price = longPrice.Item1 - longChannel.SupportChannelRatio;
                return 1;
            }
            return 0;
        }
    }
}