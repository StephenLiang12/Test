using System;
using System.Collections.Generic;
using Market.Analyzer.CandleStickPatterns;

namespace Market.Analyzer
{
    public class CandleStickPatternAnalyzer
    {
        public IList<Pattern> LoadPatterns()
        {
            IList<Pattern> list = new List<Pattern>();
            list.Add(new Marubozu());
            list.Add(new BeltHoldLines());
            list.Add(new DojiGraveStone());
            list.Add(new DojiDragonFly());
            list.Add(new Hammer());
            list.Add(new HangingMan());
            list.Add(new BearishEngulfing());
            list.Add(new BullishEngulfing());
            list.Add(new DarkCloudCover());
            list.Add(new BearishCounterAttack());
            list.Add(new BullishCounterAttack());
            list.Add(new BearishHaramiCross());
            list.Add(new BullishHaramiCross());
            list.Add(new BearishAbandonedBaby());
            list.Add(new BullishAbandonedBaby());

            list.Add(new UnknownPattern());
            return list;
        }

        public Pattern GetPattern(IList<TransactionData> orderedList, Trend currentTrend)
        {
            var list = LoadPatterns();
            foreach (var pattern in list)
            {
                if (pattern.Qualified(orderedList, currentTrend))
                    return pattern;
            }
            throw new ArgumentException("At least it will return unknown pattern");
        }
    }
}