using System;
using Market.Analyzer;

namespace Market
{
    public class Suggestion
    {
        //Console.WriteLine("Id, Name, DateTime, Volume, Action, Close, CandleStickPattern, MACD, Avg5 Trend, Avg20 Trend, Price VS Avg5,Avg5 VS Avg20");
        public long Key { get; set; }
        public DateTime TimeStamp { get; set; }
        public int StockKey { get; set; }
        public string StockId { get; set; }
        public string StockName { get; set; }
        public double ClosePrice { get; set; }
        public double Volume { get; set; }
        public string AnalyzerName { get; set; }
        public Action SuggestedAction { get; set; }
        public Term SuggestedTerm { get; set; }
        public string CandleStickPattern { get; set; }
        public double Macd { get; set; }
        public Trend Avg5Trend { get; set; }
        public Trend Avg20Trend { get; set; }
        public Trend Avg200Trend { get; set; }
        public double PriceVsAvg5 { get; set; }
        public double PriceVsAvg200 { get; set; }
        public double Avg5VsAvg20 { get; set; }
        public double Avg50VsAvg200 { get; set; }
        public double Accuracy { get; set; }
    }
}