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
}