using System;

namespace Market
{
    public class Split
    {
        public int Key { get; set; }
        public int StockKey { get; set; }
        public DateTime TimeStamp { get; set; } 
        public double SplitRatio { get; set; }
    }
}