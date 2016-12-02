using System;
using System.Collections.Generic;
using Market.Analyzer.Channels;
using Market.Suggestions.TrendChannels;

namespace Market.Suggestions
{
    public class UpUpUpTrendChannelSuggestionAnalyzer : TrendChannelSuggestionAnalyzer
    {
        public override string Name { get { return "Up-Up-Up Trend Channel"; } }

        protected override bool ApplyBreakLogic
        {
            get { return true; }
        }

        public override double CalculateForecaseCertainty(IList<TransactionData> orderedTransactions, Channel shortChannel, Tuple<double, double> shortPrice, Channel interChannel, Tuple<double, double> interPrice, Channel longChannel, Tuple<double, double> longPrice)
        {
            int count = orderedTransactions.Count;
            //Down to 20 support line if it is not a new support line - short buy, otherwise - short sell
            if (orderedTransactions[count - 1].Low <= shortPrice.Item1 && orderedTransactions[count - 1].Low > interPrice.Item1 && orderedTransactions[count - 1].Low > longPrice.Item1)
            {
                if (IsItNewSupportLine(orderedTransactions, shortChannel) == false)
                {
                    Term = Term.Short;
                    Action = Action.Buy;
                    Price = shortPrice.Item1 - shortChannel.SupportChannelRatio;
                    return 1;
                }
                if (ApplyBreakLogic)
                {
                    Term = Term.Short;
                    Action = Action.Sell;
                    Price = shortPrice.Item1 - shortChannel.SupportChannelRatio;
                    return 1;
                }
            }
            //Down to 50 support line if it is not a new support line - intermediate buy, otherwise - intermediate sell
            if (orderedTransactions[count - 1].Low <= interPrice.Item1 && orderedTransactions[count - 1].Low > longPrice.Item1)
            {
                if (IsItNewSupportLine(orderedTransactions, interChannel) == false)
                {
                    Term = Term.Intermediate;
                    Action = Action.Buy;
                    Price = interPrice.Item1 - interChannel.SupportChannelRatio;
                    return 1;
                }
                if (ApplyBreakLogic)
                {
                    Term = Term.Intermediate;
                    Action = Action.Sell;
                    Price = interPrice.Item1 - interChannel.SupportChannelRatio;
                    return 1;
                }
            }
            //Down to 100 support line if it is not a new support line - long buy, otherwise - long sell
            if (orderedTransactions[count - 1].Low <= longPrice.Item1)
            {
                if (IsItNewSupportLine(orderedTransactions, longChannel) == false)
                {
                    Term = Term.Long;
                    Action = Action.Buy;
                    Price = longPrice.Item1 - longChannel.SupportChannelRatio;
                    return 1;
                }
                if (ApplyBreakLogic)
                {
                    Term = Term.Long;
                    Action = Action.Sell;
                    Price = longPrice.Item1 - longChannel.SupportChannelRatio;
                    return 1;
                }
            }
            ////Up to 20 resistance line, if it is a new resistance line - short buy
            //if (ApplyBreakLogic && orderedTransactions[count - 1].High >= shortPrice.Item2 && orderedTransactions[count - 1].High < interPrice.Item2 && IsItNewResistanceLine(orderedTransactions, shortChannel))
            //{
            //    Term = Term.Short;
            //    Action = Action.Buy;
            //    Price = shortPrice.Item2 + shortChannel.ResistanceChannelRatio;
            //    return 1;
            //}
            ////Up to 50 resistance line, if it is a new resistance line - short buy
            //if (ApplyBreakLogic && orderedTransactions[count - 1].High >= interPrice.Item2 && orderedTransactions[count - 1].High < longPrice.Item2 && IsItNewResistanceLine(orderedTransactions, interChannel))
            //{
            //    Term = Term.Short;
            //    Action = Action.Buy;
            //    Price = interPrice.Item2 + interChannel.ResistanceChannelRatio;
            //    return 1;
            //}
            //Up to 100 resistance line, if it is a new resistance line - short buy, otherwise - short sell
            if (orderedTransactions[count - 1].High >= longPrice.Item2)
            {
                if (IsItNewResistanceLine(orderedTransactions, longChannel) == false)
                {
                    Term = Term.Short;
                    Action = Action.Sell;
                    Price = longPrice.Item2 + longChannel.ResistanceChannelRatio;
                    return 1;
                }
                //if (ApplyBreakLogic)
                //{
                //    Term = Term.Short;
                //    Action = Action.Buy;
                //    Price = longPrice.Item2 + longChannel.ResistanceChannelRatio;
                //    return 1;
                //}
            }
            return 0;
        }
    }
}