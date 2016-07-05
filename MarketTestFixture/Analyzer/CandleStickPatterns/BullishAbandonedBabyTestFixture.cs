using System.Collections.Generic;
using Market.Analyzer;
using Market.Analyzer.CandleStickPatterns;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture.Analyzer.CandleStickPatterns
{
    [TestClass]
    public class BullishAbandonedBabyTestFixture
    {
        [TestMethod]
        public void BullingAbandonedBabyNeedAtLeastThreeTransactions()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            list.Add(transactionData1);
            BullishAbandonedBaby pattern = new BullishAbandonedBaby();
            Assert.IsFalse(pattern.Qualified(list, Trend.Up));
            TransactionData transactionData2 = new TransactionData();
            list.Add(transactionData2);
            Assert.IsFalse(pattern.Qualified(list, Trend.Up));
        }

        [TestMethod]
        public void AbleToFindBullingAbandonedBabyPattern()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 20;
            transactionData1.Close = 18;
            transactionData1.High = 20.5;
            transactionData1.Low = 17.8;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 17.05;
            transactionData2.Close = 17.1;
            transactionData2.High = 18.1;
            transactionData2.Low = 16.5;
            list.Add(transactionData2);
            TransactionData transactionData3 = new TransactionData();
            transactionData3.Open = 17.8;
            transactionData3.Close = 19.2;
            transactionData3.High = 19.5;
            transactionData3.Low = 17.5;
            list.Add(transactionData3);
            BullishAbandonedBaby pattern = new BullishAbandonedBaby();
            Assert.IsTrue(pattern.Qualified(list, Trend.Vibration));
            Assert.AreEqual(Trend.Vibration, pattern.CurrentTrend);
            Assert.AreEqual(Trend.Up, pattern.UpcomingTrend);
        }

        [TestMethod]
        public void BullingAbandonedBabyPatternRequireFirstTransactionIsBearish()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 18;
            transactionData1.Close = 20;
            transactionData1.High = 20.5;
            transactionData1.Low = 17.8;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 17.05;
            transactionData2.Close = 17.1;
            transactionData2.High = 18.1;
            transactionData2.Low = 16.5;
            list.Add(transactionData2);
            TransactionData transactionData3 = new TransactionData();
            transactionData3.Open = 17.8;
            transactionData3.Close = 19.2;
            transactionData3.High = 19.5;
            transactionData3.Low = 17.5;
            list.Add(transactionData3);
            BullishAbandonedBaby pattern = new BullishAbandonedBaby();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void BullingAbandonedBabyPatternRequireSecondTransactionIsDoji()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 20;
            transactionData1.Close = 18;
            transactionData1.High = 20.5;
            transactionData1.Low = 17.8;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 17;
            transactionData2.Close = 17.2;
            transactionData2.High = 18.1;
            transactionData2.Low = 16.5;
            list.Add(transactionData2);
            TransactionData transactionData3 = new TransactionData();
            transactionData3.Open = 17.8;
            transactionData3.Close = 19.2;
            transactionData3.High = 19.5;
            transactionData3.Low = 17.5;
            list.Add(transactionData3);
            BullishAbandonedBaby pattern = new BullishAbandonedBaby();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void BullingAbandonedBabyPatternRequireLastTransactionIsBullish()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 20;
            transactionData1.Close = 18;
            transactionData1.High = 20.5;
            transactionData1.Low = 17.8;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 17.05;
            transactionData2.Close = 17.2;
            transactionData2.High = 18.1;
            transactionData2.Low = 16.5;
            list.Add(transactionData2);
            TransactionData transactionData3 = new TransactionData();
            transactionData3.Open = 19.2;
            transactionData3.Close = 17.8;
            transactionData3.High = 19.5;
            transactionData3.Low = 17.5;
            list.Add(transactionData3);
            BullishAbandonedBaby pattern = new BullishAbandonedBaby();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void BullingAbandonedBabyPatternRequireGapBetweenFirstTransactionCloseAndSecondTransactionOpen()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 20;
            transactionData1.Close = 18;
            transactionData1.High = 20.5;
            transactionData1.Low = 17.8;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 17.95;
            transactionData2.Close = 18;
            transactionData2.High = 18.1;
            transactionData2.Low = 16.5;
            list.Add(transactionData2);
            TransactionData transactionData3 = new TransactionData();
            transactionData3.Open = 17.8;
            transactionData3.Close = 19.2;
            transactionData3.High = 19.5;
            transactionData3.Low = 17.5;
            list.Add(transactionData3);
            BullishAbandonedBaby pattern = new BullishAbandonedBaby();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void BullingAbandonedBabyPatternRequireGapBetweenSecondTransactionCloseAndLastTransactionOpen()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 20;
            transactionData1.Close = 18;
            transactionData1.High = 20.5;
            transactionData1.Low = 17.8;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 17.05;
            transactionData2.Close = 17.1;
            transactionData2.High = 18.1;
            transactionData2.Low = 16.5;
            list.Add(transactionData2);
            TransactionData transactionData3 = new TransactionData();
            transactionData3.Open = 17.1;
            transactionData3.Close = 19.2;
            transactionData3.High = 19.5;
            transactionData3.Low = 17.5;
            list.Add(transactionData3);
            BullishAbandonedBaby pattern = new BullishAbandonedBaby();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }
    }
}