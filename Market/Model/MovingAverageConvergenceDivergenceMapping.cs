using System.Data.Entity.ModelConfiguration;
using Market.Analyzer;

namespace Market.Model
{
    public class MovingAverageConvergenceDivergenceMapping: EntityTypeConfiguration<MovingAverageConvergenceDivergence>
    {
        public MovingAverageConvergenceDivergenceMapping()
        {
            HasKey(t => t.Key);

            ToTable("MovingAverageConvergenceDivergence");
        }
    }
}