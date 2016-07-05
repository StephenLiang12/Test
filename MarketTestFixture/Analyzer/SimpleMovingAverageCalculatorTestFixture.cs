using System;
using Market.Analyzer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture.Analyzer
{
    [TestClass]
    public class SimpleMovingAverageCalculatorTestFixture
    {
        [TestMethod]
        public void AbleToCalculateSimpleMovingAverage()
        {
            int length = 40;
            TransactionData[] transactionData = new TransactionData[length];
            for (int i = 0; i < length; i++)
            {
                transactionData[i] = new TransactionData();
                transactionData[i].Close = i + 1;
            }
            SimpleMovingAverageCalculator calculator = new SimpleMovingAverageCalculator();
            var average = calculator.CalculateAverage(transactionData, 10);
            Assert.AreEqual(10, average.NumberOfTransactions);
            for (int i = 0; i < average.Averages.Length; i++)
            {
                Console.WriteLine("Close {0} - Average {1}", transactionData[i].Close, average.Averages[i]);
                if (i < 10 - 1)
                    Assert.AreEqual(0, average.Averages[i]);
                else
                    Assert.AreEqual(5.5 + i - 9, average.Averages[i], string.Format("Average Number {0} is wrong", i + 1));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AbleToThrowExceptionIfThereIsNotEnoughTrasactionData()
        {
            int length = 40;
            TransactionData[] transactionData = new TransactionData[length];
            for (int i = 0; i < length; i++)
            {
                transactionData[i] = new TransactionData();
                transactionData[i].Close = i + 1;
            }
            SimpleMovingAverageCalculator calculator = new SimpleMovingAverageCalculator();
            calculator.CalculateAverage(transactionData, 100);
        }

        [TestMethod]
        public void AbleToCalculateSimpleMovingAverageForNewTransactions()
        {
            int length = 40;
            TransactionData[] transactionData = new TransactionData[length];
            for (int i = 0; i < length; i++)
            {
                transactionData[i] = new TransactionData();
                transactionData[i].Close = i + 1;
            }
            SimpleMovingAverageCalculator calculator = new SimpleMovingAverageCalculator();
            var average = calculator.CalculateAverage(transactionData, 10);
            int newTransactions = 10;
            TransactionData[] newTransactionData = new TransactionData[length + newTransactions];
            for (int i = 0; i < length; i++)
            {
                newTransactionData[i] = transactionData[i];
            }
            for (int i = 0; i < newTransactions; i++)
            {
                newTransactionData[length + i] = new TransactionData();
                newTransactionData[length + i].Close = length + i + 1;
            }
            MovingAverage newAverage = new MovingAverage();
            newAverage.NumberOfTransactions = average.NumberOfTransactions;
            newAverage.Averages = new double[length + newTransactions];
            for (int i = 0; i < length; i++)
            {
                newAverage.Averages[i] = average.Averages[i];
            }
            calculator.CalculateAverage(newTransactionData, newAverage, newTransactions);
            for (int i = 0; i < newAverage.Averages.Length; i++)
            {
                Console.WriteLine("Close {0} - Average {1}", newTransactionData[i].Close, newAverage.Averages[i]);
                if (i < 10 - 1)
                    Assert.AreEqual(0, newAverage.Averages[i]);
                else
                    Assert.AreEqual(5.5 + i - 9, newAverage.Averages[i], string.Format("Average Number {0} is wrong", i + 1));
            }
        }

    }
}