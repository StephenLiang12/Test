--Create Stock table
if not exists (select * from sysobjects where name = 'Stock' and xtype = 'u')
	CREATE TABLE Stock (
		[Key]	int Primary Key Identity(1,1),
		Id		varchar(50) NOT NULL unique,
		Name	nvarchar(50) NOT NULL, 
		AvgVolume	float not null default 0,
		AbleToGetTransactionDataFromWeb bit not null default 1
	)

--Create TransactionData table
if not exists (select * from sysobjects where name = 'TransactionData' and xtype = 'u')
	CREATE TABLE TransactionData (
		[Key]	bigint Primary Key Identity(1,1),
		StockKey	int not null references Stock([Key]),
		TimeStamp	DateTime not null,
		Period	int not null,
		[Open]	float not null,
		[Close]	float not null,
		High	float not null,
		Low		float not null,
		Volume	float not null
	)

--Add SimpleAvg5 Colume on TransactionData table
if (not exists (select c.name from sysobjects o
				inner join syscolumns c on o.id = c.id
				where o.name = 'TransactionData'
					and o.xtype = 'u'
					and c.name = 'SimpleAvg5'))
	alter table TransactionData add SimpleAvg5 float not null default 0

--Add SimpleAvg10 Colume on TransactionData table
if (not exists (select c.name from sysobjects o
				inner join syscolumns c on o.id = c.id
				where o.name = 'TransactionData'
					and o.xtype = 'u'
					and c.name = 'SimpleAvg10'))
	alter table TransactionData add SimpleAvg10 float not null default 0

--Add SimpleAvg20 Colume on TransactionData table
if (not exists (select c.name from sysobjects o
				inner join syscolumns c on o.id = c.id
				where o.name = 'TransactionData'
					and o.xtype = 'u'
					and c.name = 'SimpleAvg20'))
	alter table TransactionData add SimpleAvg20 float not null default 0

--Add SimpleAvg50 Colume on TransactionData table
if (not exists (select c.name from sysobjects o
				inner join syscolumns c on o.id = c.id
				where o.name = 'TransactionData'
					and o.xtype = 'u'
					and c.name = 'SimpleAvg50'))
	alter table TransactionData add SimpleAvg50 float not null default 0

--Add SimpleAvg100 Colume on TransactionData table
if (not exists (select c.name from sysobjects o
				inner join syscolumns c on o.id = c.id
				where o.name = 'TransactionData'
					and o.xtype = 'u'
					and c.name = 'SimpleAvg100'))
	alter table TransactionData add SimpleAvg100 float not null default 0

--Add SimpleAvg200 Colume on TransactionData table
if (not exists (select c.name from sysobjects o
				inner join syscolumns c on o.id = c.id
				where o.name = 'TransactionData'
					and o.xtype = 'u'
					and c.name = 'SimpleAvg200'))
	alter table TransactionData add SimpleAvg200 float not null default 0

--Create Split table
if not exists (select * from sysobjects where name = 'Split' and xtype = 'u')
	CREATE TABLE Split (
		[Key]	bigint Primary Key Identity(1,1),
		StockKey	int not null references Stock([Key]),
		TimeStamp	DateTime not null,
		SplitRatio	float not null,
		unique (StockKey, TimeStamp)
	)

--Create Suggestion table
if not exists (select * from sysobjects where name = 'Suggestion' and xtype = 'u')
	CREATE TABLE Suggestion (
		[Key]	bigint Primary Key Identity(1,1),
		StockKey	int not null references Stock([Key]),
		TimeStamp	DateTime not null,
		StockId		varchar(50) NOT NULL,
		StockName	nvarchar(50) NOT NULL, 
		Volume		float not null,
		ClosePrice	float not null,
		SuggestedAction	int not null,
		SuggestedTerm	int not null,
		CandleStickPattern	nvarchar(50) NOT NULL,
		MACD	float not null,
		Avg5Trend	int not null,
		Avg20Trend	int not null,
		Avg200Trend int not null,
		PriceVSAvg5	float not null,
		PriceVSAvg200	float not null,
		Avg5VSAvg20	float not null,
		Avg50VSAvg200	float not null,
		Accuracy	float not null
	)

