using System;

namespace Market
{
    public class TransactionData
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
        public double SimpleAvg5 { get; set; }
        public double SimpleAvg10 { get; set; }
        public double SimpleAvg20 { get; set; }
        public double SimpleAvg50 { get; set; }
        public double SimpleAvg100 { get; set; }
        public double SimpleAvg200 { get; set; }
    }
}