using System;
using System.Collections.Generic;

namespace Market.Analyzer.CandleStickPatterns
{
    public class BullishCounterAttack : ReversePattern
    {
        public BullishCounterAttack()
        {
            Name = "BullishCounterAttack";
        }

        public override bool Qualified(IList<TransactionData> orderedList, Trend currentTrend)
        {
            if (orderedList.Count < 2)
                return false;
            var lastTransaction = orderedList[orderedList.Count - 1];
            var firstTransaction = orderedList[orderedList.Count - 2];
            //First transaction is bearish and last transaction is bullish
            if (lastTransaction.Open > lastTransaction.Close || firstTransaction.Close > firstTransaction.Open)
                return false;
            var lastChange = Math.Abs(lastTransaction.Close - lastTransaction.Open);
            var firstChange = Math.Abs(firstTransaction.Close - firstTransaction.Open);
            //Both transaction have middle change
            if (lastChange / lastTransaction.Close < CandleStickConstant.MiddleChange || firstChange / firstTransaction.Close < CandleStickConstant.MiddleChange)
                return false;
            //Both transaction have similiar close
            if (lastTransaction.Open < firstTransaction.Close && Math.Abs(lastTransaction.Close - firstTransaction.Close) / lastChange < CandleStickConstant.Doji)
            {
                CurrentTrend = currentTrend;
                UpcomingTrend = Trend.Up;
                return true;
            }
            return false;
        }
    }
}