--Add Analyzer Name Column on Suggestion table
if (not exists (select c.name from sysobjects o
				inner join syscolumns c on o.id = c.id
				where o.name = 'Suggestion'
					and o.xtype = 'u'
					and c.name = 'AnalyzerName'))
	alter table Suggestion add AnalyzerName nvarchar(200)

--Add Suggested Price Column on Suggestion table
if (not exists (select c.name from sysobjects o
				inner join syscolumns c on o.id = c.id
				where o.name = 'Suggestion'
					and o.xtype = 'u'
					and c.name = 'SuggestedPrice'))
	alter table Suggestion add SuggestedPrice float null

--Create TransactionSimulator table
if not exists (select * from sysobjects where name = 'TransactionSimulator' and xtype = 'u')
	CREATE TABLE TransactionSimulator (
		[Key]	bigint Primary Key Identity(1,1),
		StockKey	int not null references Stock([Key]),
		SuggestionKey	bigint not null references Suggestion([Key]),
		BuyPrice	float not null,
		BuyDate		DateTime not null,
		SellPrice	float null,
		SellDate	DateTime null,
		Volume	float not null
	)

--Create Channel table
if not exists (select * from sysobjects where name = 'Channel' and xtype = 'u')
	CREATE TABLE Channel (
		[Key]	bigint Primary Key Identity(1,1),
		StockKey	int not null references Stock([Key]),
		ChannelTrend	int not null,
		StartDate		DateTime not null,
		EndDate		DateTime not null,
		ResistanceStartPrice	float not null,
		SupportStartPrice	float not null,
		ResistanceChannelRatio	float not null,
		SupportChannelRatio	float not null,
		Length	int not null
	)

--Add SuggestedPrice Column on Suggestion table
if (not exists (select c.name from sysobjects o
				inner join syscolumns c on o.id = c.id
				where o.name = 'Suggestion'
					and o.xtype = 'u'
					and c.name = 'SuggestedPrice'))
	alter table Suggestion add SuggestedPrice float

--Create MovingAverageConvergenceDivergence table
if not exists (select * from sysobjects where name = 'MovingAverageConvergenceDivergence' and xtype = 'u')
	CREATE TABLE MovingAverageConvergenceDivergence (
		[Key]	bigint Primary Key Identity(1,1),
		StockKey	int not null references Stock([Key]),
		TimeStamp	DateTime not null,
		MACD	float not null,
		Signal	float not null,
		Histogram	float not null
	)

--Create OriginalTransactionData table
if not exists (select * from sysobjects where name = 'OriginalTransactionData' and xtype = 'u')
	CREATE TABLE OriginalTransactionData (
		[Key]	bigint Primary Key Identity(1,1),
		StockKey	int not null references Stock([Key]),
		TimeStamp	DateTime not null,
		Period	int not null,
		[Open]	float not null,
		[Close]	float not null,
		High	float not null,
		Low		float not null,
		Volume	float not null
	)

--set unique constraints on StockKey, TimeStamp and Period column on OriginalTransactionData
if (not exists (select i.* from sysobjects o
                    inner join sys.indexes i on o.id = i.object_id
                    where o.name = 'OriginalTransactionData'
                      and exists (select ic.* from sys.index_columns ic 
                                    inner join sys.columns c on ic.object_id = c.object_id and ic.column_id = c.column_id
                                    where o.id = ic.object_id and i.index_id = ic.index_id and c.name = 'StockKey')
                      and exists (select ic.* from sys.index_columns ic 
                                    inner join sys.columns c on ic.object_id = c.object_id and ic.column_id = c.column_id
                                    where o.id = ic.object_id and i.index_id = ic.index_id and c.name = 'TimeStamp')
                      and exists (select ic.* from sys.index_columns ic 
                                    inner join sys.columns c on ic.object_id = c.object_id and ic.column_id = c.column_id
                                    where o.id = ic.object_id and i.index_id = ic.index_id and c.name = 'Period')))
    alter table OriginalTransactionData add unique (StockKey, TimeStamp, Period)

