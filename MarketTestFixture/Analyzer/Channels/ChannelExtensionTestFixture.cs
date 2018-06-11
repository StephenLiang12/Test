using System;
using System.Collections.Generic;
using Market.Analyzer.Channels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture.Analyzer.Channels
{
    [TestClass]
    public class ChannelExtensionTestFixture
    {
        [TestMethod]
        public void AbleToCheckIfTransactionBreakResistanceLine()
        {
            Channel channel = new Channel();
            channel.StartDate = new DateTime(2018, 1, 1);
            channel.EndDate = channel.StartDate.AddDays(50);
            channel.Length = 50;
            channel.ResistanceStartPrice = 5;
            channel.ResistanceChannelRatio = 0.01;
            IList<TransactionData> transactionData = new List<TransactionData>(30);
            DateTime timeStamp = new DateTime(2018, 2, 1);
            double lowPrice = 5;
            for (int i = 0; i < 60; i++)
            {
                TransactionData transaction = new TransactionData();
                transactionData.Add(transaction);
                transaction.TimeStamp = timeStamp;
                transaction.Low = lowPrice;
                timeStamp = timeStamp.AddDays(1);
                lowPrice += 0.02;
            }

            int breakIndex;
            Assert.IsTrue(channel.BreakResistanceLine(transactionData, out breakIndex));
            Assert.AreEqual(32, breakIndex);
            Assert.AreEqual(new DateTime(2018, 3, 5), transactionData[32].TimeStamp);
        }
    }
}