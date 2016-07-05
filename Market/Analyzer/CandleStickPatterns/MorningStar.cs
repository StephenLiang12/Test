using System.Collections.Generic;

namespace Market.Analyzer.CandleStickPatterns
{
    public class MorningStar : Pattern
    {
        public MorningStar()
        {
            Name = "MorningStar";
            CurrentTrend = Trend.Down;
            UpcomingTrend = Trend.Up;
        }

        public override bool Qualified(IList<TransactionData> orderedList, Trend currentTrend)
        {
            throw new System.NotImplementedException();
        }
    }
}