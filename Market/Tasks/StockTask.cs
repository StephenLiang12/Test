﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using Market.Analyzer;
using Market.Analyzer.Channels;
using Market.Analyzer.MACD;
using Market.Web;
using Market.Web.Factory;

namespace Market.Tasks
{
    public class StockTask : IStockTask
    {
        private readonly ITransactionWebRequestFactory webRequestFactory;

        public StockTask()
        {
            webRequestFactory = new TransactionWebRequestFactory();
        }

        public int AddStockFromEodData(StreamReader reader)
        {
            StockContext context = new StockContext();
            string firstLine = reader.ReadLine();
            int count = 0;
            do
            {
                string line = reader.ReadLine();
                string[] items = line.Split(',');
                double volume = Convert.ToDouble(items[5]);
                string stockid = items[0];
                if (volume < 100000 || context.Stocks.Any(s => s.Id == stockid))
                    continue;
                Stock stock = new Stock();
                stock.Id = stockid + ".TO";
                stock.Name = items[1];
                stock.AvgVolume = volume;
                context.Stocks.Add(stock);
                context.SaveChanges();
                count++;
            } while (reader.EndOfStream == false);
            return count;
        }

        public int AddStockFromEodSimpleData(StreamReader reader)
        {
            StockContext context = new StockContext();
            string firstLine = reader.ReadLine();
            int count = 0;
            do
            {
                string line = reader.ReadLine();
                string[] items = line.Split(',');
                string stockid = items[0];
                if (stockid.Contains('.'))
                    continue;
                Stock stock = new Stock();
                stock.Id = stockid + ".TO";
                stock.Name = items[1];
                //stock.AvgVolume = volume;
                context.Stocks.Add(stock);
                context.SaveChanges();
                count++;
            } while (reader.EndOfStream == false);
            return count;
        }

        public int GetTransactionDataFromInternet(string stockId)
        {
            int count = 0;
            StockContext context = new StockContext();
            int stockKey = context.Stocks.First(s => s.Id == stockId).Key;
            DateTime startDateTime = new DateTime(2011, 1, 1);
            if (context.TransactionData.Any(t => t.StockKey == stockKey))
            {
                DateTime lastDateTime = context.OriginalTransactionData.Where(t => t.StockKey == stockKey).Max(t => t.TimeStamp);
                startDateTime = lastDateTime;
            }
            if (startDateTime < DateTime.Today)
            {
                var webRequest = webRequestFactory.CreateTransactionWebRequest(stockId, startDateTime);
                try
                {
                    if (context.Stocks.Any(s => s.Id == stockId) == false)
                    {
                        throw new ArgumentException("Unknow Stock Id {0}", stockId);
                    }
                    var stock = context.Stocks.First(s => s.Id == stockId);
                    double sumOfVolume = 0;
                    foreach (var data in webRequest.GetOriginalTransactionDataFromInternet())
                    {
                        data.StockKey = stock.Key;
                        if (context.OriginalTransactionData.Any(
                                d =>
                                    d.StockKey == data.StockKey && d.TimeStamp == data.TimeStamp &&
                                    d.Period == data.Period) == false)
                        {
                            context.OriginalTransactionData.Add(data);
                            var d = data.GetTransactionData();
                            context.TransactionData.Add(d);
                        }
                        sumOfVolume += data.Volume;
                        count++;
                    }
                    if (count == 0)
                        return count;
                    stock.AvgVolume = Math.Round(sumOfVolume / count);
                    stock.AbleToGetTransactionDataFromWeb = true;
                    context.SaveChanges();
                }
                catch (WebException ex)
                {
                    var stock = context.Stocks.First(s => s.Id == stockId);
                    Console.WriteLine("Error on {0}: {1}", stock.Id, ex.Message);
                    stock.AbleToGetTransactionDataFromWeb = false;
                    context.SaveChanges();
                    return count;
                }
            }
            return count;
        }

