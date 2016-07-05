using System.Collections.Generic;

namespace Market.Analyzer.CandleStickPatterns
{
    public abstract class Pattern
    {
        public string Name { get; protected set; }
        public Trend CurrentTrend { get; protected set; }
        public Trend UpcomingTrend { get; protected set; }
        public abstract bool Qualified(IList<TransactionData> orderedList, Trend currentTrend);
    }
}