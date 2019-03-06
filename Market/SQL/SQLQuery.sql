select StockKey, MAX(Timestamp)
from TransactionData
group by StockKey
having MAX(Timestamp) < '2015-01-09'

select * from Stock
where [Key] = 259
where id = 'L.TO'

select * from TransactionData
where StockKey = 231
order by TimeStamp

select TimeStamp, Volume, ClosePrice, SuggestedAction, SuggestedTerm, Accuracy from Suggestion
where StockKey = 5
 and Accuracy > 0
order by TimeStamp

select SuggestedTerm, Avg(Accuracy), sum(case when accuracy = 0 then 1 else 0 end), sum(case when accuracy > 0 then 1 else 0 end)
from Suggestion
group by SuggestedTerm

select * from Suggestion
order by TimeStamp desc

--delete from TransactionSimulator
select * from TransactionSimulator
where stockKey = 185

select * from TransactionData
where StockKey = 45
  and TimeStamp between '2013-08-01' and '2013-08-15'
order by TimeStamp

select TS.StockKey, sum(sellPrice * TS.volume) - sum(BuyPrice * TS.Volume), count(*), Avg(s.ClosePrice), avg(s.Volume)
from TransactionSimulator TS
inner join Suggestion s on TS.SuggestionKey = s.[Key]
where SellDate is not null
  and s.SuggestedTerm = 3
group by TS.stockKey
order by 2

select StockKey, TimeStamp, StockId, StockName, Volume, ClosePrice, SuggestedAction, SuggestedTerm, AnalyzerName, Pattern from Suggestion
where TimeStamp = '2019-01-02'
  and volume > 100000
  and SuggestedAction = 1
order by StockId

select StockKey, TimeStamp, StockId, StockName, Volume, ClosePrice, SuggestedAction, SuggestedTerm, AnalyzerName, Pattern from Suggestion
where StockId LIKE 'HOD%'
order by TimeStamp desc

select * from MovingAverageConvergenceDivergence
where StockKey = 11
order by TimeStamp desc

select * from TransactionData
where StockKey = 477
order by TimeStamp desc
