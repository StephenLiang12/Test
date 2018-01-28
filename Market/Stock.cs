namespace Market
{
    public class Stock
    {
        public int Key { get; set; }
        public string Id { get; set; } 
        public string Name { get; set; }
        public double AvgVolume { get; set; }
        public bool AbleToGetTransactionDataFromWeb { get; set; }
        public double AvgDaysChannel50Reverse { get; set; }
        public double AvgDaysChannel100Reverse { get; set; }
        public double AvgDaysChannel200Reverse { get; set; }
    }
}