        public void RegenerateTransactionDataFromOriginalData()
        {
            StockContext context = new StockContext();
            IList<int> splits = context.Splits.Select(s => s.StockKey).Distinct().ToList();
            foreach (int stockKey in splits)
            //var stockKey = 479;
            {
                RegenerateTransactionDataFromOriginalData(stockKey);
                context.Database.ExecuteSqlCommand("delete from MovingAverageConvergenceDivergence where StockKey = " + stockKey);
                context.Database.ExecuteSqlCommand("delete from Channel where StockKey = " + stockKey);
                CalculateMovingAverageConvergenceDivergence(stockKey);
            }
        }
        public void RegenerateTransactionDataFromOriginalData(int stockKey)
        {
            StockContext context = new StockContext();
            context.Database.ExecuteSqlCommand("delete from TransactionData where StockKey = " + stockKey);
            foreach (var source in context.OriginalTransactionData.Where(t => t.StockKey == stockKey).OrderBy(t => t.TimeStamp))
            {
                context.TransactionData.Add(source.GetTransactionData());
            }
            context.SaveChanges();
            foreach (var split in context.Splits.Where(s => s.StockKey == stockKey).OrderBy(s => s.TimeStamp))
            {
                ApplySplitOnTransactionData(stockKey, split);
            }
        }

