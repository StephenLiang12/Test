using System;
using System.Collections.Generic;
using Market.Analyzer.Channels;

namespace Market.Suggestions
{
    /// Up, Up, Up  |   Up, Up, En  |   Up, Up, Dn  |   Up, En, Dn  |   Up, En, En  |   Up, En, Up  |   Up, Dn, Dn  |   Up, Dn, En  |   Up, Dn, Up  |
    /// SB          |   SB      SS  |           SS  |           SS  |           SS  |   SB          |           SS  |           SS  |               |
    /// IB          |   IB          |   SB          |   SB          |   SB          |   IB      SS  |           IS  |           IS  |   SB      SS  |
    /// LB      SS  |   LB          |   IB          |   IB          |   IB          |   LB          |   SB          |   SB          |   IB          |
    /// 
    /// 
    /// En, Up, Up  |   En, Up, En  |   En, Up, Dn  |   En, En, Dn  |   En, En, En  |   En, En, Up  |   En, Dn, Dn  |   En, Dn, En  |   En, Dn, Up  |
    /// SB          |           SS  |           SS  |           SS  |               |   SB          |           SS  |           SS  |               |
    /// IB          |   SB          |   SB          |           IS  |   SB      SS  |   IB      SS  |           IS  |           IS  |           SS  |
    /// LB      SS  |   IB          |   IB          |   SB          |   IB      IS  |   LB          |   SB          |   SB          |   SB          |
    /// 
    /// 
    /// Dn, Up, Up  |   Dn, Up, En  |   Dn, Up, Dn  |   Dn, En, Dn  |   Dn, En, En  |   Dn, En, Up  |   Dn, Dn, Dn  |   Dn, Dn, En  |   Dn, Dn, Up  |
    ///             |           SS  |           SS  |           SS  |               |   SB          |           SS  |           SS  |               |
    /// SB      SS  |   SB          |               |           IS  |           SS  |           SS  |           IS  |           IS  |           SS  |
    ///         IS  |               |               |               |           IS  |           IS  |           LS  |           LS  |           IS  |
    public class TrendChannelSuggestionAnalyzer : ISuggestionAnalyzer
    {
        private TrendChannelAnalyzer trendChannelAnalyzer = new TrendChannelAnalyzer();
        private readonly static TrendChannelSuggestionAnalyzer UpUpUp = new UpUpUpTrendChannelSuggestionAnalyzer();
        private readonly static TrendChannelSuggestionAnalyzer UpUpEven = new UpUpEvenTrendChannelSuggestionAnalyzer();
        private readonly static TrendChannelSuggestionAnalyzer UpUpDown = new UpUpDownTrendChannelSuggestionAnalyzer();
        private readonly static TrendChannelSuggestionAnalyzer UpEvenDown = new UpEvenDownTrendChannelSuggestionAnalyzer();
        private readonly static TrendChannelSuggestionAnalyzer UpEvenEven = new UpEvenEvenTrendChannelSuggestionAnalyzer();
        private readonly static TrendChannelSuggestionAnalyzer UpEvenUp = new UpEvenUpTrendChannelSuggestionAnalyzer();
        private readonly static TrendChannelSuggestionAnalyzer UpDownDown = new UpDownDownTrendChannelSuggestionAnalyzer();
        private readonly static TrendChannelSuggestionAnalyzer UpDownEven = new UpDownEvenTrendChannelSuggestionAnalyzer();
        private readonly static TrendChannelSuggestionAnalyzer UpDownUp = new UpDownUpTrendChannelSuggestionAnalyzer();
        private readonly static TrendChannelSuggestionAnalyzer EvenUpUp = new EvenUpUpTrendChannelSuggestionAnalyzer();
        private readonly static TrendChannelSuggestionAnalyzer EvenUpEven = new EvenUpEvenTrendChannelSuggestionAnalyzer();
        private readonly static TrendChannelSuggestionAnalyzer EvenUpDown = new EvenUpDownTrendChannelSuggestionAnalyzer();
        private readonly static TrendChannelSuggestionAnalyzer EvenEvenDown = new EvenEvenDownTrendChannelSuggestionAnalyzer();
        private readonly static TrendChannelSuggestionAnalyzer EvenEvenEven = new EvenEvenEvenTrendChannelSuggestionAnalyzer();
        private readonly static TrendChannelSuggestionAnalyzer EvenEvenUp = new EvenEvenUpTrendChannelSuggestionAnalyzer();
        private readonly static TrendChannelSuggestionAnalyzer EvenDownDown = new EvenDownDownTrendChannelSuggestionAnalyzer();
        private readonly static TrendChannelSuggestionAnalyzer EvenDownEven = new UpDownEvenTrendChannelSuggestionAnalyzer();
        private readonly static TrendChannelSuggestionAnalyzer EvenDownUp = new UpDownUpTrendChannelSuggestionAnalyzer();
        private readonly static TrendChannelSuggestionAnalyzer DownUpUp = new DownUpUpTrendChannelSuggestionAnalyzer();
        private readonly static TrendChannelSuggestionAnalyzer DownUpEven = new DownUpEvenTrendChannelSuggestionAnalyzer();
        private readonly static TrendChannelSuggestionAnalyzer DownUpDown = new DownUpDownTrendChannelSuggestionAnalyzer();
        private readonly static TrendChannelSuggestionAnalyzer DownEvenDown = new DownEvenDownTrendChannelSuggestionAnalyzer();
        private readonly static TrendChannelSuggestionAnalyzer DownEvenEven = new DownEvenEvenTrendChannelSuggestionAnalyzer();
        private readonly static TrendChannelSuggestionAnalyzer DownEvenUp = new DownEvenUpTrendChannelSuggestionAnalyzer();
        private readonly static TrendChannelSuggestionAnalyzer DownDownDown = new DownDownDownTrendChannelSuggestionAnalyzer();
        private readonly static TrendChannelSuggestionAnalyzer DownDownEven = new DownDownEvenTrendChannelSuggestionAnalyzer();
        private readonly static TrendChannelSuggestionAnalyzer DownDownUp = new DownDownUpTrendChannelSuggestionAnalyzer();
        public virtual string Name { get; private set; }
        public Term Term { get; protected set; }
        public Action Action { get; protected set; }
        public double Price { get; protected set; }

