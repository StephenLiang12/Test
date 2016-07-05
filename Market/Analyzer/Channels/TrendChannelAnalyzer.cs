using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Market.Analyzer.Channels
{
    public class TrendChannelAnalyzer
    {
        private readonly MovingAverageAnalyzer movingAverageAnalyzer;

        public TrendChannelAnalyzer()
        {
            movingAverageAnalyzer = new MovingAverageAnalyzer();
        }

        public Channel AnalyzeTrendChannel(IList<TransactionData> orderedTransactions)
        {
            Channel channel = new Channel();
            channel.StartDate = orderedTransactions[0].TimeStamp;
            channel.ResistanceStartPrice = orderedTransactions.Max(t => t.High);
            channel.SupportStartPrice = orderedTransactions.Min(t => t.Low);
            channel.Length = orderedTransactions.Count;
            channel.ChannelTrend =
                movingAverageAnalyzer.AnalyzeMovingTrend(GetMovingAverage(orderedTransactions).Averages);
            if ((int) channel.ChannelTrend > 0)
            {
                channel = BuildSupportLine(channel, orderedTransactions);
                channel = DeduceResistanceLine(channel, orderedTransactions);
            }
            else
            {
                channel = BuildResistanceLine(channel, orderedTransactions);
                channel = DeduceSupportLine(channel, orderedTransactions);
            }
            channel.StockKey = orderedTransactions[0].StockKey;
            channel.StartDate = orderedTransactions[0].TimeStamp;
            channel.EndDate = orderedTransactions[orderedTransactions.Count - 1].TimeStamp;
            return channel;
        }

        public MovingAverage GetMovingAverage(IList<TransactionData> orderedTransacions)
        {
            MovingAverage movingAverage = new MovingAverage();
            int count = orderedTransacions.Count;
            movingAverage.Averages = new double[count];
            if (count <= 20)
            {
                movingAverage.NumberOfTransactions = 0;
                for (int i = 0; i < count; i++)
                {
                    movingAverage.Averages[i] = (orderedTransacions[i].High + orderedTransacions[i].Low)/2;
                }
            }
            else if (count <= 100 && count > 20)
            {
                movingAverage.NumberOfTransactions = 5;
                for (int i = 0; i < count; i++)
                {
                    movingAverage.Averages[i] = orderedTransacions[i].SimpleAvg5;
                }
            }
            else 
            {
                movingAverage.NumberOfTransactions = 20;
                for (int i = 0; i < count; i++)
                {
                    movingAverage.Averages[i] = orderedTransacions[i].SimpleAvg20;
                }
            }
            return movingAverage;
        }

        public Channel BuildResistanceLine(Channel channel, IList<TransactionData> orderedTransactions)
        {
            double highPrice = double.MinValue;
            int highIndex = 0;
            for (int i = 0; i < orderedTransactions.Count; i++)
            {
                if (highPrice < orderedTransactions[i].High)
                {
                    highPrice = orderedTransactions[i].High;
                    highIndex = i;
                }
            }
            var bestChannel = channel;
            for (int i = 0; i < orderedTransactions.Count; i++)
            {
                var newChannel = new Channel();
                var result = CalculateStartPriceAndRatio(orderedTransactions[i].High, i, highPrice, highIndex);
                newChannel.ChannelTrend = bestChannel.ChannelTrend;
                newChannel.Length = bestChannel.Length;
                newChannel.ResistanceStartPrice = result.Item1;
                newChannel.SupportStartPrice = bestChannel.SupportStartPrice;
                newChannel.ResistanceChannelRatio = result.Item2;
                var size1 = newChannel.Size();
                var size2 = bestChannel.Size();
                var coverPercentage1 = newChannel.CoverPercentage(orderedTransactions);
                var coverPercentage2 = bestChannel.CoverPercentage(orderedTransactions);
                Debug.WriteLine("Size1 : {0} - Size2 : {1}, Coverage1 : {2} - Coverage2 : {3}", size1, size2, coverPercentage1, coverPercentage2);
                if (size1 < size2 &&
                    coverPercentage1 >= coverPercentage2)
                    bestChannel = newChannel;
            }
            return bestChannel;
        }

        public Channel BuildSupportLine(Channel channel, IList<TransactionData> orderedTransactions)
        {
            double lowPrice = double.MaxValue;
            int lowIndex = 0;
            for (int i = 0; i < orderedTransactions.Count; i++)
            {
                if (lowPrice > orderedTransactions[i].Low)
                {
                    lowPrice = orderedTransactions[i].Low;
                    lowIndex = i;
                }
            }
            var bestChannel = channel;
            for (int i = 0; i < orderedTransactions.Count; i++)
            {
                var newChannel = new Channel();
                var result = CalculateStartPriceAndRatio(orderedTransactions[i].Low, i, lowPrice, lowIndex);
                newChannel.ChannelTrend = bestChannel.ChannelTrend;
                newChannel.Length = bestChannel.Length;
                newChannel.ResistanceStartPrice = bestChannel.ResistanceStartPrice;
                newChannel.SupportStartPrice = result.Item1;
                newChannel.SupportChannelRatio = result.Item2;
                var size1 = newChannel.Size();
                var size2 = bestChannel.Size();
                var coverPercentage1 = newChannel.CoverPercentage(orderedTransactions);
                var coverPercentage2 = bestChannel.CoverPercentage(orderedTransactions);
                Debug.WriteLine("Size1 : {0} - Size2 : {1}, Coverage1 : {2} - Coverage2 : {3}", size1, size2, coverPercentage1, coverPercentage2);
                if (size1 < size2 &&
                    coverPercentage1 >= coverPercentage2)
                    bestChannel = newChannel;
            }
            return bestChannel;
        }

        public Channel DeduceResistanceLine(Channel channel, IList<TransactionData> orderedTransactions)
        {
            var bestChannel = channel;
            bestChannel.ResistanceChannelRatio = channel.SupportChannelRatio;
            bestChannel.ResistanceStartPrice = orderedTransactions[0].High;
            for (int i = 1; i < orderedTransactions.Count; i++)
            {
                var newChannel = new Channel();
                var resistanceStartPrice = CalculatePriceAt(0, bestChannel.ResistanceChannelRatio, orderedTransactions[i].High, i);
                newChannel.ChannelTrend = bestChannel.ChannelTrend;
                newChannel.Length = bestChannel.Length;
                newChannel.ResistanceStartPrice = resistanceStartPrice;
                newChannel.SupportStartPrice = bestChannel.SupportStartPrice;
                newChannel.ResistanceChannelRatio = bestChannel.ResistanceChannelRatio;
                newChannel.SupportChannelRatio = bestChannel.SupportChannelRatio;
                if (newChannel.Size() < bestChannel.Size() &&
                    newChannel.CoverPercentage(orderedTransactions) >= bestChannel.CoverPercentage(orderedTransactions))
                    bestChannel = newChannel;
            }
            return bestChannel;
        }

        public Channel DeduceSupportLine(Channel channel, IList<TransactionData> orderedTransactions)
        {
            var bestChannel = channel;
            bestChannel.SupportChannelRatio = channel.ResistanceChannelRatio;
            bestChannel.SupportStartPrice = orderedTransactions[0].Low;
            for (int i = 1; i < orderedTransactions.Count; i++)
            {
                var newChannel = new Channel();
                var supportStartPrice = CalculatePriceAt(0, bestChannel.SupportChannelRatio, orderedTransactions[i].Low, i);
                newChannel.ChannelTrend = bestChannel.ChannelTrend;
                newChannel.Length = bestChannel.Length;
                newChannel.ResistanceStartPrice = bestChannel.ResistanceStartPrice;
                newChannel.SupportStartPrice = supportStartPrice;
                newChannel.ResistanceChannelRatio = bestChannel.ResistanceChannelRatio;
                newChannel.SupportChannelRatio = bestChannel.SupportChannelRatio;
                if (newChannel.Size() < bestChannel.Size() &&
                    newChannel.CoverPercentage(orderedTransactions) >= bestChannel.CoverPercentage(orderedTransactions))
                    bestChannel = newChannel;
            }
            return bestChannel;
        }

        public Tuple<double, double> CalculateStartPriceAndRatio(double price1, int index1, double price2, int index2)
        {
            double ratio = 0;
            if (index1 != index2)
                ratio = (price2 - price1)/(index2 - index1);
            double startPrice = CalculatePriceAt(0, ratio, price2, index2);
            Tuple<double, double> result = new Tuple<double, double>(startPrice, ratio);
            Debug.WriteLine("Price1 : {0}, Index1 : {1}, Price2 : {2}, Index2 : {3} - Start Price: {4}, Ratio : {5}", price1, index1, price2, index2, startPrice, ratio);
            return result;
        }

        public double CalculatePriceAt(int index2, double ratio, double price1, int index1)
        {
            return price1 + (index2 - index1)*ratio;
        }
    }
}