﻿using System.Collections.Generic;

namespace Market.Suggestions
{
    public interface ISuggestionAnalyzer
    {
        string Name { get; }
        Term Term { get; }
        Action Action { get; }
        string Pattern { get; }
        double Price { get; }
        double CalculateForecaseCertainty(IList<TransactionData> orderedTransactions);
    }
}