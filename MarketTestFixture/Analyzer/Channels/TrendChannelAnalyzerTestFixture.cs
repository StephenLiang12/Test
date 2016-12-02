using System;
using System.Collections.Generic;
using System.Linq;
using Market.Analyzer.Channels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture.Analyzer.Channels
{
    [TestClass]
    public class TrendChannelAnalyzerTestFixture
    {
        [TestMethod]
        public void AbleToValidateChannelCalculation()
        {
            StockContext context = new StockContext();
            TrendChannelAnalyzer analyzer = new TrendChannelAnalyzer();
            IList<TransactionData> orderedList =
                context.TransactionData.Where(t => t.StockKey == 200 && t.TimeStamp >= new DateTime(2013, 3, 27) && t.TimeStamp <= new DateTime(2013, 6, 6)).OrderBy(t => t.TimeStamp).ToList();
            try
            {
                var length = 50;
                var partialList = orderedList.GetFrontPartial(length);
                var channel = analyzer.AnalyzeTrendChannel(partialList);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}