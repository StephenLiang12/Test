using System;
using System.Linq;
using Market.Analyzer;
using Market.Analyzer.MACD;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture.Analyzer
{
    [TestClass]
    public class MovingAverageConvergenceDivergenceTestFixture : DbContextTestFixtureBase
    {
        [TestMethod]
        public void AbleToFindRiseAboveZero()
        {
            int stockKey = 96;
            StockContext context = new StockContext();
            var list = context.MovingAverageConvergenceDivergences.Where(m => m.StockKey == stockKey && m.TimeStamp >= new DateTime(2016, 5, 1) && m.TimeStamp <= new DateTime(2016, 11, 30)).OrderBy(m => m.TimeStamp).ToList();
            MovingAverageConvergenceDivergencePatternAnalyzer analyzer = new MovingAverageConvergenceDivergencePatternAnalyzer();
            Assert.AreEqual(MovingAverageConvergenceDivergenceFeature.RiseAboveZero, analyzer.Analyze(list));
        }

        [TestMethod]
        public void AbleToFindAllAboveZero()
        {
            int stockKey = 477;
            StockContext context = new StockContext();
            var list = context.MovingAverageConvergenceDivergences.Where(m => m.StockKey == stockKey && m.TimeStamp >= new DateTime(2015, 5, 1) && m.TimeStamp <= new DateTime(2016, 1, 6)).OrderBy(m => m.TimeStamp).ToList();
            MovingAverageConvergenceDivergencePatternAnalyzer analyzer = new MovingAverageConvergenceDivergencePatternAnalyzer();
            Assert.AreEqual(MovingAverageConvergenceDivergenceFeature.AllAboveZero, analyzer.Analyze(list));
        }

        [TestMethod]
        public void AbleToFindPeak()
        {
            int stockKey = 477;
            StockContext context = new StockContext();
            var list = context.MovingAverageConvergenceDivergences.Where(m => m.StockKey == stockKey && m.TimeStamp >= new DateTime(2016, 10, 1) && m.TimeStamp <= new DateTime(2017, 3, 15)).OrderBy(m => m.TimeStamp).ToList();
            MovingAverageConvergenceDivergencePatternAnalyzer analyzer = new MovingAverageConvergenceDivergencePatternAnalyzer();
            Assert.AreEqual(MovingAverageConvergenceDivergenceFeature.Peak, analyzer.Analyze(list));
        }

        [TestMethod]
        public void AbleToFindPeakAfterDeeperBottom()
        {
            int stockKey = 456;
            StockContext context = new StockContext();
            var list = context.MovingAverageConvergenceDivergences.Where(m => m.StockKey == stockKey && m.TimeStamp >= new DateTime(2016, 10, 1) && m.TimeStamp <= new DateTime(2017, 3, 8)).OrderBy(m => m.TimeStamp).ToList();
            MovingAverageConvergenceDivergencePatternAnalyzer analyzer = new MovingAverageConvergenceDivergencePatternAnalyzer();
            Assert.AreEqual(MovingAverageConvergenceDivergenceFeature.Peak, analyzer.Analyze(list));
        }

        [TestMethod]
        public void AbleToFindDropBelowZero()
        {
            int stockKey = 479;
            StockContext context = new StockContext();
            var list = context.MovingAverageConvergenceDivergences.Where(m => m.StockKey == stockKey && m.TimeStamp >= new DateTime(2016, 8, 1) && m.TimeStamp <= new DateTime(2017, 1, 3)).OrderBy(m => m.TimeStamp).ToList();
            MovingAverageConvergenceDivergencePatternAnalyzer analyzer = new MovingAverageConvergenceDivergencePatternAnalyzer();
            Assert.AreEqual(MovingAverageConvergenceDivergenceFeature.DropBelowZero, analyzer.Analyze(list));
        }

        [TestMethod]
        public void AbleToFindAllBelowZero()
        {
            int stockKey = 479;
            StockContext context = new StockContext();
            var list = context.MovingAverageConvergenceDivergences.Where(m => m.StockKey == stockKey && m.TimeStamp >= new DateTime(2015, 7, 1) && m.TimeStamp <= new DateTime(2016, 1, 8)).OrderBy(m => m.TimeStamp).ToList();
            MovingAverageConvergenceDivergencePatternAnalyzer analyzer = new MovingAverageConvergenceDivergencePatternAnalyzer();
            Assert.AreEqual(MovingAverageConvergenceDivergenceFeature.AllBelowZero, analyzer.Analyze(list));
        }

        [TestMethod]
        public void AbleToFindBottom()
        {
            int stockKey = 479;
            StockContext context = new StockContext();
            var list = context.MovingAverageConvergenceDivergences.Where(m => m.StockKey == stockKey && m.TimeStamp >= new DateTime(2016, 10, 1) && m.TimeStamp <= new DateTime(2017, 3, 15)).OrderBy(m => m.TimeStamp).ToList();
            MovingAverageConvergenceDivergencePatternAnalyzer analyzer = new MovingAverageConvergenceDivergencePatternAnalyzer();
            Assert.AreEqual(MovingAverageConvergenceDivergenceFeature.Bottom, analyzer.Analyze(list));
        }

        [TestMethod]
        public void AbleToFindRiseAboveZeroAfterSharpDrop()
        {
            int stockKey = 479;
            StockContext context = new StockContext();
            var list = context.MovingAverageConvergenceDivergences.Where(m => m.StockKey == stockKey && m.TimeStamp >= new DateTime(2015, 2, 1) && m.TimeStamp <= new DateTime(2015, 8, 18)).OrderBy(m => m.TimeStamp).ToList();
            MovingAverageConvergenceDivergencePatternAnalyzer analyzer = new MovingAverageConvergenceDivergencePatternAnalyzer();
            Assert.AreEqual(MovingAverageConvergenceDivergenceFeature.RiseAboveZeroAfterSharpDrop, analyzer.Analyze(list));
        }

        [TestMethod]
        public void AbleToFindDropBelowZeroAfterSharpRise()
        {
            int stockKey = 477;
            StockContext context = new StockContext();
            var list = context.MovingAverageConvergenceDivergences.Where(m => m.StockKey == stockKey && m.TimeStamp >= new DateTime(2015, 7, 1) && m.TimeStamp <= new DateTime(2016, 1, 26)).OrderBy(m => m.TimeStamp).ToList();
            MovingAverageConvergenceDivergencePatternAnalyzer analyzer = new MovingAverageConvergenceDivergencePatternAnalyzer();
            Assert.AreEqual(MovingAverageConvergenceDivergenceFeature.DropBelowZeroAfterSharpRise, analyzer.Analyze(list));
        }

        [TestMethod]
        public void AbleToFindFluctuateAroundZero()
        {
            int stockKey = 96;
            StockContext context = new StockContext();
            var list = context.MovingAverageConvergenceDivergences.Where(m => m.StockKey == stockKey && m.TimeStamp >= new DateTime(2016, 11, 1) && m.TimeStamp <= new DateTime(2017, 3, 29)).OrderBy(m => m.TimeStamp).ToList();
            MovingAverageConvergenceDivergencePatternAnalyzer analyzer = new MovingAverageConvergenceDivergencePatternAnalyzer();
            Assert.AreEqual(MovingAverageConvergenceDivergenceFeature.FluctuateAroundZero, analyzer.Analyze(list));
        }

        [TestMethod]
        public void AbleToFindRiseFromZeroAfterFluctuation()
        {
            int stockKey = 96;
            StockContext context = new StockContext();
            var list = context.MovingAverageConvergenceDivergences.Where(m => m.StockKey == stockKey && m.TimeStamp >= new DateTime(2016, 11, 1) && m.TimeStamp <= new DateTime(2017, 3, 29)).OrderBy(m => m.TimeStamp).ToList();
            MovingAverageConvergenceDivergencePatternAnalyzer analyzer = new MovingAverageConvergenceDivergencePatternAnalyzer();
            var result = analyzer.Analyze(list);
            Assert.AreEqual(MovingAverageConvergenceDivergenceFeature.FluctuateAroundZero, result);
            var analysis = list[list.Count - 1].CopyToAnalysis();
            analysis.Feature = result;
            context.MovingAverageConvergenceDivergenceAnalyses.Add(analysis);
            context.SaveChanges();
            list = context.MovingAverageConvergenceDivergences.Where(m => m.StockKey == stockKey && m.TimeStamp >= new DateTime(2016, 11, 1) && m.TimeStamp <= new DateTime(2017, 3, 30)).OrderBy(m => m.TimeStamp).ToList();
            Assert.AreEqual(MovingAverageConvergenceDivergenceFeature.RiseFromZeroAfterFluctuation, analyzer.Analyze(list));
        }

        [TestMethod]
        public void AbleToFindDropBelowZeroAfterFluctuation()
        {
            int stockKey = 479;
            StockContext context = new StockContext();
            var list = context.MovingAverageConvergenceDivergences.Where(m => m.StockKey == stockKey && m.TimeStamp >= new DateTime(2016, 10, 1) && m.TimeStamp <= new DateTime(2017, 3, 2)).OrderBy(m => m.TimeStamp).ToList();
            MovingAverageConvergenceDivergencePatternAnalyzer analyzer = new MovingAverageConvergenceDivergencePatternAnalyzer();
            var result = analyzer.Analyze(list);
            Assert.AreEqual(MovingAverageConvergenceDivergenceFeature.FluctuateAroundZero, result);
            var analysis = list[list.Count - 1].CopyToAnalysis();
            analysis.Feature = result;
            context.MovingAverageConvergenceDivergenceAnalyses.Add(analysis);
            context.SaveChanges();
            list = context.MovingAverageConvergenceDivergences.Where(m => m.StockKey == stockKey && m.TimeStamp >= new DateTime(2016, 10, 1) && m.TimeStamp <= new DateTime(2017, 3, 8)).OrderBy(m => m.TimeStamp).ToList();
            Assert.AreEqual(MovingAverageConvergenceDivergenceFeature.DropBelowZeroAfterFluctuation, analyzer.Analyze(list));
        }

        [TestMethod]
        public void AbleToFindRetrieveFromZero()
        {
            int stockKey = 456;
            StockContext context = new StockContext();
            var list = context.MovingAverageConvergenceDivergences.Where(m => m.StockKey == stockKey && m.TimeStamp >= new DateTime(2016, 11, 1) && m.TimeStamp <= new DateTime(2017, 4, 3)).OrderBy(m => m.TimeStamp).ToList();
            MovingAverageConvergenceDivergencePatternAnalyzer analyzer = new MovingAverageConvergenceDivergencePatternAnalyzer();
            Assert.AreEqual(MovingAverageConvergenceDivergenceFeature.RetrieveFromZero, analyzer.Analyze(list));
        }

        [TestMethod]
        public void AbleToFindRiseFromZero()
        {
            int stockKey = 459;
            StockContext context = new StockContext();
            var list = context.MovingAverageConvergenceDivergences.Where(m => m.StockKey == stockKey && m.TimeStamp >= new DateTime(2016, 11, 1) && m.TimeStamp <= new DateTime(2017, 4, 3)).OrderBy(m => m.TimeStamp).ToList();
            MovingAverageConvergenceDivergencePatternAnalyzer analyzer = new MovingAverageConvergenceDivergencePatternAnalyzer();
            Assert.AreEqual(MovingAverageConvergenceDivergenceFeature.RiseFromZero, analyzer.Analyze(list));
        }

        [TestMethod]
        public void ShouldNotTreatSecondDayOfRiseAboveZeroAsRiseFromZero()
        {
            int stockKey = 477;
            StockContext context = new StockContext();
            var list = context.MovingAverageConvergenceDivergences.Where(m => m.StockKey == stockKey && m.TimeStamp >= new DateTime(2016, 10, 1) && m.TimeStamp <= new DateTime(2017, 3, 2)).OrderBy(m => m.TimeStamp).ToList();
            MovingAverageConvergenceDivergencePatternAnalyzer analyzer = new MovingAverageConvergenceDivergencePatternAnalyzer();
            Assert.AreEqual(MovingAverageConvergenceDivergenceFeature.Unkown, analyzer.Analyze(list));
        }

        [TestMethod]
        public void IfPeakDoesNotDropMuchShouldNotTreatIsAsRiseFromZero()
        {
            int stockKey = 613;
            StockContext context = new StockContext();
            var list = context.MovingAverageConvergenceDivergences.Where(m => m.StockKey == stockKey && m.TimeStamp >= new DateTime(2016, 11, 1) && m.TimeStamp <= new DateTime(2017, 5, 1)).OrderBy(m => m.TimeStamp).ToList();
            MovingAverageConvergenceDivergencePatternAnalyzer analyzer = new MovingAverageConvergenceDivergencePatternAnalyzer();
            Assert.AreEqual(MovingAverageConvergenceDivergenceFeature.Unkown, analyzer.Analyze(list));
        }

    }
}