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
                context.TransactionData.Where(t => t.StockKey == 5 && t.TimeStamp >= new DateTime(2015, 9, 15) && t.TimeStamp <= new DateTime(2016, 6, 29)).OrderBy(t => t.TimeStamp).ToList();
            try
            {
                //var length = 200;
                //var partialList = orderedList.GetFrontPartial(length);
                var channel = analyzer.AnalyzeTrendChannel(orderedList);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}