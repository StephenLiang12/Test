using System.Data.Entity.ModelConfiguration;

namespace Market.Model
{
    public class TransactionSimulatorDataMapping : EntityTypeConfiguration<TransactionSimulator>
    {
        public TransactionSimulatorDataMapping()
        {
            HasKey(t => t.Key);

            ToTable("TransactionSimulator");
        }
    }
}