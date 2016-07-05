using System;
using System.Collections.Generic;

namespace Market.Analyzer.CandleStickPatterns
{
    public class DarkCloudCover : Pattern
    {
        public DarkCloudCover()
        {
            Name = "DarkCloudCover";
        }

        public override bool Qualified(IList<TransactionData> orderedList, Trend currentTrend)
        {
            if (orderedList.Count < 2)
                return false;
            var lastTransaction = orderedList[orderedList.Count - 1];
            var firstTransaction = orderedList[orderedList.Count - 2];
            //Last transaction is bearish and first transaction is bullish
            if (lastTransaction.Open < lastTransaction.Close || firstTransaction.Close < firstTransaction.Open)
                return false;
            var lastChange = Math.Abs(lastTransaction.Close - lastTransaction.Open);
            var firstChange = Math.Abs(firstTransaction.Close - firstTransaction.Open);
            //Both transactions have middle change
            if (lastChange / lastTransaction.Close < CandleStickConstant.MiddleChange || firstChange/firstTransaction.Close < CandleStickConstant.MiddleChange)
                return false;
            //Last transaction is high open, but close is less than the middle of first transaction
            if (lastTransaction.Open > firstTransaction.Close && lastTransaction.Close < firstTransaction.Open + firstChange/2)
            {
                CurrentTrend = currentTrend;
                UpcomingTrend = Trend.Down;
                return true;
            }
            return false;
        }
    }
}