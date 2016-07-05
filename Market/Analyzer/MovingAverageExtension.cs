namespace Market.Analyzer
{
    public static class MovingAverageExtension
    {
        public static MovingAverage GetPartial(this MovingAverage avg, int count)
        {
            MovingAverage partial = new MovingAverage();
            partial.NumberOfTransactions = avg.NumberOfTransactions;
            if (count > avg.Averages.Length)
                count = avg.Averages.Length;
            partial.Averages = new double[count];
            for (int i = 0; i < count; i++)
            {
                partial.Averages[i] = avg.Averages[i];
            }
            return partial;
        }
    }
}