--set unique constraints on StockKey, TimeStamp and Period column on TransactionData
if (not exists (select i.* from sysobjects o
                    inner join sys.indexes i on o.id = i.object_id
                    where o.name = 'TransactionData'
                      and exists (select ic.* from sys.index_columns ic 
                                    inner join sys.columns c on ic.object_id = c.object_id and ic.column_id = c.column_id
                                    where o.id = ic.object_id and i.index_id = ic.index_id and c.name = 'StockKey')
                      and exists (select ic.* from sys.index_columns ic 
                                    inner join sys.columns c on ic.object_id = c.object_id and ic.column_id = c.column_id
                                    where o.id = ic.object_id and i.index_id = ic.index_id and c.name = 'TimeStamp')
                      and exists (select ic.* from sys.index_columns ic 
                                    inner join sys.columns c on ic.object_id = c.object_id and ic.column_id = c.column_id
                                    where o.id = ic.object_id and i.index_id = ic.index_id and c.name = 'Period')))
    alter table TransactionData add unique (StockKey, TimeStamp, Period)

--Add Applied Colume on Split table
if (not exists (select c.name from sysobjects o
				inner join syscolumns c on o.id = c.id
				where o.name = 'Split'
					and o.xtype = 'u'
					and c.name = 'Applied'))
	alter table Split add Applied bit not null default 0

--Add Action Colume on TransactionSimulator table
if (not exists (select c.name from sysobjects o
				inner join syscolumns c on o.id = c.id
				where o.name = 'TransactionSimulator'
					and o.xtype = 'u'
					and c.name = 'Action'))
	alter table TransactionSimulator add Action int not null default 0

--Add TimeStamp Colume on TransactionSimulator table
if (not exists (select c.name from sysobjects o
				inner join syscolumns c on o.id = c.id
				where o.name = 'TransactionSimulator'
					and o.xtype = 'u'
					and c.name = 'TimeStamp'))
	alter table TransactionSimulator add TimeStamp DateTime not null default '2000-1-1'

--Add Price Colume on TransactionSimulator table
if (not exists (select c.name from sysobjects o
				inner join syscolumns c on o.id = c.id
				where o.name = 'TransactionSimulator'
					and o.xtype = 'u'
					and c.name = 'Price'))
	alter table TransactionSimulator add Price float not null default 0

--Remove BuyPrice Colume on TransactionSimulator table
if (exists (select c.name from sysobjects o
				inner join syscolumns c on o.id = c.id
				where o.name = 'TransactionSimulator'
					and o.xtype = 'u'
					and c.name = 'BuyPrice'))
	alter table TransactionSimulator drop column BuyPrice

--Remove BuyDate Colume on TransactionSimulator table
if (exists (select c.name from sysobjects o
				inner join syscolumns c on o.id = c.id
				where o.name = 'TransactionSimulator'
					and o.xtype = 'u'
					and c.name = 'BuyDate'))
	alter table TransactionSimulator drop column BuyDate

--Remove SellPrice Colume on TransactionSimulator table
if (exists (select c.name from sysobjects o
				inner join syscolumns c on o.id = c.id
				where o.name = 'TransactionSimulator'
					and o.xtype = 'u'
					and c.name = 'SellPrice'))
	alter table TransactionSimulator drop column SellPrice

--Remove SellDate Colume on TransactionSimulator table
if (exists (select c.name from sysobjects o
				inner join syscolumns c on o.id = c.id
				where o.name = 'TransactionSimulator'
					and o.xtype = 'u'
					and c.name = 'SellDate'))
	alter table TransactionSimulator drop column SellDate

--set unique constraints on StockKey and TimeStamp column on MovingAverageConvergenceDivergence
if (not exists (select i.* from sysobjects o
                    inner join sys.indexes i on o.id = i.object_id
                    where o.name = 'MovingAverageConvergenceDivergence'
                      and exists (select ic.* from sys.index_columns ic 
                                    inner join sys.columns c on ic.object_id = c.object_id and ic.column_id = c.column_id
                                    where o.id = ic.object_id and i.index_id = ic.index_id and c.name = 'StockKey')
                      and exists (select ic.* from sys.index_columns ic 
                                    inner join sys.columns c on ic.object_id = c.object_id and ic.column_id = c.column_id
                                    where o.id = ic.object_id and i.index_id = ic.index_id and c.name = 'TimeStamp')))
    alter table MovingAverageConvergenceDivergence add unique (StockKey, TimeStamp)

