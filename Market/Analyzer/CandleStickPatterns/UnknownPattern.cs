using System.Collections.Generic;

namespace Market.Analyzer.CandleStickPatterns
{
    public class UnknownPattern : Pattern
    {
        public UnknownPattern()
        {
            Name = "Unknown";
        }

        public override bool Qualified(IList<TransactionData> orderedList, Trend currentTrend)
        {
            CurrentTrend = currentTrend;
            UpcomingTrend = Trend.Vibration;
            return true;
        }
    }
}