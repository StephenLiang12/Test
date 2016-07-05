using System.Collections.Generic;
using Market.Analyzer;
using Market.Analyzer.CandleStickPatterns;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture.Analyzer.CandleStickPatterns
{
    [TestClass]
    public class MarubozuTestFixture
    {
        [TestMethod]
        public void AbleToFindUpMarubozu()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = transactionData.Low = 1;
            transactionData.Close = transactionData.High = 1.5;
            list.Add(transactionData);
            Marubozu pattern = new Marubozu();
            Assert.IsTrue(pattern.Qualified(list, Trend.Vibration));
            Assert.AreEqual(Trend.Vibration, pattern.CurrentTrend);
            Assert.AreEqual(Trend.Up, pattern.UpcomingTrend);
        }

        [TestMethod]
        public void AbleToFindDownMarubozu()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = transactionData.High = 2;
            transactionData.Close = transactionData.Low = 1.5;
            list.Add(transactionData);
            Marubozu pattern = new Marubozu();
            Assert.IsTrue(pattern.Qualified(list, Trend.Vibration));
            Assert.AreEqual(Trend.Vibration, pattern.CurrentTrend);
            Assert.AreEqual(Trend.Down, pattern.UpcomingTrend);
        }

        [TestMethod]
        public void IfOpenIsNotEqualLowItisNotUpMarubozu()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = 1;
            transactionData.Low = 0.8;
            transactionData.Close = transactionData.High = 1.5;
            list.Add(transactionData);
            Marubozu pattern = new Marubozu();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void IfCloseIsNotEqualHighItisNotUpMarubozu()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = 1;
            transactionData.Low = 1;
            transactionData.Close = 1.2;
            transactionData.High = 1.5;
            list.Add(transactionData);
            Marubozu pattern = new Marubozu();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void IfOpenIsNotEqualHighItisNotDownMarubozu()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = 1.8;
            transactionData.Low = 1;
            transactionData.Close = 1;
            transactionData.High = 2;
            list.Add(transactionData);
            Marubozu pattern = new Marubozu();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void IfCloseIsNotEqualLowItisNotDownMarubozu()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = 2;
            transactionData.Low = 1;
            transactionData.Close = 1.2;
            transactionData.High = 2;
            list.Add(transactionData);
            Marubozu pattern = new Marubozu();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void IfChangeIsLessThan5PercentOfCloseItisNotDownMarubozu()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = 1;
            transactionData.Low = 1;
            transactionData.Close = 1.02;
            transactionData.High = 1.02;
            list.Add(transactionData);
            Marubozu pattern = new Marubozu();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }
    }
}