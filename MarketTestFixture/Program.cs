using System;
using Market.Tasks;
using Market.TestFixture.Tasks;

namespace Market.TestFixture
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            StockTask task = new StockTask();
            StockTaskTestFixture stockTaskTest = new StockTaskTestFixture();
            stockTaskTest.AbleToGetAllTmxTransactionDataFrbomInternet();
            Console.WriteLine("Got all Transaction data from Internet");
            stockTaskTest.AbleToGetSplitFromInternet();
            Console.WriteLine("Got all Split from Internet");
            task.CalculateMovingAverageConvergenceDivergence();
            Console.WriteLine("Calculated Moving Average Convergence Devergence");
            //task.AnalyzeTrendChannel();
            //Console.WriteLine("Analyzed Channel");
            //Console.ReadLine();
            IntegrationTest integrationTest = new IntegrationTest();
            integrationTest.GetLatestSuggestions();
            Console.WriteLine("Generate latest Suggestions");
            Console.ReadLine();
            //StockTask task = new StockTask();
            //task.RegenerateTransactionDataFromOriginalData();
            //Console.WriteLine("Regenrate Transaction Data from split");
            //Console.ReadLine();
        }
    }
}