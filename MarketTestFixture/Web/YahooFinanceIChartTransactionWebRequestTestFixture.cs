using System;
using System.IO;
using Market.TestFixture.Data;
using Market.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture.Web
{
    [TestClass]
    public class YahooFinanceIChartTransactionWebRequestTestFixture
    {
        [TestMethod]
        public void AbleToGetWebRequestUrl()
        {
            YahooFinanceIchartTransactionWebRequest webRequest = new YahooFinanceIchartTransactionWebRequest();
            webRequest.StockId = "TD.TO";
            webRequest.EndDate = new DateTime(2014,2,1);
            webRequest.StartDate = new DateTime(2014,1,1);
            string expectedUrl =
                "http://ichart.yahoo.com/table.csv?s=TD.TO&a=0&b=1&c=2014&d=1&e=1&f=2014&g=d&ignore=.csv";
            Assert.AreEqual(expectedUrl, webRequest.GenerateTransactionDataWebRequestUrl());
        }

        [TestMethod]
        [Ignore]
        public void AbleToGetStockInfo()
        {
            YahooFinanceIchartTransactionWebRequest webRequest = new YahooFinanceIchartTransactionWebRequest();
            var stock = webRequest.GetStockInfo(SampleDataReader.YahooFinanceTransactionDataReader);
            Assert.AreEqual("TD.TO", stock.Id);
            Assert.AreEqual("TORONTO-DOMINION BANK", stock.Name);
            Assert.AreEqual(Period.Day, webRequest.TransactionPeriod);
            Assert.AreEqual(717800, stock.AvgVolume);
        }

        [TestMethod]
        public void AbleToGetTransactionData()
        {
            YahooFinanceIchartTransactionWebRequest webRequest = new YahooFinanceIchartTransactionWebRequest();
            StreamReader reader = SampleDataReader.YahooFinanceTransactionDataReader;
            string firstLine = reader.ReadLine();
            OriginalTransactionData data;
            bool hasData = webRequest.GetTransactionData(reader, out data);
            Assert.IsTrue(hasData);
            Assert.AreEqual(new DateTime(2014, 9, 29), data.TimeStamp);
            Assert.AreEqual(3.55, data.Close);
            Assert.AreEqual(3.57, data.High);
            Assert.AreEqual(3.17, data.Low);
            Assert.AreEqual(3.32, data.Open);
            Assert.AreEqual(23800, data.Volume);
        }

        [TestMethod]
        public void AbleToGetSplit()
        {
            YahooFinanceIchartTransactionWebRequest webRequest = new YahooFinanceIchartTransactionWebRequest();
            StreamReader reader = SampleDataReader.YahooFinanceDividendReader;
            string firstLine = reader.ReadLine();
            Split split;
            bool hasData = webRequest.GetSplit(reader, out split);
            Assert.IsTrue(hasData);
            Assert.AreEqual(new DateTime(2014, 2, 3), split.TimeStamp);
            Assert.AreEqual(2, split.SplitRatio);
            hasData = webRequest.GetSplit(reader, out split);
            Assert.IsFalse(hasData);
        }
    }
}