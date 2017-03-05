using System;
using System.Linq;
using Market.Analyzer;
using Market.TestFixture.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture.Analyzer
{
    [TestClass]
    public class MovingAverageAnalyzerTestFixture
    {
        [TestMethod]
        public void ComparePriceAverageShouldBeGreaterThan0IfPriceIsAboveAverage()
        {
            var list = SampleDataReader.GetTransactionData();
            var orderedList = list.OrderBy(t => t.TimeStamp).ToList().GetFrontPartial(450);
            SimpleMovingAverageCalculator calculator = new SimpleMovingAverageCalculator();
            var average = calculator.CalculateAverage(orderedList, 20);
            MovingAverageAnalyzer analyzer = new MovingAverageAnalyzer();
            double priceCompareAverage = analyzer.PriceCompareAverage(orderedList, average);
            Console.WriteLine(priceCompareAverage);
            Assert.IsTrue(priceCompareAverage > 0);
        }

        [TestMethod]
        public void ComparePriceAverageShouldReturn0IfPriceIsFlucatingAroundAverage()
        {
            var list = SampleDataReader.GetTransactionData().OrderBy(t => t.TimeStamp).ToList();
            SimpleMovingAverageCalculator calculator = new SimpleMovingAverageCalculator();
            var average = calculator.CalculateAverage(list, 100);
            MovingAverageAnalyzer analyzer = new MovingAverageAnalyzer();
            Assert.AreEqual(0, analyzer.PriceCompareAverage(list, average));
        }

        [TestMethod]
        public void ComparePriceAverageShouldBeLessThan0IfPriceIsBelowAverage()
        {
            var list = SampleDataReader.GetTransactionData().OrderBy(t => t.TimeStamp).ToList();
            SimpleMovingAverageCalculator calculator = new SimpleMovingAverageCalculator();
            var average = calculator.CalculateAverage(list, 20);
            MovingAverageAnalyzer analyzer = new MovingAverageAnalyzer();
            double priceCompareAverage = analyzer.PriceCompareAverage(list, average);
            Console.WriteLine(priceCompareAverage);
            Assert.IsTrue(priceCompareAverage < 0);
        }

        [TestMethod]
        public void AbleToGetUpTrend()
        {
            MovingAverageAnalyzer analyzer = new MovingAverageAnalyzer();
            double[] data = new[]
            {
                10, 9.98, 10.02, 10.08, 10.05, 10.12, 10.2, 10.18, 10.22, 10.26, 10.27, 10.25, 10.26, 10.28, 10.31, 10.3,
                10.31, 10.34, 10.3, 10.36
            };
            var trend = analyzer.AnalyzeMovingTrend(data);
            Assert.AreEqual(Trend.Up, trend);
        }

        [TestMethod]
        public void AbleToGetTopTrend()
        {
            MovingAverageAnalyzer analyzer = new MovingAverageAnalyzer();
            double[] data = new[]
            {
                10, 9.98, 10.02, 10.08, 10.05, 10.12, 10.2, 10.18, 10.22, 10.26, 10.27, 10.25, 10.20, 10.18, 10.16, 10.13, 10.11, 10.04, 10.03, 10.06
            };
            var trend = analyzer.AnalyzeMovingTrend(data);
            Assert.AreEqual(Trend.Top, trend);
        }

        [TestMethod]
        public void AbleToGetTopUpTrend()
        {
            MovingAverageAnalyzer analyzer = new MovingAverageAnalyzer();
            double[] data = new[]
            {
                10, 9.98, 10.02, 10.08, 10.15, 10.12, 10.2, 10.28, 10.32, 10.36, 10.37, 10.45, 10.36, 10.38, 10.31, 10.3, 10.31, 10.34, 10.3, 10.28
            };
            var trend = analyzer.AnalyzeMovingTrend(data);
            Assert.AreEqual(Trend.TopUp, trend);
        }

        [TestMethod]
        public void AbleToGetVibrationUpTrend()
        {
            MovingAverageAnalyzer analyzer = new MovingAverageAnalyzer();
            double[] data = new[]
            {
                10, 9.98, 10.12, 9.08, 10.15, 10.12, 10.2, 10.08, 9.78, 9.96, 10.07, 10.23, 10.36, 10.38, 10.31, 10.2, 10.11, 9.94, 10.13, 10.22
            };
            var trend = analyzer.AnalyzeMovingTrend(data);
            Assert.AreEqual(Trend.VibrationUp, trend);
        }

    }
}