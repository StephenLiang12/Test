using System.Collections.Generic;
using Market.Analyzer;
using Market.Analyzer.CandleStickPatterns;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture.Analyzer.CandleStickPatterns
{
    [TestClass]
    public class HammerTestFixture
    {
        [TestMethod]
        public void AbleToFindHammer()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = transactionData.High = 1;
            transactionData.Close = 0.9;
            transactionData.Low = 0.5;
            list.Add(transactionData);
            Hammer pattern = new Hammer();
            Assert.IsTrue(pattern.Qualified(list, Trend.Down));
            Assert.AreEqual(Trend.Down, pattern.CurrentTrend);
            Assert.AreEqual(Trend.Up, pattern.UpcomingTrend);
        }

        [TestMethod]
        public void HammerOnlyExistsInDownTrend()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = transactionData.High = 1;
            transactionData.Close = 1;
            transactionData.Low = 0.5;
            list.Add(transactionData);
            Hammer pattern = new Hammer();
            Assert.IsFalse(pattern.Qualified(list, Trend.Up));
        }

        [TestMethod]
        public void IfOpenIsLessThanCloseItIsNotHammer()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = 0.8;
            transactionData.High = 1.2;
            transactionData.Close = 1;
            transactionData.Low = 0.5;
            list.Add(transactionData);
            Hammer pattern = new Hammer();
            Assert.IsFalse(pattern.Qualified(list, Trend.Down));
        }

        [TestMethod]
        public void IfTransactionRangeIsLessThan3PercentOfCloseItIsNotHammer()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = transactionData.High = 1;
            transactionData.Close = 1;
            transactionData.Low = 0.9;
            list.Add(transactionData);
            Hammer pattern = new Hammer();
            Assert.IsFalse(pattern.Qualified(list, Trend.Down));
        }

        [TestMethod]
        public void IfOpenIsNotEqualToHighItIsNotHammer()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = 0.95;
            transactionData.High = 1;
            transactionData.Close = 1;
            transactionData.Low = 0.5;
            list.Add(transactionData);
            Hammer pattern = new Hammer();
            Assert.IsFalse(pattern.Qualified(list, Trend.Down));
        }

        [TestMethod]
        public void IfRangeIsLessThan2TimesOfChangeItIsNotHammer()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            transactionData.Open = 1;
            transactionData.High = 1;
            transactionData.Close = 0.6;
            transactionData.Low = 0.5;
            list.Add(transactionData);
            Hammer pattern = new Hammer();
            Assert.IsFalse(pattern.Qualified(list, Trend.Down));
        }
    }
}