using System.Collections.Generic;
using Market.Analyzer;
using Market.Analyzer.CandleStickPatterns;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture.Analyzer.CandleStickPatterns
{
    [TestClass]
    public class BullishEngulfingTestFixture
    {
        [TestMethod]
        public void BullishEngulfingNeedAtLeastTwoTransaction()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            list.Add(transactionData);
            BullishEngulfing pattern = new BullishEngulfing();
            Assert.IsFalse(pattern.Qualified(list, Trend.Down));
        }

        [TestMethod]
        public void AbleToFindBullishEngulfingPattern()
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
            BullishEngulfing pattern = new BullishEngulfing();
            Assert.IsTrue(pattern.Qualified(list, Trend.Vibration));
            Assert.AreEqual(Trend.Vibration, pattern.CurrentTrend);
            Assert.AreEqual(Trend.Up, pattern.UpcomingTrend);
        }

        [TestMethod]
        public void BullishEngulfingPatternRequireLastTransactionIsBullish()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 2;
            transactionData1.Close = 1.8;
            transactionData1.High = 2.05;
            transactionData1.Low = 1.78;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 2.2;
            transactionData2.Close = 1.75;
            transactionData2.High = 2.21;
            transactionData2.Low = 1.68;
            list.Add(transactionData2);
            BullishEngulfing pattern = new BullishEngulfing();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void BullishEngulfingPatternRequireLastSecondTransactionIsBearish()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 1.8;
            transactionData1.Close = 2;
            transactionData1.High = 2.05;
            transactionData1.Low = 1.78;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 1.75;
            transactionData2.Close = 2.2;
            transactionData2.High = 2.21;
            transactionData2.Low = 1.68;
            list.Add(transactionData2);
            BullishEngulfing pattern = new BullishEngulfing();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void BullishEngulfingPatternRequireRangeOfLastTransactionIsTwiceOfRangeOfLastSecondTransaction()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 2;
            transactionData1.Close = 1.8;
            transactionData1.High = 2.05;
            transactionData1.Low = 1.78;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 2;
            transactionData2.Close = 2.2;
            transactionData2.High = 2.21;
            transactionData2.Low = 1.95;
            list.Add(transactionData2);
            BullishEngulfing pattern = new BullishEngulfing();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void BullishEngulfingPatternRequireLastTransactionEngulfLastSecondTransaction()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 2;
            transactionData1.Close = 1.8;
            transactionData1.High = 2.05;
            transactionData1.Low = 1.78;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 1.9;
            transactionData2.Close = 2.3;
            transactionData2.High = 2.41;
            transactionData2.Low = 1.85;
            list.Add(transactionData2);
            BullishEngulfing pattern = new BullishEngulfing();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

    }
}