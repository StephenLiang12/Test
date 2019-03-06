using System;
using System.Collections.Generic;
using System.Linq;
using Market.Analyzer;
using Market.Suggestions;
using Market.Suggestions.MACD;
using Market.Suggestions.TrendChannels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture.Suggestions
{
    [TestClass]
    public class TrendChannelTriangleBreakSuggestionAnalyzerTestFixture
    {
        [TestMethod]
        public void AbleToRunTest()
        {
            StockContext context = new StockContext();
            TrendChannelTriangleBreakSuggestionAnalyzer trendChannelTriangleBreakSuggestionAnalyzer = new TrendChannelTriangleBreakSuggestionAnalyzer();
            Console.WriteLine("Id, Name, DateTime, Action, Close, CandleStickPattern, MACD, Avg20 Trend, Avg200 Trend, Price VS Avg5,Avg5 VS Avg20");
            IList<ISuggestionAnalyzer> analyzers = new List<ISuggestionAnalyzer>();
            //analyzers.Add(new ShortTermSecondBounceAfterLongTermDownSuggestionAnalyzer());
            //analyzers.Add(new LongTermBuyAfterLongTermPrepareSuggestionAnalyzer());
            analyzers.Add(trendChannelTriangleBreakSuggestionAnalyzer);

            foreach (var stock in context.Stocks.Where(s => s.Key >= Properties.Settings.Default.MinStockKey && s.Key <= Properties.Settings.Default.MaxStockKey).ToList())
            {
                if (stock.AbleToGetTransactionDataFromWeb == false)
                    continue;
                //var stock = context.Stocks.First(s => s.Id == "HOD.TO");
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
                            suggestion.Pattern = suggestionAnalyzer.Pattern;
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
    }
}