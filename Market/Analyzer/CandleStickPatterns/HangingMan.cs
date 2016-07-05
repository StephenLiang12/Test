using System;
using System.Collections.Generic;

namespace Market.Analyzer.CandleStickPatterns
{
    public class HangingMan : Pattern
    {
        public HangingMan()
        {
            Name = "HangingMan";
            CurrentTrend = Trend.Up;
            UpcomingTrend = Trend.Down;
        }

        public override bool Qualified(IList<TransactionData> orderedList, Trend currentTrend)
        {
            if (currentTrend != Trend.Up)
                return false;
            var lastTransaction = orderedList[orderedList.Count - 1];
            var range = lastTransaction.High - lastTransaction.Low;
            var change = lastTransaction.Close - lastTransaction.Open;
            //It has middle range
            if (range / lastTransaction.Close < CandleStickConstant.MiddleChange)
                return false;
            //Close is close to high, it is bullish and change is 1/3 of range
            if (Math.Abs(lastTransaction.High - lastTransaction.Close)/range <= CandleStickConstant.Doji &&
                lastTransaction.Close > lastTransaction.Open &&
                change / range < 1d / 3)
                return true;
            return false;
        }
    }
}