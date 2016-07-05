using System.Data.Entity.ModelConfiguration;

namespace Market.Model
{
    public class SuggestionMapping : EntityTypeConfiguration<Suggestion>
    {
        public SuggestionMapping()
        {
            HasKey(t => t.Key);
            Property((t => t.StockId)).HasMaxLength(50);
            Property((t => t.StockName)).HasMaxLength(50);

            ToTable("Suggestion");
        }
    }
}