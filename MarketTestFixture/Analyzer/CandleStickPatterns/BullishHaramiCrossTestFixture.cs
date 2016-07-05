using System.Collections.Generic;
using Market.Analyzer;
using Market.Analyzer.CandleStickPatterns;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture.Analyzer.CandleStickPatterns
{
    [TestClass]
    public class BullishHaramiCrossTestFixture
    {
        [TestMethod]
        public void BullishHaramiCrossNeedAtLeastTwoTransactions()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            list.Add(transactionData1);
            BullishHaramiCross pattern = new BullishHaramiCross();
            Assert.IsFalse(pattern.Qualified(list, Trend.Up));
        }

        [TestMethod]
        public void AbleToFindBullishHaramiCrossPattern()
        {
            var list = GetTransactionData();
            BullishHaramiCross pattern = new BullishHaramiCross();
            Assert.IsTrue(pattern.Qualified(list, Trend.Down));
            Assert.AreEqual(Trend.Down, pattern.CurrentTrend);
            Assert.AreEqual(Trend.Up, pattern.UpcomingTrend);
        }

        private static IList<TransactionData> GetTransactionData()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 20;
            transactionData1.Close = 18;
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
        public void BullishHaramiCrossPatternRequireFirstTransactionIsBearish()
        {
            IList<TransactionData> list = GetTransactionData();
            var swap = list[0].Open;
            list[0].Open = list[0].Close;
            list[0].Close = swap;
            BullishHaramiCross pattern = new BullishHaramiCross();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void BullishHaramiCrossPatternRequireLastTransactionIsDoji()
        {
            IList<TransactionData> list = GetTransactionData();
            list[1].Open -= 0.1;
            list[1].Close += 0.1;
            BullishHaramiCross pattern = new BullishHaramiCross();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void BullishHaramiCrossPatternRequireFirstTransactionHasMiddleChange()
        {
            IList<TransactionData> list = GetTransactionData();
            list[0].Open = list[0].Close + 0.5;
            BullishHaramiCross pattern = new BullishHaramiCross();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void BullishHaramiCrossPatternRequireLastTransactionCloseIsInTheMiddleOfFirstTransaction()
        {
            IList<TransactionData> list = GetTransactionData();
            list[1].Open -= 1.5;
            list[1].Close -= 1.5;
            list[1].High -= 1.5;
            list[1].Low -= 1.5;
            BullishHaramiCross pattern = new BullishHaramiCross();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }
    }
}