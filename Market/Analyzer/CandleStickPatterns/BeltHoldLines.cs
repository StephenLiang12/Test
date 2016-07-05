using System;
using System.Collections.Generic;

namespace Market.Analyzer.CandleStickPatterns
{
    public class BeltHoldLines : Pattern
    {
        public BeltHoldLines()
        {
            Name = "BeltHoldLines";
        }

        public override bool Qualified(IList<TransactionData> orderedList, Trend currentTrend)
        {
            var lastTransaction = orderedList[orderedList.Count - 1];
            var change = lastTransaction.High - lastTransaction.Low;
            //It is big change
            if (change/lastTransaction.Close < CandleStickConstant.BigChange)
                return false;
            //Open is equal to low and close is close to high
            if (Math.Abs(lastTransaction.Open - lastTransaction.Low) <= 0.01 &&
                Math.Abs(lastTransaction.Close - lastTransaction.High)/change <= CandleStickConstant.Doji)
            {
                CurrentTrend = currentTrend;
                UpcomingTrend = Trend.Up;
                return true;
            }
            //Open is equal to high and close is close to low
            if (Math.Abs(lastTransaction.Open - lastTransaction.High) <= 0.01 &&
                Math.Abs(lastTransaction.Close - lastTransaction.Low)/change <= CandleStickConstant.Doji)
            {
                CurrentTrend = currentTrend;
                UpcomingTrend = Trend.Down;
                return true;
            }
            return false;
        }
    }
}