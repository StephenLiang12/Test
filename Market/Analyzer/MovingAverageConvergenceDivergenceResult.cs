using System;

namespace Market.Analyzer
{
    public class MovingAverageConvergenceDivergence
    {
        public long Key { get; set; }
        public int StockKey { get; set; }
        public DateTime TimeStamp { get; set; }
        public double MACD { get; set; }
        public double Signal { get; set; }
        public double Histogram { get; set; }
    }
    public class MovingAverageConvergenceDivergenceResult
    {
        public int Convergence { get; set; }
        public double Divergence { get; set; }
    }
}