using System.Data.Entity;
using Market.Analyzer;
using Market.Analyzer.Channels;
using Market.Analyzer.MACD;
using Market.Model;

namespace Market
{
    public class StockContext: BaseContext<StockContext>
    {
        public StockContext() : base("StockContext")
        {}

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<OriginalTransactionData> OriginalTransactionData { get; set; }
        public DbSet<TransactionData> TransactionData { get; set; }
        public DbSet<Split> Splits { get; set; }
        public DbSet<Suggestion> Suggestions { get; set; }
        public DbSet<TransactionSimulator> TransactionSimulators { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<MovingAverageConvergenceDivergence> MovingAverageConvergenceDivergences { get; set; }
        public DbSet<MovingAverageConvergenceDivergenceAnalysis> MovingAverageConvergenceDivergenceAnalyses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new StockMapping());
            modelBuilder.Configurations.Add(new OriginalTransactionDataMapping());
            modelBuilder.Configurations.Add(new TransactionDataMapping());
            modelBuilder.Configurations.Add(new SplitMapping());
            modelBuilder.Configurations.Add(new SuggestionMapping());
            modelBuilder.Configurations.Add(new TransactionSimulatorMapping());
            modelBuilder.Configurations.Add(new ChannelMapping());
            modelBuilder.Configurations.Add(new MovingAverageConvergenceDivergenceMapping());
            modelBuilder.Configurations.Add(new MovingAverageConvergenceDivergenceAnalysisMapping());
        }
    }
}