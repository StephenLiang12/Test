using System;
using Market.Tasks;
using Market.TestFixture.Tasks;

namespace Market.TestFixture
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                DailyRoutine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(e.StackTrace);
                Console.ReadLine();
            }
        }

        private static void Rerun()
        {
            IntegrationTest test = new IntegrationTest();
            Console.WriteLine("Rerun Analyzer from {0} to {1}", Properties.Settings.Default.MinStockKey,
                Properties.Settings.Default.MaxStockKey);
            test.RerunFromBeginning(Properties.Settings.Default.MinStockKey, Properties.Settings.Default.MaxStockKey);
            Console.WriteLine("Rerun complete");
            Console.ReadLine();
        }

        private static void DailyRoutine()
        {
            StockTask task = new StockTask();
            StockTaskTestFixture stockTaskTest = new StockTaskTestFixture();
            stockTaskTest.AbleToGetAllTmxTransactionDataFromInternet();
            Console.WriteLine("Got all Transaction data from Internet");
            //stockTaskTest.AbleToGetSplitFromInternet();
            //Console.WriteLine("Got all Split from Internet");
            task.CalculateMovingAverageConvergenceDivergence(Properties.Settings.Default.MinStockKey,
                Properties.Settings.Default.MaxStockKey);
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