using System;
using System.Collections.Generic;

namespace Market.Analyzer.CandleStickPatterns
{
    public class Marubozu : Pattern
    {
        public Marubozu()
        {
            Name = "Marubozu";
        }

        public override bool Qualified(IList<TransactionData> orderedList, Trend currentTrend)
        {
            var lastTransaction = orderedList[orderedList.Count - 1];
            var change = lastTransaction.High - lastTransaction.Low;
            //It has big change
            if (change/lastTransaction.Close < CandleStickConstant.BigChange)
                return false;
            //Open is low and close is high
            if (Math.Abs(lastTransaction.Open - lastTransaction.Low) <= 0.01 &&
                Math.Abs(lastTransaction.Close - lastTransaction.High) <= 0.01)
            {
                CurrentTrend = currentTrend;
                UpcomingTrend = Trend.Up;
                return true;
            }
            //Open is high and close is low
            if (Math.Abs(lastTransaction.Open - lastTransaction.High) <= 0.01 &&
                Math.Abs(lastTransaction.Close - lastTransaction.Low) <= 0.01)
            {
                CurrentTrend = currentTrend;
                UpcomingTrend = Trend.Down;
                return true;
            }
            return false;
        }
    }
}