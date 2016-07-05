using System;
using System.Collections.Generic;

namespace Market.Analyzer.CandleStickPatterns
{
    public class BullishEngulfing : Pattern
    {
        public BullishEngulfing()
        {
            Name = "BullishEngulfing";
        }

        public override bool Qualified(IList<TransactionData> orderedList, Trend currentTrend)
        {
            if (orderedList.Count < 2)
                return false;
            var lastTransaction = orderedList[orderedList.Count - 1];
            var firstTransaction = orderedList[orderedList.Count - 2];
            //Last transaction is bullish and first transaction is bearish
            if (lastTransaction.Close < lastTransaction.Open || firstTransaction.Open < firstTransaction.Close)
                return false;
            var lastChange = Math.Abs(lastTransaction.Close - lastTransaction.Open);
            var firstChange = Math.Abs(firstTransaction.Open - firstTransaction.Close);
            //Last transaction has middle change
            if (lastChange/lastTransaction.Close < CandleStickConstant.MiddleChange)
                return false;
            //Last transaction change is more than twice of first transaction change
            if (lastChange < 2*firstChange)
                return false;
            //Last transaction change is over first transaction change
            if (lastTransaction.Open < firstTransaction.Close && lastTransaction.Close > firstTransaction.Open)
            {
                CurrentTrend = currentTrend;
                UpcomingTrend = Trend.Up;
                return true;
            }
            return false;
        }
    }
}