using System;

namespace Market
{
    public class OriginalTransactionData
    {
        public long Key { get; set; }
        public int StockKey { get; set; }
        public DateTime TimeStamp { get; set; }
        public Period Period { get; set; }
        public double Open { get; set; }
        public double Close { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Volume { get; set; }
    }
}