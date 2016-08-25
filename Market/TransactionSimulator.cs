using System;

namespace Market
{
    public class TransactionSimulator
    {
        public long Key { get; set; }
        public int StockKey { get; set; }
        public long SuggestionKey { get; set; }
        public Action Action { get; set; }
        public DateTime TimeStamp { get; set; }
        public double Price { get; set; }
        public int Volume { get; set; }
    }
}