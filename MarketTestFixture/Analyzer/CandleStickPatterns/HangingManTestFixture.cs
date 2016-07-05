using System.Collections.Generic;
using Market.Analyzer;
using Market.Analyzer.CandleStickPatterns;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture.Analyzer.CandleStickPatterns
{
    [TestClass]
    public class HangingManTestFixture
    {
        [TestMethod]
        public void AbleToFindHangingMan()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = 0.9;
            transactionData.High = 1;
            transactionData.Close = 1;
            transactionData.Low = 0.5;
            list.Add(transactionData);
            HangingMan pattern = new HangingMan();
            Assert.IsTrue(pattern.Qualified(list, Trend.Up));
            Assert.AreEqual(Trend.Up, pattern.CurrentTrend);
            Assert.AreEqual(Trend.Down, pattern.UpcomingTrend);
        }

        [TestMethod]
        public void HangingManOnlyExistsInUpTrend()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = 0.9;
            transactionData.High = 1;
            transactionData.Close = 1;
            transactionData.Low = 0.5;
            list.Add(transactionData);
            HangingMan pattern = new HangingMan();
            Assert.IsFalse(pattern.Qualified(list, Trend.Down));
        }

        [TestMethod]
        public void IfCloseIsLessThanOpenItIsNotHangingMan()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = 1.2;
            transactionData.High = 1.2;
            transactionData.Close = 1;
            transactionData.Low = 0.5;
            list.Add(transactionData);
            HangingMan pattern = new HangingMan();
            Assert.IsFalse(pattern.Qualified(list, Trend.Up));
        }

        [TestMethod]
        public void IfTransactionRangeIsLessThan3PercentOfCloseItIsNotHangingMan()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = 0.9;
            transactionData.High = 1;
            transactionData.Close = 1;
            transactionData.Low = 0.9;
            list.Add(transactionData);
            HangingMan pattern = new HangingMan();
            Assert.IsFalse(pattern.Qualified(list, Trend.Up));
        }

        [TestMethod]
        public void IfCloseIsNotEqualToHighItIsNotHangingMan()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = 0.9;
            transactionData.High = 1;
            transactionData.Close = 0.95;
            transactionData.Low = 0.5;
            list.Add(transactionData);
            HangingMan pattern = new HangingMan();
            Assert.IsFalse(pattern.Qualified(list, Trend.Down));
        }

        [TestMethod]
        public void IfRangeIsLessThan2TimesOfChangeItIsNotHangingMan()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = 0.6;
            transactionData.High = 1;
            transactionData.Close = 1;
            transactionData.Low = 0.5;
            list.Add(transactionData);
            HangingMan pattern = new HangingMan();
            Assert.IsFalse(pattern.Qualified(list, Trend.Up));
        }
    }
}