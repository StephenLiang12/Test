using System.Data.Entity.ModelConfiguration;

namespace Market.Model
{
    public class SplitMapping : EntityTypeConfiguration<Split>
    {
        public SplitMapping()
        {
            HasKey(t => t.Key);

            ToTable("Split");
        }
    }
}