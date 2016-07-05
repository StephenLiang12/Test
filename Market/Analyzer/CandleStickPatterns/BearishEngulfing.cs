using System;
using System.Collections.Generic;

namespace Market.Analyzer.CandleStickPatterns
{
    public class BearishEngulfing : Pattern
    {
        public BearishEngulfing()
        {
            Name = "BearishEngulfing";
        }

        public override bool Qualified(IList<TransactionData> orderedList, Trend currentTrend)
        {
            if (orderedList.Count < 2)
                return false;
            var lastTransaction = orderedList[orderedList.Count - 1];
            var firstTransaction = orderedList[orderedList.Count - 2];
            if (lastTransaction.Open < lastTransaction.Close || firstTransaction.Close < firstTransaction.Open)
                return false;
            var lastChange = Math.Abs(lastTransaction.Close - lastTransaction.Open);
            var firstChange = Math.Abs(firstTransaction.Open - firstTransaction.Close);
            //Last transaction has middile change
            if (lastChange/lastTransaction.Close < CandleStickConstant.MiddleChange)
                return false;
            //Last transaction change is more than twice of first transaction change
            if (lastChange < 2*firstChange)
                return false;
            //Last transaction change is over first transction change
            if (lastTransaction.Close < firstTransaction.Open && lastTransaction.Open > firstTransaction.Close)
            {
                CurrentTrend = currentTrend;
                UpcomingTrend = Trend.Down;
                return true;
            }
            return false;
        }
    }
}