using System;

namespace Market.Analyzer.MACD
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

    public enum MovingAverageConvergenceDivergenceFeature
    {
        FluctuateAroundZero = 0,
        RiseAboveZero = 1,
        Bottom = 2,
        AllAboveZero = 3,
        RiseAboveZeroAfterSharpDrop = 4,
        //After Peak drop to zero, but before reaching zero, rise again
        RiseFromZero = 5,
        RiseFromZeroAfterFluctuation = 6,
        DropBelowZero = -1,
        Peak = -2,
        AllBelowZero = -3,
        DropBelowZeroAfterSharpRise = -4,
        //After Bottom rise to zero, but before reaching zero, drop again
        RetrieveFromZero = -5,
        DropBelowZeroAfterFluctuation = -6,
        Unkown = -999
    }
    public class MovingAverageConvergenceDivergenceAnalysis
    {
        public long Key { get; set; }
        public int StockKey { get; set; }
        public DateTime TimeStamp { get; set; }
        public double MACD { get; set; }
        public double Signal { get; set; }
        public double Histogram { get; set; }
        public MovingAverageConvergenceDivergenceFeature Feature { get; set; } 
    }

    public static class MovingAverageConvergenceDivergenceExtension
    {
        public static MovingAverageConvergenceDivergenceAnalysis CopyToAnalysis(
            this MovingAverageConvergenceDivergence movingAverageConvergenceDivergence)
        {
            MovingAverageConvergenceDivergenceAnalysis analysis = new MovingAverageConvergenceDivergenceAnalysis();
            analysis.StockKey = movingAverageConvergenceDivergence.StockKey;
            analysis.MACD = movingAverageConvergenceDivergence.MACD;
            analysis.Signal = movingAverageConvergenceDivergence.Signal;
            analysis.Histogram = movingAverageConvergenceDivergence.Histogram;
            analysis.TimeStamp = movingAverageConvergenceDivergence.TimeStamp;
            return analysis;
        }
    }
}