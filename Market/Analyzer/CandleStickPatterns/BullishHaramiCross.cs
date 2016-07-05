using System;
using System.Collections.Generic;

namespace Market.Analyzer.CandleStickPatterns
{
    public class BullishHaramiCross : ReversePattern
    {
        public BullishHaramiCross()
        {
            Name = "HaramiCross";
        }

        public override bool Qualified(IList<TransactionData> orderedList, Trend currentTrend)
        {
            if (orderedList.Count < 2)
                return false;
            var lastTransaction = orderedList[orderedList.Count - 1];
            var firstTransaction = orderedList[orderedList.Count - 2];
            //First Transaction is bearish
            if (firstTransaction.Open < firstTransaction.Close)
                return false;
            //First Transaction has middle change
            var fistChange = firstTransaction.Open - firstTransaction.Close;
            if (fistChange/firstTransaction.Close < CandleStickConstant.MiddleChange)
                return false;
            //Last Transaction is Doji
            var lastRange = lastTransaction.High - lastTransaction.Low;
            var lastChange = Math.Abs(lastTransaction.Close - lastTransaction.Open);
            if (lastChange / lastRange > CandleStickConstant.Doji)
                return false;
            //Last Transaction is in the middle of first transaction
            if (lastTransaction.Close < firstTransaction.Open && lastTransaction.Close > firstTransaction.Close)
            {
                ReverseCurrentTrend(currentTrend);
                return true;
            }
            return false;
        }
    }
}