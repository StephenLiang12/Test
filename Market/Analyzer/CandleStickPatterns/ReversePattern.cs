namespace Market.Analyzer.CandleStickPatterns
{
    public abstract class ReversePattern : Pattern
    {
        public void ReverseCurrentTrend(Trend currentTrend)
        {
            CurrentTrend = currentTrend;
            if (CurrentTrend == Trend.Up)
                UpcomingTrend = Trend.Down;
            else if (CurrentTrend == Trend.Down)
                UpcomingTrend = Trend.Up;
            else
                UpcomingTrend = Trend.Vibration;
        }
    }
}