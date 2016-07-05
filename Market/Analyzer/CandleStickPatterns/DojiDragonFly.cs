using System;
using System.Collections.Generic;

namespace Market.Analyzer.CandleStickPatterns
{
    public class DojiDragonFly : ReversePattern
    {
        public DojiDragonFly()
        {
            Name = "DojiDragonFly";
        }

        public override bool Qualified(IList<TransactionData> orderedList, Trend currentTrend)
        {
            var lastTransaction = orderedList[orderedList.Count - 1];
            var range = lastTransaction.High - lastTransaction.Low;
            //It is have middle range
            if (range / lastTransaction.Close < CandleStickConstant.MiddleChange)
                return false;
            //Open and close are close to high
            if (Math.Abs(lastTransaction.Open - lastTransaction.High)/range <= CandleStickConstant.Doji &&
                Math.Abs(lastTransaction.Close - lastTransaction.High)/range <= CandleStickConstant.Doji)
            {
                ReverseCurrentTrend(currentTrend);
                return true;
            }
            return false;
        }
    }
}