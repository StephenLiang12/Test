using System.Collections.Generic;
using Market.Analyzer;
using Market.Analyzer.CandleStickPatterns;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture.Analyzer.CandleStickPatterns
{
    [TestClass]
    public class BearishHaramiCrossTestFixture
    {
        [TestMethod]
        public void BearishHaramiCrossNeedAtLeastTwoTransactions()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            list.Add(transactionData1);
            BearishHaramiCross pattern = new BearishHaramiCross();
            Assert.IsFalse(pattern.Qualified(list, Trend.Up));
        }

        [TestMethod]
        public void AbleToFindBearishHaramiCrossPattern()
        {
            var list = GetTransactionData();
            BearishHaramiCross pattern = new BearishHaramiCross();
            Assert.IsTrue(pattern.Qualified(list, Trend.Up));
            Assert.AreEqual(Trend.Up, pattern.CurrentTrend);
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
            transactionData2.Open = 19.05;
            transactionData2.Close = 19.1;
            transactionData2.High = 20.1;
            transactionData2.Low = 18.5;
            list.Add(transactionData2);
            return list;
        }

        [TestMethod]
        public void BearishHaramiCrossPatternRequireFirstTransactionIsBearish()
        {
            IList<TransactionData> list = GetTransactionData();
            var swap = list[0].Open;
            list[0].Open = list[0].Close;
            list[0].Close = swap;
            BearishHaramiCross pattern = new BearishHaramiCross();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void BearishHaramiCrossPatternRequireLastTransactionIsDoji()
        {
            IList<TransactionData> list = GetTransactionData();
            list[1].Open -= 0.1;
            list[1].Close += 0.1;
            BearishHaramiCross pattern = new BearishHaramiCross();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void BearishHaramiCrossPatternRequireFirstTransactionHasMiddleChange()
        {
            IList<TransactionData> list = GetTransactionData();
            list[0].Open = list[0].Close - 0.5;
            BearishHaramiCross pattern = new BearishHaramiCross();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void BearishHaramiCrossPatternRequireLastTransactionCloseIsInTheMiddleOfFirstTransaction()
        {
            IList<TransactionData> list = GetTransactionData();
            list[1].Open -= 1.5;
            list[1].Close -= 1.5;
            list[1].High -= 1.5;
            list[1].Low -= 1.5;
            BearishHaramiCross pattern = new BearishHaramiCross();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }
    }
}