using System.Collections.Generic;
using Market.Analyzer;
using Market.Analyzer.CandleStickPatterns;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture.Analyzer.CandleStickPatterns
{
    [TestClass]
    public class BearishCounterAttackTestFixture
    {
        [TestMethod]
        public void BearishCounterAttackNeedAtLeastTwoTransaction()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            list.Add(transactionData);
            BearishCounterAttack pattern = new BearishCounterAttack();
            Assert.IsFalse(pattern.Qualified(list, Trend.Up));
        }

        [TestMethod]
        public void AbleToFindBearishCounterAttackPattern()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 18;
            transactionData1.Close = 20;
            transactionData1.High = 20.5;
            transactionData1.Low = 17.8;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 22.5;
            transactionData2.Close = 20.1;
            transactionData2.High = 23.1;
            transactionData2.Low = 19.8;
            list.Add(transactionData2);
            BearishCounterAttack pattern = new BearishCounterAttack();
            Assert.IsTrue(pattern.Qualified(list, Trend.Vibration));
            Assert.AreEqual(Trend.Vibration, pattern.CurrentTrend);
            Assert.AreEqual(Trend.Down, pattern.UpcomingTrend);
        }

        [TestMethod]
        public void BearishCounterAttackPatternRequireLastTransactionIsBearish()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 18;
            transactionData1.Close = 20;
            transactionData1.High = 20.5;
            transactionData1.Low = 17.8;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 20.1;
            transactionData2.Close = 22.5;
            transactionData2.High = 23.1;
            transactionData2.Low = 19.8;
            list.Add(transactionData2);
            BearishCounterAttack pattern = new BearishCounterAttack();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void BearishCounterAttackPatternRequireLastSecondTransactionIsBullish()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 20;
            transactionData1.Close = 18;
            transactionData1.High = 20.5;
            transactionData1.Low = 17.8;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 22.5;
            transactionData2.Close = 20.1;
            transactionData2.High = 23.1;
            transactionData2.Low = 19.8;
            list.Add(transactionData2);
            BearishCounterAttack pattern = new BearishCounterAttack();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void BearishCounterAttackPatternRequireChangeOfLastTwoTransactionIsGreaterThan3PercentOfClose()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 19.6;
            transactionData1.Close = 20;
            transactionData1.High = 20.5;
            transactionData1.Low = 17.8;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 22.5;
            transactionData2.Close = 20.1;
            transactionData2.High = 23.1;
            transactionData2.Low = 19.8;
            list.Add(transactionData2);
            BearishCounterAttack pattern = new BearishCounterAttack();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void BearishCounterAttackPatternRequireCloseOfLastTransactionIsNearCloseOfLastSecondTransaction()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 18;
            transactionData1.Close = 20;
            transactionData1.High = 20.5;
            transactionData1.Low = 17.8;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 22.5;
            transactionData2.Close = 20.3;
            transactionData2.High = 23.1;
            transactionData2.Low = 19.8;
            list.Add(transactionData2);
            BearishCounterAttack pattern = new BearishCounterAttack();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }
    }
}