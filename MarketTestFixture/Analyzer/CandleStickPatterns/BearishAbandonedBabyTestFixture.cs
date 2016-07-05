using System.Collections.Generic;
using Market.Analyzer;
using Market.Analyzer.CandleStickPatterns;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture.Analyzer.CandleStickPatterns
{
    [TestClass]
    public class BearishAbandonedBabyTestFixture
    {
        [TestMethod]
        public void BearingAbandonedBabyNeedAtLeastThreeTransactions()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            list.Add(transactionData1);
            BearishAbandonedBaby pattern = new BearishAbandonedBaby();
            Assert.IsFalse(pattern.Qualified(list, Trend.Up));
            TransactionData transactionData2 = new TransactionData();
            list.Add(transactionData2);
            Assert.IsFalse(pattern.Qualified(list, Trend.Up));
        }

        [TestMethod]
        public void AbleToFindBearingAbandonedBabyPattern()
        {
            var list = GetTransactionData();
            BearishAbandonedBaby pattern = new BearishAbandonedBaby();
            Assert.IsTrue(pattern.Qualified(list, Trend.Vibration));
            Assert.AreEqual(Trend.Vibration, pattern.CurrentTrend);
            Assert.AreEqual(Trend.Down, pattern.UpcomingTrend);
        }

        private static IList<TransactionData> GetTransactionData()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 18;
            transactionData1.Close = 20;
            transactionData1.High = 20.5;
            transactionData1.Low = 17.8;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 21.05;
            transactionData2.Close = 21.1;
            transactionData2.High = 22.1;
            transactionData2.Low = 20.5;
            list.Add(transactionData2);
            TransactionData transactionData3 = new TransactionData();
            transactionData3.Open = 19.2;
            transactionData3.Close = 18.5;
            transactionData3.High = 19.5;
            transactionData3.Low = 17.5;
            list.Add(transactionData3);
            return list;
        }

        [TestMethod]
        public void BearingAbandonedBabyPatternRequireFirstTransactionIsBullish()
        {
            IList<TransactionData> list = GetTransactionData();
            var swap = list[0].Open;
            list[0].Open = list[0].Close;
            list[0].Close = swap;
            BearishAbandonedBaby pattern = new BearishAbandonedBaby();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void BearingAbandonedBabyPatternRequireSecondTransactionIsDoji()
        {
            IList<TransactionData> list = GetTransactionData();
            list[1].Open -= 0.1;
            list[1].Close += 0.05;
            BearishAbandonedBaby pattern = new BearishAbandonedBaby();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void BearingAbandonedBabyPatternRequireLastTransactionIsBullish()
        {
            IList<TransactionData> list = GetTransactionData();
            var swap = list[2].Open;
            list[2].Open = list[2].Close;
            list[2].Close = swap;
            BearishAbandonedBaby pattern = new BearishAbandonedBaby();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void BearingAbandonedBabyPatternRequireGapBetweenFirstTransactionCloseAndSecondTransactionOpen()
        {
            IList<TransactionData> list = GetTransactionData();
            list[1].Open -= 1;
            list[1].Close -= 1;
            list[1].Low -= 1;
            list[1].High -= 1;
            BearishAbandonedBaby pattern = new BearishAbandonedBaby();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void BearingAbandonedBabyPatternRequireGapBetweenSecondTransactionCloseAndLastTransactionOpen()
        {
            IList<TransactionData> list = GetTransactionData();
            list[2].Open += 2;
            list[2].Close += 2;
            list[2].High += 2;
            list[2].Low += 2;
            BearishAbandonedBaby pattern = new BearishAbandonedBaby();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }
    }
}