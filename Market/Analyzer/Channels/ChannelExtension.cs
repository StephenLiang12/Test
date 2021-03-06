﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Market.Analyzer.Channels
{
    public static class ChannelExtension
    {
        private const double LongTermPercentageThreshold = 0.05;
        private const double IntermediateTermPercentageThreshold = 0.02;
        private const double ShortTermPercentageThreshold = 0.01;

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
                if (orderedTransactions[i].Open - lowPrice < -0.00005 || orderedTransactions[i].Close - lowPrice < -0.00005)
                    outOfCoverage++;
                else if (orderedTransactions[i].Low - lowPrice < -0.00005)
                    outOfCoverage += 0.5;
                if (orderedTransactions[i].Open - highPrice > 0.00005 || orderedTransactions[i].Close - highPrice > 0.00005)
                    outOfCoverage++;
                else if (orderedTransactions[i].High - highPrice > 0.00005)
                    outOfCoverage += 0.5;
            }
            return (count - outOfCoverage)/count;
        }

        public static double GetPercentageThreshold(this Channel channel)
        {
            if (channel.Length == 200)
                return LongTermPercentageThreshold;
            if (channel.Length == 100)
                return IntermediateTermPercentageThreshold;
            if (channel.Length == 50)
                return ShortTermPercentageThreshold;
            return 0;
        }

        public static int GetSupportSign(this Channel channel)
        {
            double abs = Math.Abs(channel.SupportChannelRatio/channel.SupportStartPrice*channel.Length);
            if (abs > channel.GetPercentageThreshold())
            {
                if (channel.SupportChannelRatio > 0)
                    return 1;
                return -1;
            }
            return 0;
        }

        public static int GetResistanceSign(this Channel channel)
        {
            double abs = Math.Abs(channel.ResistanceChannelRatio/channel.ResistanceStartPrice*channel.Length);
            if (abs > channel.GetPercentageThreshold())
            {
                if (channel.ResistanceChannelRatio > 0)
                    return 1;
                return -1;
            }
            return 0;
        }

        public static bool IsSupportStrong(this Channel channel, IList<TransactionData> orderedTransactions)
        {
            int count = 0;
            var transactions = orderedTransactions.GetRearPartial(channel.Length);
            for (int i = 0; i < transactions.Count; i++)
            {
                var supportPrice = channel.SupportStartPrice + channel.SupportChannelRatio*i;
                var resistancePrice = channel.ResistanceStartPrice + channel.ResistanceChannelRatio*i;
                if ((transactions[i].Low - supportPrice) / (resistancePrice - supportPrice) <= 0.1)
                    count++;
            }
            return (count -2) * 25 >= channel.Length;
        }

        public static bool DoesCloseNearSupportLine(this Channel channel, IList<TransactionData> orderedTransactions)
        {
            for (int i = 0; i < 5; i++)
            {
                var supportPrice = channel.SupportStartPrice + channel.SupportChannelRatio * (channel.Length - i - 1);
                var resistancePrice = channel.ResistanceStartPrice + channel.ResistanceChannelRatio * (channel.Length - i - 1);
                var gap = resistancePrice - supportPrice;
                if (gap < 0.005)
                    return true;
                var percentageFromSupport = (orderedTransactions[orderedTransactions.Count - i - 1].Low - supportPrice) / gap;
                var closeEnough = percentageFromSupport <= 0.15;
                if (closeEnough)
                    return true;
            }
            return false;
        }

        public static bool IsResistanceStrong(this Channel channel, IList<TransactionData> orderedTransactions)
        {
            int count = 0;
            var transactions = orderedTransactions.GetRearPartial(channel.Length);
            for (int i = 0; i < transactions.Count; i++)
            {
                var supportPrice = channel.SupportStartPrice + channel.SupportChannelRatio * i;
                var resistancePrice = channel.ResistanceStartPrice + channel.ResistanceChannelRatio * i;
                if ((resistancePrice - transactions[i].High) /(resistancePrice - resistancePrice) <= 0.1)
                    count++;
            }
            return (count -2 ) * 25 > channel.Length;
        }

        public static bool DoesCloseNearResistanceLine(this Channel channel, IList<TransactionData> orderedTransactions)
        {
            for (int i = 0; i < 5; i++)
            {
                var supportPrice = channel.SupportStartPrice + channel.SupportChannelRatio * (channel.Length - i - 1);
                var resistancePrice = channel.ResistanceStartPrice + channel.ResistanceChannelRatio * (channel.Length -i - 1);
                var gap = resistancePrice - supportPrice;
                if (gap < 0.005)
                    return true;
                var closeEnough = (resistancePrice - orderedTransactions[orderedTransactions.Count - i - 1].High) / gap <= 0.1;
                if (closeEnough)
                    return true;
            }
            return false;
        }

        public static bool IsSpiking(this Channel channel)
        {
            return channel.ResistanceChannelRatio/channel.ResistanceStartPrice > 0.03;
        }

        public static bool IsDroping(this Channel channel)
        {
            return channel.SupportChannelRatio/channel.SupportStartPrice < -0.03;
        }

        public static bool BreakSupportLine(this Channel channel, IList<TransactionData> orderedTransactions, out int breakIndex)
        {
            int n = 0;
            for (int i = orderedTransactions.Count - 1; i >=0; i--)
            {
                if (orderedTransactions[i].TimeStamp == channel.EndDate)
                    n = i;
            }
            if (n == 0)
            {
                breakIndex = 0;
                return false;
            }

            double high = orderedTransactions.Max(t => t.High);
            double threshold = (high - orderedTransactions[n].Close) / 2;
            for (int i = n + 1; i < orderedTransactions.Count; i++)
            {
                var supportPrice = channel.SupportStartPrice + channel.SupportChannelRatio * (channel.Length + i - n);
                if (orderedTransactions[i].Close < supportPrice - threshold)
                {
                    breakIndex = i;
                    return true;
                }
            }
            breakIndex = 0;
            return false;
        }

        public static bool BreakResistanceLine(this Channel channel, IList<TransactionData> orderedTransactions, out int breakIndex)
        {
            int n = 0;
            for (int i = orderedTransactions.Count - 1; i >=0; i--)
            {
                if (orderedTransactions[i].TimeStamp == channel.EndDate)
                    n = i;
            }
            if (n == 0)
            {
                breakIndex = 0;
                return false;
            }
            double low = orderedTransactions.Where(t => t.TimeStamp >= channel.StartDate && t.TimeStamp <= channel.EndDate).Min(t => t.Low);
            double threshold = (orderedTransactions[n].Close - low) / 2;
            for (int i = n + 1; i < orderedTransactions.Count; i++)
            {
                var resistancePrice = channel.ResistanceStartPrice + channel.ResistanceChannelRatio * (channel.Length + i - n);
                if (orderedTransactions[i].Low > resistancePrice + threshold)
                {
                    breakIndex = i;
                    return true;
                }
            }
            breakIndex = 0;
            return false;
        }
    }
}