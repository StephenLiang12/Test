using System;
using Market.Analyzer;
using Market.Analyzer.Channels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture
{
    [TestClass]
    public class StockContextTestFixture : DbContextTestFixtureBase
    {
        [TestMethod]
        public void AbleToAddStock()
        {
            StockContext context = new StockContext();
            Stock stock = new Stock();
            stock.Id = "TEST.TO";
            stock.Name = "Test";
            context.Stocks.Add(stock);
            context.SaveChanges();
            Assert.IsTrue(stock.Key > 0);
        }

        [TestMethod]
        public void AbleToAddTransactionData()
        {
            StockContext context = new StockContext();
            Stock stock = new Stock();
            stock.Id = "TEST.TO";
            stock.Name = "Test";
            context.Stocks.Add(stock);
            context.SaveChanges();
            TransactionData data = new TransactionData();
            data.StockKey = stock.Key;
            data.TimeStamp = DateTime.Today;
            data.Period = Period.Day;
            context.TransactionDatas.Add(data);
            context.SaveChanges();
            Assert.IsTrue(data.Key > 0);
        }

        [TestMethod]
        public void AbleToAddSplit()
        {
            StockContext context = new StockContext();
            Stock stock = new Stock();
            stock.Id = "TEST.TO";
            stock.Name = "Test";
            context.Stocks.Add(stock);
            context.SaveChanges();
            Split split = new Split();
            split.StockKey = stock.Key;
            split.TimeStamp = DateTime.Today;
            split.SplitRatio = 2;
            context.Splits.Add(split);
            context.SaveChanges();
            Assert.IsTrue(split.Key > 0);
        }

        [TestMethod]
        public void AbleToAddSuggestion()
        {
            StockContext context = new StockContext();
            Stock stock = new Stock();
            stock.Id = "TEST.TO";
            stock.Name = "Test";
            context.Stocks.Add(stock);
            context.SaveChanges();
            Suggestion suggestion = new Suggestion();
            suggestion.StockKey = stock.Key;
            suggestion.TimeStamp = DateTime.Today;
            suggestion.StockId = stock.Id;
            suggestion.StockName = stock.Name;
            suggestion.AnalyzerName = "Unknown";
            suggestion.CandleStickPattern = "Unknown";
            context.Suggestions.Add(suggestion);
            context.SaveChanges();
            Assert.IsTrue(suggestion.Key > 0);
        }

        [TestMethod]
        public void AbleToAddTransactionSimulator()
        {
            StockContext context = new StockContext();
            Stock stock = new Stock();
            stock.Id = "TEST.TO";
            stock.Name = "Test";
            context.Stocks.Add(stock);
            context.SaveChanges();
            Suggestion suggestion = new Suggestion();
            suggestion.StockKey = stock.Key;
            suggestion.TimeStamp = DateTime.Today;
            suggestion.StockId = stock.Id;
            suggestion.StockName = stock.Name;
            suggestion.AnalyzerName = "Unknown";
            suggestion.CandleStickPattern = "Unknown";
            context.Suggestions.Add(suggestion);
            context.SaveChanges();
            TransactionSimulator transactionSimulator = new TransactionSimulator();
            transactionSimulator.StockKey = stock.Key;
            transactionSimulator.SuggestionKey = suggestion.Key;
            transactionSimulator.TimeStamp = DateTime.Today;
            context.TransactionSimulators.Add(transactionSimulator);
            context.SaveChanges();
            Assert.IsTrue(transactionSimulator.Key > 0);
        }

        [TestMethod]
        public void AbleToAddChannel()
        {
            StockContext context = new StockContext();
            Stock stock = new Stock();
            stock.Id = "TEST.TO";
            stock.Name = "Test";
            context.Stocks.Add(stock);
            context.SaveChanges();
            Channel channel = new Channel();
            channel.StockKey = stock.Key;
            channel.StartDate = DateTime.Today;
            channel.EndDate = DateTime.Today;
            channel.SupportStartPrice = 1;
            channel.ResistanceStartPrice = 2;
            channel.SupportChannelRatio = 0.2;
            channel.ResistanceChannelRatio = -0.2;
            channel.ChannelTrend = Trend.Up;
            channel.Length = 10;
            context.Channels.Add(channel);
            context.SaveChanges();
            Assert.IsTrue(channel.Key > 0);
        }
    }
}