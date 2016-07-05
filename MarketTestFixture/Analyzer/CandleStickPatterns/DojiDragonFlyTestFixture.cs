using System.Collections.Generic;
using Market.Analyzer;
using Market.Analyzer.CandleStickPatterns;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture.Analyzer.CandleStickPatterns
{
    [TestClass]
    public class DojiDragonFlyTestFixture
    {
        [TestMethod]
        public void AbleToFindUpDojiDragonFly()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = transactionData.High = 1;
            transactionData.Close = 1;
            transactionData.Low = 0.5;
            list.Add(transactionData);
            DojiDragonFly pattern = new DojiDragonFly();
            Assert.IsTrue(pattern.Qualified(list, Trend.Down));
            Assert.AreEqual(Trend.Down, pattern.CurrentTrend);
            Assert.AreEqual(Trend.Up, pattern.UpcomingTrend);
        }

        [TestMethod]
        public void AbleToFindDownDojiDragonFly()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = transactionData.High = 1;
            transactionData.Close = 1;
            transactionData.Low = 0.5;
            list.Add(transactionData);
            DojiDragonFly pattern = new DojiDragonFly();
            Assert.IsTrue(pattern.Qualified(list, Trend.Up));
            Assert.AreEqual(Trend.Up, pattern.CurrentTrend);
            Assert.AreEqual(Trend.Down, pattern.UpcomingTrend);
        }

        [TestMethod]
        public void AbleToFindDojiDragonFlyButNotUpComingTrend()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = transactionData.High = 1;
            transactionData.Close = 1;
            transactionData.Low = 0.5;
            list.Add(transactionData);
            DojiDragonFly pattern = new DojiDragonFly();
            Assert.IsTrue(pattern.Qualified(list, Trend.Vibration));
            Assert.AreEqual(Trend.Vibration, pattern.CurrentTrend);
            Assert.AreEqual(Trend.Vibration, pattern.UpcomingTrend);
        }

        [TestMethod]
        public void IfOpenIsAwayFromHignItisNotDojiDragonFly()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = 1;
            transactionData.Low = 0.5;
            transactionData.Close = 1.5;
            transactionData.High = 1.5;
            list.Add(transactionData);
            DojiDragonFly pattern = new DojiDragonFly();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void IfCloseIsAwayFromHighItisNotDojiDragonFly()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = 1.5;
            transactionData.Low = 1;
            transactionData.Close = 1.2;
            transactionData.High = 1.5;
            list.Add(transactionData);
            DojiDragonFly pattern = new DojiDragonFly();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void IfChangeIsLessThan3PercentOfCloseItisNotDojiDragonFly()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = 1;
            transactionData.Low = 1;
            transactionData.Close = 0.9;
            transactionData.High = 1;
            list.Add(transactionData);
            DojiDragonFly pattern = new DojiDragonFly();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }
    }
}