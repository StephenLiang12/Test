using System;

namespace Market.Web.Factory
{
    public interface ITransactionWebRequestFactory
    {
        TransactionWebRequest CreateTransactionWebRequest(string stockId);
        TransactionWebRequest CreateTransactionWebRequest(string stockId, DateTime startDateTime);
    }

    public class TransactionWebRequestFactory : ITransactionWebRequestFactory
    {
        public TransactionWebRequest CreateTransactionWebRequest(string stockId)
        {
            return CreateTransactionWebRequest(stockId, new DateTime(2011, 1, 1));
        }

        public TransactionWebRequest CreateTransactionWebRequest(string stockId, DateTime startDateTime)
        {
            var webRequest = new GoogleFinanceTransactionWebRequest();
            webRequest.StockId = stockId;
            webRequest.EndDate = DateTime.Today;
            webRequest.StartDate = startDateTime;
            webRequest.TransactionPeriod = Period.Day;
            return webRequest;
        }
    }
}