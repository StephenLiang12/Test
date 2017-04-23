using System;
using System.Collections.Generic;

namespace Market.Analyzer.MACD
{
    public class MovingAverageConvergenceDivergenceCalculator
    {
        private ExponentMovingAverageCalculator exponentMovingAverageCalculator = new ExponentMovingAverageCalculator();

        public MovingAverageConvergenceDivergence[] Calculate(double[] ema12, double[] ema26, int averageNumberOfSignalLine)
        {
            if (ema12.Length != ema26.Length)
                throw new ArgumentException("Length of EMA12 and EMA26 has to be the same");
            MovingAverageConvergenceDivergence[] result = new MovingAverageConvergenceDivergence[ema12.Length];
            double[] macds = new double[ema12.Length];
            for (int i = 0; i < ema12.Length; i++)
            {
                result[i] = new MovingAverageConvergenceDivergence();
                result[i].MACD = macds[i] = ema12[i] - ema26[i];
            }
            var signals = exponentMovingAverageCalculator.CalculateAverage(macds, averageNumberOfSignalLine);
            for (int i = 0; i < ema12.Length; i++)
            {
                result[i].Signal = signals.Averages[i];
                result[i].Histogram = result[i].MACD - result[i].Signal;
            }
            return result;
        }

        public MovingAverageConvergenceDivergence[] Calculate(IList<TransactionData> orderedTransactionData, int averageNumberOfEMA12, int averageNumberOfEMA26, int averageNumberOfSignalLine)
        {
            var ema12 = exponentMovingAverageCalculator.CalculateAverage(orderedTransactionData, averageNumberOfEMA12);
            var ema26 = exponentMovingAverageCalculator.CalculateAverage(orderedTransactionData, averageNumberOfEMA26);
            var result = Calculate(ema12.Averages, ema26.Averages, averageNumberOfSignalLine);
            for (int i = 0; i < result.Length; i++)
            {
                result[i].StockKey = orderedTransactionData[i].StockKey;
                result[i].TimeStamp = orderedTransactionData[i].TimeStamp;
            }
            return result;
        }
    }
}