        private int shortTerm;
        private int interTerm;
        private int longTerm;

        public TrendChannelSuggestionAnalyzer() : this(20,50,100)
        {
        }

        public TrendChannelSuggestionAnalyzer(int shortTerm, int interTerm, int longTerm)
        {
            this.shortTerm = shortTerm;
            this.interTerm = interTerm;
            this.longTerm = longTerm;
        }

        public double CalculateForecaseCertainty(IList<TransactionData> orderedList)
        {
            var shortTransactions = orderedList.GetRearPartial(shortTerm);
            var interTransactions = orderedList.GetRearPartial(interTerm);
            var longTransactions = orderedList.GetRearPartial(longTerm);
            var shortTrendChannel = trendChannelAnalyzer.AnalyzeTrendChannel(shortTransactions);
            var interTrendChannel = trendChannelAnalyzer.AnalyzeTrendChannel(interTransactions);
            var longTrendChannel = trendChannelAnalyzer.AnalyzeTrendChannel(longTransactions);
            var shortPrice = CalculateSupportResistancePrice(shortTrendChannel);
            var interPrice = CalculateSupportResistancePrice(interTrendChannel);
            var longPrice = CalculateSupportResistancePrice(longTrendChannel);

            //Long - Up, Intermediate - Up, Short - Up
            if (longTrendChannel.ChannelTrend > 0 && interTrendChannel.ChannelTrend > 0 && shortTrendChannel.ChannelTrend > 0)
            {
                var certainty = CheckAnalyzerCertainty(UpUpUp, orderedList, shortTrendChannel, shortPrice, interTrendChannel, interPrice, longTrendChannel, longPrice);
                if (certainty > 0) 
                    return certainty;
            }
            //Long - Up, Intermediate - Up, Short - Even
            if (longTrendChannel.ChannelTrend > 0 && interTrendChannel.ChannelTrend > 0 && shortTrendChannel.ChannelTrend == 0)
            {
                var certainty = CheckAnalyzerCertainty(UpUpEven, orderedList, shortTrendChannel, shortPrice, interTrendChannel, interPrice, longTrendChannel, longPrice);
                if (certainty > 0)
                    return certainty;
            }
            //Long - Up, Intermediate - Up, Short - Down
            if (longTrendChannel.ChannelTrend > 0 && interTrendChannel.ChannelTrend > 0 && shortTrendChannel.ChannelTrend < 0)
            {
                var certainty = CheckAnalyzerCertainty(UpUpDown, orderedList, shortTrendChannel, shortPrice, interTrendChannel, interPrice, longTrendChannel, longPrice);
                if (certainty > 0)
                    return certainty;
            }
            //Long - Up, Intermediate - Even, Short - Down
            if (longTrendChannel.ChannelTrend > 0 && interTrendChannel.ChannelTrend == 0 && shortTrendChannel.ChannelTrend < 0)
            {
                var certainty = CheckAnalyzerCertainty(UpEvenDown, orderedList, shortTrendChannel, shortPrice, interTrendChannel, interPrice, longTrendChannel, longPrice);
                if (certainty > 0)
                    return certainty;
            }
            //Long - Up, Intermediate - Even, Short - Even
            if (longTrendChannel.ChannelTrend > 0 && interTrendChannel.ChannelTrend == 0 && shortTrendChannel.ChannelTrend == 0)
            {
                var certainty = CheckAnalyzerCertainty(UpEvenEven, orderedList, shortTrendChannel, shortPrice, interTrendChannel, interPrice, longTrendChannel, longPrice);
                if (certainty > 0)
                    return certainty;
            }
            //Long - Up, Intermediate - Even, Short - Up
            if (longTrendChannel.ChannelTrend > 0 && interTrendChannel.ChannelTrend == 0 && shortTrendChannel.ChannelTrend > 0)
            {
                var certainty = CheckAnalyzerCertainty(UpEvenUp, orderedList, shortTrendChannel, shortPrice, interTrendChannel, interPrice, longTrendChannel, longPrice);
                if (certainty > 0)
                    return certainty;
            }
            //Long - Up, Intermediate - Down, Short - Down
            if (longTrendChannel.ChannelTrend > 0 && interTrendChannel.ChannelTrend < 0 && shortTrendChannel.ChannelTrend < 0)
            {
                var certainty = CheckAnalyzerCertainty(UpDownDown, orderedList, shortTrendChannel, shortPrice, interTrendChannel, interPrice, longTrendChannel, longPrice);
                if (certainty > 0)
                    return certainty;
            }
            //Long - Up, Intermediate - Down, Short - Even
            if (longTrendChannel.ChannelTrend > 0 && interTrendChannel.ChannelTrend < 0 && shortTrendChannel.ChannelTrend == 0)
            {
                var certainty = CheckAnalyzerCertainty(UpDownEven, orderedList, shortTrendChannel, shortPrice, interTrendChannel, interPrice, longTrendChannel, longPrice);
                if (certainty > 0)
                    return certainty;
            }
            //Long - Up, Intermediate - Down, Short - Up
            if (longTrendChannel.ChannelTrend > 0 && interTrendChannel.ChannelTrend < 0 && shortTrendChannel.ChannelTrend > 0)
            {
                var certainty = CheckAnalyzerCertainty(UpDownUp, orderedList, shortTrendChannel, shortPrice, interTrendChannel, interPrice, longTrendChannel, longPrice);
                if (certainty > 0)
                    return certainty;
            }
            //Long - Even, Intermediate - Up, Short - Up
            if (longTrendChannel.ChannelTrend == 0 && interTrendChannel.ChannelTrend > 0 && shortTrendChannel.ChannelTrend > 0)
            {
                var certainty = CheckAnalyzerCertainty(EvenUpUp, orderedList, shortTrendChannel, shortPrice, interTrendChannel, interPrice, longTrendChannel, longPrice);
                if (certainty > 0)
                    return certainty;
            }
            //Long - Even, Intermediate - Up, Short - Even
            if (longTrendChannel.ChannelTrend == 0 && interTrendChannel.ChannelTrend > 0 && shortTrendChannel.ChannelTrend == 0)
            {
                var certainty = CheckAnalyzerCertainty(EvenUpEven, orderedList, shortTrendChannel, shortPrice, interTrendChannel, interPrice, longTrendChannel, longPrice);
                if (certainty > 0)
                    return certainty;
            }
            //Long - Even, Intermediate - Up, Short - Down
            if (longTrendChannel.ChannelTrend == 0 && interTrendChannel.ChannelTrend > 0 && shortTrendChannel.ChannelTrend < 0)
            {
                var certainty = CheckAnalyzerCertainty(EvenUpDown, orderedList, shortTrendChannel, shortPrice, interTrendChannel, interPrice, longTrendChannel, longPrice);
                if (certainty > 0)
                    return certainty;
            }
            //Long - Even, Intermediate - Even, Short - Down
            if (longTrendChannel.ChannelTrend == 0 && interTrendChannel.ChannelTrend == 0 && shortTrendChannel.ChannelTrend < 0)
            {
                var certainty = CheckAnalyzerCertainty(EvenEvenDown, orderedList, shortTrendChannel, shortPrice, interTrendChannel, interPrice, longTrendChannel, longPrice);
                if (certainty > 0)
                    return certainty;
            }
            //Long - Even, Intermediate - Even, Short - Even
            if (longTrendChannel.ChannelTrend == 0 && interTrendChannel.ChannelTrend == 0 && shortTrendChannel.ChannelTrend == 0)
            {
                var certainty = CheckAnalyzerCertainty(EvenEvenEven, orderedList, shortTrendChannel, shortPrice, interTrendChannel, interPrice, longTrendChannel, longPrice);
                if (certainty > 0)
                    return certainty;
            }
            //Long - Even, Intermediate - Even, Short - Up
            if (longTrendChannel.ChannelTrend == 0 && interTrendChannel.ChannelTrend == 0 && shortTrendChannel.ChannelTrend > 0)
            {
                var certainty = CheckAnalyzerCertainty(EvenEvenUp, orderedList, shortTrendChannel, shortPrice, interTrendChannel, interPrice, longTrendChannel, longPrice);
                if (certainty > 0)
                    return certainty;
            }
            //Long - Even, Intermediate - Down, Short - Down
            if (longTrendChannel.ChannelTrend == 0 && interTrendChannel.ChannelTrend < 0 && shortTrendChannel.ChannelTrend < 0)
            {
                var certainty = CheckAnalyzerCertainty(EvenDownDown, orderedList, shortTrendChannel, shortPrice, interTrendChannel, interPrice, longTrendChannel, longPrice);
                if (certainty > 0)
                    return certainty;
            }
            //Long - Even, Intermediate - Down, Short - Even
            if (longTrendChannel.ChannelTrend == 0 && interTrendChannel.ChannelTrend < 0 && shortTrendChannel.ChannelTrend == 0)
            {
                var certainty = CheckAnalyzerCertainty(EvenDownEven, orderedList, shortTrendChannel, shortPrice, interTrendChannel, interPrice, longTrendChannel, longPrice);
                if (certainty > 0)
                    return certainty;
            }
            //Long - Even, Intermediate - Down, Short - Up
            if (longTrendChannel.ChannelTrend == 0 && interTrendChannel.ChannelTrend < 0 && shortTrendChannel.ChannelTrend > 0)
            {
                var certainty = CheckAnalyzerCertainty(EvenDownUp, orderedList, shortTrendChannel, shortPrice, interTrendChannel, interPrice, longTrendChannel, longPrice);
                if (certainty > 0)
                    return certainty;
            }
            //Long - Down, Intermediate - Up, Short - Up
            if (longTrendChannel.ChannelTrend < 0 && interTrendChannel.ChannelTrend > 0 && shortTrendChannel.ChannelTrend > 0)
            {
                var certainty = CheckAnalyzerCertainty(DownUpUp, orderedList, shortTrendChannel, shortPrice, interTrendChannel, interPrice, longTrendChannel, longPrice);
                if (certainty > 0)
                    return certainty;
            }
            //Long - Down, Intermediate - Up, Short - Even
            if (longTrendChannel.ChannelTrend < 0 && interTrendChannel.ChannelTrend > 0 && shortTrendChannel.ChannelTrend == 0)
            {
                var certainty = CheckAnalyzerCertainty(DownUpEven, orderedList, shortTrendChannel, shortPrice, interTrendChannel, interPrice, longTrendChannel, longPrice);
                if (certainty > 0)
                    return certainty;
            }
            //Long - Down, Intermediate - Up, Short - Down
            if (longTrendChannel.ChannelTrend < 0 && interTrendChannel.ChannelTrend > 0 && shortTrendChannel.ChannelTrend < 0)
            {
                var certainty = CheckAnalyzerCertainty(DownUpDown, orderedList, shortTrendChannel, shortPrice, interTrendChannel, interPrice, longTrendChannel, longPrice);
                if (certainty > 0)
                    return certainty;
            }
            //Long - Down, Intermediate - Even, Short - Down
            if (longTrendChannel.ChannelTrend < 0 && interTrendChannel.ChannelTrend == 0 && shortTrendChannel.ChannelTrend < 0)
            {
                var certainty = CheckAnalyzerCertainty(DownEvenDown, orderedList, shortTrendChannel, shortPrice, interTrendChannel, interPrice, longTrendChannel, longPrice);
                if (certainty > 0)
                    return certainty;
            }
            //Long - Down, Intermediate - Even, Short - Even
            if (longTrendChannel.ChannelTrend < 0 && interTrendChannel.ChannelTrend == 0 && shortTrendChannel.ChannelTrend == 0)
            {
                var certainty = CheckAnalyzerCertainty(DownEvenEven, orderedList, shortTrendChannel, shortPrice, interTrendChannel, interPrice, longTrendChannel, longPrice);
                if (certainty > 0)
                    return certainty;
            }
            //Long - Down, Intermediate - Even, Short - Up
            if (longTrendChannel.ChannelTrend < 0 && interTrendChannel.ChannelTrend == 0 && shortTrendChannel.ChannelTrend > 0)
            {
                var certainty = CheckAnalyzerCertainty(DownEvenUp, orderedList, shortTrendChannel, shortPrice, interTrendChannel, interPrice, longTrendChannel, longPrice);
                if (certainty > 0)
                    return certainty;
            }
            //Long - Down, Intermediate - Down, Short - Down
            if (longTrendChannel.ChannelTrend < 0 && interTrendChannel.ChannelTrend < 0 && shortTrendChannel.ChannelTrend < 0)
            {
                var certainty = CheckAnalyzerCertainty(DownDownDown, orderedList, shortTrendChannel, shortPrice, interTrendChannel, interPrice, longTrendChannel, longPrice);
                if (certainty > 0)
                    return certainty;
            }
            //Long - Down, Intermediate - Down, Short - Even
            if (longTrendChannel.ChannelTrend < 0 && interTrendChannel.ChannelTrend < 0 && shortTrendChannel.ChannelTrend == 0)
            {
                var certainty = CheckAnalyzerCertainty(DownDownEven, orderedList, shortTrendChannel, shortPrice, interTrendChannel, interPrice, longTrendChannel, longPrice);
                if (certainty > 0)
                    return certainty;
            }
            //Long - Down, Intermediate - Down, Short - Up
            if (longTrendChannel.ChannelTrend < 0 && interTrendChannel.ChannelTrend < 0 && shortTrendChannel.ChannelTrend > 0)
            {
                var certainty = CheckAnalyzerCertainty(DownDownUp, orderedList, shortTrendChannel, shortPrice, interTrendChannel, interPrice, longTrendChannel, longPrice);
                if (certainty > 0)
                    return certainty;
            }
            return 0;
        }

