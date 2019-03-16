using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Market.Analyzer;
using Market.Analyzer.MACD;
using Market.Exceptions;
using Market.Tasks;
using Market.TestFixture.Data;
using Market.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture.Tasks
{
    [TestClass]
    public class StockTaskTestFixture
    {
        [TestMethod]
        public void AbleToGetTransactionDataFromInternet()
        {
            StockTask task = new StockTask();
            Assert.AreEqual(HttpStatusCode.OK, task.GetTransactionDataFromInternet("TD.TO"));
        }

        [TestMethod]
        public void AbleToGetDividendFromInternet()
        {
            StockTask task = new StockTask();
            Assert.AreEqual(HttpStatusCode.OK, task.GetSplitFromInternet("BBD-B.TO"));
        }

        [TestMethod]
        public void AbleToAddStockFromEodData()
        {
            StockTask task = new StockTask();
            int count = task.AddStockFromEodData(SampleDataReader.EodDataReader);
            Console.WriteLine(count);
        }

        [TestMethod]
        public void AbleToAddStockFromSimpleEodData()
        {
            StockTask task = new StockTask();
            int count = task.AddStockFromEodSimpleData(SampleDataReader.EodSimpleDataReader);
            Console.WriteLine(count);
        }

        [TestMethod]
        public void AbleToGetAllTmxTransactionDataFromInternet()
        {
            StockTask task = new StockTask();
            StockContext context = new StockContext();
            //var stock = context.Stocks.First(s => s.Id == "BBD-B.TO");
            foreach (var stock in context.Stocks.Where(s => s.Key >= Properties.Settings.Default.MinStockKey && s.Key <= Properties.Settings.Default.MaxStockKey).ToList())
            {
                try
                {
                    task.GetTransactionDataFromInternet(stock.Id);
                }
                catch (IOException)
                {
                    Thread.Sleep(10 * 60 * 1000);
                    Console.WriteLine("Retry on {0}", stock.Id);
                    task.GetTransactionDataFromInternet(stock.Id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error on {0}: {1}", stock.Id, ex.Message);
                }
            }
        }

        [TestMethod]
        public void AbleToGetSplitFromInternet()
        {
            StockTask task = new StockTask();
            StockContext context = new StockContext();
            //task.GetSplitFromInternet("HOD.TO");
            foreach (var stock in context.Stocks.Where(s => s.Key >= Properties.Settings.Default.MinStockKey && s.Key <= Properties.Settings.Default.MaxStockKey).ToList())
            {
                if (stock.AbleToGetTransactionDataFromWeb)
                {
                    try
                    {
                        task.GetSplitFromInternet(stock.Id);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Get split for {0} error {1}", stock.Id, e.Message);
                    }
                }
            }
        }

        [TestMethod]
        public void AbleToCalculateSimpleAverageOnAllStocksFromBeginning()
        {
            StockContext context = new StockContext();
            foreach (var stock in context.Stocks.ToList())
            {
                if (stock.AbleToGetTransactionDataFromWeb == false)
                    continue;
                IList<TransactionData> orderedList =
                    context.TransactionData.Where(t => t.StockKey == stock.Key).OrderBy(t => t.TimeStamp).ToList();
                SimpleMovingAverageCalculator calculator = new SimpleMovingAverageCalculator();
                try
                {
                    var movingAvg = calculator.CalculateAverage(orderedList, 5);
                    for (int i = 0; i < movingAvg.Averages.Length; i++)
                    {
                        orderedList[i].SimpleAvg5 = movingAvg.Averages[i];
                    }
                    movingAvg = calculator.CalculateAverage(orderedList, 10);
                    for (int i = 0; i < movingAvg.Averages.Length; i++)
                    {
                        orderedList[i].SimpleAvg10 = movingAvg.Averages[i];
                    }
                    movingAvg = calculator.CalculateAverage(orderedList, 20);
                    for (int i = 0; i < movingAvg.Averages.Length; i++)
                    {
                        orderedList[i].SimpleAvg20 = movingAvg.Averages[i];
                    }
                    movingAvg = calculator.CalculateAverage(orderedList, 50);
                    for (int i = 0; i < movingAvg.Averages.Length; i++)
                    {
                        orderedList[i].SimpleAvg50 = movingAvg.Averages[i];
                    }
                    movingAvg = calculator.CalculateAverage(orderedList, 100);
                    for (int i = 0; i < movingAvg.Averages.Length; i++)
                    {
                        orderedList[i].SimpleAvg100 = movingAvg.Averages[i];
                    }
                    movingAvg = calculator.CalculateAverage(orderedList, 200);
                    for (int i = 0; i < movingAvg.Averages.Length; i++)
                    {
                        orderedList[i].SimpleAvg200 = movingAvg.Averages[i];
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Stock {0} does not finish all simple moving average calculation. {1}", stock.Id, e.Message);
                }
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void AbleToCalculateSimpleAverageOnAllStocksForNewTransactions()
        {
            StockContext context = new StockContext();
            int newTransactions = 20;
            foreach (var stock in context.Stocks.ToList())
            {
                if (stock.AbleToGetTransactionDataFromWeb == false)
                    continue;
                IList<TransactionData> orderedList =
                    context.TransactionData.Where(t => t.StockKey == stock.Key).OrderBy(t => t.TimeStamp).ToList();
                int count = orderedList.Count;
                MovingAverage avg5 = new MovingAverage();
                avg5.NumberOfTransactions = 5;
                avg5.Averages = new double[orderedList.Count];
                MovingAverage avg10 = new MovingAverage();
                avg10.NumberOfTransactions = 10;
                avg10.Averages = new double[orderedList.Count];
                MovingAverage avg20 = new MovingAverage();
                avg20.NumberOfTransactions = 20;
                avg20.Averages = new double[orderedList.Count];
                MovingAverage avg50 = new MovingAverage();
                avg50.NumberOfTransactions = 50;
                avg50.Averages = new double[orderedList.Count];
                MovingAverage avg100 = new MovingAverage();
                avg100.NumberOfTransactions = 100;
                avg100.Averages = new double[orderedList.Count];
                MovingAverage avg200 = new MovingAverage();
                avg200.NumberOfTransactions = 200;
                avg200.Averages = new double[orderedList.Count];
                for (int i = 0; i < orderedList.Count; i++)
                {
                    avg5.Averages[i] = orderedList[i].SimpleAvg5;
                    avg10.Averages[i] = orderedList[i].SimpleAvg10;
                    avg20.Averages[i] = orderedList[i].SimpleAvg20;
                    avg50.Averages[i] = orderedList[i].SimpleAvg50;
                    avg100.Averages[i] = orderedList[i].SimpleAvg100;
                    avg200.Averages[i] = orderedList[i].SimpleAvg200;
                }
                SimpleMovingAverageCalculator calculator = new SimpleMovingAverageCalculator();
                try
                {
                    calculator.CalculateAverage(orderedList, avg5, newTransactions);
                    for (int i = 0; i < newTransactions; i++)
                        orderedList[count - i - 1].SimpleAvg5 = avg5.Averages[count - i - 1];
                    calculator.CalculateAverage(orderedList, avg10, newTransactions);
                    for (int i = 0; i < newTransactions; i++)
                        orderedList[count - i - 1].SimpleAvg10 = avg10.Averages[count - i - 1];
                    calculator.CalculateAverage(orderedList, avg20, newTransactions);
                    for (int i = 0; i < newTransactions; i++)
                        orderedList[count - i - 1].SimpleAvg20 = avg20.Averages[count - i - 1];
                    calculator.CalculateAverage(orderedList, avg50, newTransactions);
                    for (int i = 0; i < newTransactions; i++)
                        orderedList[count - i - 1].SimpleAvg50 = avg50.Averages[count - i - 1];
                    calculator.CalculateAverage(orderedList, avg100, newTransactions);
                    for (int i = 0; i < newTransactions; i++)
                        orderedList[count - i - 1].SimpleAvg100 = avg100.Averages[count - i - 1];
                    calculator.CalculateAverage(orderedList, avg200, newTransactions);
                    for (int i = 0; i < newTransactions; i++)
                        orderedList[count - i - 1].SimpleAvg200 = avg200.Averages[count - i - 1];
                }
                catch (Exception e)
                {
                    Console.WriteLine("Stock {0} does not finish all simple moving average calculation. {1}", stock.Id, e.Message);
                }
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void AbleToAnalyzeMovingAverage()
        {
            StockContext context = new StockContext();
            //foreach (var stock in context.Stocks.ToList())
            {
                //if (stock.AbleToGetTransactionDataFromWeb == false)
                    //continue;
                var stock = context.Stocks.First(s => s.Id == "TD.TO");
                IList<TransactionData> orderedList =
                    context.TransactionData.Where(t => t.StockKey == stock.Key).OrderBy(t => t.TimeStamp).ToList();
                MovingAverageAnalyzer analyzer = new MovingAverageAnalyzer();
                MovingAverage avg5 = new MovingAverage();
                avg5.NumberOfTransactions = 5;
                avg5.Averages = new double[orderedList.Count];
                MovingAverage avg10 = new MovingAverage();
                avg10.NumberOfTransactions = 10;
                avg10.Averages = new double[orderedList.Count];
                MovingAverage avg20 = new MovingAverage();
                avg20.NumberOfTransactions = 20;
                avg20.Averages = new double[orderedList.Count];
                MovingAverage avg50 = new MovingAverage();
                avg50.NumberOfTransactions = 50;
                avg50.Averages = new double[orderedList.Count];
                MovingAverage avg100 = new MovingAverage();
                avg100.NumberOfTransactions = 100;
                avg100.Averages = new double[orderedList.Count];
                MovingAverage avg200 = new MovingAverage();
                avg200.NumberOfTransactions = 200;
                avg200.Averages = new double[orderedList.Count];
                for (int i = 0; i < orderedList.Count; i++)
                {
                    avg5.Averages[i] = orderedList[i].SimpleAvg5;
                    avg10.Averages[i] = orderedList[i].SimpleAvg10;
                    avg20.Averages[i] = orderedList[i].SimpleAvg20;
                    avg50.Averages[i] = orderedList[i].SimpleAvg50;
                    avg100.Averages[i] = orderedList[i].SimpleAvg100;
                    avg200.Averages[i] = orderedList[i].SimpleAvg200;
                }
                try
                {
                    double movingAvg5 = analyzer.PriceCompareAverage(orderedList, avg5);
                    double movingAvg10 = analyzer.PriceCompareAverage(orderedList, avg10);
                    double movingAvg20 = analyzer.PriceCompareAverage(orderedList, avg20);
                    double movingAvg50 = analyzer.PriceCompareAverage(orderedList, avg50);
                    double movingAvg100 = analyzer.PriceCompareAverage(orderedList, avg100);
                    double movingAvg200 = analyzer.PriceCompareAverage(orderedList, avg200);
                    if (movingAvg10 > 0)
                        Console.WriteLine(stock.Id + " " + stock.Name + " " + stock.AvgVolume + " " + movingAvg10);
                    Console.WriteLine("Avg5:{0};Avg10:{1};Avg20:{2};Avg50:{3};Avg100:{4};Avg200:{5}", movingAvg5, movingAvg10, movingAvg20, movingAvg50, movingAvg100, movingAvg200);
                }
                catch (Exception)
                {
                }
            }
        }

        [TestMethod]
        public void AbleToAnalyzeMovingAverageTrend()
        {
            StockContext context = new StockContext();
            //foreach (var stock in context.Stocks.ToList())
            {
                //if (stock.AbleToGetTransactionDataFromWeb == false)
                //    continue;
                Stock stock = context.Stocks.First(s => s.Id == "BNS.TO");
                IList<TransactionData> orderedList =
                    context.TransactionData.Where(t => t.StockKey == stock.Key).OrderBy(t => t.TimeStamp).ToList();
                MovingAverageAnalyzer analyzer = new MovingAverageAnalyzer();
                MovingAverage avg5 = new MovingAverage();
                avg5.NumberOfTransactions = 5;
                avg5.Averages = new double[orderedList.Count];
                MovingAverage avg10 = new MovingAverage();
                avg10.NumberOfTransactions = 10;
                avg10.Averages = new double[orderedList.Count];
                MovingAverage avg20 = new MovingAverage();
                avg20.NumberOfTransactions = 20;
                avg20.Averages = new double[orderedList.Count];
                MovingAverage avg50 = new MovingAverage();
                avg50.NumberOfTransactions = 50;
                avg50.Averages = new double[orderedList.Count];
                MovingAverage avg100 = new MovingAverage();
                avg100.NumberOfTransactions = 100;
                avg100.Averages = new double[orderedList.Count];
                MovingAverage avg200 = new MovingAverage();
                avg200.NumberOfTransactions = 200;
                avg200.Averages = new double[orderedList.Count];
                for (int i = 0; i < orderedList.Count; i++)
                {
                    avg5.Averages[i] = orderedList[i].SimpleAvg5;
                    avg10.Averages[i] = orderedList[i].SimpleAvg10;
                    avg20.Averages[i] = orderedList[i].SimpleAvg20;
                    avg50.Averages[i] = orderedList[i].SimpleAvg50;
                    avg100.Averages[i] = orderedList[i].SimpleAvg100;
                    avg200.Averages[i] = orderedList[i].SimpleAvg200;
                }
                try
                {
                    var movingAvg5 = analyzer.AnalyzeMovingTrend(avg5);
                    var movingAvg10 = analyzer.AnalyzeMovingTrend(avg10);
                    var movingAvg20 = analyzer.AnalyzeMovingTrend(avg20);
                    var movingAvg50 = analyzer.AnalyzeMovingTrend(avg50);
                    var movingAvg100 = analyzer.AnalyzeMovingTrend(avg100);
                    var movingAvg200 = analyzer.AnalyzeMovingTrend(avg200);
                    if (movingAvg200 == Trend.Up)
                        Console.WriteLine(stock.Id + " " + stock.Name + " " + stock.AvgVolume + " " + movingAvg200);
                    Console.WriteLine("Avg5:{0};Avg10:{1};Avg20:{2};Avg50:{3};Avg100:{4};Avg200:{5}", movingAvg5, movingAvg10, movingAvg20, movingAvg50, movingAvg100, movingAvg200);
                }
                catch (Exception)
                {
                }
            }
        }

        [TestMethod]
        public void AbleToAnalyzeMovingAverageCrossOver()
        {
            StockContext context = new StockContext();
            //foreach (var stock in context.Stocks.ToList())
            {
                //if (stock.AbleToGetTransactionDataFromWeb == false)
                //    continue;
                var stock = context.Stocks.First(s => s.Id == "TD.TO");
                IList<TransactionData> orderedList =
                    context.TransactionData.Where(t => t.StockKey == stock.Key).OrderBy(t => t.TimeStamp).ToList();
                MovingAverageAnalyzer analyzer = new MovingAverageAnalyzer();
                MovingAverage avg5 = new MovingAverage();
                avg5.NumberOfTransactions = 5;
                avg5.Averages = new double[orderedList.Count];
                MovingAverage avg10 = new MovingAverage();
                avg10.NumberOfTransactions = 10;
                avg10.Averages = new double[orderedList.Count];
                MovingAverage avg20 = new MovingAverage();
                avg20.NumberOfTransactions = 20;
                avg20.Averages = new double[orderedList.Count];
                MovingAverage avg50 = new MovingAverage();
                avg50.NumberOfTransactions = 50;
                avg50.Averages = new double[orderedList.Count];
                MovingAverage avg100 = new MovingAverage();
                avg100.NumberOfTransactions = 100;
                avg100.Averages = new double[orderedList.Count];
                MovingAverage avg200 = new MovingAverage();
                avg200.NumberOfTransactions = 200;
                avg200.Averages = new double[orderedList.Count];
                for (int i = 0; i < orderedList.Count; i++)
                {
                    avg5.Averages[i] = orderedList[i].SimpleAvg5;
                    avg10.Averages[i] = orderedList[i].SimpleAvg10;
                    avg20.Averages[i] = orderedList[i].SimpleAvg20;
                    avg50.Averages[i] = orderedList[i].SimpleAvg50;
                    avg100.Averages[i] = orderedList[i].SimpleAvg100;
                    avg200.Averages[i] = orderedList[i].SimpleAvg200;
                }
                try
                {
                    double movingAvg5_10 = analyzer.AverageCrossOver(avg5, avg10);
                    double movingAvg10_20 = analyzer.AverageCrossOver(avg10, avg20);
                    double movingAvg20_50 = analyzer.AverageCrossOver(avg20, avg50);
                    double movingAvg50_100 = analyzer.AverageCrossOver(avg50, avg100);
                    double movingAvg100_200 = analyzer.AverageCrossOver(avg100, avg200);
                    double movingAvg5_20 = analyzer.AverageCrossOver(avg5, avg20);
                    //if (movingAvg5_20 > 0)
                    //    Console.WriteLine(stock.Id + " " + stock.Name + " " + stock.AvgVolume + " " + movingAvg5_20);
                    Console.WriteLine("5-10:{0}; 10-20:{1};20-50:{2};50-100:{3};100-200:{4};5-20:{5}", movingAvg5_10, movingAvg10_20, movingAvg20_50, movingAvg50_100, movingAvg100_200, movingAvg5_20);

                }
                catch (Exception)
                {
                }
            }
        }

        [TestMethod]
        public void FindSpecialPoints()
        {
            int stockKey = 96;
            StockContext context = new StockContext();
            context.Database.ExecuteSqlCommand(
                "Delete from MovingAverageConvergenceDivergenceAnalysis where StockKey = " + stockKey);
            var list = context.MovingAverageConvergenceDivergences.Where(m => m.StockKey == stockKey).OrderBy(m => m.TimeStamp).ToList();
            for (int i = 200; i < list.Count; i++)
            {
                var array = list.GetFrontPartial(i).ToArray();
                MovingAverageConvergenceDivergencePatternAnalyzer analyzer = new MovingAverageConvergenceDivergencePatternAnalyzer();
                var result = analyzer.Analyze(array);
                if (result != MovingAverageConvergenceDivergenceFeature.Unkown)
                {
                    var analysis = array[array.Length - 1].CopyToAnalysis();
                    analysis.Feature = result;
                    context.MovingAverageConvergenceDivergenceAnalyses.Add(analysis);
                    context.SaveChanges();
                }
            }
        }
    }
}