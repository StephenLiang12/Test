using System.Net;

namespace Market.Tasks
{
    public interface IStockTask
    {
        HttpStatusCode GetTransactionDataFromInternet(string stockId);
        HttpStatusCode GetSplitFromInternet(string stockId);
    }
}