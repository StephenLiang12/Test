using System;
using System.Collections.Generic;

namespace Market.Analyzer
{
    public class ExponentMovingAverageCalculator
    {
        private readonly SimpleMovingAverageCalculator simpleMovingAverageCalculator = new SimpleMovingAverageCalculator();

        public MovingAverage CalculateAverage(double[] data, int numberOfTransactions)
        {
            var average = simpleMovingAverageCalculator.CalculateAverage(data, numberOfTransactions);
            var multiplier = 2d/(numberOfTransactions + 1);
            for (int i = numberOfTransactions; i < data.Length; i++)
            {
                average.Averages[i] = (data[i] - average.Averages[i-1]) * multiplier + average.Averages[i - 1];
            }
            return average;
        }

        public MovingAverage CalculateAverage(IList<TransactionData> orderedTransactions, int numberOfTransactions)
        {
            var average = simpleMovingAverageCalculator.CalculateAverage(orderedTransactions, numberOfTransactions);
            var multiplier = 2d/(numberOfTransactions + 1);
            for (int i = numberOfTransactions; i < orderedTransactions.Count; i++)
            {
                average.Averages[i] = (orderedTransactions[i].Close - average.Averages[i-1]) * multiplier + average.Averages[i - 1];
            }
            return average;
        }

        public void CalculateAverage(IList<TransactionData> orderedTransactions, MovingAverage exponentMovingAverage, int newTransactions)
        {
            int numberOfTransactions = exponentMovingAverage.NumberOfTransactions;
            var multiplier = 2d / (numberOfTransactions + 1);
            int count = orderedTransactions.Count;
            int startIndex = count - newTransactions;
            for (int i = startIndex; i < count; i++)
            {
                exponentMovingAverage.Averages[i] = (orderedTransactions[i].Close - exponentMovingAverage.Averages[i - 1]) * multiplier + exponentMovingAverage.Averages[i - 1];
            }
        }
    }
}