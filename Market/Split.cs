using System;

namespace Market
{
    public class Split
    {
        public Int64 Key { get; set; }
        public int StockKey { get; set; }
        public DateTime TimeStamp { get; set; } 
        public double SplitRatio { get; set; }
        public bool Applied { get; set; }
    }
}