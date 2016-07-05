using System;

namespace Market
{
    public class TransactionSimulator
    {
        public long Key { get; set; }
        public int StockKey { get; set; }
        public long SuggestionKey { get; set; }
        public double BuyPrice { get; set; }
        public double? SellPrice { get; set; }
        public int Volume { get; set; }
        public DateTime BuyDate { get; set; }
        public DateTime? SellDate { get; set; }
    }
}