using System;
using System.Net;
using Market.Analyzer.Channels;

namespace Market.Tasks
{
    public interface IStockTask
    {
        HttpStatusCode GetTransactionDataFromInternet(string stockId);
        HttpStatusCode GetSplitFromInternet(string stockId);
        Channel GetChannel(int stockKey, int length, DateTime startTime, DateTime endTime);
        Channel GetPreviousChannel(int stockKey, int length, DateTime endTime);
    }
}