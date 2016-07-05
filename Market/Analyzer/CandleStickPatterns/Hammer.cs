using System;
using System.Collections.Generic;

namespace Market.Analyzer.CandleStickPatterns
{
    public class Hammer : Pattern
    {
        public Hammer()
        {
            Name = "Hammer";
            CurrentTrend = Trend.Down;
            UpcomingTrend = Trend.Up;
        }

        public override bool Qualified(IList<TransactionData> orderedList, Trend currentTrend)
        {
            if (currentTrend != Trend.Down)
                return false;
            var lastTransaction = orderedList[orderedList.Count - 1];
            var range = lastTransaction.High - lastTransaction.Low;
            var change = lastTransaction.Open - lastTransaction.Close;
            //It have middle range
            if (range/lastTransaction.Close < CandleStickConstant.MiddleChange)
                return false;
            //Open is close to high, it is bearish and change is 1/3 of range
            if (Math.Abs(lastTransaction.High - lastTransaction.Open)/range <= CandleStickConstant.Doji &&
                lastTransaction.Open > lastTransaction.Close &&
                change/range < 1d/3)
                return true;
            return false;
        }
    }
}