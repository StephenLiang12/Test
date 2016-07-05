using System.Data.Entity.ModelConfiguration;

namespace Market.Model
{
    public class StockMapping : EntityTypeConfiguration<Stock>
    {
        public StockMapping()
        {
            HasKey(t => t.Key);
            Property((t => t.Id)).HasMaxLength(50);
            Property((t => t.Name)).HasMaxLength(50);

            ToTable("Stock");
        }
    }
}