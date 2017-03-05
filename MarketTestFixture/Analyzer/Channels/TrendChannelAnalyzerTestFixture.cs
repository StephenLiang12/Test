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
                context.TransactionData.Where(t => t.StockKey == 378 && t.TimeStamp >= new DateTime(2011, 9, 8) && t.TimeStamp <= new DateTime(2012, 1, 31)).OrderBy(t => t.TimeStamp).ToList();
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