using System;

namespace Market.Web.Factory
{
    public interface ITransactionWebRequestFactory
    {
        TransactionWebRequest CreateTransactionWebRequest(string stockId);
    }

    public class TransactionWebRequestFactory : ITransactionWebRequestFactory
    {
        public TransactionWebRequest CreateTransactionWebRequest(string stockId)
        {
            YahooFinanceTransactionWebRequest webRequest = new YahooFinanceTransactionWebRequest();
            webRequest.StockId = stockId;
            webRequest.EndDate = DateTime.Today;
            webRequest.StartDate = new DateTime(2011, 01, 01);
            webRequest.TransactionPeriod = Period.Day;
            return webRequest;
        }
    }
}