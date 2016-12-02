using System;
using System.Collections.Generic;
using System.Linq;
using Market.Suggestions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture.Suggestions
{
    [TestClass]
    public class LongTermBuyAfterLongTermPrepareSuggestionAnalyzerTestFixture
    {
        [TestMethod]
        public void TestLsg()
        {
            StockContext context = new StockContext();
            {
                var stock = context.Stocks.First(s => s.Key == 199);
                var endDate = new DateTime(2014, 12, 1);
                var startDate = endDate.AddDays(-400);
                IList<TransactionData> orderedList =
                    context.TransactionData.Where(t =>
                        t.StockKey == stock.Key && t.TimeStamp <= endDate && t.TimeStamp >= startDate
                        ).OrderBy(t => t.TimeStamp).ToList();
                var suggestionAnalyzer = new LongTermBuyAfterLongTermPrepareSuggestionAnalyzer();
                Suggestion suggestion = new Suggestion();
                suggestion.TimeStamp = orderedList[orderedList.Count - 1].TimeStamp;
                suggestion.StockKey = stock.Key;
                suggestion.StockId = stock.Id;
                suggestion.StockName = stock.Name;
                suggestion.CandleStickPattern = string.Empty;
                suggestion.ClosePrice = orderedList[orderedList.Count - 1].Close;
                suggestion.Volume = orderedList[orderedList.Count - 1].Volume;

                Assert.IsTrue(suggestionAnalyzer.CalculateForecaseCertainty(orderedList) > 0);
            }
        }

    }
}