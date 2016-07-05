using System;
using System.Collections.Generic;

namespace Market.Analyzer.Channels
{
    public static class ChannelExtension
    {
        public static double Size(this Channel channel)
        {
            double upperLimitEndPrice = channel.ResistanceStartPrice + channel.ResistanceChannelRatio*(channel.Length - 1);
            double lowerLimitEndPrice = channel.SupportStartPrice + channel.SupportChannelRatio*(channel.Length - 1);
            double rectangleSize = (Math.Max(upperLimitEndPrice, channel.ResistanceStartPrice) -
                                    Math.Min(lowerLimitEndPrice, channel.SupportStartPrice))*channel.Length;
            double upperTriangleSize = Math.Abs(upperLimitEndPrice - channel.ResistanceStartPrice)*channel.Length/2;
            double lowerTrangleSize = Math.Abs(lowerLimitEndPrice - channel.SupportStartPrice)*channel.Length/2;
            return rectangleSize - upperTriangleSize - lowerTrangleSize;
        }

        public static double CoverPercentage(this Channel channel, IList<TransactionData> orderedTransactions)
        {
            int count = orderedTransactions.Count;
            double outOfCoverage = 0;
            for (int i = 0; i < orderedTransactions.Count; i++)
            {
                double lowPrice = channel.SupportStartPrice + channel.SupportChannelRatio*i;
                double highPrice = channel.ResistanceStartPrice + channel.ResistanceChannelRatio*i;
                if (orderedTransactions[i].Open < lowPrice || orderedTransactions[i].Close < lowPrice)
                    outOfCoverage++;
                else if (orderedTransactions[i].Low < lowPrice)
                    outOfCoverage += 0.5;
                if (orderedTransactions[i].Open > highPrice || orderedTransactions[i].Close > highPrice)
                    outOfCoverage++;
                else if (orderedTransactions[i].High > highPrice)
                    outOfCoverage += 0.5;
            }
            return (count - outOfCoverage)/count;
        }
    }
}