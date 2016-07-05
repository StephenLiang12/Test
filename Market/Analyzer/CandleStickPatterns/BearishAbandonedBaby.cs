using System;
using System.Collections.Generic;

namespace Market.Analyzer.CandleStickPatterns
{
    public class BearishAbandonedBaby : ReversePattern
    {
        public BearishAbandonedBaby()
        {
            Name = "AbandonedBaby";
        }

        public override bool Qualified(IList<TransactionData> orderedList, Trend currentTrend)
        {
            if (orderedList.Count < 3)
                return false;
            var lastTransaction = orderedList[orderedList.Count - 1];
            var secondTransaction = orderedList[orderedList.Count - 2];
            var firstTransaction = orderedList[orderedList.Count - 3];
            //Fist Transaction is Bullish
            if (firstTransaction.Close < firstTransaction.Open)
                return false;
            //Second Transaction is Doji
            var secondRange = secondTransaction.High - secondTransaction.Low;
            var secondChange = Math.Abs(secondTransaction.Close - secondTransaction.Open);
            if (secondChange/secondRange > CandleStickConstant.Doji)
                return false;
            //Last Transaction is Bearish
            if (lastTransaction.Open < lastTransaction.Close)
                return false;
            //There are gaps between fist close and second open and second close and last open
            var firstGap = secondTransaction.Open - firstTransaction.Close;
            if (firstGap/firstTransaction.Close < CandleStickConstant.Gap)
                return false;
            var secondGap = secondTransaction.Close - lastTransaction.Open;
            if (secondGap/secondTransaction.Close < CandleStickConstant.Gap)
                return false;
            CurrentTrend = currentTrend;
            UpcomingTrend = Trend.Down;
            return true;
        }
    }
}