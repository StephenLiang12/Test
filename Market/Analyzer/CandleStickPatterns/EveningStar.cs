using System.Collections.Generic;

namespace Market.Analyzer.CandleStickPatterns
{
    public class EveningStar : Pattern
    {
        public EveningStar()
        {
            Name = "EveningStar";
            CurrentTrend = Trend.Up;
            UpcomingTrend = Trend.Down;
        }

        public override bool Qualified(IList<TransactionData> orderedList, Trend currentTrend)
        {
            throw new System.NotImplementedException();
        }
    }
}