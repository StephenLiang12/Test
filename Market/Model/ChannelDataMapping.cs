using System.Data.Entity.ModelConfiguration;
using Market.Analyzer.Channels;

namespace Market.Model
{
    public class ChannelDataMapping : EntityTypeConfiguration<Channel>
    {
        public ChannelDataMapping()
        {
            HasKey(t => t.Key);

            ToTable("Channel");
        }
    }
}