using System.Collections.Generic;
using System.Linq;
using Market.Analyzer;

namespace Market.Suggestions
{
    public class ShortTermSecondBounceAfterLongTermDownSuggestionAnalyzer : ISuggestionAnalyzer
    {
        private readonly MovingAverageAnalyzer movingAverageAnalyzer;
        public string Name { get { return "Second Bounce After Long Term Down"; } }
        public Term Term { get { return Term.Short; } }
        public Action Action { get { return Action.Buy; } }

        public ShortTermSecondBounceAfterLongTermDownSuggestionAnalyzer()
        {
            movingAverageAnalyzer = new MovingAverageAnalyzer();
        }

        public double CalculateForecaseCertainty(IList<TransactionData> orderedTransactions)
        {
            var last200Trans = orderedTransactions.GetRearPartial(200);
            var last100Trans = orderedTransactions.GetRearPartial(100);
            var last50Trans = orderedTransactions.GetRearPartial(50);
            var last20Trans = orderedTransactions.GetRearPartial(20);
            var last5Trans = orderedTransactions.GetRearPartial(5);
            MovingAverage avg20_200 = last200Trans.GetMovingAverage(20, t => t.SimpleAvg20);
            var trend200 = movingAverageAnalyzer.AnalyzeMovingTrend(avg20_200.Averages);
            MovingAverage avg10_100 = last100Trans.GetMovingAverage(10, t => t.SimpleAvg10);
            var trend100 = movingAverageAnalyzer.AnalyzeMovingTrend(avg10_100.Averages);
            MovingAverage avg5_50 = last50Trans.GetMovingAverage(5, t => t.SimpleAvg5);
            var trend50 = movingAverageAnalyzer.AnalyzeMovingTrend(avg5_50.Averages);
            if ((int)trend50 >= 0 && (int)trend100 >= 0 && (int)trend200 >= 0)
                return 0;
            MovingAverage avg5_20 = last20Trans.GetMovingAverage(5, t => t.SimpleAvg5);
            var trend20 = movingAverageAnalyzer.AnalyzeMovingTrend(avg5_20.Averages);
            MovingAverage avg5_5 = last5Trans.GetMovingAverage(5, t => t.SimpleAvg5);
            var trend5 = movingAverageAnalyzer.AnalyzeMovingTrend(avg5_5.Averages);
            if ((trend20 == Trend.Top || trend20 == Trend.TopUp || trend20 == Trend.VibrationUp || trend20 == Trend.BottomUp|| trend20 == Trend.Up) && 
                (trend5 == Trend.BottomDown || trend5 == Trend.BottomUp || trend5 == Trend.Bottom))
            {
                var highPriceIn100 = last200Trans.Max(t => t.High);
                var lowPricein100 = last100Trans.Min(t => t.Low);
                var highPriceIn50 = last50Trans.Max(t => t.High);
                var lowPriceIn50 = last50Trans.Min(t => t.Low);
                var lowPriceIn20 = last20Trans.Min(t => t.Low);
                var highPriceIn20 = last20Trans.Max(t => t.High);
                var lowPriceIn5 = last5Trans.Min(t => t.Low);
                var highPriceIn5 = last5Trans.Max(t => t.High);
                if ((highPriceIn100 - lowPricein100)/highPriceIn100 > 0.3 && lowPricein100 <= lowPriceIn50 &&
                    lowPriceIn50 <= lowPriceIn20 && lowPriceIn20 < lowPriceIn5 && lowPriceIn5 < last5Trans[last5Trans.Count - 1].Close &&
                    ((highPriceIn50 > highPriceIn5 * 1.05) || (highPriceIn20 > highPriceIn5 * 1.05)))
                    return 1;
            }
            return 0;
        }
    }
}