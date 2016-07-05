using System.Collections.Generic;
using Market.Analyzer;
using Market.Analyzer.CandleStickPatterns;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture.Analyzer.CandleStickPatterns
{
    [TestClass]
    public class DarkCloudCoverTestFixture
    {
        [TestMethod]
        public void DarkCloudCoverNeedAtLeastTwoTransaction()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            list.Add(transactionData);
            DarkCloudCover pattern = new DarkCloudCover();
            Assert.IsFalse(pattern.Qualified(list, Trend.Up));
        }

        [TestMethod]
        public void AbleToFindDarkCloudCoverPattern()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 18;
            transactionData1.Close = 20;
            transactionData1.High = 20.5;
            transactionData1.Low = 17.8;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 20.5;
            transactionData2.Close = 17.5;
            transactionData2.High = 22.1;
            transactionData2.Low = 16.8;
            list.Add(transactionData2);
            DarkCloudCover pattern = new DarkCloudCover();
            Assert.IsTrue(pattern.Qualified(list, Trend.Vibration));
            Assert.AreEqual(Trend.Vibration, pattern.CurrentTrend);
            Assert.AreEqual(Trend.Down, pattern.UpcomingTrend);
        }

        [TestMethod]
        public void DarkCloudCoverPatternRequireLastTransactionIsBearish()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 18;
            transactionData1.Close = 20;
            transactionData1.High = 20.5;
            transactionData1.Low = 17.8;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 17.5;
            transactionData2.Close = 20.5;
            transactionData2.High = 22.1;
            transactionData2.Low = 16.8;
            list.Add(transactionData2);
            DarkCloudCover pattern = new DarkCloudCover();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void DarkCloudCoverPatternRequireLastSecondTransactionIsBullish()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 20;
            transactionData1.Close = 18;
            transactionData1.High = 20.5;
            transactionData1.Low = 17.8;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 20.5;
            transactionData2.Close = 17.5;
            transactionData2.High = 22.1;
            transactionData2.Low = 16.8;
            list.Add(transactionData2);
            DarkCloudCover pattern = new DarkCloudCover();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void DarkCloudCoverPatternRequireChangeOfLastTwoTransactionIsGreaterThan3PercentOfClose()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 18;
            transactionData1.Close = 18.2;
            transactionData1.High = 20.5;
            transactionData1.Low = 17.8;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 20.5;
            transactionData2.Close = 17.5;
            transactionData2.High = 22.1;
            transactionData2.Low = 16.8;
            list.Add(transactionData2);
            DarkCloudCover pattern = new DarkCloudCover();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void DarkCloudCoverPatternRequireCloseOfLastTransactionIsLessThanTheMiddleLastSecondTransaction()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 18;
            transactionData1.Close = 20;
            transactionData1.High = 20.5;
            transactionData1.Low = 17.8;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 20.5;
            transactionData2.Close = 19.5;
            transactionData2.High = 22.1;
            transactionData2.Low = 16.8;
            list.Add(transactionData2);
            DarkCloudCover pattern = new DarkCloudCover();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }
    }
}