        public void CalculateMovingAverageConvergenceDivergence(int minStockKey, int maxStockKey)
        {
            StockContext context = new StockContext();
            foreach (var stock in context.Stocks.Where(s => s.Key >= minStockKey && s.Key <= maxStockKey))
            {
                try
                {
                    CalculateMovingAverageConvergenceDivergence(stock.Key);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        public void CalculateMovingAverageConvergenceDivergence(int stockKey)
        {
            StockContext context = new StockContext();
            MovingAverageConvergenceDivergenceCalculator calculator = new MovingAverageConvergenceDivergenceCalculator();
            IList<TransactionData> orderedList = context.TransactionData.Where(t => t.StockKey == stockKey).OrderBy(t => t.TimeStamp).ToList();
            {
                var result = calculator.Calculate(orderedList, 12, 26, 9);
                foreach (var movingAverageConvergenceDivergence in result)
                {
                    if (context.MovingAverageConvergenceDivergences.Any(m => m.StockKey == stockKey && m.TimeStamp == movingAverageConvergenceDivergence.TimeStamp) == false)
                        context.MovingAverageConvergenceDivergences.Add(movingAverageConvergenceDivergence);

                }
                context.SaveChanges();
            }
        }

        public void AnalyzeTrendChannel()
        {
            StockContext context = new StockContext();
            foreach (var stock in context.Stocks)
            {
                AnalyzeTrendChannel(stock.Key, 20);
                AnalyzeTrendChannel(stock.Key, 50);
                AnalyzeTrendChannel(stock.Key, 100);
                AnalyzeTrendChannel(stock.Key, 200);
            }
        }

        public void AnalyzeTrendChannel(int stockKey, int length)
        {
            try
            {
                StockContext updateContext = new StockContext();
                IList<TransactionData> orderedList =
                    updateContext.TransactionData.Where(t => t.StockKey == stockKey).OrderBy(t => t.TimeStamp).ToList();
                var partialList = orderedList.GetFrontPartial(length);
                for (int i = length; i <= orderedList.Count; i++)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var timeStamp = partialList[0].TimeStamp;
                    if (updateContext.Channels.Any(c => c.StockKey == stockKey && c.StartDate == timeStamp && c.Length == length) == false)
                    {
                        TrendChannelAnalyzer analyzer = new TrendChannelAnalyzer();
                        var channel = analyzer.AnalyzeTrendChannel(partialList);
                        updateContext.Channels.Add(channel);
                        updateContext.SaveChanges();
                    }
                    if (i < orderedList.Count)
                    {
                        partialList.RemoveAt(0);
                        partialList.Add(orderedList[i]);
                    }
                    stopwatch.Stop();
                    Console.WriteLine("Calculate {0} length channel {1} seconds.", length, stopwatch.Elapsed.TotalSeconds);
                }
                orderedList.Clear();
                partialList.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public int GetSplitFromInternet(string stockId)
        {
            StockContext context = new StockContext();
            var webRequest = webRequestFactory.CreateTransactionWebRequest(stockId);
            if (context.Stocks.Any(s => s.Id == stockId) == false)
            {
                throw new ArgumentException("Unknow Stock Id {0}", stockId);
            }
            var stock = context.Stocks.First(s => s.Id == stockId);
            int count = 0;
            foreach (var split in webRequest.GetSplitFromInternet())
            {
                split.StockKey = stock.Key;
                DateTime splitTimeStamp = split.TimeStamp;
                if (split.SplitRatio == Double.PositiveInfinity || context.Splits.Any(s => s.StockKey == stock.Key && s.TimeStamp == splitTimeStamp))
                    continue;
                context.Splits.Add(split);
                count++;
            }
            context.SaveChanges();
            return count;
        }

        public Channel GetChannel(int stockKey, int length, DateTime endTime)
        {
            var stockContext = new StockContext();
            return stockContext.Channels.FirstOrDefault(c => c.StockKey == stockKey && c.Length == length && c.EndDate == endTime);
        }

        public void ApplySplitOnTransactionData(int stockKey, Split split)
        {
            var context = new StockContext();
            foreach (
                var transaction in context.TransactionData.Where(t => t.StockKey == stockKey && t.TimeStamp < split.TimeStamp))
            {
                transaction.Open *= split.SplitRatio;
                transaction.Close *= split.SplitRatio;
                transaction.High *= split.SplitRatio;
                transaction.Low *= split.SplitRatio;
            }
            context.SaveChanges();
        }

        private void ApplySplitOnChannels(int stockKey, Split split)
        {
            var context = new StockContext();
            foreach (
                var channel in context.Channels.Where(t => t.StockKey == stockKey && t.EndDate < split.TimeStamp))
            {
                channel.SupportStartPrice *= split.SplitRatio;
                channel.ResistanceStartPrice *= split.SplitRatio;
            }
            context.SaveChanges();
        }

        public Channel GetChannel(int stockKey, int length, DateTime startTime, DateTime endTime)
        {
            var stockContext = new StockContext();
            var trendChannelAnalyzer = new TrendChannelAnalyzer();
            var channel =
                stockContext.Channels.FirstOrDefault(c => c.StockKey == stockKey && c.Length == length && c.EndDate == endTime);
            if (channel != null)
                return channel;
            IList<TransactionData> orderedList =
                stockContext.TransactionData.Where(t => t.StockKey == stockKey && t.TimeStamp >= startTime && t.TimeStamp <= endTime)
                    .OrderBy(t => t.TimeStamp)
                    .ToList();
            var partialList = orderedList.GetRearPartial(length);
            channel = trendChannelAnalyzer.AnalyzeTrendChannel(partialList);
            stockContext.Channels.Add(channel);
            stockContext.SaveChanges();
            return channel;
        }

        public Channel GetPreviousChannel(int stockKey, int length, DateTime endTime)
        {
            var stockContext = new StockContext();
            var channels = stockContext.Channels.Where(c => c.StockKey == stockKey && c.Length == length && c.EndDate < endTime).ToList();
            if (channels.Count > 0)
            {
                var maxDate = channels.Max(c => c.EndDate);
                return channels.First(c => c.EndDate == maxDate);
            }
            return null;
        }

        public IEnumerable<Channel> GetChannels(int stockKey, int length, DateTime startTime, DateTime endTime)
        {
            var stockContext = new StockContext();
            var channels = stockContext.Channels.Where(c => c.StockKey == stockKey && c.Length == length && c.EndDate>= startTime && c.EndDate < endTime).ToList();
            return channels;
        }
    }
}