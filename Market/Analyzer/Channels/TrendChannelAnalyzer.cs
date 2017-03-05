using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            channel = BuildSupportLine(channel, orderedTransactions);
            channel = BuildResistanceLine(channel, orderedTransactions);
            channel.StockKey = orderedTransactions[0].StockKey;
            channel.StartDate = orderedTransactions[0].TimeStamp;
            channel.EndDate = orderedTransactions[orderedTransactions.Count - 1].TimeStamp;
            if (channel.SupportChannelRatio > 0)
            {
                var upPercentage = channel.SupportChannelRatio/channel.SupportStartPrice;
                if (upPercentage > 0.001)
                {
                    if (channel.ChannelTrend <= 0)
                    {
                        switch (channel.ChannelTrend)
                        {
                            case Trend.Bottom:
                            case Trend.BottomDown:
                            case Trend.Down:
                                Console.WriteLine("{0} Channel for {1} starting on {2} was {3} changed to {4}", channel.Length, channel.StockKey, channel.StartDate, channel.ChannelTrend, Trend.BottomUp);
                                channel.ChannelTrend = Trend.BottomUp;
                                break;
                            case Trend.VibrationDown:
                            case Trend.Vibration:
                                Console.WriteLine("{0} Channel for {1} starting on {2} was {3} changed to {4}", channel.Length, channel.StockKey, channel.StartDate, channel.ChannelTrend, Trend.VibrationUp);
                                channel.ChannelTrend = Trend.VibrationUp;
                                break;
                            case Trend.Top:
                            case Trend.TopDown:
                                Console.WriteLine("{0} Channel for {1} starting on {2} was {3} changed to {4}", channel.Length, channel.StockKey, channel.StartDate, channel.ChannelTrend, Trend.TopUp);
                                channel.ChannelTrend = Trend.TopUp;
                                break;
                            case Trend.TopBottom:
                            case Trend.TopBottomDown:
                                Console.WriteLine("{0} Channel for {1} starting on {2} was {3} changed to {4}", channel.Length, channel.StockKey, channel.StartDate, channel.ChannelTrend, Trend.TopBottomUp);
                                channel.ChannelTrend = Trend.TopBottomUp;
                                break;
                            case Trend.BottomTop:
                            case Trend.BottomTopDown:
                                Console.WriteLine("{0} Channel for {1} starting on {2} was {3} changed to {4}", channel.Length, channel.StockKey, channel.StartDate, channel.ChannelTrend, Trend.BottomTopUp);
                                channel.ChannelTrend = Trend.BottomTopUp;
                                break;
                        }
                    }
                }
            }
            if (channel.ResistanceChannelRatio < 0)
            {
                var downPercentage = channel.ResistanceChannelRatio/channel.ResistanceStartPrice;
                if (downPercentage < -0.001)
                {
                    if (channel.ChannelTrend >= 0)
                    {
                        switch (channel.ChannelTrend)
                        {
                            case Trend.Bottom:
                            case Trend.BottomUp:
                                Console.WriteLine("{0} Channel for {1} starting on {2} was {3} changed to {4}", channel.Length, channel.StockKey, channel.StartDate, channel.ChannelTrend, Trend.BottomDown);
                                channel.ChannelTrend = Trend.BottomDown;
                                break;
                            case Trend.VibrationUp:
                            case Trend.Vibration:
                                Console.WriteLine("{0} Channel for {1} starting on {2} was {3} changed to {4}", channel.Length, channel.StockKey, channel.StartDate, channel.ChannelTrend, Trend.VibrationDown);
                                channel.ChannelTrend = Trend.VibrationDown;
                                break;
                            case Trend.Top:
                            case Trend.TopUp:
                            case Trend.Up:
                                Console.WriteLine("{0} Channel for {1} starting on {2} was {3} changed to {4}", channel.Length, channel.StockKey, channel.StartDate, channel.ChannelTrend, Trend.TopDown);
                                channel.ChannelTrend = Trend.TopDown;
                                break;
                            case Trend.TopBottom:
                            case Trend.TopBottomUp:
                                Console.WriteLine("{0} Channel for {1} starting on {2} was {3} changed to {4}", channel.Length, channel.StockKey, channel.StartDate, channel.ChannelTrend, Trend.TopBottomDown);
                                channel.ChannelTrend = Trend.TopBottomDown;
                                break;
                            case Trend.BottomTop:
                            case Trend.BottomTopUp:
                                Console.WriteLine("{0} Channel for {1} starting on {2} was {3} changed to {4}", channel.Length, channel.StockKey, channel.StartDate, channel.ChannelTrend, Trend.BottomTopDown);
                                channel.ChannelTrend = Trend.BottomTopDown;
                                break;
                        }
                    }
                }
            }
            return channel;
        }

        public MovingAverage GetMovingAverage(IList<TransactionData> orderedTransacions)
        {
            MovingAverage movingAverage = new MovingAverage();
            int count = orderedTransacions.Count;
            movingAverage.Averages = new double[count];
            //if (count <= 20)
            {
                movingAverage.NumberOfTransactions = 0;
                for (int i = 0; i < count; i++)
                {
                    movingAverage.Averages[i] = (orderedTransacions[i].High + orderedTransacions[i].Low)/2;
                }
            }
            //else if (count <= 100 && count > 20)
            //{
            //    movingAverage.NumberOfTransactions = 5;
            //    for (int i = 0; i < count; i++)
            //    {
            //        movingAverage.Averages[i] = orderedTransacions[i].SimpleAvg5;
            //    }
            //}
            //else 
            //{
            //    movingAverage.NumberOfTransactions = 20;
            //    for (int i = 0; i < count; i++)
            //    {
            //        movingAverage.Averages[i] = orderedTransacions[i].SimpleAvg20;
            //    }
            //}
            return movingAverage;
        }

        public Channel BuildResistanceLine(Channel channel, IList<TransactionData> orderedTransactions)
        {
            var bestChannel = channel;
            for (int i = 0; i < orderedTransactions.Count; i++)
            {
                for (int j = i; j < orderedTransactions.Count; j++)
                {
                    var newChannel = new Channel();
                    var result = CalculateStartPriceAndRatio(orderedTransactions[i].High, i, orderedTransactions[j].High, j);
                    newChannel.ChannelTrend = bestChannel.ChannelTrend;
                    newChannel.Length = bestChannel.Length;
                    newChannel.ResistanceStartPrice = result.Item1;
                    newChannel.ResistanceChannelRatio = result.Item2;
                    newChannel.SupportStartPrice = bestChannel.SupportStartPrice;
                    newChannel.SupportChannelRatio = bestChannel.SupportChannelRatio;
                    var size1 = newChannel.Size();
                    var size2 = bestChannel.Size();
                    var coverPercentage1 = newChannel.CoverPercentage(orderedTransactions);
                    var coverPercentage2 = bestChannel.CoverPercentage(orderedTransactions);
                    //Debug.WriteLine("Size1 : {0} - Size2 : {1}, Coverage1 : {2} - Coverage2 : {3}", size1, size2, coverPercentage1, coverPercentage2);
                    if (size1 < size2 &&
                        coverPercentage1 >= coverPercentage2)
                        bestChannel = newChannel;
                }
            }
            return bestChannel;
        }

        public Channel BuildSupportLine(Channel channel, IList<TransactionData> orderedTransactions)
        {
            var bestChannel = channel;
            for (int i = 0; i < orderedTransactions.Count; i++)
            {
                for (int j = i; j < orderedTransactions.Count; j++)
                {
                    var newChannel = new Channel();
                    var result = CalculateStartPriceAndRatio(orderedTransactions[i].Low, i, orderedTransactions[j].Low, j);
                    newChannel.ChannelTrend = bestChannel.ChannelTrend;
                    newChannel.Length = bestChannel.Length;
                    newChannel.ResistanceStartPrice = bestChannel.ResistanceStartPrice;
                    newChannel.ResistanceChannelRatio = bestChannel.ResistanceChannelRatio;
                    newChannel.SupportStartPrice = result.Item1;
                    newChannel.SupportChannelRatio = result.Item2;
                    var size1 = newChannel.Size();
                    var size2 = bestChannel.Size();
                    var coverPercentage1 = newChannel.CoverPercentage(orderedTransactions);
                    var coverPercentage2 = bestChannel.CoverPercentage(orderedTransactions);
                    if (size1 < size2 &&
                        coverPercentage1 >= coverPercentage2)
                        bestChannel = newChannel;
                }
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
            //Debug.WriteLine("Price1 : {0}, Index1 : {1}, Price2 : {2}, Index2 : {3} - Start Price: {4}, Ratio : {5}", price1, index1, price2, index2, startPrice, ratio);
            return result;
        }

        public double CalculatePriceAt(int index2, double ratio, double price1, int index1)
        {
            return price1 + (index2 - index1)*ratio;
        }
    }
}