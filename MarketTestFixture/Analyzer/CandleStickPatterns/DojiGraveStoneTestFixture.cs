using System.Collections.Generic;
using Market.Analyzer;
using Market.Analyzer.CandleStickPatterns;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture.Analyzer.CandleStickPatterns
{
    [TestClass]
    public class DojiGraveStoneTestFixture
    {
        [TestMethod]
        public void AbleToFindUpDojiGraveStone()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = transactionData.Low = 1;
            transactionData.Close = 1;
            transactionData.High = 1.5;
            list.Add(transactionData);
            DojiGraveStone pattern = new DojiGraveStone();
            Assert.IsTrue(pattern.Qualified(list, Trend.Down));
            Assert.AreEqual(Trend.Down, pattern.CurrentTrend);
            Assert.AreEqual(Trend.Up, pattern.UpcomingTrend);
        }

        [TestMethod]
        public void AbleToFindDownDojiGraveStone()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = transactionData.Low = 1;
            transactionData.Close = 1;
            transactionData.High = 1.5;
            list.Add(transactionData);
            DojiGraveStone pattern = new DojiGraveStone();
            Assert.IsTrue(pattern.Qualified(list, Trend.Up));
            Assert.AreEqual(Trend.Up, pattern.CurrentTrend);
            Assert.AreEqual(Trend.Down, pattern.UpcomingTrend);
        }

        [TestMethod]
        public void AbleToFindDojiGraveStoneButNotUpComingTrend()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = transactionData.Low = 1;
            transactionData.Close = 1;
            transactionData.High = 1.5;
            list.Add(transactionData);
            DojiGraveStone pattern = new DojiGraveStone();
            Assert.IsTrue(pattern.Qualified(list, Trend.Vibration));
            Assert.AreEqual(Trend.Vibration, pattern.CurrentTrend);
            Assert.AreEqual(Trend.Vibration, pattern.UpcomingTrend);
        }

        [TestMethod]
        public void IfOpenIsAwayFromLowItisNotDojiGraveStone()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = 1;
            transactionData.Low = 0.8;
            transactionData.Close = 0.8;
            transactionData.High = 1.5;
            list.Add(transactionData);
            DojiGraveStone pattern = new DojiGraveStone();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void IfCloseIsAwayFromLowItisNotDojiGraveStone()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = 1;
            transactionData.Low = 1;
            transactionData.Close = 1.2;
            transactionData.High = 1.5;
            list.Add(transactionData);
            DojiGraveStone pattern = new DojiGraveStone();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void IfChangeIsLessThan3PercentOfCloseItisNotDojiGraveStone()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = 1;
            transactionData.Low = 1;
            transactionData.Close = 1;
            transactionData.High = 1.02;
            list.Add(transactionData);
            DojiGraveStone pattern = new DojiGraveStone();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }
    }
}