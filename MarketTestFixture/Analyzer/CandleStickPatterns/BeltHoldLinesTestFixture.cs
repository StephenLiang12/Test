using System.Collections.Generic;
using Market.Analyzer;
using Market.Analyzer.CandleStickPatterns;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture.Analyzer.CandleStickPatterns
{
    [TestClass]
    public class BeltHoldLinesTestFixture
    {
        [TestMethod]
        public void AbleToFindUpBeltHoldLines()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = transactionData.Low = 1;
            transactionData.Close = 1.48;
            transactionData.High = 1.5;
            list.Add(transactionData);
            BeltHoldLines pattern = new BeltHoldLines();
            Assert.IsTrue(pattern.Qualified(list, Trend.Vibration));
            Assert.AreEqual(Trend.Vibration, pattern.CurrentTrend);
            Assert.AreEqual(Trend.Up, pattern.UpcomingTrend);
        }

        [TestMethod]
        public void AbleToFindDownBeltHoldLines()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = transactionData.High = 2;
            transactionData.Close = 1.48;
            transactionData.Low = 1.5;
            list.Add(transactionData);
            BeltHoldLines pattern = new BeltHoldLines();
            Assert.IsTrue(pattern.Qualified(list, Trend.Vibration));
            Assert.AreEqual(Trend.Vibration, pattern.CurrentTrend);
            Assert.AreEqual(Trend.Down, pattern.UpcomingTrend);
        }

        [TestMethod]
        public void IfOpenIsNotEqualLowItisNotUpBeltHoldLines()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = 1;
            transactionData.Low = 0.8;
            transactionData.Close = 1.48;
            transactionData.High = 1.5;
            list.Add(transactionData);
            BeltHoldLines pattern = new BeltHoldLines();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void IfCloseIsAwayFromHighItisNotUpBeltHoldLines()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = 1;
            transactionData.Low = 1;
            transactionData.Close = 1.4;
            transactionData.High = 1.5;
            list.Add(transactionData);
            BeltHoldLines pattern = new BeltHoldLines();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void IfOpenIsNotEqualHighItisNotDownBeltHoldLines()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = 1.8;
            transactionData.Low = 1;
            transactionData.Close = 1;
            transactionData.High = 2;
            list.Add(transactionData);
            BeltHoldLines pattern = new BeltHoldLines();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void IfCloseIsAwayFromLowItisNotDownBeltHoldLines()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = 2;
            transactionData.Low = 1;
            transactionData.Close = 1.2;
            transactionData.High = 2;
            list.Add(transactionData);
            BeltHoldLines pattern = new BeltHoldLines();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void IfChangeIsLessThan3PercentOfCloseItisNotDownBeltHoldLines()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = 1;
            transactionData.Low = 1;
            transactionData.Close = 1.02;
            transactionData.High = 1.02;
            list.Add(transactionData);
            BeltHoldLines pattern = new BeltHoldLines();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }
    }
}