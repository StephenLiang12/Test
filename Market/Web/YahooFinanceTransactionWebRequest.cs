using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Market.Web
{
    public class YahooFinanceTransactionWebRequest : TransactionWebRequest
    {
        private const string SetCookieKey = "Set-Cookie";
        private const string ExpireKey = "expires";
        private const string PathKey = "path";
        private const string DomainKey = "domain";
        private const string CrumbStore = "CrumbStore";
        private const string Crumb = "crumb";
        private const string TransactionUrlFormat =
            "https://query1.Finance.yahoo.com/v7/finance/download/{0}?period1={1}&period2={2}&interval=1d&events=history";

        private const string SplitUrlFormat =
                "https://query1.finance.yahoo.com/v7/finance/download/{0}?period1={1}&period2={2}&interval=1d&events=split";
        private static readonly DateTime ZeroDateTime = new DateTime(1969, 12, 31, 18, 0, 0);

        private Cookie cookie;
        private string crumb;

        public override string GenerateTransactionDataWebRequestUrl()
        {
            return string.Format(TransactionUrlFormat, StockId, (StartDate - ZeroDateTime).TotalSeconds, (EndDate- ZeroDateTime).TotalSeconds);
        }

        public override string GenerateSplitWebRequestUrl()
        {
            return string.Format(SplitUrlFormat, StockId, (StartDate - ZeroDateTime).TotalSeconds, (EndDate - ZeroDateTime).TotalSeconds);
        }

        public override IEnumerable<OriginalTransactionData> GetOriginalTransactionDataFromInternet()
        {
            if (cookie == null || string.IsNullOrEmpty(crumb))
                GetCookieAndCrumb(out cookie, out crumb);
            IList<OriginalTransactionData> transactions = new List<OriginalTransactionData>();
            var url = GenerateTransactionDataWebRequestUrl() + "&crumb=" + crumb;
            //Console.WriteLine(url);
            var request = WebRequest.Create(url);
            request.Method = "GET";
            ((HttpWebRequest) request).UserAgent = "Mozilla/5.0 (Windows NT 6.1; Trident/7.0; rv:11.0) like Gecko";
            //request.Headers.Add(HttpRequestHeader.Cookie, cookieString);
            CookieContainer cookieContainer = new CookieContainer();
            ((HttpWebRequest) request).CookieContainer = cookieContainer;
            if (cookie != null)
                cookieContainer.Add(cookie);
            using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
            {
                var statusCode = response.StatusCode;
                if (statusCode == HttpStatusCode.OK)
                {
                    var dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    string firstLine = reader.ReadLine();
                    OriginalTransactionData data;
                    while (GetTransactionData(reader, out data))
                    {
                        if (transactions.Any(
                                d =>d.TimeStamp == data.TimeStamp && d.Period == data.Period) == false)
                            transactions.Add(data);
                    }
                    reader.Close();
                    response.Close();
                }
            }
            return transactions;
        }

        private void GetCookieAndCrumb(out Cookie cookie, out string crumb)
        {
            WebRequest request = WebRequest.Create("https://ca.finance.yahoo.com/quote/TD.TO/history?p=TD.TO");
            request.Method = "GET";
            ((HttpWebRequest) request).UserAgent = "Mozilla/5.0 (Windows NT 6.1; Trident/7.0; rv:11.0) like Gecko";
            cookie = null;
            crumb = null;
            HttpStatusCode statusCode = HttpStatusCode.OK;
            using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
            {
                statusCode = response.StatusCode;
                if (statusCode == HttpStatusCode.OK)
                {
                    string cookieString = response.Headers[SetCookieKey];
                    cookie = GetCookie(cookieString);
                    var dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    //var writer = File.CreateText(@"c:\Test.txt");
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
                            break;
                        }
                        //writer.WriteLine(line);
                    } while (reader.EndOfStream == false);
                    //writer.Close();
                    reader.Close();
                    response.Close();
                    //Console.WriteLine(crumb);
                }
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

        public override bool GetTransactionData(StreamReader reader, out OriginalTransactionData data)
        {
            data = new OriginalTransactionData();
            string line = reader.ReadLine();
            if (reader.EndOfStream && string.IsNullOrEmpty(line))
                return false;
            string[] items = line.Split(',');
            string[] dateItems = items[0].Split('-');
            data.TimeStamp = new DateTime(Convert.ToInt32(dateItems[0]), Convert.ToInt32(dateItems[1]), Convert.ToInt32(dateItems[2]));
            data.Period = TransactionPeriod;
            data.Close = Math.Round(Convert.ToDouble(items[4]), 2);
            data.High = Math.Round(Convert.ToDouble(items[2]), 2);
            data.Low = Math.Round(Convert.ToDouble(items[3]), 2);
            data.Open = Math.Round(Convert.ToDouble(items[1]), 2);
            data.Volume = Convert.ToDouble(items[6]);
            return true;
        }

        public override IEnumerable<Split> GetSplitFromInternet()
        {
            if (cookie == null || string.IsNullOrEmpty(crumb))
                GetCookieAndCrumb(out cookie, out crumb);
            IList<Split> splits = new List<Split>();
            var url = GenerateSplitWebRequestUrl() + "&crumb=" + crumb;
            //Console.WriteLine(url);
            var request = WebRequest.Create(url);
            request.Method = "GET";
            ((HttpWebRequest)request).UserAgent = "Mozilla/5.0 (Windows NT 6.1; Trident/7.0; rv:11.0) like Gecko";
            CookieContainer cookieContainer = new CookieContainer();
            ((HttpWebRequest)request).CookieContainer = cookieContainer;
            if (cookie != null)
                cookieContainer.Add(cookie);
            WebResponse response;
            try
            {
                response = request.GetResponse();
            }
            catch (WebException)
            {
                return splits;
            }
            var statusCode = ((HttpWebResponse)response).StatusCode;
            if (statusCode == HttpStatusCode.OK)
            {
                var dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                //var writer = File.CreateText(@"c:\Split.txt");
                //do
                //{
                //    string line = reader.ReadLine();
                //    writer.WriteLine(line);
                //} while (reader.EndOfStream == false);
                //writer.Close();
                string firstLine = reader.ReadLine();
                Split split;
                while (GetSplit(reader, out split))
                {
                    if (splits.Any(s => s.TimeStamp == split.TimeStamp))
                        continue;
                    splits.Add(split);
                }
                reader.Close();
                response.Close();
            }
            return splits;
        }

        public override bool GetSplit(StreamReader reader, out Split split)
        {
            split = new Split();
            string line = reader.ReadLine();
            if (string.IsNullOrEmpty(line))
                return false;
            string[] items = line.Split(',');
            split.TimeStamp = Convert.ToDateTime(items[0]);
            string[] ratioItems = items[1].Split('/');
            split.SplitRatio = Convert.ToDouble(ratioItems[0]) / Convert.ToDouble(ratioItems[1]);
            return true;
        }
    }
}