using System.Collections.Generic;
using System.Linq;
using Market.Analyzer;

namespace Market.Suggestions
{
    public class LongTermBuyAfterLongTermPrepareSuggestionAnalyzer : ISuggestionAnalyzer
    {
        private readonly MovingAverageAnalyzer movingAverageAnalyzer;
        public string Name { get { return "LongTermBuyAfterLongTermPrepare"; } }
        public Term Term { get { return Term.Long; } }
        public Action Action { get { return Action.Buy; } }
        public string Pattern { get; }
        public double Price { get; private set; }

        public LongTermBuyAfterLongTermPrepareSuggestionAnalyzer()
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
            MovingAverage avg200of200 = last200Trans.GetMovingAverage(200, t => t.SimpleAvg200);
            MovingAverage avg200of100 = last100Trans.GetMovingAverage(100, t => t.SimpleAvg200);
            MovingAverage avg200of20 = last20Trans.GetMovingAverage(20, t => t.SimpleAvg200);
            MovingAverage avg200of5 = last5Trans.GetMovingAverage(5, t => t.SimpleAvg200);
            MovingAverage avg50of50 = last50Trans.GetMovingAverage(50, t => t.SimpleAvg50);
            MovingAverage avg20of20 = last20Trans.GetMovingAverage(20, t => t.SimpleAvg20);
            MovingAverage avg5of5 = last5Trans.GetMovingAverage(5, t => t.SimpleAvg5);
            MovingAverage close5 = last5Trans.GetMovingAverage(5, t => t.Close);
            var trend200of200 = movingAverageAnalyzer.AnalyzeMovingTrend(avg200of200.Averages);
            var trend200of100 = movingAverageAnalyzer.AnalyzeMovingTrend(avg200of100.Averages);
            var trend200of20 = movingAverageAnalyzer.AnalyzeMovingTrend(avg200of20.Averages);
            var trend200of5 = movingAverageAnalyzer.AnalyzeMovingTrend(avg200of5.Averages);
            var trend50of50 = movingAverageAnalyzer.AnalyzeMovingTrend(avg50of50.Averages);
            var trend20of20 = movingAverageAnalyzer.AnalyzeMovingTrend(avg20of20.Averages);
            var trend5of5 = movingAverageAnalyzer.AnalyzeMovingTrend(avg5of5.Averages);
            var trendClose5 = movingAverageAnalyzer.AnalyzeMovingTrend(close5.Averages);
            if ((int)trend5of5 < 0 || (int)trend200of5 < 0)
                return 0;
            if ((trend200of200 == Trend.TopUp || trend200of200 == Trend.Up || trend200of200 == Trend.Bottom || trend200of200 == Trend.BottomUp || trend200of200 == Trend.BottomDown || trend200of200 == Trend.Vibration || trend200of200 == Trend.VibrationUp || trend200of200 == Trend.VibrationDown) &&
                (trend200of100 == Trend.TopUp || trend200of100 == Trend.Up || trend200of100 == Trend.Bottom || trend200of100 == Trend.BottomUp || trend200of100 == Trend.BottomDown || trend200of100 == Trend.Vibration || trend200of100 == Trend.VibrationUp || trend200of100 == Trend.VibrationDown) &&
                (trend200of20 == Trend.TopUp || trend200of20 == Trend.Up || trend200of20 == Trend.Bottom || trend200of20 == Trend.BottomUp || trend200of20 == Trend.Vibration || trend200of20 == Trend.VibrationUp || trend200of20 == Trend.Up) &&
                (trend50of50 == Trend.TopUp || trend50of50 == Trend.Up || trend50of50 == Trend.Bottom || trend50of50 == Trend.BottomUp || trend50of50 == Trend.Vibration || trend50of50 == Trend.VibrationUp || trend50of50 == Trend.Up) &&
                (trend20of20 == Trend.Up || trend20of20 == Trend.BottomUp || trend20of20 == Trend.VibrationUp) &&
                (trend5of5 == Trend.Up || trend5of5 == Trend.BottomUp || trend5of5 == Trend.VibrationUp || trend5of5 == Trend.TopUp) &&
                (trendClose5 == Trend.Up || trendClose5 == Trend.BottomUp || trendClose5 == Trend.VibrationUp || trendClose5 == Trend.TopUp))
            {
                var highPriceIn200 = last200Trans.Max(t => t.High);
                var lowPriceIn200 = last200Trans.Min(t => t.Low);
                var lowPriceIn20 = last20Trans.Min(t => t.Low);
                var highPriceIn20 = last20Trans.Max(t => t.High);
                var lowPriceIn5 = last5Trans.Min(t => t.Low);
                var highPriceIn5 = last5Trans.Max(t => t.High);
                var incrementRatioOf200 = (highPriceIn200 - lowPriceIn200)/200;
                var incrementRatioOf20 = (highPriceIn20 - lowPriceIn20)/20;
                var incrementRatioOf5 = (highPriceIn5 - lowPriceIn5)/5;
                if (incrementRatioOf5 > incrementRatioOf20 && incrementRatioOf20 > incrementRatioOf200 &&
                    highPriceIn5 >= highPriceIn20 && highPriceIn20 >= highPriceIn200)
                    return 1;
            }
            return 0;
        }
    }
}