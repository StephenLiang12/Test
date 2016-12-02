namespace Market
{
    public static class OriginalTransactionDataExtension
    {
        public static TransactionData GetTransactionData(this OriginalTransactionData source)
        {
            TransactionData d = new TransactionData
            {
                StockKey = source.StockKey,
                TimeStamp = source.TimeStamp,
                Period = source.Period,
                Open = source.Open,
                Close = source.Close,
                High = source.High,
                Low = source.Low
            };
            return d;
        }
    }
}