        private double CheckAnalyzerCertainty(TrendChannelSuggestionAnalyzer analyzer, IList<TransactionData> orderedList, Channel shortTrendChannel, Tuple<double, double> shortPrice,
            Channel interTrendChannel, Tuple<double, double> interPrice, Channel longTrendChannel, Tuple<double, double> longPrice)
        {
            var certainty = analyzer.CalculateForecaseCertainty(orderedList, shortTrendChannel, shortPrice, interTrendChannel,
                interPrice, longTrendChannel, longPrice);
            if (certainty > 0)
            {
                SetSuggestion(UpUpUp);
            }
            return certainty;
        }

        private void SetSuggestion(TrendChannelSuggestionAnalyzer suggestionAnalyzer)
        {
            Name = suggestionAnalyzer.Name;
            Term = suggestionAnalyzer.Term;
            Action = suggestionAnalyzer.Action;
            Price = suggestionAnalyzer.Price;
        }

        public virtual double CalculateForecaseCertainty(IList<TransactionData> orderedTransactions, Channel shortChannel,
            Tuple<double, double> shortPrice, Channel interChannel, Tuple<double, double> interPrice, Channel longChannel,
            Tuple<double, double> longPrice)
        {
            return 0;
        }

        protected Tuple<double, double> CalculateSupportResistancePrice(Channel trendChannel)
        {
            int resistanceIndex = trendChannel.Length - 1;
            int supportIndex = trendChannel.Length - 1;
            if (trendChannel.ChannelTrend > 0)
            {
                resistanceIndex--;
                supportIndex++;
            }
            else if (trendChannel.ChannelTrend < 0)
            {
                resistanceIndex++;
                supportIndex--;
            }
            double resistancePrice = trendChannelAnalyzer.CalculatePriceAt(resistanceIndex,
                trendChannel.ResistanceChannelRatio, trendChannel.ResistanceStartPrice, 0);
            double supportPrice = trendChannelAnalyzer.CalculatePriceAt(supportIndex,
                trendChannel.SupportChannelRatio, trendChannel.SupportStartPrice, 0);
            return new Tuple<double, double>(supportPrice, resistancePrice);
        }
    }
}