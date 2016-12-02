using System;
using System.IO;

namespace Market.Web
{
    public abstract class TransactionWebRequest
    {
        public string StockId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Period TransactionPeriod { get; set; }
        public abstract string GenerateTransactionDataWebRequestUrl();
        public abstract string GenerateDividendWebRequestUrl();
        public abstract bool GetTransactionData(StreamReader reader, out OriginalTransactionData data);
        public abstract bool GetSplit(StreamReader reader, out Split split);
    }
}