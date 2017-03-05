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
        TopBottomUp = 6,
        BottomTopUp = 7,
        TopBottom = 8,
        VibrationDown = -3,
        TopDown = -4,
        BottomDown = -5,
        TopBottomDown = -6,
        BottomTopDown = -7,
        BottomTop = -8,
        Unknown = -999
    }

    public static class TrendExtension
    {
        public static int GetSign(this Trend trend)
        {
            if (trend == Trend.Up || trend == Trend.TopUp || trend == Trend.BottomUp || trend == Trend.VibrationUp || trend == Trend.TopBottomUp)
                return 1;
            if (trend == Trend.Down || trend == Trend.TopDown || trend == Trend.BottomDown || trend == Trend.VibrationDown || trend == Trend.BottomTopDown)
                return -1;
            return 0;
        }
    }
}