--set unique constraints on StockKey, StartDate and Length column on Channel
if (not exists (select i.* from sysobjects o
                    inner join sys.indexes i on o.id = i.object_id
                    where o.name = 'Channel'
                      and exists (select ic.* from sys.index_columns ic 
                                    inner join sys.columns c on ic.object_id = c.object_id and ic.column_id = c.column_id
                                    where o.id = ic.object_id and i.index_id = ic.index_id and c.name = 'StockKey')
                      and exists (select ic.* from sys.index_columns ic 
                                    inner join sys.columns c on ic.object_id = c.object_id and ic.column_id = c.column_id
                                    where o.id = ic.object_id and i.index_id = ic.index_id and c.name = 'StartDate')
                      and exists (select ic.* from sys.index_columns ic 
                                    inner join sys.columns c on ic.object_id = c.object_id and ic.column_id = c.column_id
                                    where o.id = ic.object_id and i.index_id = ic.index_id and c.name = 'Length')))
    alter table Channel add unique (StockKey, StartDate, Length)

--Create MovingAverageConvergenceDivergenceAnalysis table
if not exists (select * from sysobjects where name = 'MovingAverageConvergenceDivergenceAnalysis' and xtype = 'u')
	CREATE TABLE MovingAverageConvergenceDivergenceAnalysis (
		[Key]	bigint Primary Key Identity(1,1),
		StockKey	int not null references Stock([Key]),
		TimeStamp	DateTime not null,
		MACD	float not null,
		Signal	float not null,
		Histogram	float not null,
		Feature	int
	)

--Add AvgDaysChannel50Reverse Column on Stock table
if (not exists (select c.name from sysobjects o
				inner join syscolumns c on o.id = c.id
				where o.name = 'Stock'
					and o.xtype = 'u'
					and c.name = 'AvgDaysChannel50Reverse'))
	alter table Stock add AvgDaysChannel50Reverse float not null default 0

--Add AvgDaysChannel100Reverse Column on Stock table
if (not exists (select c.name from sysobjects o
				inner join syscolumns c on o.id = c.id
				where o.name = 'Stock'
					and o.xtype = 'u'
					and c.name = 'AvgDaysChannel100Reverse'))
	alter table Stock add AvgDaysChannel100Reverse float not null default 0

--Add AvgDaysChannel200Reverse Column on Stock table
if (not exists (select c.name from sysobjects o
				inner join syscolumns c on o.id = c.id
				where o.name = 'Stock'
					and o.xtype = 'u'
					and c.name = 'AvgDaysChannel200Reverse'))
	alter table Stock add AvgDaysChannel200Reverse float not null default 0

--Create MovingAverageConvergenceDivergenceFeatureAnalysis table
if not exists (select * from sysobjects where name = 'MovingAverageConvergenceDivergenceFeatureAnalysis' and xtype = 'u')
	CREATE TABLE MovingAverageConvergenceDivergenceFeatureAnalysis (
		[Key]	bigint Primary Key Identity(1,1),
		StockKey	int not null references Stock([Key]),
		FeatureKey	INT NOT null,
		FeatureName	NVARCHAR(100) NOT NULL,
		Count int NOT NULL,
		AverageAccuracy	FLOAT NOT NULL,
		AverageChangePercentage FLOAT NOT NULL,
		MaxChangePercentage	FLOAT NOT NULL
	)

--Add Pattern Name Column on Suggestion table
if (not exists (select c.name from sysobjects o
				inner join syscolumns c on o.id = c.id
				where o.name = 'Suggestion'
					and o.xtype = 'u'
					and c.name = 'Pattern'))
	alter table Suggestion add Pattern nvarchar(200)

--Create TrendChannelBreakAnalysis table
if not exists (select * from sysobjects where name = 'TrendChannelBreakAnalysis' and xtype = 'u')
	CREATE TABLE TrendChannelBreakAnalysis (
		[Key]	bigint Primary Key Identity(1,1),
		StockKey	int not null references Stock([Key]),
		FeatureName	NVARCHAR(100) NOT NULL,
		Count int NOT NULL,
		AverageAccuracy	FLOAT NOT NULL,
		AverageChangePercentage FLOAT NOT NULL,
		MaxChangePercentage	FLOAT NOT NULL
	)

