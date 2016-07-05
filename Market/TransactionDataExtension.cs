using System;
using System.Collections.Generic;
using Market.Analyzer;

namespace Market
{
    public static class TransactionDataExtension
    {
        public static MovingAverage GetMovingAverage(this IList<TransactionData> transactionData, int numberOfTransaction,
            Func<TransactionData, double> getValue)
        {
            MovingAverage movingAverage = new MovingAverage();
            movingAverage.NumberOfTransactions = numberOfTransaction;
            movingAverage.Averages = new double[transactionData.Count];
            int i = 0;
            foreach (var tran in transactionData)
            {
                movingAverage.Averages[i] = getValue(tran);
                i++;
            }
            return movingAverage;
        }
    }
}