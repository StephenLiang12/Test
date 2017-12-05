using System;
using System.IO;

namespace Market.Web
{
    public class YahooFinanceTransactionWebRequest : TransactionWebRequest
    {
        private const string TransactionUrlFormat =
            "https://query1.Finance.yahoo.com/v7/finance/download/{0}?period1={1}&period2={2}&interval=1d&events=history";
        private static readonly DateTime ZeroDateTime = new DateTime(1969, 12, 31, 18, 0, 0);

        public override string GenerateTransactionDataWebRequestUrl()
        {
            return string.Format(TransactionUrlFormat, StockId, (StartDate - ZeroDateTime).TotalSeconds, (EndDate- ZeroDateTime).TotalSeconds);
        }

        public override string GenerateDividendWebRequestUrl()
        {
            throw new System.NotImplementedException();
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

        public override bool GetSplit(StreamReader reader, out Split split)
        {
            throw new System.NotImplementedException();
        }
    }
}