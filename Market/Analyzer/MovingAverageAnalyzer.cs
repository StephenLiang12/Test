using System;
using System.Collections.Generic;


namespace Market.Analyzer
{
    public class MovingAverageAnalyzer
    {
        private const double Threshold = 0.1;

        public double PriceCompareAverage(IList<TransactionData> orderedTransactions, MovingAverage average)
        {
            int[] compare = new int[orderedTransactions.Count];
            int count = orderedTransactions.Count;
            if (count < average.NumberOfTransactions*2)
                throw new ArgumentException(
                    "Number of Transacions is less than the twice of average number of transactions. It will cause inaccurate comparison.");
            for (int i = 0; i < compare.Length; i++)
            {
                TransactionData transaction = orderedTransactions[i];
                if (transaction.High < average.Averages[i])
                    compare[i] = -1;
                else if (transaction.Low > average.Averages[i])
                    compare[i] = 1;
                else
                    compare[i] = 0;
            }
            bool upCrossOver = false;
            int numberTransactionsAfterCrossOver = 0;
            if (compare[compare.Length - 1] > 0)
            {
                upCrossOver = true;
                numberTransactionsAfterCrossOver = 1;
            }
            else if (compare[compare.Length - 1] < 0)
            {
                upCrossOver = false;
                numberTransactionsAfterCrossOver = -1;
            }
            else
                return 0;
            for (int i = compare.Length - 2; i >= 0; i--)
            {
                if (upCrossOver)
                {
                    if (compare[i] >= 0)
                        numberTransactionsAfterCrossOver++;
                    else
                        break;
                }
                else
                {
                    if (compare[i] <= 0)
                        numberTransactionsAfterCrossOver--;
                    else
                        break;
                }
            }
            return ((double) numberTransactionsAfterCrossOver)/average.NumberOfTransactions;
        }

        public double AverageCrossOver(MovingAverage average1, MovingAverage average2)
        {
            int numberTransactionsAfterCrossOver = 0;
            int count = average1.Averages.Length;
            bool upCrossOver = false;
            if (average2.Averages[count - 1] < average1.Averages[count - 1])
            {
                upCrossOver = true;
                numberTransactionsAfterCrossOver = 1;
            }
            else
                numberTransactionsAfterCrossOver = -1;
            for (int i = 2; i < average1.NumberOfTransactions; i++)
            {
                if (upCrossOver)
                {
                    if (average2.Averages[count - i] < average1.Averages[count - i])
                        numberTransactionsAfterCrossOver++;
                    else
                        break;
                }
                else
                {
                    if (average2.Averages[count - i - 1] > average1.Averages[count - i - 1])
                        numberTransactionsAfterCrossOver--;
                    else
                        break;
                }
            }
            return ((double)numberTransactionsAfterCrossOver) / average1.NumberOfTransactions;
        }

        public Trend AnalyzeMovingTrend(MovingAverage avg)
        {
            double[] data = new double[avg.NumberOfTransactions];
            double open, close, high, low;
            int startIndex = avg.Averages.Length - avg.NumberOfTransactions;
            high = 0;
            open = low = avg.Averages[startIndex];
            close = avg.Averages[avg.Averages.Length - 1];
            for (int i = startIndex; i < avg.Averages.Length; i++)
            {
                double d = avg.Averages[i];
                data[i - startIndex] = d;
                if (d > high)
                {
                    high = d;
                }
                if (d < low)
                {
                    low = d;
                }
            }
            double closeRatio = (close - low) / (high - low);
            double openRatio = (open - low) / (high - low);
            if ((closeRatio > 0.9) && (openRatio < 0.25))
                return Trend.Up;
            if ((closeRatio < 0.25) && (openRatio > 0.9))
                return Trend.Down;
            if ((closeRatio > 0.75) && (openRatio > 0.75))
                return Trend.Bottom;
            if ((closeRatio < 0.25) && (openRatio < 0.25))
                return Trend.Top;
            return Trend.Vibration;
        }

