using System;
using System.Collections.Generic;
using System.IO;
using Market.TestFixture.Data;
using Market.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture.Web
{
    [TestClass]
    public class GoogleFinanceTransactionWebRequestTestFixture
    {
        [TestMethod]
        public void AbleToGetWebRequestUrl()
        {
            GoogleFinanceTransactionWebRequest webRequest = new GoogleFinanceTransactionWebRequest();
            webRequest.StockId = "TD.TO";
            webRequest.EndDate = new DateTime(2017, 9, 6);
            webRequest.StartDate = new DateTime(2017, 2, 1);
            Console.WriteLine(webRequest.GenerateTransactionDataWebRequestUrl());
            var dateTime1 = new DateTime(2017, 9, 2);
            var dateTime2 = new DateTime(2017, 9, 7);
            Console.WriteLine(dateTime2.AddSeconds(-1504764000));
            Console.WriteLine((dateTime2.AddSeconds(-1504764000) - dateTime1).TotalSeconds);
        }

        [TestMethod]
        public void AbleToGetTransactionData()
        {
            GoogleFinanceTransactionWebRequest webRequest = new GoogleFinanceTransactionWebRequest();
            StreamReader reader = SampleDataReader.GoogleFinanceTransactionDataReader;
            string firstLine = reader.ReadLine();
            IList<OriginalTransactionData> data = new List<OriginalTransactionData>();
            OriginalTransactionData transaction;
            while (webRequest.GetTransactionData(reader, out transaction))
            {
                data.Add(transaction);
            }
            Assert.AreEqual(30, data.Count);
            Assert.AreEqual(new DateTime(2012, 8, 2), data[0].TimeStamp);
            Assert.AreEqual(39.10, data[0].Open);
            Assert.AreEqual(39.34, data[0].High);
            Assert.AreEqual(38.89, data[0].Low);
            Assert.AreEqual(39, data[0].Close);
            Assert.AreEqual(3219688, data[0].Volume);
            Assert.AreEqual(new DateTime(2012, 6, 21), data[29].TimeStamp);
            Assert.AreEqual(40.30, data[29].Open);
            Assert.AreEqual(40.46, data[29].High);
            Assert.AreEqual(39.12, data[29].Low);
            Assert.AreEqual(39.18, data[29].Close);
            Assert.AreEqual(4157894, data[29].Volume);
        }

    }
}