using System;
using System.Collections.Generic;

namespace Market.Analyzer.CandleStickPatterns
{
    public class DojiGraveStone : ReversePattern
    {
        public DojiGraveStone()
        {
            Name = "DojiGraveStone";
        }

        public override bool Qualified(IList<TransactionData> orderedList, Trend currentTrend)
        {
            var lastTransaction = orderedList[orderedList.Count - 1];
            var change = lastTransaction.High - lastTransaction.Low;
            //It has middle range
            if (change / lastTransaction.Close < CandleStickConstant.MiddleChange)
                return false;
            //Open and close are close to low
            if (Math.Abs(lastTransaction.Open - lastTransaction.Low)/change <= CandleStickConstant.Doji &&
                Math.Abs(lastTransaction.Close - lastTransaction.Low)/change <= CandleStickConstant.Doji)
            {
                ReverseCurrentTrend(currentTrend);
                return true;
            }
            return false;
        }
    }
}