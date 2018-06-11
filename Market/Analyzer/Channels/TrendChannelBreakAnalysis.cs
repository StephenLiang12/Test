namespace Market.Analyzer.Channels
{
    public class TrendChannelBreakAnalysis
    {
        public long Key { get; set; }
        public int StockKey { get; set; }
        public string FeatureName { get; set; }
        public int Count { get; set; }
        public double AverageAccuracy { get; set; }
        public double AverageChangePercentage { get; set; }
        public double MaxChangePercentage { get; set; }
    }
}