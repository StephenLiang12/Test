using System;
using System.Collections.Generic;
using System.Linq;
using Market.Analyzer;
using Market.Analyzer.Channels;
using Market.Suggestions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;

namespace Market.TestFixture
{
    [TestClass]
    public class IntegrationTest
    {
        [TestMethod]
        public void AbleToGetShortTermBuyingStocks()
        {
            StockContext context = new StockContext();
            MovingAverageAnalyzer analyzer = new MovingAverageAnalyzer();
            MovingAverageConvergenceDivergenceAnalyzer macdAnalyzer = new MovingAverageConvergenceDivergenceAnalyzer();
            CandleStickPatternAnalyzer candleStickPatternAnalyzer = new CandleStickPatternAnalyzer();
            Console.WriteLine("Id, Name, DateTime, Volume, Action, Close, CandleStickPattern, MACD, Avg5 Trend, Avg20 Trend, Price VS Avg5,Avg5 VS Avg20");
            foreach (var stock in context.Stocks.ToList())
            {
                if (stock.AbleToGetTransactionDataFromWeb == false)
                    continue;
                IList<TransactionData> orderedList =
                    context.TransactionDatas.Where(t => t.StockKey == stock.Key).OrderBy(t => t.TimeStamp).ToList();
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
                try
                {
                    for (int i = 2; i >=0; i--)
                    {
                        var partialList = orderedList.GetFrontPartial(orderedList.Count - i);
                        var partialAvg5 = avg5.GetPartial(orderedList.Count - i);
                        var partialAvg10 = avg10.GetPartial(orderedList.Count - i);
                        var partialAvg20 = avg20.GetPartial(orderedList.Count - i);
                        var partialAvg50 = avg50.GetPartial(orderedList.Count - i);
                        var partialAvg200 = avg200.GetPartial(orderedList.Count - i);
                        double priceMovingAvg5 = analyzer.PriceCompareAverage(partialList, partialAvg5);
                        double movingAvg5_20 = analyzer.AverageCrossOver(partialAvg5, partialAvg20);
                        var movingTrend5 = analyzer.AnalyzeMovingTrend(partialAvg5);
                        var pattern = candleStickPatternAnalyzer.GetPattern(partialList, movingTrend5);
                        var movingTrend20 = analyzer.AnalyzeMovingTrend(partialAvg20);
                        var movingTrend200 = analyzer.AnalyzeMovingTrend(partialAvg200);
                        var signalLineCrossOver10_20_6 = macdAnalyzer.AnalyzeSignalLineCrossOver(partialList, partialAvg10, partialAvg20, 8);
                        if (pattern.UpcomingTrend == Trend.Up || ((movingTrend20 == Trend.Vibration || movingTrend20 == Trend.Bottom) && priceMovingAvg5 > 0))
                            Console.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", stock.Id, stock.Name,
                                partialList[partialList.Count - 1].TimeStamp, partialList[partialList.Count - 1].Volume,
                                "Short Term Buy", partialList[partialList.Count - 1].Close, pattern.Name,
                                signalLineCrossOver10_20_6.Divergence, movingTrend5, movingTrend20, priceMovingAvg5,
                                movingAvg5_20);
                        if (pattern.UpcomingTrend == Trend.Down || ((movingTrend20 == Trend.Vibration || movingTrend20 == Trend.Top) && priceMovingAvg5 < 0))
                            Console.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", stock.Id, stock.Name,
                                partialList[partialList.Count - 1].TimeStamp, partialList[partialList.Count - 1].Volume,
                                "Short Term Sell", partialList[partialList.Count - 1].Close, pattern.Name,
                                signalLineCrossOver10_20_6.Divergence, movingTrend5, movingTrend20, priceMovingAvg5,
                                movingAvg5_20);
                    }

                }
                catch (Exception)
                {
                }
            }
        }

        [TestMethod]
        public void AbleToGetShortTermSellingStocks()
        {
            StockContext context = new StockContext();
            MovingAverageAnalyzer analyzer = new MovingAverageAnalyzer();
            MovingAverageConvergenceDivergenceAnalyzer macdAnalyzer = new MovingAverageConvergenceDivergenceAnalyzer();
            CandleStickPatternAnalyzer candleStickPatternAnalyzer = new CandleStickPatternAnalyzer();
            Console.WriteLine("Id, Name, Volume, CandleStickPattern, MACD, Avg20 Trend, Avg200 Trend, Price VS Avg5,Avg5 VS Avg20");
            foreach (var stock in context.Stocks.ToList())
            {
                if (stock.AbleToGetTransactionDataFromWeb == false)
                    continue;
                IList<TransactionData> orderedList =
                    context.TransactionDatas.Where(t => t.StockKey == stock.Key).OrderBy(t => t.TimeStamp).ToList();
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
                try
                {
                    double priceMovingAvg5 = analyzer.PriceCompareAverage(orderedList, avg5);
                    double movingAvg5_20 = analyzer.AverageCrossOver(avg5, avg20);
                    var movingTrend5 = analyzer.AnalyzeMovingTrend(avg5);
                    var pattern = candleStickPatternAnalyzer.GetPattern(orderedList, movingTrend5);
                    var movingTrend20 = analyzer.AnalyzeMovingTrend(avg20);
                    var movingTrend200 = analyzer.AnalyzeMovingTrend(avg200);
                    var signalLineCrossOver10_20_6 = macdAnalyzer.AnalyzeSignalLineCrossOver(orderedList, avg10,
                        avg20, 8);
                    if (pattern.UpcomingTrend == Trend.Down)
                        Console.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8}", stock.Id, stock.Name, stock.AvgVolume, pattern.Name, signalLineCrossOver10_20_6.Divergence, movingTrend20, movingTrend200, priceMovingAvg5, movingAvg5_20);

                }
                catch (Exception)
                {
                }
            }
        }

        [TestMethod]
        public void AbleToGetIntermediaTermBuyingStocks()
        {
            StockContext context = new StockContext();
            MovingAverageAnalyzer analyzer = new MovingAverageAnalyzer();
            MovingAverageConvergenceDivergenceAnalyzer macdAnalyzer = new MovingAverageConvergenceDivergenceAnalyzer();
            CandleStickPatternAnalyzer candleStickPatternAnalyzer = new CandleStickPatternAnalyzer();
            Console.WriteLine("Id, Name, DateTime, Volume, Action, Close, CandleStickPattern, MACD, Avg20 Trend, Avg200 Trend, Price VS Avg20,Avg5 VS Avg20");
            foreach (var stock in context.Stocks.ToList())
            {
                if (stock.AbleToGetTransactionDataFromWeb == false)
                    continue;
                IList<TransactionData> orderedList =
                    context.TransactionDatas.Where(t => t.StockKey == stock.Key).OrderBy(t => t.TimeStamp).ToList();
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
                try
                {
                    for (int i = 2; i >= 0; i--)
                    {
                        var partialList = orderedList.GetFrontPartial(orderedList.Count - i);
                        var partialAvg5 = avg5.GetPartial(orderedList.Count - i);
                        var partialAvg10 = avg10.GetPartial(orderedList.Count - i);
                        var partialAvg20 = avg20.GetPartial(orderedList.Count - i);
                        var partialAvg50 = avg50.GetPartial(orderedList.Count - i);
                        var partialAvg200 = avg200.GetPartial(orderedList.Count - i);
                        double priceMovingAvg5 = analyzer.PriceCompareAverage(partialList, partialAvg5);
                        double priceMovingAvg20 = analyzer.PriceCompareAverage(partialList, partialAvg20);
                        double movingAvg5_20 = analyzer.AverageCrossOver(partialAvg5, partialAvg20);
                        var movingTrend5 = analyzer.AnalyzeMovingTrend(partialAvg5);
                        var movingTrend50 = analyzer.AnalyzeMovingTrend(partialAvg50);
                        var pattern = candleStickPatternAnalyzer.GetPattern(partialList, movingTrend5);
                        var movingTrend20 = analyzer.AnalyzeMovingTrend(partialAvg20);
                        var movingTrend200 = analyzer.AnalyzeMovingTrend(partialAvg200);
                        var signalLineCrossOver10_20_6 = macdAnalyzer.AnalyzeSignalLineCrossOver(partialList,
                            partialAvg10, partialAvg20, 8);
                        if (priceMovingAvg20 > 0 && movingAvg5_20 > 0 && signalLineCrossOver10_20_6.Convergence <= 0 &&
                            signalLineCrossOver10_20_6.Divergence > 0 && movingTrend50 != Trend.Down)
                            Console.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", stock.Id, stock.Name,
                                partialList[partialList.Count - 1].TimeStamp, partialList[partialList.Count - 1].Volume,
                                "Inter Term Buy", partialList[partialList.Count - 1].Close, pattern.Name,
                                signalLineCrossOver10_20_6.Divergence, movingTrend20,
                                movingTrend200, priceMovingAvg5, movingAvg5_20);
                        if (priceMovingAvg20 < 0 && movingAvg5_20 < 0 && signalLineCrossOver10_20_6.Convergence >= 0 &&
                            signalLineCrossOver10_20_6.Divergence < 0 && movingTrend50 != Trend.Up)
                            Console.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", stock.Id, stock.Name,
                                partialList[partialList.Count - 1].TimeStamp, partialList[partialList.Count - 1].Volume,
                                "Inter Term Sell", partialList[partialList.Count - 1].Close, pattern.Name,
                                signalLineCrossOver10_20_6.Divergence, movingTrend20,
                                movingTrend200, priceMovingAvg20, movingAvg5_20);
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        [TestMethod]
        public void AbleToGetLongTermBuyingStocks()
        {
            StockContext context = new StockContext();
            MovingAverageAnalyzer analyzer = new MovingAverageAnalyzer();
            MovingAverageConvergenceDivergenceAnalyzer macdAnalyzer = new MovingAverageConvergenceDivergenceAnalyzer();
            CandleStickPatternAnalyzer candleStickPatternAnalyzer = new CandleStickPatternAnalyzer();
            Console.WriteLine("Id, Name, DateTime, Volume, Action, Close, CandleStickPattern, MACD, Avg20 Trend, Avg200 Trend, Price VS Avg200,Avg50 VS Avg200");
            foreach (var stock in context.Stocks.ToList())
            {
                if (stock.AbleToGetTransactionDataFromWeb == false)
                    continue;
                IList<TransactionData> orderedList =
                    context.TransactionDatas.Where(t => t.StockKey == stock.Key).OrderBy(t => t.TimeStamp).ToList();
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
                try
                {
                    for (int i = 2; i >= 0; i--)
                    {
                        var partialList = orderedList.GetFrontPartial(orderedList.Count - i);
                        var partialAvg5 = avg5.GetPartial(orderedList.Count - i);
                        var partialAvg10 = avg10.GetPartial(orderedList.Count - i);
                        var partialAvg20 = avg20.GetPartial(orderedList.Count - i);
                        var partialAvg50 = avg50.GetPartial(orderedList.Count - i);
                        var partialAvg200 = avg200.GetPartial(orderedList.Count - i);
                        double priceMovingAvg5 = analyzer.PriceCompareAverage(partialList, partialAvg5);
                        double priceMovingAvg20 = analyzer.PriceCompareAverage(partialList, partialAvg20);
                        double priceMovingAvg200 = analyzer.PriceCompareAverage(partialList, partialAvg200);
                        double movingAvg5_20 = analyzer.AverageCrossOver(partialAvg5, partialAvg20);
                        double movingAvg50_200 = analyzer.AverageCrossOver(partialAvg50, partialAvg200);
                        var movingTrend5 = analyzer.AnalyzeMovingTrend(partialAvg5);
                        var movingTrend50 = analyzer.AnalyzeMovingTrend(partialAvg50);
                        var pattern = candleStickPatternAnalyzer.GetPattern(partialList, movingTrend5);
                        var movingTrend20 = analyzer.AnalyzeMovingTrend(partialAvg20);
                        var movingTrend200 = analyzer.AnalyzeMovingTrend(partialAvg200);
                        var signalLineCrossOver10_20_6 = macdAnalyzer.AnalyzeSignalLineCrossOver(partialList,
                            partialAvg10, partialAvg20, 8);
                        if (priceMovingAvg200 > 0 && movingAvg50_200 > 0 && signalLineCrossOver10_20_6.Divergence > 0)
                            Console.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", stock.Id, stock.Name,
                                partialList[partialList.Count - 1].TimeStamp, partialList[partialList.Count - 1].Volume,
                                "Long Term Buy", partialList[partialList.Count - 1].Close, pattern.Name,
                                signalLineCrossOver10_20_6.Divergence, movingTrend20, movingTrend200, priceMovingAvg200,
                                movingAvg50_200);
                        if (priceMovingAvg200 < 0 && movingAvg50_200 < 0 && signalLineCrossOver10_20_6.Divergence < 0)
                            Console.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", stock.Id, stock.Name,
                                partialList[partialList.Count - 1].TimeStamp, partialList[partialList.Count - 1].Volume,
                                "Long Term Sell", partialList[partialList.Count - 1].Close, pattern.Name,
                                signalLineCrossOver10_20_6.Divergence, movingTrend20, movingTrend200, priceMovingAvg200,
                                movingAvg50_200);
                    }

                }
                catch (Exception)
                {
                }
            }
        }

        [TestMethod]
        public void TestSuggestion()
        {
            StockContext context = new StockContext();
            MovingAverageAnalyzer analyzer = new MovingAverageAnalyzer();
            MovingAverageConvergenceDivergenceAnalyzer macdAnalyzer = new MovingAverageConvergenceDivergenceAnalyzer();
            CandleStickPatternAnalyzer candleStickPatternAnalyzer = new CandleStickPatternAnalyzer();
            Console.WriteLine("Id, Name, DateTime, Action, Close, CandleStickPattern, MACD, Avg20 Trend, Avg200 Trend, Price VS Avg5,Avg5 VS Avg20");
            foreach (var stock in context.Stocks.ToList())
            {
                if (stock.AbleToGetTransactionDataFromWeb == false || stock.Key != 369)
                    continue;
                //var stock = context.Stocks.First(s => s.Id == "TRP.TO");
                IList<TransactionData> orderedList =
                    context.TransactionDatas.Where(t => t.StockKey == stock.Key).OrderBy(t => t.TimeStamp).ToList();
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
                        var signalLineCrossOver10_20_6 = macdAnalyzer.AnalyzeSignalLineCrossOver(partialList,
                            partialAvg10,
                            partialAvg20, 8);
                        Suggestion suggestion = new Suggestion();
                        suggestion.TimeStamp = partialList[j - 1].TimeStamp;
                        suggestion.StockKey = stock.Key;
                        suggestion.StockId = stock.Id;
                        suggestion.StockName = stock.Name;
                        suggestion.ClosePrice = partialList[j - 1].Close;
                        suggestion.Volume = partialList[j - 1].Volume;
                        suggestion.CandleStickPattern = partialPattern.Name;
                        suggestion.Macd = signalLineCrossOver10_20_6.Divergence;
                        suggestion.Avg5Trend = partialMovingTrend5;
                        suggestion.Avg20Trend = movingTrend20;
                        suggestion.Avg200Trend = movingTrend200;
                        suggestion.PriceVsAvg5 = priceMovingAvg5;
                        suggestion.PriceVsAvg200 = priceMovingAvg200;
                        suggestion.Avg5VsAvg20 = movingAvg5_20;
                        suggestion.Avg50VsAvg200 = movingAvg50_200;

                        if (partialPattern.UpcomingTrend == Trend.Up ||
                            ((movingTrend20 == Trend.Vibration || movingTrend20 == Trend.Bottom) && priceMovingAvg5 > 0))
                        {
                            suggestion.SuggestedTerm = Term.Short;
                            suggestion.SuggestedAction = Action.Buy;
                            context.Suggestions.Add(suggestion);
                            context.SaveChanges();
                        }
                        if (partialPattern.UpcomingTrend == Trend.Down ||
                            ((movingTrend20 == Trend.Vibration || movingTrend20 == Trend.Top) && priceMovingAvg5 < 0))
                        {
                            suggestion.SuggestedTerm = Term.Short;
                            suggestion.SuggestedAction = Action.Sell;
                            context.Suggestions.Add(suggestion);
                            context.SaveChanges();
                        }
                        if (priceMovingAvg20 > 0 && movingAvg5_20 > 0 && signalLineCrossOver10_20_6.Convergence <= 0 &&
                            signalLineCrossOver10_20_6.Divergence > 0 && movingTrend50 != Trend.Down)
                        {
                            suggestion.SuggestedTerm = Term.Intermediate;
                            suggestion.SuggestedAction = Action.Buy;
                            context.Suggestions.Add(suggestion);
                            context.SaveChanges();
                        }
                        if (priceMovingAvg20 < 0 && movingAvg5_20 < 0 && signalLineCrossOver10_20_6.Convergence >= 0 &&
                            signalLineCrossOver10_20_6.Divergence < 0 && movingTrend50 != Trend.Up)
                        {
                            suggestion.SuggestedTerm = Term.Intermediate;
                            suggestion.SuggestedAction = Action.Sell;
                            context.Suggestions.Add(suggestion);
                            context.SaveChanges();
                        }
                        if (priceMovingAvg200 > 0 && movingAvg50_200 > 0 && signalLineCrossOver10_20_6.Divergence > 0)
                        {
                            suggestion.SuggestedTerm = Term.Long;
                            suggestion.SuggestedAction = Action.Buy;
                            context.Suggestions.Add(suggestion);
                            context.SaveChanges();
                        }
                        if (priceMovingAvg200 < 0 && movingAvg50_200 < 0 && signalLineCrossOver10_20_6.Divergence < 0)
                        {
                            suggestion.SuggestedTerm = Term.Long;
                            suggestion.SuggestedAction = Action.Sell;
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
        public void TestSuggestionAnalyzer()
        {
            StockContext context = new StockContext();
            MovingAverageAnalyzer analyzer = new MovingAverageAnalyzer();
            MovingAverageConvergenceDivergenceAnalyzer macdAnalyzer = new MovingAverageConvergenceDivergenceAnalyzer();
            CandleStickPatternAnalyzer candleStickPatternAnalyzer = new CandleStickPatternAnalyzer();
            Console.WriteLine("Id, Name, DateTime, Action, Close, CandleStickPattern, MACD, Avg20 Trend, Avg200 Trend, Price VS Avg5,Avg5 VS Avg20");
            foreach (var stock in context.Stocks.ToList())
            {
                if (stock.AbleToGetTransactionDataFromWeb == false)
                    continue;
                //var stock = context.Stocks.First(s => s.Id == "TRP.TO");
                IList<TransactionData> orderedList =
                    context.TransactionDatas.Where(t => t.StockKey == stock.Key).OrderBy(t => t.TimeStamp).ToList();
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
                        var signalLineCrossOver10_20_6 = macdAnalyzer.AnalyzeSignalLineCrossOver(partialList,
                            partialAvg10,
                            partialAvg20, 8);
                        Suggestion suggestion = new Suggestion();
                        suggestion.TimeStamp = partialList[j - 1].TimeStamp;
                        suggestion.StockKey = stock.Key;
                        suggestion.StockId = stock.Id;
                        suggestion.StockName = stock.Name;
                        suggestion.ClosePrice = partialList[j - 1].Close;
                        suggestion.Volume = partialList[j - 1].Volume;
                        suggestion.CandleStickPattern = partialPattern.Name;
                        suggestion.Macd = signalLineCrossOver10_20_6.Divergence;
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
                    foreach (var transaction in context.TransactionDatas.Where(t => t.StockKey == stock.Key))
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

        [TestMethod]
        public void SimulateTransactions()
        {
            StockContext context = new StockContext();
            foreach (var stock in context.Stocks)
            {
                if (context.Suggestions.Any(s => s.StockKey == stock.Key))
                {
                    SortedList<DateTime, TransactionData> trasactionSortedList = new SortedList<DateTime, TransactionData>();
                    foreach (var transaction in context.TransactionDatas.Where(t => t.StockKey == stock.Key))
                    {
                        trasactionSortedList.Add(transaction.TimeStamp, transaction);
                    }
                    Dictionary<string, SortedList<DateTime, Suggestion>> suggestionSortedDic = new Dictionary<string, SortedList<DateTime, Suggestion>>();
                    foreach (var suggestion in context.Suggestions.Where(s => s.StockKey == stock.Key))
                    {
                        if (suggestionSortedDic.ContainsKey(suggestion.AnalyzerName) == false)
                            suggestionSortedDic[suggestion.AnalyzerName] = new SortedList<DateTime, Suggestion>();
                        suggestionSortedDic[suggestion.AnalyzerName].Add(suggestion.TimeStamp, suggestion);
                    }
                    foreach (var pair in suggestionSortedDic)
                    {
                        Dictionary<long, TransactionSimulator> simulcationDic = context.TransactionSimulators.Where(t => t.StockKey == stock.Key).ToDictionary(t => t.SuggestionKey);
                        TransactionSimulator existingTransactionSimulator = null;
                        foreach (var suggestion in pair.Value.Values)
                        {
                            if (existingTransactionSimulator == null && simulcationDic.ContainsKey(suggestion.Key))
                                existingTransactionSimulator = simulcationDic[suggestion.Key];
                            if (existingTransactionSimulator == null || (existingTransactionSimulator.SellDate.HasValue && existingTransactionSimulator.SellDate <= suggestion.TimeStamp))
                            {
                                int index = trasactionSortedList.IndexOfKey(suggestion.TimeStamp);
                                if (index < trasactionSortedList.Count - 1)
                                {
                                    var nextDayTransaction = trasactionSortedList.Values[index + 1];
                                    var newTransactionSimulator = new TransactionSimulator();
                                    newTransactionSimulator.StockKey = stock.Key;
                                    newTransactionSimulator.SuggestionKey = suggestion.Key;
                                    newTransactionSimulator.BuyDate = nextDayTransaction.TimeStamp;
                                    newTransactionSimulator.BuyPrice = (nextDayTransaction.Low +
                                                                        nextDayTransaction.High)/2;
                                    newTransactionSimulator.Volume =
                                        Convert.ToInt32(50000/newTransactionSimulator.BuyPrice);
                                    context.TransactionSimulators.Add(newTransactionSimulator);
                                    existingTransactionSimulator = newTransactionSimulator;
                                }
                                else
                                    break;
                            }

                            if (existingTransactionSimulator.SellDate.HasValue == false)
                            {
                                double price = existingTransactionSimulator.BuyPrice;
                                int startIndex = trasactionSortedList.IndexOfKey(existingTransactionSimulator.BuyDate);
                                for (int i = startIndex; i < trasactionSortedList.Count; i++)
                                {
                                    TransactionData transactionData = trasactionSortedList.Values[i];
                                    if (transactionData.Close > price)
                                        price = transactionData.Close;
                                    else
                                    {
                                        double drop = (price - transactionData.Close)/price;
                                        double dropLevel = 0.02;
                                        if (suggestion.SuggestedTerm == Term.Long)
                                            dropLevel = 0.05;
                                        if (drop > dropLevel)
                                        {
                                            if (i + 1 > trasactionSortedList.Count - 1)
                                                break;
                                            existingTransactionSimulator.SellDate =
                                                trasactionSortedList.Values[i + 1].TimeStamp;
                                            existingTransactionSimulator.SellPrice =
                                                (trasactionSortedList.Values[i + 1].Low +
                                                 trasactionSortedList.Values[i + 1].High)/2;
                                            break;
                                        }
                                    }
                                }
                                if (existingTransactionSimulator.SellDate.HasValue == false)
                                    break;
                            }
                        }
                    }
                }
            }
            context.SaveChanges();
        }

        [TestMethod]
        public void GetLatestSuggestions()
        {
            StockContext context = new StockContext();
            MovingAverageAnalyzer analyzer = new MovingAverageAnalyzer();
            MovingAverageConvergenceDivergenceAnalyzer macdAnalyzer = new MovingAverageConvergenceDivergenceAnalyzer();
            CandleStickPatternAnalyzer candleStickPatternAnalyzer = new CandleStickPatternAnalyzer();
            Console.WriteLine("Id, Name, DateTime, Action, Close, CandleStickPattern, MACD, Avg20 Trend, Avg200 Trend, Price VS Avg5,Avg5 VS Avg20");
            IList<ISuggestionAnalyzer> analyzers = new List<ISuggestionAnalyzer>();
            analyzers.Add(new ShortTermSecondBounceAfterLongTermDownSuggestionAnalyzer());
            analyzers.Add(new LongTermBuyAfterLongTermPrepareSuggestionAnalyzer());

            foreach (var stock in context.Stocks.ToList())
            {
                if (stock.AbleToGetTransactionDataFromWeb == false)
                    continue;
                //var stock = context.Stocks.First(s => s.Id == "TRP.TO");
                try
                {
                    IList<TransactionData> orderedList =
                                context.TransactionDatas.Where(t => t.StockKey == stock.Key).OrderBy(t => t.TimeStamp).ToList();
                    int count = orderedList.Count;
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
                    var partialMovingTrend5 = analyzer.AnalyzeMovingTrend(avg5);
                    var partialPattern = candleStickPatternAnalyzer.GetPattern(orderedList, partialMovingTrend5);
                    double priceMovingAvg5 = analyzer.PriceCompareAverage(orderedList, avg5);
                    double priceMovingAvg20 = analyzer.PriceCompareAverage(orderedList, avg20);
                    double priceMovingAvg200 = analyzer.PriceCompareAverage(orderedList, avg200);
                    double movingAvg5_20 = analyzer.AverageCrossOver(avg5, avg20);
                    double movingAvg50_200 = analyzer.AverageCrossOver(avg50, avg200);
                    var movingTrend10 = analyzer.AnalyzeMovingTrend(avg10);
                    var movingTrend20 = analyzer.AnalyzeMovingTrend(avg20);
                    var movingTrend50 = analyzer.AnalyzeMovingTrend(avg50);
                    var movingTrend200 = analyzer.AnalyzeMovingTrend(avg200);
                    var signalLineCrossOver10_20_6 = macdAnalyzer.AnalyzeSignalLineCrossOver(orderedList, avg10, avg20, 8);
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
                            suggestion.CandleStickPattern = partialPattern.Name;
                            suggestion.Macd = signalLineCrossOver10_20_6.Divergence;
                            suggestion.Avg5Trend = partialMovingTrend5;
                            suggestion.Avg20Trend = movingTrend20;
                            suggestion.Avg200Trend = movingTrend200;
                            suggestion.PriceVsAvg5 = priceMovingAvg5;
                            suggestion.PriceVsAvg200 = priceMovingAvg200;
                            suggestion.Avg5VsAvg20 = movingAvg5_20;
                            suggestion.Avg50VsAvg200 = movingAvg50_200;
                            suggestion.AnalyzerName = suggestionAnalyzer.Name;
                            suggestion.SuggestedTerm = suggestionAnalyzer.Term;
                            suggestion.SuggestedAction = suggestionAnalyzer.Action;
                            context.Suggestions.Add(suggestion);
                            context.SaveChanges();
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        [TestMethod]
        public void AbleToCalculateChannel()
        {
            StockContext context = new StockContext();
            TrendChannelAnalyzer analyzer = new TrendChannelAnalyzer();
            foreach (var stock in context.Stocks.ToList())
            {
                if (stock.Key != 200)
                    continue;
                IList<TransactionData> orderedList =
                    context.TransactionDatas.Where(t => t.StockKey == stock.Key).OrderBy(t => t.TimeStamp).ToList();
                try
                {
                    var partialList = orderedList.GetFrontPartial(100);
                    for (int i = 100; i < orderedList.Count; i++)
                    {
                        var channel = analyzer.AnalyzeTrendChannel(partialList);
                        context.Channels.Add(channel);
                        context.SaveChanges();
                        partialList.RemoveAt(0);
                        partialList.Add(orderedList[i]);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

    }
}