namespace Market.Analyzer
{
    public enum Trend
    {
        Up =  1,
        Down = -1,
        Bottom = -2,
        Top = 2,
        Vibration = 0,
        VibrationUp = 3,
        TopUp = 4,
        BottomUp = 5,
        VibrationDown = -3,
        TopDown = -4,
        BottomDown = -5
    }

    public static class TrendExtension
    {
        public static int GetSign(this Trend trend)
        {
            if (trend == Trend.Up || trend == Trend.TopUp || trend == Trend.BottomUp)
                return 1;
            if (trend == Trend.Down || trend == Trend.TopDown || trend == Trend.BottomDown)
                return -1;
            return 0;
        }
    }
}