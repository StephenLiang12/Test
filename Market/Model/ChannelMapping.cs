using System.Data.Entity.ModelConfiguration;
using Market.Analyzer.Channels;

namespace Market.Model
{
    public class ChannelMapping : EntityTypeConfiguration<Channel>
    {
        public ChannelMapping()
        {
            HasKey(t => t.Key);

            ToTable("Channel");
        }
    }
    public class TrendChannelBreakAnalysisMapping : EntityTypeConfiguration<TrendChannelBreakAnalysis>
    {
        public TrendChannelBreakAnalysisMapping()
        {
            HasKey(t => t.Key);

            ToTable("TrendChannelBreakAnalysis");
        }
    }
}