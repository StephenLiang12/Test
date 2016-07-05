using System;

namespace Market.Analyzer.Channels
{
    public class Channel
    {
        public long Key { get; set; }
        public int StockKey { get; set; }
        public Trend ChannelTrend { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double ResistanceStartPrice { get; set; }
        public double SupportStartPrice { get; set; }
        public double ResistanceChannelRatio { get; set; }
        public double SupportChannelRatio { get; set; }
        public int Length { get; set; }
    }
}