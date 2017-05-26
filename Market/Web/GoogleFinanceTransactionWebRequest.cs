using System;
using System.IO;

namespace Market.Web
{
    public class GoogleFinanceTransactionWebRequest : TransactionWebRequest
    {
        private bool reachPriceTable = false;
        private const string TableClass = "gf-table historical_price";
        private const string DatePrefix = "<td class=\"lm\">";
        private const string PricePrefix = "<td class=\"rgt\">";
        private const string VolumePrefix = "<td class=\"rgt rm\">";
        private const string TransactionUrlFormat =
                "http://www.google.com/finance/historical?q=TSE:{0}&startdate={1}+{2}%2C+{3}&enddate={4}+{5}%2C+{6}";

        public override string GenerateTransactionDataWebRequestUrl()
        {
            reachPriceTable = false;
            return string.Format(TransactionUrlFormat, RemoveTOAtEnd(StockId), ConverMonthToAbreviation(StartDate), StartDate.Day, StartDate.Year, ConverMonthToAbreviation(EndDate), EndDate.Day, EndDate.Year);
        }

        public override string GenerateDividendWebRequestUrl()
        {
            throw new System.NotImplementedException();
        }

        public override bool GetTransactionData(StreamReader reader, out OriginalTransactionData data)
        {
            data = new OriginalTransactionData();
            string line;
            while (reachPriceTable == false)
            {
                line = reader.ReadLine();
                if (reader.EndOfStream && string.IsNullOrEmpty(line))
                    return false;
                if (line.Contains(TableClass))
                {
                    //Skip Table Header
                    for (int i = 0; i < 7; i++)
                    {
                        line = reader.ReadLine();
                    }
                    reachPriceTable = true;
                }
            }
            //Read Transaction Data
            //Read <tr>
            line = reader.ReadLine();
            //Read Timestamp
            line = reader.ReadLine();
            if (string.IsNullOrEmpty(line) || line.StartsWith(DatePrefix) == false)
                return false;
            string date = line.Remove(0, DatePrefix.Length);
            data.TimeStamp = Convert.ToDateTime(date);
            //Read Open Price
            line = reader.ReadLine();
            if (string.IsNullOrEmpty(line) || line.StartsWith(PricePrefix) == false)
                return false;
            string price = line.Remove(0, PricePrefix.Length);
            if (price != "-")
                data.Open = Convert.ToDouble(price);
            //Read High Price
            line = reader.ReadLine();
            if (string.IsNullOrEmpty(line) || line.StartsWith(PricePrefix) == false)
                return false;
            price = line.Remove(0, PricePrefix.Length);
            if (price != "-")
                data.High = Convert.ToDouble(price);
            //Read Low Price
            line = reader.ReadLine();
            if (string.IsNullOrEmpty(line) || line.StartsWith(PricePrefix) == false)
                return false;
            price = line.Remove(0, PricePrefix.Length);
            if (price != "-")
                data.Low = Convert.ToDouble(price);
            //Read Close Price
            line = reader.ReadLine();
            if (string.IsNullOrEmpty(line) || line.StartsWith(PricePrefix) == false)
                return false;
            price = line.Remove(0, PricePrefix.Length);
            data.Close = Convert.ToDouble(price);
            if (data.Open == 0)
                data.Open = data.Close;
            if (data.Low == 0)
                data.Low = data.Close;
            if (data.High == 0)
                data.High = data.Close;
            //Read Volume
            line = reader.ReadLine();
            if (string.IsNullOrEmpty(line) || line.StartsWith(VolumePrefix) == false)
                return false;
            string volume = line.Remove(0, VolumePrefix.Length);
            data.Volume = Convert.ToDouble(volume);
            data.Period = Period.Day;
            return true;
        }

        public override bool GetSplit(StreamReader reader, out Split split)
        {
            throw new System.NotImplementedException();
        }

        private string RemoveTOAtEnd(string id)
        {
            if (id.EndsWith(".TO"))
                return id.Substring(0, id.Length - 3);
            return id;
        }

        private string ConverMonthToAbreviation(DateTime date)
        {
            return date.ToString("MMM");
        }
    }
}