using System.Data.Entity.ModelConfiguration;

namespace Market.Model
{
    public class TransactionSimulatorMapping : EntityTypeConfiguration<TransactionSimulator>
    {
        public TransactionSimulatorMapping()
        {
            HasKey(t => t.Key);

            ToTable("TransactionSimulator");
        }
    }
}