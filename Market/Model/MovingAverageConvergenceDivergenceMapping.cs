using System.Data.Entity.ModelConfiguration;
using Market.Analyzer;
using Market.Analyzer.MACD;

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

    public class MovingAverageConvergenceDivergenceAnalysisMapping: EntityTypeConfiguration<MovingAverageConvergenceDivergenceAnalysis>
    {
        public MovingAverageConvergenceDivergenceAnalysisMapping()
        {
            HasKey(t => t.Key);

            ToTable("MovingAverageConvergenceDivergenceAnalysis");
        }
    }
}