        public Trend AnalyzeMovingTrend(double[] data)
        {
            int crossLine = 0;
            int dotAboveLine = 0;
            int dotBelowLine = 0;
            int dotAroundLine = 0;
            double variancePercent = 0.001*data.Length;
            double sumAboveLine = 0;
            double sumBelowLine = 0;
            int highIndex, lowIndex;
            double open, high, low;
            high = low = open = data[0];
            highIndex = lowIndex = 0;
            double close = data[data.Length - 1];
            double ratio = (close - open)/(data.Length - 1);
            double changePercent = (close - open)/close;
            double line = open;
            int sign = 0;
            for (int i = 1; i < data.Length; i++)
            {
                line += ratio;
                if (data[i] > high)
                {
                    high = data[i];
                    highIndex = i;
                }
                if (data[i] < low)
                {
                    low = data[i];
                    lowIndex = i;
                }
                if (Math.Abs(data[i] - line) > line*changePercent*Threshold)
                {
                    if (data[i] > line)
                    {
                        dotAboveLine++;
                        sumAboveLine += data[i] - line;
                        if (sign == -1)
                            crossLine++;
                        sign = 1;
                    }
                    else
                    {
                        dotBelowLine++;
                        sumBelowLine += data[i] - line;
                        if (sign == 1)
                            crossLine++;
                        sign = -1;
                    }
                }
                else
                    dotAroundLine++;
            }
            double percentAboveLine = (double)dotAboveLine/data.Length;
            double percentBelowLine = (double)dotBelowLine/data.Length;
            double percentAroundLine = (double)dotAroundLine/data.Length;
            double percentCrossLine = (double) crossLine/data.Length;
            var hasTop = HasTop(open, high, highIndex, variancePercent, ratio, percentCrossLine);
            var hasBottom = HasBottom(open, low, lowIndex, variancePercent, ratio, percentCrossLine);
            if (Math.Abs(changePercent) > variancePercent)
            {
                if (percentCrossLine > Threshold)
                {
                    if (changePercent > 0)
                        return Trend.VibrationUp;
                    return Trend.VibrationDown;
                }
                if (hasTop)
                {
                    if (hasBottom)
                    {
                        if (changePercent > 0)
                        {
                            if (highIndex < lowIndex)
                                return Trend.TopBottomUp;
                            return Trend.BottomTopUp;
                        }
                        if (highIndex < lowIndex)
                            return Trend.TopBottomDown;
                        return Trend.BottomTopDown;
                    }
                    if (changePercent > 0)
                        return Trend.TopUp;
                    return Trend.TopDown;
                }
                if (hasBottom)
                {
                    if (changePercent > 0)
                        return Trend.BottomUp;
                    return Trend.BottomDown;
                }
                if (changePercent > 0)
                    return Trend.Up;
                return Trend.Down;
            }
            if (percentAroundLine > 0.8)
                return Trend.Vibration;
            if (percentAboveLine > 0.5 && hasTop && !hasBottom)
                return Trend.Top;
            if (percentBelowLine > 0.5 && hasBottom && !hasTop)
                return Trend.Bottom;
            if (percentCrossLine > Threshold)
                return Trend.Vibration;
            return Trend.Unknown;
        }

        private bool HasTop(double open, double high, int highIndex, double variancePercent, double changeRatio, double crossLinePercentage)
        {
            double highLine = open + highIndex*changeRatio;
            double highVsLine = (high - highLine)/highLine;
            return crossLinePercentage < variancePercent && highVsLine > variancePercent ;
        }

        private bool HasBottom(double open, double low, int lowIndex, double variancePercent, double changeRatio, double crossLinePercentage)
        {
            double lowLine = open + lowIndex * changeRatio;
            double lowVsLine = (lowLine - low) / lowLine;
            return crossLinePercentage < variancePercent && lowVsLine > variancePercent ;
        }
    }
}