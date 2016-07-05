using System.Collections.Generic;

namespace Market.Analyzer.CandleStickPatterns
{
    public class Piercing : Pattern
    {
        public Piercing()
        {
            Name = "Piercing";
            CurrentTrend = Trend.Down;
            UpcomingTrend = Trend.Up;
        }

        public override bool Qualified(IList<TransactionData> orderedList, Trend currentTrend)
        {
            throw new System.NotImplementedException();
        }
    }
}