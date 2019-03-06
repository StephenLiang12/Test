using System;
using System.Collections.Generic;
using System.Linq;
using Market.Suggestions;
using Market.Suggestions.TrendChannels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture.Suggestions
{
    [TestClass]
    public class TrendChannelBreakSuggestionAnalyzerTestFixture
    {
        [TestMethod]
        public void AbleToRunTest()
        {
            StockContext context = new StockContext();
            TrendChannelBreakSuggestionAnalyzer trendChannelBreakSuggestionAnalyzer = new TrendChannelBreakSuggestionAnalyzer();
            Console.WriteLine("Id, Name, DateTime, Action, Close, CandleStickPattern, MACD, Avg20 Trend, Avg200 Trend, Price VS Avg5,Avg5 VS Avg20");
            IList<ISuggestionAnalyzer> analyzers = new List<ISuggestionAnalyzer>();
            analyzers.Add(trendChannelBreakSuggestionAnalyzer);

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