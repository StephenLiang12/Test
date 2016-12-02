using System.Data.Entity.ModelConfiguration;

namespace Market.Model
{
    public class OriginalTransactionDataMapping : EntityTypeConfiguration<OriginalTransactionData>
    {
        public OriginalTransactionDataMapping()
        {
            HasKey(t => t.Key);

            ToTable("OriginalTransactionData");
        }
    }
}