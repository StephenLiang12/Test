using System;
using System.IO;
using Market.Exceptions;

namespace Market.Web
{
    public class YahooFinanceTransactionWebRequest : TransactionWebRequest
    {
        private const string TransactionUrlFormat =
            "http://ichart.yahoo.com/table.csv?s={0}&a={1}&b={2}&c={3}&d={4}&e={5}&f={6}&g={7}&ignore=.csv";
        private const string DividendUrlFormat =
            "http://ichart.yahoo.com/x?s={0}&a={1}&b={2}&c={3}&d={4}&e={5}&f={6}&g=v&ignore=.csv";

        private const int StartLine = 18;
        private const string Ticker = "ticker:";
        private const string CompanyName = "Company-Name:";
        private const string PreviousClosePrice = "previous_close_price:";
        private const string Unit = "unit:";
        private const string Volume = "volume:";
        private const string Split = "SPLIT";

        public override string GenerateTransactionDataWebRequestUrl()
        {
            return string.Format(TransactionUrlFormat, StockId, StartDate.Month - 1, StartDate.Day, StartDate.Year, EndDate.Month - 1, EndDate.Day, EndDate.Year, "d");
        }

        public override string GenerateDividendWebRequestUrl()
        {
            return string.Format(DividendUrlFormat, StockId, StartDate.Month - 1, StartDate.Day, StartDate.Year, EndDate.Month - 1, EndDate.Day, EndDate.Year);
        }

        public Stock GetStockInfo(StreamReader reader)
        {
            string[] lines = new string[18];
            bool noPreviousClose = false;
            for (int i = 0; i < StartLine; i++)
            {
                lines[i] = reader.ReadLine();
                if (i == 3 && lines[i].Contains("No symbol found"))
                    throw new NoSymolFoundException(string.Format("Cannot find Symbol: {0}", StockId));
                if (i == 9 && lines[i].Contains(PreviousClosePrice) == false)
                    noPreviousClose = true;
                if (noPreviousClose && i == 16)
                    break;
                if (reader.EndOfStream)
                    throw new ArgumentException("Data cannot be parsed by YahooFinanceTransactionWebRequest");
            }
            Stock stock = new Stock();
            if (lines[1].StartsWith(Ticker))
                stock.Id = lines[1].Remove(0, Ticker.Length).ToUpper();
            else
                throw new ArgumentException("Cannot get ticker in second line from YahooFinanceTransactionWebRequest");
            if (lines[2].StartsWith(CompanyName))
                stock.Name = lines[2].Remove(0, CompanyName.Length).ToUpper();
            else
                throw new ArgumentException("Cannot get Company Name in third line from YahooFinanceTransactionWebRequest");
            if (lines[4].StartsWith(Unit))
            {
                string period = lines[4].Remove(0, Unit.Length);
                TransactionPeriod = (Period)Enum.Parse(typeof(Period), Capitalize(period));
            }
            else
                throw new ArgumentException("Cannot get Unit in fifth line from YahooFinanceTransactionWebRequest");
            int volumeLineNumber = 17;
            if (noPreviousClose)
                volumeLineNumber = 16;
            if (lines[volumeLineNumber].StartsWith(Volume))
            {
                string volumeRange = lines[volumeLineNumber].Remove(0, Volume.Length);
                string[] volumes = volumeRange.Split(',');
                stock.AvgVolume = Convert.ToDouble(volumes[0]);
            }
            else
                throw new ArgumentException("Cannot get Volume in eighteenth line from YahooFinanceTransactionWebRequest");
            return stock;
        }

        private string Capitalize(string s)
        {
            if (s.Length == 0)
                return s;
            string fistCharacter = s.Substring(0, 1).ToUpper();
            string rest = s.Remove(0, 1).ToLower();
            return fistCharacter + rest;
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
            data.Close = Convert.ToDouble(items[4]);
            data.High = Convert.ToDouble(items[2]);
            data.Low = Convert.ToDouble(items[3]);
            data.Open = Convert.ToDouble(items[1]);
            data.Volume = Convert.ToDouble(items[5]);
            return true;
        }

        public override bool GetSplit(StreamReader reader, out Split split)
        {
            split = new Split();
            do
            {
                string line = reader.ReadLine();
                if (line.StartsWith(Split))
                {
                    string[] items = line.Split(',');
                    string timeStampString = items[1].Trim(' ');
                    split.TimeStamp = new DateTime(Convert.ToInt32(timeStampString.Substring(0, 4)),
                        Convert.ToInt32(timeStampString.Substring(4, 2)),
                        Convert.ToInt32(timeStampString.Substring(6, 2)));
                    string[] ratioItems = items[2].Split(':');
                    split.SplitRatio = Convert.ToDouble(ratioItems[0])/Convert.ToDouble(ratioItems[1]);
                    return true;
                }
            } while (reader.EndOfStream == false);
            return false;
        }
    }
}