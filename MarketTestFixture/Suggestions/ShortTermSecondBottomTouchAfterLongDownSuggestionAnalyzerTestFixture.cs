using System;
using System.Collections.Generic;
using System.Linq;
using Market.Analyzer;
using Market.Suggestions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture.Suggestions
{
    [TestClass]
    public class ShortTermSecondBottomTouchAfterLongDownSuggestionAnalyzerTestFixture
    {
        [TestMethod]
        public void TestEca()
        {
            StockContext context = new StockContext();
            MovingAverageAnalyzer analyzer = new MovingAverageAnalyzer();
            MovingAverageConvergenceDivergenceAnalyzer macdAnalyzer = new MovingAverageConvergenceDivergenceAnalyzer();
            CandleStickPatternAnalyzer candleStickPatternAnalyzer = new CandleStickPatternAnalyzer();
            {
                var stock = context.Stocks.First(s => s.Key == 359);
                var endDate = new DateTime(2015, 1, 5);
                var startDate = endDate.AddDays(-300);
                IList<TransactionData> orderedList =
                    context.TransactionData.Where(t =>
                        t.StockKey == stock.Key && t.TimeStamp <= endDate && t.TimeStamp >= startDate
                        ).OrderBy(t => t.TimeStamp).ToList();
                ShortTermSecondBounceAfterLongTermDownSuggestionAnalyzer suggestionAnalyzer = new ShortTermSecondBounceAfterLongTermDownSuggestionAnalyzer();
                Suggestion suggestion = new Suggestion();
                suggestion.TimeStamp = orderedList[orderedList.Count - 1].TimeStamp;
                suggestion.StockKey = stock.Key;
                suggestion.StockId = stock.Id;
                suggestion.StockName = stock.Name;
                suggestion.CandleStickPattern = string.Empty;
                suggestion.ClosePrice = orderedList[orderedList.Count - 1].Close;
                suggestion.Volume = orderedList[orderedList.Count - 1].Volume;

                if (suggestionAnalyzer.CalculateForecaseCertainty(orderedList) > 0)
                {
                    suggestion.SuggestedTerm = suggestionAnalyzer.Term;
                    suggestion.SuggestedAction = suggestionAnalyzer.Action;
                    context.Suggestions.Add(suggestion);
                    context.SaveChanges();
                }
            }
        }
    }
}