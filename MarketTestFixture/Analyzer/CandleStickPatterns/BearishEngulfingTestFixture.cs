using System.Collections.Generic;
using Market.Analyzer;
using Market.Analyzer.CandleStickPatterns;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture.Analyzer.CandleStickPatterns
{
    [TestClass]
    public class BearishEngulfingTestFixture
    {
        [TestMethod]
        public void BearishEngulfingNeedAtLeastTwoTransaction()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            list.Add(transactionData);
            BearishEngulfing pattern = new BearishEngulfing();
            Assert.IsFalse(pattern.Qualified(list, Trend.Up));
        }

        [TestMethod]
        public void AbleToFindBearishEngulfingPattern()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 1.8;
            transactionData1.Close = 2;
            transactionData1.High = 2.05;
            transactionData1.Low = 1.78;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 2.2;
            transactionData2.Close = 1.75;
            transactionData2.High = 2.21;
            transactionData2.Low = 1.68;
            list.Add(transactionData2);
            BearishEngulfing pattern = new BearishEngulfing();
            Assert.IsTrue(pattern.Qualified(list, Trend.Vibration));
            Assert.AreEqual(Trend.Vibration, pattern.CurrentTrend);
            Assert.AreEqual(Trend.Down, pattern.UpcomingTrend);
        }

        [TestMethod]
        public void BearishEngulfingPatternRequireLastTransactionIsBearish()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 2;
            transactionData1.Close = 1.8;
            transactionData1.High = 2.05;
            transactionData1.Low = 1.78;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 1.75;
            transactionData2.Close = 2.2;
            transactionData2.High = 2.21;
            transactionData2.Low = 1.68;
            list.Add(transactionData2);
            BearishEngulfing pattern = new BearishEngulfing();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void BearishEngulfingPatternRequireLastSecondTransactionIsBullish()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 2;
            transactionData1.Close = 1.8;
            transactionData1.High = 2.05;
            transactionData1.Low = 1.78;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 1.75;
            transactionData2.Close = 2.2;
            transactionData2.High = 2.21;
            transactionData2.Low = 1.68;
            list.Add(transactionData2);
            BearishEngulfing pattern = new BearishEngulfing();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void BearishEngulfingPatternRequireRangeOfLastTransactionIsTwiceOfRangeOfLastSecondTransaction()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 1.8;
            transactionData1.Close = 2;
            transactionData1.High = 2.05;
            transactionData1.Low = 1.78;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 2.2;
            transactionData2.Close = 2;
            transactionData2.High = 2.21;
            transactionData2.Low = 1.95;
            list.Add(transactionData2);
            BearishEngulfing pattern = new BearishEngulfing();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void BearishEngulfingPatternRequireLastTransactionEngulfLastSecondTransaction()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 1.8;
            transactionData1.Close = 2;
            transactionData1.High = 2.05;
            transactionData1.Low = 1.78;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 1.9;
            transactionData2.Close = 1.5;
            transactionData2.High = 2.0;
            transactionData2.Low = 1.45;
            list.Add(transactionData2);
            BearishEngulfing pattern = new BearishEngulfing();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }
    }
}