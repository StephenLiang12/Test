using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Market.Web
{
    public abstract class TransactionWebRequest
    {
        public string StockId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Period TransactionPeriod { get; set; }
        public abstract string GenerateTransactionDataWebRequestUrl();
        public abstract string GenerateSplitWebRequestUrl();
        public abstract bool GetTransactionData(StreamReader reader, out OriginalTransactionData data);
        public abstract bool GetSplit(StreamReader reader, out Split split);

        public virtual IEnumerable<OriginalTransactionData> GetOriginalTransactionDataFromInternet()
        {
            IList<OriginalTransactionData> transactions = new List<OriginalTransactionData>();
            WebRequest request = WebRequest.Create(GenerateTransactionDataWebRequestUrl());
            request.Method = "GET";
            ((HttpWebRequest) request).UserAgent = "Mozilla/5.0 (Windows NT 6.1; Trident/7.0; rv:11.0) like Gecko";
            HttpStatusCode statusCode = HttpStatusCode.OK;
            using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
            {
                statusCode = response.StatusCode;
                if (statusCode == HttpStatusCode.OK)
                {
                    var dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    string firstLine = reader.ReadLine();
                    OriginalTransactionData data;
                    while (GetTransactionData(reader, out data))
                    {
                        transactions.Add(data);
                    }
                    reader.Close();
                    response.Close();
                }
            }
            return transactions;
        }

        public virtual IEnumerable<Split> GetSplitFromInternet()
        {
            IList<Split> splits = new List<Split>();
            WebRequest request = WebRequest.Create(GenerateSplitWebRequestUrl());
            request.Method = "GET";
            ((HttpWebRequest)request).UserAgent = ".NET Framework Client";
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
                //var writer = File.CreateText(@"c:\Dividend.txt");
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

    }
}