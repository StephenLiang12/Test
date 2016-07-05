using System.Collections.Generic;
using Market.Analyzer;
using Market.Analyzer.CandleStickPatterns;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture.Analyzer.CandleStickPatterns
{
    [TestClass]
    public class BullishCounterAttackTestFixture
    {
        [TestMethod]
        public void BullishCounterAttackNeedAtLeastTwoTransaction()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData = new TransactionData();
            list.Add(transactionData);
            BullishCounterAttack pattern = new BullishCounterAttack();
            Assert.IsFalse(pattern.Qualified(list, Trend.Up));
        }

        [TestMethod]
        public void AbleToFindBullishCounterAttackPattern()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 20;
            transactionData1.Close = 18;
            transactionData1.High = 20.5;
            transactionData1.Low = 17.8;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 16.5;
            transactionData2.Close = 18.05;
            transactionData2.High = 18.1;
            transactionData2.Low = 15.8;
            list.Add(transactionData2);
            BullishCounterAttack pattern = new BullishCounterAttack();
            Assert.IsTrue(pattern.Qualified(list, Trend.Vibration));
            Assert.AreEqual(Trend.Vibration, pattern.CurrentTrend);
            Assert.AreEqual(Trend.Up, pattern.UpcomingTrend);
        }

        [TestMethod]
        public void BullishCounterAttackPatternRequireLastTransactionIsBullish()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 20;
            transactionData1.Close = 18;
            transactionData1.High = 20.5;
            transactionData1.Low = 17.8;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 18.05;
            transactionData2.Close = 16.5;
            transactionData2.High = 18.1;
            transactionData2.Low = 15.8;
            list.Add(transactionData2);
            BullishCounterAttack pattern = new BullishCounterAttack();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void BullishCounterAttackPatternRequireLastSecondTransactionIsBearish()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 18;
            transactionData1.Close = 20;
            transactionData1.High = 20.5;
            transactionData1.Low = 17.8;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 16.5;
            transactionData2.Close = 18.05;
            transactionData2.High = 18.1;
            transactionData2.Low = 15.8;
            list.Add(transactionData2);
            BullishCounterAttack pattern = new BullishCounterAttack();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void BullishCounterAttackPatternRequireChangeOfLastTwoTransactionIsGreaterThan3PercentOfClose()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 20;
            transactionData1.Close = 19.5;
            transactionData1.High = 20.5;
            transactionData1.Low = 17.8;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 16.5;
            transactionData2.Close = 18.05;
            transactionData2.High = 18.1;
            transactionData2.Low = 15.8;
            list.Add(transactionData2);
            BullishCounterAttack pattern = new BullishCounterAttack();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }

        [TestMethod]
        public void BullishCounterAttackPatternRequireCloseOfLastTransactionIsNearCloseOfLastSecondTransaction()
        {
            IList<TransactionData> list = new List<TransactionData>();
            TransactionData transactionData1 = new TransactionData();
            transactionData1.Open = 20;
            transactionData1.Close = 18;
            transactionData1.High = 20.5;
            transactionData1.Low = 17.8;
            list.Add(transactionData1);
            TransactionData transactionData2 = new TransactionData();
            transactionData2.Open = 16.5;
            transactionData2.Close = 18.2;
            transactionData2.High = 18.1;
            transactionData2.Low = 15.8;
            list.Add(transactionData2);
            BullishCounterAttack pattern = new BullishCounterAttack();
            Assert.IsFalse(pattern.Qualified(list, Trend.Vibration));
        }
    }
}