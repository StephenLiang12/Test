using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Market.Web;

namespace Market.TestFixture.Data
{
    public class SampleDataReader
    {
        public static StreamReader GoogleFinanceTransactionDataReader
        {
            get
            {
                return new StreamReader(GetEmbeddedFile("Toronto-Dominion Bank TSETD historical prices – Google Finance.htm"));
            }
        }

        public static StreamReader YahooFinanceTransactionDataReader
        {
            get
            {
                return new StreamReader(GetEmbeddedFile("TD.TO.csv"));
            }
        }

        public static StreamReader YahooFinanceDividendReader
        {
            get
            {
                return new StreamReader(GetEmbeddedFile("YahooFinanceDividend.txt"));
            }
        }

        public static StreamReader EodDataReader
        {
            get
            {
                return new StreamReader(GetEmbeddedFile("EodData.csv"));
            }
        }

        public static StreamReader EodSimpleDataReader
        {
            get
            {
                return new StreamReader(GetEmbeddedFile("TSX.csv"));
            }
        }

        public static IList<TransactionData> GetTransactionData()
        {
            YahooFinanceTransactionWebRequest webRequest = new YahooFinanceTransactionWebRequest();
            StreamReader reader = new StreamReader(GetEmbeddedFile("AverageSampleData.txt"));
            string fistLine = reader.ReadLine();
            IList<TransactionData> list = new List<TransactionData>();
            OriginalTransactionData data;
            while (webRequest.GetTransactionData(reader, out data))
            {
                list.Add(data.GetTransactionData());
            }
            reader.Close();
            return list;
        }
        /// <summary>
        /// Extracts an embedded file out of a given assembly.
        /// </summary>
        /// <param name="fileName">The name of the file to extract.</param>
        /// <returns>A stream containing the file data.</returns>
        public static Stream GetEmbeddedFile(string fileName)
        {
            Assembly a = Assembly.GetExecutingAssembly();  
            string assemblyName = a.GetName().Name;
                         
            try
            {
                string resoureName = "Market.TestFixture.Data." + fileName;
                Stream str = a.GetManifestResourceStream(resoureName);
                
                if (str == null)
                    throw new Exception("Could not locate embedded resource '" + fileName + "' in assembly '" + assemblyName + "'");
                return str;
            }
            catch (Exception e)
            {
                throw new Exception(assemblyName + ": " + e.Message);
            }
        }
    }
}
