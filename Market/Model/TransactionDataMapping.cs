using System.Data.Entity.ModelConfiguration;

namespace Market.Model
{
    public class TransactionDataMapping: EntityTypeConfiguration<TransactionData>
    {
        public TransactionDataMapping()
        {
            HasKey(t => t.Key);

            ToTable("TransactionData");
        }
    }
}