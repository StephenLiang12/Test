using System;
using System.Collections.Generic;
using System.Linq;
using Market.Analyzer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture.Analyzer
{
    [TestClass]
    public class ExponentMovingAverageCalculatorTestFixture
    {
        [TestMethod]
        public void AbleToCalculateExponentMovingAverage()
        {
            int length = 20;
            TransactionData[] transactionData = new TransactionData[length];
            double[] closes = new[] {22.27, 22.19, 22.08, 22.17, 22.18, 22.13, 22.23, 22.43, 22.24, 22.29, 22.15, 22.39, 22.38, 22.61, 23.36, 24.05, 23.75, 23.83, 23.95, 23.63};
            double[] results = new[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 22.22, 22.21, 22.24, 22.27, 22.33, 22.52, 22.80, 22.97, 23.13, 23.28, 23.34};
            for (int i = 0; i < length; i++)
            {
                transactionData[i] = new TransactionData();
                transactionData[i].Close = closes[i];
            }
            ExponentMovingAverageCalculator calculator = new ExponentMovingAverageCalculator();
            var average = calculator.CalculateAverage(transactionData, 10);
            Assert.AreEqual(10, average.NumberOfTransactions);
            for (int i = 0; i < average.Averages.Length; i++)
            {
                Console.WriteLine("Close {0} - Average {1}", transactionData[i].Close, average.Averages[i]);
                Assert.AreEqual(results[i], average.Averages[i], 0.005, string.Format("Average Number {0} is wrong", i + 1));
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
        public void AbleToCalculateExponentMovingAverageForNewTransactions()
        {
            int length = 20;
            TransactionData[] transactionData = new TransactionData[length];
            double[] closes = new[] { 22.27, 22.19, 22.08, 22.17, 22.18, 22.13, 22.23, 22.43, 22.24, 22.29, 22.15, 22.39, 22.38, 22.61, 23.36, 24.05, 23.75, 23.83, 23.95, 23.63 };
            double[] results = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 22.22, 22.21, 22.24, 22.27, 22.33, 22.52, 22.80, 22.97, 23.13, 23.28, 23.34 };
            for (int i = 0; i < length; i++)
            {
                transactionData[i] = new TransactionData();
                transactionData[i].Close = closes[i];
            }
            ExponentMovingAverageCalculator calculator = new ExponentMovingAverageCalculator();
            var average = calculator.CalculateAverage(transactionData, 10);
            int newTransactions = 10;
            double[] newCloses = new[] { 23.82, 23.87, 23.65, 23.19, 23.10, 23.33, 22.68, 23.10, 22.40, 22.17 };
            double[] newResults = new[] { 23.43, 23.51, 23.53, 23.47,23.40, 23.39, 23.26, 23.23, 23.08, 22.92 };
            TransactionData[] newTransactionData = new TransactionData[length + newTransactions];
            for (int i = 0; i < length; i++)
            {
                newTransactionData[i] = transactionData[i];
            }
            for (int i = 0; i < newTransactions; i++)
            {
                newTransactionData[length + i] = new TransactionData();
                newTransactionData[length + i].Close = newCloses[i];
            }
            MovingAverage newAverage = new MovingAverage();
            newAverage.NumberOfTransactions = average.NumberOfTransactions;
            newAverage.Averages = new double[length + newTransactions];
            for (int i = 0; i < length; i++)
            {
                newAverage.Averages[i] = average.Averages[i];
            }
            calculator.CalculateAverage(newTransactionData, newAverage, newTransactions);
            Assert.AreEqual(10, newAverage.NumberOfTransactions);
            for (int i = 0; i < newAverage.Averages.Length; i++)
            {
                Console.WriteLine("Close {0} - Average {1}", newTransactionData[i].Close, newAverage.Averages[i]);
                if (i < length)
                    Assert.AreEqual(results[i], newAverage.Averages[i], 0.005, string.Format("Average Number {0} is wrong", i + 1));
                else
                    Assert.AreEqual(newResults[i - length], newAverage.Averages[i], 0.005, string.Format("Average Number {0} is wrong", i + 1));
            }
        }

        [TestMethod]
        public void AbleToCalculateExponentMovingAverageForStock()
        {
            StockContext context = new StockContext();
            ExponentMovingAverageCalculator calculator = new ExponentMovingAverageCalculator();
            foreach (var stock in context.Stocks.ToList())
            {
                if (stock.Key != 199)
                    continue;
                IList<TransactionData> orderedList =
                    context.TransactionData.Where(t => t.StockKey == stock.Key).OrderBy(t => t.TimeStamp).ToList();
                try
                {
                    var result12 = calculator.CalculateAverage(orderedList, 12);
                    var result26 = calculator.CalculateAverage(orderedList, 26);
                    for (int i = 0; i < result12.Averages.Length; i++)
                    {
                        Console.WriteLine("TimeStamp: {4} - Close: {0} - Average 12: {1} - Average 26: {2} - MACD: {3} ", orderedList[i].Close, result12.Averages[i], result26.Averages[i], result12.Averages[i] - result26.Averages[i], orderedList[i].TimeStamp);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}