using System;
using System.Collections.Generic;

namespace Market.Analyzer
{
    public class MovingAverageConvergenceDivergenceAnalyzer
    {
        public MovingAverageConvergenceDivergenceResult AnalyzeSignalLineCrossOver(IList<TransactionData> orderedList, MovingAverage average1, MovingAverage average2, int signalLineTransactions)
        {
            double weight1 = 2d/(average1.NumberOfTransactions + 1);
            double weight2 = 2d/(average2.NumberOfTransactions + 1);
            double weight3 = 2d/(signalLineTransactions + 1);
            double[] macd = new double[average1.Averages.Length];
            double[] ema1 = new double[average1.Averages.Length];
            double[] ema2 = new double[average1.Averages.Length];
            double[] signalLine = new double[average1.Averages.Length];
            ema1[0] = average1.Averages[0];
            ema2[0] = average2.Averages[0];
            macd[0] = ema1[0] - ema2[0];
            signalLine[0] = macd[0];
            for (int i = 1; i < macd.Length; i++)
            {
                ema1[i] = (orderedList[i].Close - ema1[i - 1])*weight1 + ema1[i - 1];
                ema2[i] = (orderedList[i].Close - ema2[i - 1])*weight2 + ema2[i - 1];
                macd[i] = ema1[i] - ema2[i];
                signalLine[i] = (macd[i] - macd[i - 1])*weight3 + macd[i - 1];
            }
            double lastMacd = macd[macd.Length - 1];
            double lastSignalLine = signalLine[signalLine.Length - 1];
            //Console.WriteLine("MACD - {0}, Signal Line - {1}", lastMacd, lastSignalLine);
            int numberTransactionsAfterCrossOver = 0;
            bool upCrossOver = false;
            if (lastMacd > lastSignalLine)
            {
                upCrossOver = true;
                numberTransactionsAfterCrossOver = 1;
            }
            else
                numberTransactionsAfterCrossOver = -1;
            int breakPoint = 0;
            for (int i = 2; i <= signalLineTransactions; i++)
            {
                if (macd[macd.Length - i] > 0 && signalLine[signalLine.Length - i] > 0)
                    breakPoint = 1;
                else if (macd[macd.Length - i] < 0 && signalLine[signalLine.Length - i] < 0)
                    breakPoint = -1;
                if (upCrossOver)
                {
                    if (macd[macd.Length - i] >= signalLine[macd.Length - i])
                        numberTransactionsAfterCrossOver++;
                    else
                        break;
                }
                else
                {
                    if (macd[macd.Length - i] <= signalLine[macd.Length - i])
                        numberTransactionsAfterCrossOver--;
                    else
                        break;
                }
            }
            MovingAverageConvergenceDivergenceResult result = new MovingAverageConvergenceDivergenceResult();
            result.Convergence = breakPoint;
            result.Divergence = ((double)numberTransactionsAfterCrossOver)/signalLineTransactions;
            return result;
        }
    }
}