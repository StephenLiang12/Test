﻿using System;
using System.Collections.Generic;
using System.Net;
using Market.Analyzer.Channels;

namespace Market.Tasks
{
    public interface IStockTask
    {
        int GetTransactionDataFromInternet(string stockId);
        int GetSplitFromInternet(string stockId);
        Channel GetChannel(int stockKey, int length, DateTime endTime);
        Channel GetChannel(int stockKey, int length, DateTime startTime, DateTime endTime);
        Channel GetPreviousChannel(int stockKey, int length, DateTime endTime);
        IEnumerable<Channel> GetChannels(int stockKey, int length, DateTime startTime, DateTime endTime);
        void ApplySplitOnTransactionData(int stockKey, Split split);
    }
}