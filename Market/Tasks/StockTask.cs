using System;
using System.IO;
using System.Linq;
using System.Net;
using Market.Web;
using Market.Web.Factory;

namespace Market.Tasks
{
    public class StockTask : IStockTask
    {
        private readonly StockContext context;
        private readonly ITransactionWebRequestFactory webRequestFactory;

        public StockTask()
        {
            context = new StockContext();
            webRequestFactory = new TransactionWebRequestFactory();
        }

        public int AddStockFromEodData(StreamReader reader)
        {
            string firstLine = reader.ReadLine();
            int count = 0;
            do
            {
                string line = reader.ReadLine();
                string[] items = line.Split(',');
                double volume = Convert.ToDouble(items[5]);
                string stockid = items[0];
                if (volume < 100000 || context.Stocks.Any(s => s.Id == stockid))
                    continue;
                Stock stock = new Stock();
                stock.Id = stockid + ".TO";
                stock.Name = items[1];
                stock.AvgVolume = volume;
                context.Stocks.Add(stock);
                context.SaveChanges();
                count++;
            } while (reader.EndOfStream == false);
            return count;
        }

        public HttpStatusCode GetTransactionDataFromInternet(string stockId)
        {
            var webRequest = webRequestFactory.CreateTransactionWebRequest(stockId);
            WebRequest request = WebRequest.Create(webRequest.GenerateTransactionDataWebRequestUrl());
            request.Method = "GET";
            ((HttpWebRequest)request).UserAgent = ".NET Framework Client";
            WebResponse response;
            try
            {
                response = request.GetResponse();
            }
            catch (WebException)
            {
                var stock = context.Stocks.First(s => s.Id == stockId);
                stock.AbleToGetTransactionDataFromWeb = false;
                context.SaveChanges();
                return HttpStatusCode.NotFound;
            }
            var statusCode = ((HttpWebResponse)response).StatusCode;
            if (statusCode == HttpStatusCode.OK)
            {
                var dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                //var writer = File.CreateText(@"c:\Test.txt");
                //do
                //{
                //    string line = reader.ReadLine();
                //    writer.WriteLine(line);
                //} while (reader.EndOfStream == false);
                //writer.Close();
                if (context.Stocks.Any(s => s.Id == stockId) == false)
                {
                    throw new ArgumentException("Unknow Stock Id {0}", stockId);
                }
                var stock = context.Stocks.First(s => s.Id == stockId);
                string firstLine = reader.ReadLine();
                double sumOfVolume = 0;
                int count = 0;
                TransactionData data;
                while (webRequest.GetTransactionData(reader, out data))
                {
                    data.StockKey = stock.Key;
                    if (context.TransactionDatas.Any(
                            d =>
                                d.StockKey == data.StockKey && d.TimeStamp == data.TimeStamp &&
                                d.Period == data.Period) == false)
                        context.TransactionDatas.Add(data);
                    sumOfVolume += data.Volume;
                    count++;
                }
                if (count == 0)
                    return statusCode;
                stock.AvgVolume = Math.Round(sumOfVolume/count);
                stock.AbleToGetTransactionDataFromWeb = true;
                context.SaveChanges();
                reader.Close();
                response.Close();
            }
            return statusCode;
        }

        public HttpStatusCode GetSplitFromInternet(string stockId)
        {
            var webRequest = webRequestFactory.CreateTransactionWebRequest(stockId);
            WebRequest request = WebRequest.Create(webRequest.GenerateDividendWebRequestUrl());
            request.Method = "GET";
            ((HttpWebRequest)request).UserAgent = ".NET Framework Client";
            WebResponse response;
            try
            {
                response = request.GetResponse();
            }
            catch (WebException)
            {
                return HttpStatusCode.NotFound;
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
                if (context.Stocks.Any(s => s.Id == stockId) == false)
                {
                    throw new ArgumentException("Unknow Stock Id {0}", stockId);
                }
                var stock = context.Stocks.First(s => s.Id == stockId);
                string firstLine = reader.ReadLine();
                Split split;
                while (webRequest.GetSplit(reader, out split))
                {
                    split.StockKey = stock.Key;
                    DateTime splitTimeStamp = split.TimeStamp;
                    if (context.Splits.Any(s => s.StockKey == stock.Key && s.TimeStamp == splitTimeStamp))
                        continue;
                    context.Splits.Add(split);
                    foreach (var transaction in context.TransactionDatas.Where(t => t.StockKey == stock.Key && t.TimeStamp < splitTimeStamp))
                    {
                        transaction.Open /= split.SplitRatio;
                        transaction.Close /= split.SplitRatio;
                        transaction.High /= split.SplitRatio;
                        transaction.Low /= split.SplitRatio;
                    }
                }
                context.SaveChanges();
                reader.Close();
                response.Close();
            }
            return statusCode;
        }
    }
}