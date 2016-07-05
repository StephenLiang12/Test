using System;
using System.Collections.Generic;
using System.Linq;

namespace Market.Analyzer
{
    public class SimpleMovingAverageCalculator
    {
        public MovingAverage CalculateAverage(IList<TransactionData> orderedTransactions, int numberOfTransactions)
        {
            if (orderedTransactions.Count < numberOfTransactions)
                throw new ArgumentException(string.Format("Not enough transaction data for {0} simple moving average calculation", numberOfTransactions));
            MovingAverage average = new MovingAverage();
            average.Averages = new double[orderedTransactions.Count];
            average.NumberOfTransactions = numberOfTransactions;
            double sum = 0;
            for (int i = 0; i < numberOfTransactions; i++)
            {
                average.Averages[i] = 0;
                sum += orderedTransactions[i].Close;
            }
            average.Averages[numberOfTransactions - 1] = sum/numberOfTransactions;
            for (int i = numberOfTransactions; i < orderedTransactions.Count; i++)
            {
                sum -= orderedTransactions[i - numberOfTransactions].Close;
                sum += orderedTransactions[i].Close;
                average.Averages[i] = sum/numberOfTransactions;
            }
            return average;
        }

        public void CalculateAverage(IList<TransactionData> orderedTransactions, MovingAverage movingAverage, int newTransactions)
        {
            int numberOfTransactions = movingAverage.NumberOfTransactions;
            int count = orderedTransactions.Count;
            double sum = 0;
            int startIndex =  count - (numberOfTransactions + newTransactions) + 1;
            int i = 0;
            while (i < numberOfTransactions)
            {
                sum += orderedTransactions[startIndex + i].Close;
                i++;
            }
            movingAverage.Averages[count - newTransactions] = sum/numberOfTransactions;
            for (int j = 0; j < newTransactions - 1; j++)
            {
                sum -= orderedTransactions[startIndex  + j].Close;
                sum += orderedTransactions[count - newTransactions + j + 1].Close;
                movingAverage.Averages[count - newTransactions + j + 1] = sum / numberOfTransactions;
            }
        }
    }
}