using System;
using System.Collections.Generic;
using System.Linq;
using Market.Analyzer;
using Market.Analyzer.Channels;
using Market.Suggestions;
using Market.Suggestions.MACD;
using Market.Suggestions.TrendChannels;
using Market.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture
{
    [TestClass]
    public class IntegrationTest
    {
        [TestMethod]
        public void TestChannelReverse()
        {
            StockContext context = new StockContext();
            foreach (var stock in context.Stocks)
            {
                int stockKey = stock.Key;
                IList<Channel> channels = context.Channels.Where(c => c.StockKey == stockKey && c.Length == 200)
                    .OrderBy(c => c.StartDate).ToList();
                int maxDays200;
                int minDays200;
                int sumOfDays200;
                int numberOfReverse200;
                ReviewChannels(channels, context, out maxDays200, out minDays200, out sumOfDays200, out numberOfReverse200);
                channels = context.Channels.Where(c => c.StockKey == stockKey && c.Length == 100)
                    .OrderBy(c => c.StartDate).ToList();
                int maxDays100;
                int minDays100;
                int sumOfDays100;
                int numberOfReverse100;
                ReviewChannels(channels, context, out maxDays100, out minDays100, out sumOfDays100, out numberOfReverse100);
                channels = context.Channels.Where(c => c.StockKey == stockKey && c.Length == 50)
                    .OrderBy(c => c.StartDate).ToList();
                int maxDays50;
                int minDays50;
                int sumOfDays50;
                int numberOfReverse50;
                ReviewChannels(channels, context, out maxDays50, out minDays50, out sumOfDays50, out numberOfReverse50);
                if (numberOfReverse50 > 0)
                    stock.AvgDaysChannel50Reverse = sumOfDays50 / (double)numberOfReverse50;
                else
                    stock.AvgDaysChannel50Reverse = 0;
                if (numberOfReverse100 > 0)
                    stock.AvgDaysChannel100Reverse = sumOfDays100 / (double)numberOfReverse100;
                else
                    stock.AvgDaysChannel100Reverse = 0;
                if (numberOfReverse200 > 0)
                    stock.AvgDaysChannel200Reverse = sumOfDays200 / (double)numberOfReverse200;
                else
                    stock.AvgDaysChannel200Reverse = 0;
            }
            context.SaveChanges();
        }

        private static void ReviewChannels(IList<Channel> channels, StockContext context, out int maxDays, out int minDays,
            out int sumOfDays, out int numberOfReverse)
        {
            Channel previousChannel = null;
            maxDays = 0;
            minDays = int.MaxValue;
            sumOfDays = 0;
            numberOfReverse = 0;
            DateTime minStarTime = DateTime.Today, maxStartTime = DateTime.Today;
            foreach (var channel in channels)
            {
                if (previousChannel == null || previousChannel.ChannelTrend.GetSign() == 0)
                {
                    previousChannel = channel;
                    continue;
                }
                if ((previousChannel.ChannelTrend.GetSign() > 0 && channel.ChannelTrend.GetSign() < 0) ||
                    (previousChannel.ChannelTrend.GetSign() < 0 && channel.ChannelTrend.GetSign() > 0))
                {
                    int days = context.TransactionData.Count(t => t.StockKey == previousChannel.StockKey && t.TimeStamp >= previousChannel.EndDate &&
                                                                  t.TimeStamp <= channel.EndDate);
                    if (maxDays < days)
                    {
                        maxDays = days;
                        maxStartTime = previousChannel.StartDate;
                    }
                    if (minDays > days)
                    {
                        minDays = days;
                        minStarTime = previousChannel.StartDate;
                    }
                    sumOfDays += days;
                    numberOfReverse++;
                    previousChannel = channel;
                }
            }
            Console.WriteLine("Max Start Date: {0}, Min Start Date: {1}", maxStartTime, minStarTime);
        }

        [TestMethod]
        public void RerunFromBeginning(int minStockKey, int maxStockKey)
        {
            StockContext context = new StockContext();
            MACDSuggestionAnalyzer analyzer = new MACDSuggestionAnalyzer();
            StockTask stockTask = new StockTask();
            Console.WriteLine("Id, Name, DateTime, Action, Close, CandleStickPattern, MACD, Avg20 Trend, Avg200 Trend, Price VS Avg5,Avg5 VS Avg20");
            IList<int> list = new List<int>(new []{538,573,1054,917,982,1101,501,503,504,956,940,876});
            foreach (var stock in context.Stocks.Where(s => list.Contains(s.Key) == false && s.Key > minStockKey && s.Key < maxStockKey).ToList())
            {
                stockTask.CalculateMovingAverageConvergenceDivergence(stock.Key);
                IList<TransactionData> orderedList =
                    context.TransactionData.Where(t => t.StockKey == stock.Key).OrderBy(t => t.TimeStamp).ToList();
                int j = 200;
                while (j < orderedList.Count)
                {
                    try
                    {
                        var partialList = orderedList.GetFrontPartial(j);
                        if (analyzer.CalculateForecaseCertainty(partialList) > 0)
                        {
                            Suggestion suggestion = new Suggestion();
                            suggestion.TimeStamp = orderedList[j - 1].TimeStamp;
                            suggestion.StockKey = stock.Key;
                            suggestion.StockId = stock.Id;
                            suggestion.StockName = stock.Name;
                            suggestion.ClosePrice = orderedList[j - 1].Close;
                            suggestion.Volume = orderedList[j - 1].Volume;
                            suggestion.CandleStickPattern = "Unknown";
                            //suggestion.CandleStickPattern = partialPattern.Name;
                            //suggestion.Macd = signalLineCrossOver10_20_6.Divergence;
                            //suggestion.Avg5Trend = partialMovingTrend5;
                            //suggestion.Avg20Trend = movingTrend20;
                            //suggestion.Avg200Trend = movingTrend200;
                            //suggestion.PriceVsAvg5 = priceMovingAvg5;
                            //suggestion.PriceVsAvg200 = priceMovingAvg200;
                            //suggestion.Avg5VsAvg20 = movingAvg5_20;
                            //suggestion.Avg50VsAvg200 = movingAvg50_200;
                            suggestion.AnalyzerName = analyzer.Name;
                            suggestion.SuggestedTerm = analyzer.Term;
                            suggestion.SuggestedAction = analyzer.Action;
                            if (context.Suggestions.Any(
                                    s =>
                                        s.StockKey == suggestion.StockKey && s.TimeStamp == suggestion.TimeStamp &&
                                        s.AnalyzerName == suggestion.AnalyzerName &&
                                        s.SuggestedAction == suggestion.SuggestedAction &&
                                        s.SuggestedTerm == suggestion.SuggestedTerm) == false)
                            {
                                context.Suggestions.Add(suggestion);
                                context.SaveChanges();
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                    j++;
                }
            }
        }

        [TestMethod]
        public void TestSuggestionAnalyzer()
        {
            StockContext context = new StockContext();
            MovingAverageAnalyzer analyzer = new MovingAverageAnalyzer();
            CandleStickPatternAnalyzer candleStickPatternAnalyzer = new CandleStickPatternAnalyzer();
            Console.WriteLine("Id, Name, DateTime, Action, Close, CandleStickPattern, MACD, Avg20 Trend, Avg200 Trend, Price VS Avg5,Avg5 VS Avg20");
            foreach (var stock in context.Stocks.ToList())
            {
                if (stock.AbleToGetTransactionDataFromWeb == false)
                    continue;
                //var stock = context.Stocks.First(s => s.Id == "TRP.TO");
                IList<TransactionData> orderedList =
                    context.TransactionData.Where(t => t.StockKey == stock.Key).OrderBy(t => t.TimeStamp).ToList();
                var suggestionAnalyzer = new LongTermBuyAfterLongTermPrepareSuggestionAnalyzer();
                MovingAverage avg5 = new MovingAverage();
                avg5.NumberOfTransactions = 5;
                avg5.Averages = new double[orderedList.Count];
                MovingAverage avg10 = new MovingAverage();
                avg10.NumberOfTransactions = 10;
                avg10.Averages = new double[orderedList.Count];
                MovingAverage avg20 = new MovingAverage();
                avg20.NumberOfTransactions = 20;
                avg20.Averages = new double[orderedList.Count];
                MovingAverage avg50 = new MovingAverage();
                avg50.NumberOfTransactions = 50;
                avg50.Averages = new double[orderedList.Count];
                MovingAverage avg100 = new MovingAverage();
                avg100.NumberOfTransactions = 100;
                avg100.Averages = new double[orderedList.Count];
                MovingAverage avg200 = new MovingAverage();
                avg200.NumberOfTransactions = 200;
                avg200.Averages = new double[orderedList.Count];
                for (int i = 0; i < orderedList.Count; i++)
                {
                    avg5.Averages[i] = orderedList[i].SimpleAvg5;
                    avg10.Averages[i] = orderedList[i].SimpleAvg10;
                    avg20.Averages[i] = orderedList[i].SimpleAvg20;
                    avg50.Averages[i] = orderedList[i].SimpleAvg50;
                    avg100.Averages[i] = orderedList[i].SimpleAvg100;
                    avg200.Averages[i] = orderedList[i].SimpleAvg200;
                }
                int j = 200;
                while (j < orderedList.Count)
                {
                    try
                    {
                        var partialList = orderedList.GetFrontPartial(j);
                        var partialAvg5 = avg5.GetPartial(j);
                        var partialAvg10 = avg10.GetPartial(j);
                        var partialAvg20 = avg20.GetPartial(j);
                        var partialAvg50 = avg50.GetPartial(j);
                        var partialAvg200 = avg200.GetPartial(j);
                        var partialMovingTrend5 = analyzer.AnalyzeMovingTrend(partialAvg5);
                        var partialPattern = candleStickPatternAnalyzer.GetPattern(partialList, partialMovingTrend5);
                        double priceMovingAvg5 = analyzer.PriceCompareAverage(partialList, partialAvg5);
                        double priceMovingAvg20 = analyzer.PriceCompareAverage(partialList, partialAvg20);
                        double priceMovingAvg200 = analyzer.PriceCompareAverage(partialList, partialAvg200);
                        double movingAvg5_20 = analyzer.AverageCrossOver(partialAvg5, partialAvg20);
                        double movingAvg50_200 = analyzer.AverageCrossOver(partialAvg50, partialAvg200);
                        var movingTrend10 = analyzer.AnalyzeMovingTrend(partialAvg10);
                        var movingTrend20 = analyzer.AnalyzeMovingTrend(partialAvg20);
                        var movingTrend50 = analyzer.AnalyzeMovingTrend(partialAvg50);
                        var movingTrend200 = analyzer.AnalyzeMovingTrend(partialAvg200);
                        Suggestion suggestion = new Suggestion();
                        suggestion.TimeStamp = partialList[j - 1].TimeStamp;
                        suggestion.StockKey = stock.Key;
                        suggestion.StockId = stock.Id;
                        suggestion.StockName = stock.Name;
                        suggestion.ClosePrice = partialList[j - 1].Close;
                        suggestion.Volume = partialList[j - 1].Volume;
                        suggestion.CandleStickPattern = partialPattern.Name;
                        suggestion.Avg5Trend = partialMovingTrend5;
                        suggestion.Avg20Trend = movingTrend20;
                        suggestion.Avg200Trend = movingTrend200;
                        suggestion.PriceVsAvg5 = priceMovingAvg5;
                        suggestion.PriceVsAvg200 = priceMovingAvg200;
                        suggestion.Avg5VsAvg20 = movingAvg5_20;
                        suggestion.Avg50VsAvg200 = movingAvg50_200;

                        if (suggestionAnalyzer.CalculateForecaseCertainty(partialList) > 0)
                        {
                            suggestion.AnalyzerName = suggestionAnalyzer.Name;
                            suggestion.SuggestedTerm = suggestionAnalyzer.Term;
                            suggestion.SuggestedAction = suggestionAnalyzer.Action;
                            context.Suggestions.Add(suggestion);
                            context.SaveChanges();
                        }
                    }
                    catch (Exception)
                    {
                    }
                    j++;
                }
            }
        }

        [TestMethod]
        public void TestValuation()
        {
            StockContext context = new StockContext();
            foreach (var stock in context.Stocks)
            {
                //var stock = context.Stocks.First(s => s.Key == 7);
                if (context.Suggestions.Any(s => s.StockKey == stock.Key))
                {
                    SortedList<DateTime, TransactionData> trasactionSortedList = new SortedList<DateTime, TransactionData>();
                    foreach (var transaction in context.TransactionData.Where(t => t.StockKey == stock.Key))
                    {
                        trasactionSortedList.Add(transaction.TimeStamp, transaction);
                    }
                    foreach (var suggestion in context.Suggestions.Where(s => s.StockKey == stock.Key))
                    {
                        int start = trasactionSortedList.IndexOfKey(suggestion.TimeStamp);

                        int end;
                        double percentage = 0;
                        double shortAccuracy = 0;
                        double intermediateAccuracy = 0;
                        double longAccuracy = 0;
                        if (suggestion.SuggestedTerm == Term.Short)
                        {
                            end = start + 10;
                            percentage = 0.05;
                            try
                            {
                                shortAccuracy = CalculateAccuracy(trasactionSortedList, suggestion.SuggestedAction, start, end, suggestion.ClosePrice, percentage, 0);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Suggestion {0} starts with {1} end at {2} throw exception: {3}", suggestion.StockId, start, end, e.Message);
                            }
                        }
                        if (suggestion.SuggestedTerm == Term.Intermediate)
                        {
                            end = start + 20;
                            percentage = 0.1;
                            try
                            {
                                intermediateAccuracy = CalculateAccuracy(trasactionSortedList, suggestion.SuggestedAction, start, end, suggestion.ClosePrice, percentage, 0.02);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Suggestion {0} starts with {1} end at {2} throw exception: {3}", suggestion.StockId, start, end, e.Message);
                            }
                        }
                        if (suggestion.SuggestedTerm == Term.Long)
                        {
                            end = start + 50;
                            percentage = 0.2;
                            try
                            {
                                longAccuracy = CalculateAccuracy(trasactionSortedList, suggestion.SuggestedAction, start, end, suggestion.ClosePrice, percentage, 0.05);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Suggestion {0} starts with {1} end at {2} throw exception: {3}", suggestion.StockId, start, end, e.Message);
                            }
                        }
                        suggestion.Accuracy = Math.Max(shortAccuracy, Math.Max(intermediateAccuracy, longAccuracy));
                    }
                }
            }
            context.SaveChanges();
        }

        private static double CalculateAccuracy(SortedList<DateTime, TransactionData> trasactionSortedList, Action action,
            int start, int end, double closePrice, double expectedPercentage, double minimumPercentage)
        {
            double startPrice = trasactionSortedList.Values[start].Close;
            double realPrice = startPrice;
            int sign = 1;
            if (action == Action.Sell)
                sign = -1;
            for (int i = start + 1; i <= end; i++)
            {
                if (i > trasactionSortedList.Count - 1)
                    break;
                if (action == Action.Buy)
                {
                    if (realPrice < trasactionSortedList.Values[i].Close)
                        realPrice = trasactionSortedList.Values[i].Close;
                }
                else
                {
                    if (realPrice > trasactionSortedList.Values[i].Close)
                        realPrice = trasactionSortedList.Values[i].Close;
                }
            }
            double priceDiff = realPrice - startPrice;
            double accurate = 0;
            if (Math.Sign(priceDiff) == sign)
            {
                double percertage = sign*priceDiff/closePrice;
                if (percertage < minimumPercentage)
                    return 0;
                accurate = percertage/expectedPercentage;
                if (accurate > 1)
                    accurate = 1;
            }
            return accurate;
        }

        //[TestMethod]
        //public void SimulateTransactions()
        //{
        //    StockContext context = new StockContext();
        //    foreach (var stock in context.Stocks)
        //    {
        //        if (context.Suggestions.Any(s => s.StockKey == stock.Key))
        //        {
        //            SortedList<DateTime, TransactionData> trasactionSortedList = new SortedList<DateTime, TransactionData>();
        //            foreach (var transaction in context.TransactionDatas.Where(t => t.StockKey == stock.Key))
        //            {
        //                trasactionSortedList.Add(transaction.TimeStamp, transaction);
        //            }
        //            Dictionary<string, SortedList<DateTime, Suggestion>> suggestionSortedDic = new Dictionary<string, SortedList<DateTime, Suggestion>>();
        //            foreach (var suggestion in context.Suggestions.Where(s => s.StockKey == stock.Key))
        //            {
        //                if (suggestionSortedDic.ContainsKey(suggestion.AnalyzerName) == false)
        //                    suggestionSortedDic[suggestion.AnalyzerName] = new SortedList<DateTime, Suggestion>();
        //                suggestionSortedDic[suggestion.AnalyzerName].Add(suggestion.TimeStamp, suggestion);
        //            }
        //            foreach (var pair in suggestionSortedDic)
        //            {
        //                Dictionary<long, TransactionSimulator> simulcationDic = context.TransactionSimulators.Where(t => t.StockKey == stock.Key).ToDictionary(t => t.SuggestionKey);
        //                TransactionSimulator existingTransactionSimulator = null;
        //                foreach (var suggestion in pair.Value.Values)
        //                {
        //                    if (existingTransactionSimulator == null && simulcationDic.ContainsKey(suggestion.Key))
        //                        existingTransactionSimulator = simulcationDic[suggestion.Key];
        //                    if (existingTransactionSimulator == null || (existingTransactionSimulator.SellDate.HasValue && existingTransactionSimulator.SellDate <= suggestion.TimeStamp))
        //                    {
        //                        int index = trasactionSortedList.IndexOfKey(suggestion.TimeStamp);
        //                        if (index < trasactionSortedList.Count - 1)
        //                        {
        //                            var nextDayTransaction = trasactionSortedList.Values[index + 1];
        //                            var newTransactionSimulator = new TransactionSimulator();
        //                            newTransactionSimulator.StockKey = stock.Key;
        //                            newTransactionSimulator.SuggestionKey = suggestion.Key;
        //                            newTransactionSimulator.BuyDate = nextDayTransaction.TimeStamp;
        //                            newTransactionSimulator.BuyPrice = (nextDayTransaction.Low +
        //                                                                nextDayTransaction.High)/2;
        //                            newTransactionSimulator.Volume =
        //                                Convert.ToInt32(50000/newTransactionSimulator.BuyPrice);
        //                            context.TransactionSimulators.Add(newTransactionSimulator);
        //                            existingTransactionSimulator = newTransactionSimulator;
        //                        }
        //                        else
        //                            break;
        //                    }

        //                    if (existingTransactionSimulator.SellDate.HasValue == false)
        //                    {
        //                        double price = existingTransactionSimulator.BuyPrice;
        //                        int startIndex = trasactionSortedList.IndexOfKey(existingTransactionSimulator.BuyDate);
        //                        for (int i = startIndex; i < trasactionSortedList.Count; i++)
        //                        {
        //                            TransactionData transactionData = trasactionSortedList.Values[i];
        //                            if (transactionData.Close > price)
        //                                price = transactionData.Close;
        //                            else
        //                            {
        //                                double drop = (price - transactionData.Close)/price;
        //                                double dropLevel = 0.02;
        //                                if (suggestion.SuggestedTerm == Term.Long)
        //                                    dropLevel = 0.05;
        //                                if (drop > dropLevel)
        //                                {
        //                                    if (i + 1 > trasactionSortedList.Count - 1)
        //                                        break;
        //                                    existingTransactionSimulator.SellDate =
        //                                        trasactionSortedList.Values[i + 1].TimeStamp;
        //                                    existingTransactionSimulator.SellPrice =
        //                                        (trasactionSortedList.Values[i + 1].Low +
        //                                         trasactionSortedList.Values[i + 1].High)/2;
        //                                    break;
        //                                }
        //                            }
        //                        }
        //                        if (existingTransactionSimulator.SellDate.HasValue == false)
        //                            break;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    context.SaveChanges();
        //}

        [TestMethod]
        public void GetLatestSuggestions()
        {
            StockContext context = new StockContext();
            MovingAverageAnalyzer analyzer = new MovingAverageAnalyzer();
            CandleStickPatternAnalyzer candleStickPatternAnalyzer = new CandleStickPatternAnalyzer();
            MACDSuggestionAnalyzer macdSuggestionAnalyzer = new MACDSuggestionAnalyzer();
            Console.WriteLine("Id, Name, DateTime, Action, Close, CandleStickPattern, MACD, Avg20 Trend, Avg200 Trend, Price VS Avg5,Avg5 VS Avg20");
            IList<ISuggestionAnalyzer> analyzers = new List<ISuggestionAnalyzer>();
            //analyzers.Add(new ShortTermSecondBounceAfterLongTermDownSuggestionAnalyzer());
            //analyzers.Add(new LongTermBuyAfterLongTermPrepareSuggestionAnalyzer());
            analyzers.Add(macdSuggestionAnalyzer);

            foreach (var stock in context.Stocks.Where(s => s.Key >= Properties.Settings.Default.MinStockKey && s.Key <= Properties.Settings.Default.MaxStockKey).ToList())
            {
                if (stock.AbleToGetTransactionDataFromWeb == false)
                    continue;
                //var stock = context.Stocks.First(s => s.Id == "TRP.TO");
                try
                {
                    IList<TransactionData> orderedList =
                                context.TransactionData.Where(t => t.StockKey == stock.Key).OrderBy(t => t.TimeStamp).ToList();
                    int count = orderedList.Count;
                    //MovingAverage avg5 = new MovingAverage();
                    //avg5.NumberOfTransactions = 5;
                    //avg5.Averages = new double[orderedList.Count];
                    //MovingAverage avg10 = new MovingAverage();
                    //avg10.NumberOfTransactions = 10;
                    //avg10.Averages = new double[orderedList.Count];
                    //MovingAverage avg20 = new MovingAverage();
                    //avg20.NumberOfTransactions = 20;
                    //avg20.Averages = new double[orderedList.Count];
                    //MovingAverage avg50 = new MovingAverage();
                    //avg50.NumberOfTransactions = 50;
                    //avg50.Averages = new double[orderedList.Count];
                    //MovingAverage avg100 = new MovingAverage();
                    //avg100.NumberOfTransactions = 100;
                    //avg100.Averages = new double[orderedList.Count];
                    //MovingAverage avg200 = new MovingAverage();
                    //avg200.NumberOfTransactions = 200;
                    //avg200.Averages = new double[orderedList.Count];
                    //for (int i = 0; i < orderedList.Count; i++)
                    //{
                    //    avg5.Averages[i] = orderedList[i].SimpleAvg5;
                    //    avg10.Averages[i] = orderedList[i].SimpleAvg10;
                    //    avg20.Averages[i] = orderedList[i].SimpleAvg20;
                    //    avg50.Averages[i] = orderedList[i].SimpleAvg50;
                    //    avg100.Averages[i] = orderedList[i].SimpleAvg100;
                    //    avg200.Averages[i] = orderedList[i].SimpleAvg200;
                    //}
                    //var partialMovingTrend5 = analyzer.AnalyzeMovingTrend(avg5);
                    //var partialPattern = candleStickPatternAnalyzer.GetPattern(orderedList, partialMovingTrend5);
                    //double priceMovingAvg5 = analyzer.PriceCompareAverage(orderedList, avg5);
                    //double priceMovingAvg20 = analyzer.PriceCompareAverage(orderedList, avg20);
                    //double priceMovingAvg200 = analyzer.PriceCompareAverage(orderedList, avg200);
                    //double movingAvg5_20 = analyzer.AverageCrossOver(avg5, avg20);
                    //double movingAvg50_200 = analyzer.AverageCrossOver(avg50, avg200);
                    //var movingTrend10 = analyzer.AnalyzeMovingTrend(avg10);
                    //var movingTrend20 = analyzer.AnalyzeMovingTrend(avg20);
                    //var movingTrend50 = analyzer.AnalyzeMovingTrend(avg50);
                    //var movingTrend200 = analyzer.AnalyzeMovingTrend(avg200);
                    //var signalLineCrossOver10_20_6 = movingAverageConvergenceDivergenceAnalyzer.AnalyzeSignalLineCrossOver(orderedList, avg10, avg20, 8);
                    foreach (var suggestionAnalyzer in analyzers)
                    {
                        if (suggestionAnalyzer.CalculateForecaseCertainty(orderedList) > 0)
                        {
                            Suggestion suggestion = new Suggestion();
                            suggestion.TimeStamp = orderedList[count - 1].TimeStamp;
                            suggestion.StockKey = stock.Key;
                            suggestion.StockId = stock.Id;
                            suggestion.StockName = stock.Name;
                            suggestion.ClosePrice = orderedList[count - 1].Close;
                            suggestion.Volume = orderedList[count - 1].Volume;
                            suggestion.CandleStickPattern = "Unknown";
                            //suggestion.CandleStickPattern = partialPattern.Name;
                            //suggestion.Macd = signalLineCrossOver10_20_6.Divergence;
                            //suggestion.Avg5Trend = partialMovingTrend5;
                            //suggestion.Avg20Trend = movingTrend20;
                            //suggestion.Avg200Trend = movingTrend200;
                            //suggestion.PriceVsAvg5 = priceMovingAvg5;
                            //suggestion.PriceVsAvg200 = priceMovingAvg200;
                            //suggestion.Avg5VsAvg20 = movingAvg5_20;
                            //suggestion.Avg50VsAvg200 = movingAvg50_200;
                            suggestion.AnalyzerName = suggestionAnalyzer.Name;
                            suggestion.SuggestedTerm = suggestionAnalyzer.Term;
                            suggestion.SuggestedAction = suggestionAnalyzer.Action;
                            if (context.Suggestions.Any(
                                    s =>
                                        s.StockKey == suggestion.StockKey && s.TimeStamp == suggestion.TimeStamp &&
                                        s.AnalyzerName == suggestion.AnalyzerName &&
                                        s.SuggestedAction == suggestion.SuggestedAction &&
                                        s.SuggestedTerm == suggestion.SuggestedTerm) == false)
                            {
                                context.Suggestions.Add(suggestion);
                                context.SaveChanges();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        [TestMethod]
        public void AbleToCalculateChannel()
        {
            StockContext context = new StockContext();
            StockTask stockTask = new StockTask();
            stockTask.AnalyzeTrendChannel(168, 100);
            //foreach (var stock in context.Stocks.ToList())
            //{
            //    if (context.OriginalTransactionData.Where(t => t.StockKey == stock.Key).Count() > 100)
            //    {
            //        StockTask stockTask = new StockTask();
            //        stockTask.AnalyzeTrendChannel(stock.Key, 20);
            //        stockTask.AnalyzeTrendChannel(stock.Key, 50);
            //        stockTask.AnalyzeTrendChannel(stock.Key, 100);
            //    }
            //}
        }

        [TestMethod]
        public void AbleToCalculateMovingAverageConvergenceDivergence()
        {
            StockTask stockTask = new StockTask();
            stockTask.CalculateMovingAverageConvergenceDivergence(479);
        }


        [TestMethod]
        public void AbleToGenerateSuggestionByTrendChannelAnalyzer()
        {
            StockContext context = new StockContext();
            MovingAverageAnalyzer analyzer = new MovingAverageAnalyzer();
            CandleStickPatternAnalyzer candleStickPatternAnalyzer = new CandleStickPatternAnalyzer();
            TrendChannelSuggestionAnalyzer suggestionAnalyzer = new TrendChannelSuggestionAnalyzer();
            Console.WriteLine("Id, Name, DateTime, Action, Close, CandleStickPattern, MACD, Avg20 Trend, Avg200 Trend, Price VS Avg5,Avg5 VS Avg20");
            foreach (var stock in context.Stocks.ToList())
            {
                if (stock.Key != 258)
                    continue;
                IList<TransactionData> orderedList =
                    context.TransactionData.Where(t => t.StockKey == stock.Key).OrderBy(t => t.TimeStamp).ToList();
                MovingAverage avg5 = new MovingAverage();
                avg5.NumberOfTransactions = 5;
                avg5.Averages = new double[orderedList.Count];
                MovingAverage avg10 = new MovingAverage();
                avg10.NumberOfTransactions = 10;
                avg10.Averages = new double[orderedList.Count];
                MovingAverage avg20 = new MovingAverage();
                avg20.NumberOfTransactions = 20;
                avg20.Averages = new double[orderedList.Count];
                MovingAverage avg50 = new MovingAverage();
                avg50.NumberOfTransactions = 50;
                avg50.Averages = new double[orderedList.Count];
                MovingAverage avg100 = new MovingAverage();
                avg100.NumberOfTransactions = 100;
                avg100.Averages = new double[orderedList.Count];
                MovingAverage avg200 = new MovingAverage();
                avg200.NumberOfTransactions = 200;
                avg200.Averages = new double[orderedList.Count];
                for (int i = 0; i < orderedList.Count; i++)
                {
                    avg5.Averages[i] = orderedList[i].SimpleAvg5;
                    avg10.Averages[i] = orderedList[i].SimpleAvg10;
                    avg20.Averages[i] = orderedList[i].SimpleAvg20;
                    avg50.Averages[i] = orderedList[i].SimpleAvg50;
                    avg100.Averages[i] = orderedList[i].SimpleAvg100;
                    avg200.Averages[i] = orderedList[i].SimpleAvg200;
                }
                int j = 200;
                while (j < orderedList.Count)
                {
                    try
                    {
                        var partialList = orderedList.GetFrontPartial(j);
                        var partialAvg5 = avg5.GetPartial(j);
                        var partialAvg10 = avg10.GetPartial(j);
                        var partialAvg20 = avg20.GetPartial(j);
                        var partialAvg50 = avg50.GetPartial(j);
                        var partialAvg200 = avg200.GetPartial(j);
                        var partialMovingTrend5 = analyzer.AnalyzeMovingTrend(partialAvg5);
                        var partialPattern = candleStickPatternAnalyzer.GetPattern(partialList, partialMovingTrend5);
                        double priceMovingAvg5 = analyzer.PriceCompareAverage(partialList, partialAvg5);
                        double priceMovingAvg20 = analyzer.PriceCompareAverage(partialList, partialAvg20);
                        //double priceMovingAvg200 = analyzer.PriceCompareAverage(partialList, partialAvg200);
                        double movingAvg5_20 = analyzer.AverageCrossOver(partialAvg5, partialAvg20);
                        //double movingAvg50_200 = analyzer.AverageCrossOver(partialAvg50, partialAvg200);
                        var movingTrend10 = analyzer.AnalyzeMovingTrend(partialAvg10);
                        var movingTrend20 = analyzer.AnalyzeMovingTrend(partialAvg20);
                        var movingTrend50 = analyzer.AnalyzeMovingTrend(partialAvg50);
                        var movingTrend200 = analyzer.AnalyzeMovingTrend(partialAvg200);
                        Suggestion suggestion = new Suggestion();

                        suggestion.TimeStamp = partialList[j - 1].TimeStamp;
                        suggestion.StockKey = stock.Key;
                        suggestion.StockId = stock.Id;
                        suggestion.StockName = stock.Name;
                        suggestion.ClosePrice = partialList[j - 1].Close;
                        suggestion.Volume = partialList[j - 1].Volume;
                        suggestion.CandleStickPattern = partialPattern.Name;
                        suggestion.Avg5Trend = partialMovingTrend5;
                        suggestion.Avg20Trend = movingTrend20;
                        suggestion.Avg200Trend = movingTrend200;
                        suggestion.PriceVsAvg5 = priceMovingAvg5;
                        //suggestion.PriceVsAvg200 = priceMovingAvg200;
                        suggestion.Avg5VsAvg20 = movingAvg5_20;
                        //suggestion.Avg50VsAvg200 = movingAvg50_200;


                        var forecaseCertainty = suggestionAnalyzer.CalculateForecaseCertainty(partialList);
                        if (forecaseCertainty > 0)
                        {
                            suggestion.AnalyzerName = suggestionAnalyzer.Name;
                            suggestion.SuggestedAction = suggestionAnalyzer.Action;
                            suggestion.SuggestedTerm = suggestionAnalyzer.Term;
                            suggestion.SuggestedPrice = suggestionAnalyzer.Price;
                            context.Suggestions.Add(suggestion);
                            context.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                    }
                    j++;
                }
            }
        }

        [TestMethod]
        public void VerifySuggestion()
        {
            StockContext context = new StockContext();
            SortedList<DateTime, Suggestion> suggestionSortedList = new SortedList<DateTime, Suggestion>();
            int stockKey = 479;
            double expectedAmmount = 10000;
            double existPercentage = 10;
            foreach (var suggestion in context.Suggestions.Where(s => s.StockKey == stockKey))
            {
                if (suggestionSortedList.ContainsKey(suggestion.TimeStamp) == false)
                    suggestionSortedList.Add(suggestion.TimeStamp, suggestion);
            }
            var transactionSimulators = new List<TransactionSimulator>();
            Dictionary<Term, int> availableVolume = new Dictionary<Term, int>(3);
            availableVolume.Add(Term.Short, 0);
            availableVolume.Add(Term.Intermediate, 0);
            availableVolume.Add(Term.Long, 0);
            double holdingValue = 0;
            foreach (var pair in suggestionSortedList)
            {
                if (pair.Value.SuggestedAction == Action.Buy)
                {
                    var timeStamp = pair.Key.AddDays(1);
                    if (context.TransactionData.Any(t => t.StockKey == stockKey && t.TimeStamp >= timeStamp))
                    {
                        while (context.TransactionData.Any(t => t.StockKey == stockKey && t.TimeStamp == timeStamp) == false)
                        {
                            timeStamp = timeStamp.AddDays(1);
                        }
                        var transaction = context.TransactionData.First(t => t.StockKey == stockKey && t.TimeStamp == timeStamp);
                        var transactionSimulator = new TransactionSimulator();
                        transactionSimulator.StockKey = stockKey;
                        transactionSimulator.SuggestionKey = pair.Value.Key;
                        transactionSimulator.Action = pair.Value.SuggestedAction;
                        transactionSimulator.TimeStamp = timeStamp;
                        if (pair.Value.SuggestedPrice == 0)
                            transactionSimulator.Price = transaction.Open;
                        else if (transaction.Low > pair.Value.SuggestedPrice)
                            transactionSimulator.Price = transaction.Close;
                        else if (transaction.High < pair.Value.SuggestedPrice)
                            transactionSimulator.Price = transaction.Open;
                        else
                            transactionSimulator.Price = pair.Value.SuggestedPrice.Value;
                        transactionSimulator.Volume = Convert.ToInt32(expectedAmmount/transactionSimulator.Price);
                        availableVolume[pair.Value.SuggestedTerm] += transactionSimulator.Volume;
                        transactionSimulators.Add(transactionSimulator);
                        holdingValue += expectedAmmount;
                    }
                }
                else if (pair.Value.SuggestedAction == Action.Sell)
                {
                    if (availableVolume.Sum(a => a.Value) == 0)
                        continue;
                    double holdingPrice = holdingValue/availableVolume.Sum(a => a.Value);
                    var closePrice = pair.Value.ClosePrice;
                    var exists = (holdingPrice - closePrice)/holdingPrice*100 > existPercentage;
                    if (pair.Value.SuggestedTerm == Term.Unlimited)
                        exists = true;
                    if (pair.Value.SuggestedTerm == Term.Short && holdingPrice > closePrice && exists == false)
                        continue;
                    var timeStamp = pair.Key.AddDays(1);
                    if (context.TransactionData.Any(t => t.StockKey == stockKey && t.TimeStamp >= timeStamp))
                    {
                        while (context.TransactionData.Any(t => t.StockKey == stockKey && t.TimeStamp == timeStamp) ==
                               false)
                        {
                            timeStamp = timeStamp.AddDays(1);
                        }
                        var transaction =
                            context.TransactionData.First(t => t.StockKey == stockKey && t.TimeStamp == timeStamp);
                        var transactionSimulator = new TransactionSimulator();
                        transactionSimulator.StockKey = stockKey;
                        transactionSimulator.SuggestionKey = pair.Value.Key;
                        transactionSimulator.TimeStamp = timeStamp;
                        transactionSimulator.Action = pair.Value.SuggestedAction;
                        if (pair.Value.SuggestedPrice == 0)
                            transactionSimulator.Price = transaction.Open;
                        else if (transaction.High > pair.Value.SuggestedPrice)
                            transactionSimulator.Price = transaction.Close;
                        else if (transaction.Low < pair.Value.SuggestedPrice)
                            transactionSimulator.Price = transaction.Open;
                        else
                            transactionSimulator.Price = pair.Value.SuggestedPrice.Value;
                        int suggestedVolume = Convert.ToInt32(expectedAmmount/transactionSimulator.Price);
                        if (exists)
                        {
                            suggestedVolume = availableVolume.Sum(a => a.Value);
                            transactionSimulator.Volume = suggestedVolume;
                            availableVolume[Term.Short] = 0;
                            availableVolume[Term.Intermediate] = 0;
                            availableVolume[Term.Long] = 0;
                        }
                        else
                        {
                            if (availableVolume[Term.Short] > suggestedVolume)
                            {
                                transactionSimulator.Volume = suggestedVolume;
                                availableVolume[Term.Short] -= suggestedVolume;
                            }
                            else
                            {
                                suggestedVolume = (suggestedVolume - availableVolume[Term.Short]);
                                if (pair.Value.SuggestedTerm == Term.Short)
                                    suggestedVolume = suggestedVolume/2;
                                transactionSimulator.Volume = availableVolume[Term.Short];
                                availableVolume[Term.Short] = 0;
                                if (availableVolume[Term.Intermediate] > suggestedVolume)
                                {
                                    transactionSimulator.Volume += suggestedVolume;
                                    availableVolume[Term.Intermediate] -= suggestedVolume;
                                }
                                else
                                {
                                    suggestedVolume = (suggestedVolume - availableVolume[Term.Intermediate]);
                                    if (pair.Value.SuggestedTerm != Term.Long)
                                        suggestedVolume = suggestedVolume/2;
                                    transactionSimulator.Volume += availableVolume[Term.Intermediate];
                                    availableVolume[Term.Intermediate] = 0;
                                    if (availableVolume[Term.Long] > suggestedVolume)
                                    {
                                        transactionSimulator.Volume += suggestedVolume;
                                        availableVolume[Term.Long] -= suggestedVolume;
                                    }
                                    else
                                    {
                                        transactionSimulator.Volume += availableVolume[Term.Long];
                                        availableVolume[Term.Long] = 0;
                                    }
                                }
                            }
                        }
                        transactionSimulators.Add(transactionSimulator);
                        holdingValue = holdingPrice*availableVolume.Sum(a => a.Value);
                    }
                }
            }
            context.TransactionSimulators.AddRange(transactionSimulators);
            context.SaveChanges();
            double maxInvestment = 0;
            double cost = 0;
            double accumulationCost = 0;
            double _return = 0;
            int remainVolume = 0;
            int buy = 0, sell = 0;
            int buyVolume = 0, sellVolume = 0;
            Console.WriteLine("TimeStamp,price,volume,amount,remain");
            foreach (var simulator in transactionSimulators)
            {
                var ammount = simulator.Price*simulator.Volume;
                if (simulator.Action == Action.Buy)
                {
                    buy++;
                    buyVolume += simulator.Volume;
                    cost += ammount;
                    accumulationCost += ammount;
                    remainVolume += simulator.Volume;
                    if (accumulationCost > maxInvestment)
                        maxInvestment = accumulationCost;
                    Console.WriteLine("{0},{1},{2},{3},{4}", simulator.TimeStamp, simulator.Price, simulator.Volume, -ammount, remainVolume);
                }
                if (simulator.Action == Action.Sell)
                {
                    sell++;
                    sellVolume += simulator.Volume;
                    _return += ammount;
                    accumulationCost -= ammount;
                    remainVolume -= simulator.Volume;
                    Console.WriteLine("{0},{1},{2},{3},{4}", simulator.TimeStamp, simulator.Price, -simulator.Volume, ammount, remainVolume);
                }
            }
            Console.WriteLine("Total Transactions: {0}, Buy - {1} - {3}, Sell - {2} - {4}", transactionSimulators.Count, buy,sell, buyVolume, sellVolume);
            Console.WriteLine("Profit: {0}", _return - cost);
            Console.WriteLine("Max Investment: {0}, Remain {1}", maxInvestment, remainVolume * transactionSimulators[transactionSimulators.Count - 1].Price);
        }

        [TestMethod]
        public void Learn()
        {
            StockContext context = new StockContext();
            Console.WriteLine("TimeStamp, Action, Term, Histogram, Suggested Price, Closed Price, Channel 20, Channel 50, Channel 100");
            IList<Suggestion> suggestions = context.Suggestions.Where(s => s.StockKey == 378).OrderBy(s => s.TimeStamp).ToList();
            foreach (var suggestion in suggestions)
            {
                var transactionData =
                    context.TransactionData.First(t => t.StockKey == 378 && t.TimeStamp == suggestion.TimeStamp);
                var macd = context.MovingAverageConvergenceDivergences.First(m => m.StockKey == 378 && m.TimeStamp == suggestion.TimeStamp);
                var channel20 = context.Channels.First(c => c.StockKey == 378 && c.EndDate == suggestion.TimeStamp && c.Length == 20);
                var channel50 = context.Channels.First(c => c.StockKey == 378 && c.EndDate == suggestion.TimeStamp && c.Length == 50);
                var channel100 = context.Channels.First(c => c.StockKey == 378 && c.EndDate == suggestion.TimeStamp && c.Length == 100);
                Console.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}", suggestion.TimeStamp, suggestion.SuggestedAction, suggestion.SuggestedTerm, macd.Histogram, suggestion.SuggestedPrice, transactionData.Close, channel20.ChannelTrend, channel50.ChannelTrend, channel100.ChannelTrend);
            }
        }

        [TestMethod]
        public void RerunAnalyzer()
        {
            StockContext context = new StockContext();
            var suggestionAnalyzer = new MACDSuggestionAnalyzer();
            var transactionData =
                context.TransactionData.Where(t => t.StockKey == 477 && t.TimeStamp <= new DateTime(2016, 1, 6) && t.TimeStamp >= new DateTime(2015, 1, 6))
                    .OrderBy(t => t.TimeStamp)
                    .ToList();
            var result = suggestionAnalyzer.CalculateForecaseCertainty(transactionData);
        }

        [TestMethod]
        public void AbleToGenerateSuggestionBySelctedSuggestionAnalyzer()
        {
            StockContext context = new StockContext();
            MovingAverageAnalyzer analyzer = new MovingAverageAnalyzer();
            CandleStickPatternAnalyzer candleStickPatternAnalyzer = new CandleStickPatternAnalyzer();
            IList<ISuggestionAnalyzer> suggestionAnalyzers = new List<ISuggestionAnalyzer>();
            MACDSuggestionAnalyzer macdSuggestionAnalyzer = new MACDSuggestionAnalyzer();
            TrendChannelBreakSuggestionAnalyzer trendChannelBreakSuggestionAnalyzer = new TrendChannelBreakSuggestionAnalyzer();
            suggestionAnalyzers.Add(macdSuggestionAnalyzer);
            //suggestionAnalyzers.Add(trendChannelBreakSuggestionAnalyzer);
            Console.WriteLine("Id, Name, DateTime, Action, Close, CandleStickPattern, MACD, Avg20 Trend, Avg200 Trend, Price VS Avg5,Avg5 VS Avg20");
            foreach (var stock in context.Stocks.ToList())
            {
                if (stock.Key != 479)
                    continue;
                IList<TransactionData> orderedList =
                    context.TransactionData.Where(t => t.StockKey == stock.Key).OrderBy(t => t.TimeStamp).ToList();
                MovingAverage avg5 = new MovingAverage();
                avg5.NumberOfTransactions = 5;
                avg5.Averages = new double[orderedList.Count];
                MovingAverage avg10 = new MovingAverage();
                avg10.NumberOfTransactions = 10;
                avg10.Averages = new double[orderedList.Count];
                MovingAverage avg20 = new MovingAverage();
                avg20.NumberOfTransactions = 20;
                avg20.Averages = new double[orderedList.Count];
                MovingAverage avg50 = new MovingAverage();
                avg50.NumberOfTransactions = 50;
                avg50.Averages = new double[orderedList.Count];
                MovingAverage avg100 = new MovingAverage();
                avg100.NumberOfTransactions = 100;
                avg100.Averages = new double[orderedList.Count];
                MovingAverage avg200 = new MovingAverage();
                avg200.NumberOfTransactions = 200;
                avg200.Averages = new double[orderedList.Count];
                for (int i = 0; i < orderedList.Count; i++)
                {
                    avg5.Averages[i] = orderedList[i].SimpleAvg5;
                    avg10.Averages[i] = orderedList[i].SimpleAvg10;
                    avg20.Averages[i] = orderedList[i].SimpleAvg20;
                    avg50.Averages[i] = orderedList[i].SimpleAvg50;
                    avg100.Averages[i] = orderedList[i].SimpleAvg100;
                    avg200.Averages[i] = orderedList[i].SimpleAvg200;
                }
                int j = 200;
                while (j < orderedList.Count)
                {
                    try
                    {
                        var partialList = orderedList.GetFrontPartial(j);
                        var partialAvg5 = avg5.GetPartial(j);
                        var partialAvg10 = avg10.GetPartial(j);
                        var partialAvg20 = avg20.GetPartial(j);
                        var partialAvg50 = avg50.GetPartial(j);
                        var partialAvg200 = avg200.GetPartial(j);
                        var partialMovingTrend5 = analyzer.AnalyzeMovingTrend(partialAvg5);
                        var partialPattern = candleStickPatternAnalyzer.GetPattern(partialList, partialMovingTrend5);
                        double priceMovingAvg5 = analyzer.PriceCompareAverage(partialList, partialAvg5);
                        double priceMovingAvg20 = analyzer.PriceCompareAverage(partialList, partialAvg20);
                        //double priceMovingAvg200 = analyzer.PriceCompareAverage(partialList, partialAvg200);
                        double movingAvg5_20 = analyzer.AverageCrossOver(partialAvg5, partialAvg20);
                        //double movingAvg50_200 = analyzer.AverageCrossOver(partialAvg50, partialAvg200);
                        var movingTrend10 = analyzer.AnalyzeMovingTrend(partialAvg10);
                        var movingTrend20 = analyzer.AnalyzeMovingTrend(partialAvg20);
                        var movingTrend50 = analyzer.AnalyzeMovingTrend(partialAvg50);
                        var movingTrend200 = analyzer.AnalyzeMovingTrend(partialAvg200);
                        Suggestion suggestion = new Suggestion();

                        suggestion.TimeStamp = partialList[j - 1].TimeStamp;
                        suggestion.StockKey = stock.Key;
                        suggestion.StockId = stock.Id;
                        suggestion.StockName = stock.Name;
                        suggestion.ClosePrice = partialList[j - 1].Close;
                        suggestion.Volume = partialList[j - 1].Volume;
                        suggestion.CandleStickPattern = partialPattern.Name;
                        suggestion.Avg5Trend = partialMovingTrend5;
                        suggestion.Avg20Trend = movingTrend20;
                        suggestion.Avg200Trend = movingTrend200;
                        suggestion.PriceVsAvg5 = priceMovingAvg5;
                        //suggestion.PriceVsAvg200 = priceMovingAvg200;
                        suggestion.Avg5VsAvg20 = movingAvg5_20;
                        //suggestion.Avg50VsAvg200 = movingAvg50_200;

                        foreach (var suggestionAnalyzer in suggestionAnalyzers)
                        {
                            var forecaseCertainty = suggestionAnalyzer.CalculateForecaseCertainty(partialList);
                            if (forecaseCertainty > 0)
                            {
                                suggestion.AnalyzerName = macdSuggestionAnalyzer.Name;
                                suggestion.SuggestedAction = macdSuggestionAnalyzer.Action;
                                suggestion.SuggestedTerm = macdSuggestionAnalyzer.Term;
                                suggestion.SuggestedPrice = macdSuggestionAnalyzer.Price;
                                context.Suggestions.Add(suggestion);
                                context.SaveChanges();
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                    }
                    j++;
                }
            }
        }

    }
}