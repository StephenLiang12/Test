using System;
using System.IO;
using System.Net;
using Market.TestFixture.Data;
using Market.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture.Web
{
    [TestClass]
    public class YahooFinanceTransactionWebRequestTestFixture
    {
        private const string SetCookieKey = "Set-Cookie";
        private const string ExpireKey = "expires";
        private const string PathKey = "path";
        private const string DomainKey = "domain";
        private const string CrumbStore = "CrumbStore";
        private const string Crumb = "crumb";

        [TestMethod]
        public void AbleToGetWebRequestUrl()
        {
            YahooFinanceTransactionWebRequest webRequest = new YahooFinanceTransactionWebRequest();
            webRequest.StockId = "ABB.TO";
            webRequest.EndDate = new DateTime(2017, 9, 7);
            webRequest.StartDate = new DateTime(2017, 9, 5);
            Console.WriteLine(webRequest.GenerateTransactionDataWebRequestUrl());
        }

        [TestMethod]
        public void Test()
        {
            string cookieString = string.Empty;
            WebRequest request = WebRequest.Create("https://ca.finance.yahoo.com/quote/TD.TO/history?p=TD.TO");
            request.Method = "GET";
            ((HttpWebRequest)request).UserAgent = "Mozilla/5.0 (Windows NT 6.1; Trident/7.0; rv:11.0) like Gecko";
            //request.UseDefaultCredentials = true;
            //request.PreAuthenticate = true;
            //request.Credentials = CredentialCache.DefaultCredentials;
            Cookie cookie = null;
            string crumb = null;
            HttpStatusCode statusCode = HttpStatusCode.OK;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    statusCode = response.StatusCode;
                    if (statusCode == HttpStatusCode.OK)
                    {
                        foreach (string header in response.Headers)
                        {
                            Console.WriteLine("{0}: {1}", header, response.Headers[header]);
                        }
                        cookieString = response.Headers[SetCookieKey];
                        Console.WriteLine(cookieString);
                        cookie = GetCookie(cookieString);
                        var dataStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(dataStream);
                        var writer = File.CreateText(@"c:\Test.txt");
                        do
                        {
                            string line = reader.ReadLine();
                            if (string.IsNullOrEmpty(line) == false && line.Contains(CrumbStore))
                            {
                                int cs = line.IndexOf(CrumbStore);
                                int cr = line.IndexOf(Crumb, cs + 10);
                                int cl = line.IndexOf(":", cr + 1);
                                int q1 = line.IndexOf("\"", cl + 1);
                                int q2 = line.IndexOf("\"", q1 + 1);
                                crumb = line.Substring(q1 + 1, q2 - q1 - 1);
                                crumb = crumb.Replace(@"\u002F", "/");
                            }
                            writer.WriteLine(line);
                        } while (reader.EndOfStream == false);
                        writer.Close();
                        reader.Close();
                        response.Close();
                        Console.WriteLine(crumb);
                    }
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
            }
            YahooFinanceTransactionWebRequest webRequest = new YahooFinanceTransactionWebRequest();
            webRequest.StockId = "AAR-UN.TO";
            webRequest.EndDate = new DateTime(2017, 9, 7);
            webRequest.StartDate = new DateTime(2017, 9, 5);
            var url = webRequest.GenerateTransactionDataWebRequestUrl() + "&crumb=" + crumb;
            Console.WriteLine(url);
            request = WebRequest.Create(url);
            request.Method = "GET";
            ((HttpWebRequest)request).UserAgent = "Mozilla/5.0 (Windows NT 6.1; Trident/7.0; rv:11.0) like Gecko";
            //request.Headers.Add(HttpRequestHeader.Cookie, cookieString);
            CookieContainer cookieContainer = new CookieContainer();
            ((HttpWebRequest)request).CookieContainer = cookieContainer;
            if (cookie != null)
                cookieContainer.Add(cookie);
            //request.UseDefaultCredentials = true;
            //request.PreAuthenticate = true;
            //request.Credentials = CredentialCache.DefaultCredentials;
            statusCode = HttpStatusCode.OK;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    statusCode = response.StatusCode;
                    if (statusCode == HttpStatusCode.OK)
                    {
                        var dataStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(dataStream);
                        var writer = File.CreateText(@"c:\Test.txt");
                        do
                        {
                            string line = reader.ReadLine();
                            writer.WriteLine(line);
                        } while (reader.EndOfStream == false);
                        writer.Close();
                        reader.Close();
                        response.Close();
                    }
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private Cookie GetCookie(string cookieString)
        {
            var items = cookieString.Split(';');
            if (items.Length == 0)
                return null;
            var indexOfEquator = items[0].IndexOf("=");
            var name = items[0].Substring(0, indexOfEquator);
            var value = items[0].Substring(indexOfEquator + 1, items[0].Length - indexOfEquator - 1);
            Cookie cookie = new Cookie(name, value);
            foreach (var item in items)
            {
                if (item.Trim().StartsWith(ExpireKey))
                {
                    indexOfEquator = item.IndexOf("=");
                    var expireValue = item.Substring(indexOfEquator + 1, item.Length - indexOfEquator - 1);
                    cookie.Expires = Convert.ToDateTime(expireValue);
                }
                if (item.Trim().StartsWith(PathKey))
                {
                    indexOfEquator = item.IndexOf("=");
                    cookie.Path = item.Substring(indexOfEquator + 1, item.Length - indexOfEquator - 1);
                }
                if (item.Trim().StartsWith(DomainKey))
                {
                    indexOfEquator = item.IndexOf("=");
                    cookie.Domain = item.Substring(indexOfEquator + 1, item.Length - indexOfEquator - 1);
                }
            }
            return cookie;
        }

        [TestMethod]
        public void AbleToGetSplitFromInternet()
        {
            YahooFinanceTransactionWebRequest webRequest = new YahooFinanceTransactionWebRequest();
            webRequest.StockId = "HOU.TO";
            webRequest.StartDate = new DateTime(2016,1,1);
            webRequest.EndDate = new DateTime(2017,1,1);
            webRequest.GetSplitFromInternet();
        }

        [TestMethod]
        public void AbleToGetTransactionData()
        {
            YahooFinanceTransactionWebRequest webRequest = new YahooFinanceTransactionWebRequest();
            StreamReader reader = SampleDataReader.YahooFinanceTransactionDataReader;
            string firstLine = reader.ReadLine();
            OriginalTransactionData data;
            bool hasData = webRequest.GetTransactionData(reader, out data);
            Assert.IsTrue(hasData);
            Assert.AreEqual(new DateTime(2014, 1, 2), data.TimeStamp);
            Assert.AreEqual(49.44, data.Close);
            Assert.AreEqual(49.7, data.High);
            Assert.AreEqual(49.02, data.Low);
            Assert.AreEqual(49.67, data.Open);
            Assert.AreEqual(3221200, data.Volume);
        }

        [TestMethod]
        public void AbleToGetSplit()
        {
            YahooFinanceTransactionWebRequest webRequest = new YahooFinanceTransactionWebRequest();
            StreamReader reader = SampleDataReader.YahooFinanceSplitDataReader;
            string firstLine = reader.ReadLine();
            Split split;
            bool hasData = webRequest.GetSplit(reader, out split);
            Assert.IsTrue(hasData);
            Assert.AreEqual(new DateTime(2016, 5, 30), split.TimeStamp);
            Assert.AreEqual(0.5, split.SplitRatio);
        }
    }
}