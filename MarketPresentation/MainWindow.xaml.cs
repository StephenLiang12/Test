using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Market.Analyzer;
using Market.Analyzer.Channels;
using Market.Analyzer.MACD;

namespace Market.Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly StockContext stockContext = new StockContext();
        private const int X0Pixel = 30;
        private const int Y0Pixel = 30;
        private const int XLengthPixel = 1500;
        private const int YLengthPixel = 500;
        private const int macdLengthPixel = 160;
        private const int XLabelPixel = 50;
        private const int YLabelPixel = 50;
        private const int MacdLabelPixel = 40;
        private const int TransactionLinePixel = 10;

        private IList<UIElement> chartDrawingElements = new List<UIElement>();
        private IList<UIElement> macdDrawingElements = new List<UIElement>();
        private IList<UIElement> days200DrawingElements = new List<UIElement>();
        private IList<UIElement> days100DrawingElements = new List<UIElement>();
        private IList<UIElement> days50DrawingElements = new List<UIElement>();
        private IList<UIElement> days20DrawingElements = new List<UIElement>();
        private Dictionary<DateTime, double> xPixelDictionary = new Dictionary<DateTime, double>();
        private double startPrice;
        private double priceIncrementPerLabel;
        private double macdIncrementPerLabel;
        private double volumePerPixel;

        public MainWindow()
        {
            InitializeComponent();
            StockComboBox.Items.Clear();
            foreach (var stock in stockContext.Stocks)
            {
                StockComboBox.Items.Add(stock.Id);
            }
        }

        private void StockComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StartDateComboBox.Items.Clear();
            if (StockComboBox.SelectedValue == null)
                return;

            var stockKey = stockContext.Stocks.First(s => s.Id == StockComboBox.SelectedValue).Key;
            foreach (var transactionData in stockContext.TransactionData.Where(t => t.StockKey == stockKey).OrderBy(t => t.TimeStamp))
            {
                StartDateComboBox.Items.Add(transactionData.TimeStamp);
            }
        }

        private void StartDateComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EndDateComboBox.Items.Clear();
            if (StockComboBox.SelectedValue == null)
                return;

            if (StartDateComboBox.SelectedValue == null)
                return;

            var stockKey = stockContext.Stocks.First(s => s.Id == StockComboBox.SelectedValue).Key;
            var startDate = Convert.ToDateTime(StartDateComboBox.SelectedValue.ToString());
            foreach (var transactionData in stockContext.TransactionData.Where(t => t.StockKey == stockKey).OrderBy(t => t.TimeStamp))
            {
                if (transactionData.TimeStamp > startDate)
                    EndDateComboBox.Items.Add(transactionData.TimeStamp);
            }
        }

        private void SearchButton_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var drawingElement in chartDrawingElements)
            {
                ChartCanvas.Children.Remove(drawingElement);
            }
            chartDrawingElements.Clear();
            foreach (var drawingElement in macdDrawingElements)
            {
                MacdCanvas.Children.Remove(drawingElement);
            }
            macdDrawingElements.Clear();
            xPixelDictionary.Clear();
            if (StockComboBox.SelectedValue == null)
                return;

            if (StartDateComboBox.SelectedValue == null)
                return;
            var stockKey = stockContext.Stocks.First(s => s.Id == StockComboBox.SelectedValue).Key;
            var startDate = Convert.ToDateTime(StartDateComboBox.SelectedValue);
            var endDate = stockContext.TransactionData.Where(t => t.StockKey == stockKey).Max(t => t.TimeStamp);
            if (string.IsNullOrEmpty(EndDateComboBox.SelectedValue.ToString()) == false)
                endDate = Convert.ToDateTime(EndDateComboBox.SelectedValue);
            var orderedTransactionList =
                stockContext.TransactionData.Where(
                    t => t.StockKey == stockKey && t.TimeStamp >= startDate && t.TimeStamp <= endDate)
                    .OrderBy(t => t.TimeStamp).ToList();
            int count = orderedTransactionList.Count();
            var transactionsPerLine = GetTransactionPerLine(count);
            int dateTimeLabelIncrement = GetDateTimeLabelIncrement(count, transactionsPerLine);
            var dateTimeLabels = AddDateTimeLabel(orderedTransactionList, dateTimeLabelIncrement);
            var maxPrice = orderedTransactionList.Max(t => t.High);
            var minPrice = orderedTransactionList.Min(t => t.Low);
            var maxVolume = transactionsPerLine * orderedTransactionList.Max(t => t.Volume);
            var minVolume = transactionsPerLine * orderedTransactionList.Min(t => t.Volume);
            volumePerPixel = (maxVolume - minVolume) / 100d;
            TotalTransactionLabel.Content = count;
            LowLabel.Content = minPrice;
            HighLabel.Content = maxPrice;
            var diff = maxPrice - minPrice;
            priceIncrementPerLabel = GetPriceIncrementPerLabel(diff);
            startPrice = GetStartPriceLabel(minPrice, priceIncrementPerLabel);
            AddPriceLabel(startPrice, priceIncrementPerLabel);
            var macds = stockContext.MovingAverageConvergenceDivergences.Where(t => t.StockKey == stockKey && t.TimeStamp >= startDate && t.TimeStamp <= endDate);
            double macdHeight =0;
            foreach (var macd in macds)
            {
                if (macdHeight < Math.Abs(macd.MACD))
                    macdHeight = Math.Abs(macd.MACD);
                if (macdHeight < Math.Abs(macd.Signal))
                    macdHeight = Math.Abs(macd.Signal);
                if (macdHeight < Math.Abs(macd.Histogram))
                    macdHeight = Math.Abs(macd.Histogram);
            }
            macdIncrementPerLabel = GetMacdIncrementPerLabel(macdHeight);
            AddMacdLabel(macdIncrementPerLabel);
            var transactionIntervalPixel = TransactionLinePixel;
            var transactionPerDateTimeLabel = XLabelPixel/TransactionLinePixel;
            if (transactionsPerLine == 1)
            {
                transactionIntervalPixel = XLabelPixel / dateTimeLabelIncrement;
                transactionPerDateTimeLabel = dateTimeLabelIncrement;
            }

            int i = 0;
            int j = 0;
            int k = 0;
            MovingAverageConvergenceDivergence previousMacd = null;
            int previousX = 0;
            int x = 0;
            while (i < orderedTransactionList.Count)
            {
                previousX = x;
                x = (j + 1) * XLabelPixel + k * transactionIntervalPixel + X0Pixel;
                xPixelDictionary.Add(orderedTransactionList[i].TimeStamp, x);
                var open = orderedTransactionList[i].Open;
                var endIndex = i + transactionsPerLine - 1;
                if (endIndex >= orderedTransactionList.Count)
                    endIndex = orderedTransactionList.Count - 1;
                var close = orderedTransactionList[endIndex].Close;
                var high = orderedTransactionList[i].High;
                var low = orderedTransactionList[i].Low;
                var volume = orderedTransactionList[i].Volume;
                for (int n = i + 1; n <= endIndex; n++)
                {
                    if (orderedTransactionList[n].High > high)
                        high = orderedTransactionList[n].High;
                    if (orderedTransactionList[n].Low < low)
                        low = orderedTransactionList[n].Low;
                    volume += orderedTransactionList[n].Volume;
                }
                Line line = new Line();
                line.Stroke = new SolidColorBrush(Colors.Black);
                line.X1 = x;
                line.X2 = x;
                line.Y1 = GetYForPrice(high);
                line.Y2 = GetYForPrice(low);
                ChartCanvas.Children.Add(line);
                chartDrawingElements.Add(line);
                line = new Line();
                line.Stroke = new SolidColorBrush(Colors.Black);
                line.X1 = x - 3;
                line.X2 = x;
                line.Y1 = GetYForPrice(open);
                line.Y2 = GetYForPrice(open);
                ChartCanvas.Children.Add(line);
                chartDrawingElements.Add(line);
                line = new Line();
                line.Stroke = new SolidColorBrush(Colors.Black);
                line.X1 = x;
                line.X2 = x + 3;
                line.Y1 = GetYForPrice(close);
                line.Y2 = GetYForPrice(close);
                ChartCanvas.Children.Add(line);
                chartDrawingElements.Add(line);
                Rectangle rec = new Rectangle();
                rec.Stroke = new SolidColorBrush(Colors.Black);
                rec.Fill = new SolidColorBrush(Colors.LightGray);
                rec.Width = 6;
                rec.Height = (volume - minVolume) / volumePerPixel + 50;
                MacdCanvas.Children.Add(rec);
                Canvas.SetTop(rec, GetYForVolume(volume - minVolume));
                Canvas.SetLeft(rec, x - 3);
                macdDrawingElements.Add(rec);
                previousMacd = AddMovingAverageConvergenceDivergence(previousMacd, orderedTransactionList[endIndex], previousX, x);
                i += transactionsPerLine;
                k++;
                if (k == transactionPerDateTimeLabel)
                {
                    k = 0;
                    j++;
                }
            }
        }

        private void AddMacdLabel(double macdIncrementPerLabel)
        {
            double macd = 0;
            AddMacdLabel(macd, 0);
            for (int i = 1; i < 5; i++)
            {
                AddMacdLabel(macd + i * macdIncrementPerLabel, i);
                AddMacdLabel(macd - i * macdIncrementPerLabel, -i);
                AddMaceLine(i);
                AddMaceLine(-i);
            }
        }

        private void AddMaceLine(int i)
        {
            Line line = new Line();
            line.Stroke = new SolidColorBrush(Colors.Aquamarine);
            line.X1 = ZeroLine.X1 - 3;
            line.X2 = ZeroLine.X2;
            line.Y1 = ZeroLine.Y1 + i*MacdLabelPixel;
            line.Y2 = ZeroLine.Y1 + i*MacdLabelPixel;
            MacdCanvas.Children.Add(line);
            macdDrawingElements.Add(line);
        }

        private void AddMacdLabel(double macd, int index)
        {
            TextBox textBox = new TextBox();
            textBox.Text = macd.ToString();
            textBox.FontSize = 10;
            textBox.BorderThickness = new Thickness(0);
            Canvas.SetLeft(textBox, 0);
            Canvas.SetBottom(textBox, ZeroLine.Y1 - 20 + index * MacdLabelPixel);
            MacdCanvas.Children.Add(textBox);
            macdDrawingElements.Add(textBox);
        }

        private double GetMacdIncrementPerLabel(double macdHeight)
        {
            int log = Convert.ToInt32(Math.Log10(macdHeight)) - 1;
            int decimalPlaces = -log;
            if (decimalPlaces < 0)
                decimalPlaces = 0;
            double increment = Math.Round(macdHeight/4, decimalPlaces) + Math.Pow(10, log);
            return increment;
        }

        private MovingAverageConvergenceDivergence AddMovingAverageConvergenceDivergence(MovingAverageConvergenceDivergence previousMacd, TransactionData transactionData, int prevoiusX, int x)
        {
            MovingAverageConvergenceDivergence macd =
                stockContext.MovingAverageConvergenceDivergences.FirstOrDefault(
                    m => m.StockKey == transactionData.StockKey && m.TimeStamp == transactionData.TimeStamp);
            if (previousMacd == null || macd == null)
                return macd;
            Line line = new Line();
            line.Stroke = new SolidColorBrush(Colors.Blue);
            line.X1 = prevoiusX;
            line.X2 = x;
            line.Y1 = GetYForMacd(previousMacd.MACD);
            line.Y2 = GetYForMacd(macd.MACD);
            MacdCanvas.Children.Add(line);
            macdDrawingElements.Add(line);
            line = new Line();
            line.Stroke = new SolidColorBrush(Colors.Red);
            line.X1 = prevoiusX;
            line.X2 = x;
            line.Y1 = GetYForMacd(previousMacd.Signal);
            line.Y2 = GetYForMacd(macd.Signal);
            MacdCanvas.Children.Add(line);
            macdDrawingElements.Add(line);
            line = new Line();
            line.Stroke = new SolidColorBrush(Colors.Black);
            line.X1 = prevoiusX;
            line.X2 = x;
            line.Y1 = GetYForMacd(previousMacd.Histogram);
            line.Y2 = GetYForMacd(macd.Histogram);
            MacdCanvas.Children.Add(line);
            macdDrawingElements.Add(line);
            return macd;
        }

        private DateTime[] AddDateTimeLabel(List<TransactionData> orderedTransactionList, int dateTimeLabelIncrement)
        {
            List<DateTime> dateTimeLabels = new List<DateTime>();
            int i = 0;
            int j = 0;
            while (i < orderedTransactionList.Count)
            {
                TextBox textBox = new TextBox();
                dateTimeLabels.Add(orderedTransactionList[i].TimeStamp);
                if (dateTimeLabelIncrement >= 30)
                    textBox.Text = orderedTransactionList[i].TimeStamp.ToString("yy-MM");
                else
                    textBox.Text = orderedTransactionList[i].TimeStamp.ToString("MM-dd");
                textBox.FontSize = 10;
                textBox.BorderThickness = new Thickness(0);
                var x = (j + 1)*XLabelPixel + X0Pixel;
                Canvas.SetLeft(textBox, x - 10);
                Canvas.SetBottom(textBox, YLabelPixel);
                ChartCanvas.Children.Add(textBox);
                chartDrawingElements.Add(textBox);
                Line line = new Line();
                line.Stroke = new SolidColorBrush(Colors.Aquamarine);
                line.X1 = x;
                line.X2 = x;
                line.Y1 = DateLine.Y1 - 500;
                line.Y2 = DateLine.Y1 + 3;
                ChartCanvas.Children.Add(line);
                chartDrawingElements.Add(line);
                line = new Line();
                line.Stroke = new SolidColorBrush(Colors.Aquamarine);
                line.X1 = x;
                line.X2 = x;
                line.Y1 = StartLine.Y1;
                line.Y2 = StartLine.Y2;
                MacdCanvas.Children.Add(line);
                chartDrawingElements.Add(line);
                i += dateTimeLabelIncrement;
                j++;
            }
            return dateTimeLabels.ToArray();
        }

        private int GetTransactionPerLine(int count)
        {
            int maxTransactionCount = XLengthPixel/TransactionLinePixel;
            return count/maxTransactionCount + 1;
        }

        private int GetDateTimeLabelIncrement(int count, int transactionPerLine)
        {
            if (transactionPerLine == 1)
            {
                int maxDateTimeCount = XLengthPixel/XLabelPixel;
                return count/maxDateTimeCount + 1;
            }
            return transactionPerLine * XLabelPixel/TransactionLinePixel;
        }

        private void AddPriceLabel(double startPrice, double priceIncrementPerLabel)
        {
            int i = 0;
            double price = startPrice - priceIncrementPerLabel;
            double y = 0;
            while (y < PriceLine.Y2)
            {
                TextBox textBox = new TextBox();
                textBox.Text = price.ToString("0.00");
                textBox.FontSize = 10;
                textBox.BorderThickness = new Thickness(0);
                Canvas.SetLeft(textBox, 0);
                Canvas.SetBottom(textBox, y + YLabelPixel);
                ChartCanvas.Children.Add(textBox);
                chartDrawingElements.Add(textBox);
                Line line = new Line();
                line.Stroke = new SolidColorBrush(Colors.Aquamarine);
                line.X1 = PriceLine.X1 - 3;
                line.X2 = DateLine.X2;
                line.Y1 = DateLine.Y1 - y;
                line.Y2 = DateLine.Y1 - y;
                ChartCanvas.Children.Add(line);
                chartDrawingElements.Add(line);
                price += priceIncrementPerLabel;
                y += YLabelPixel;
            }
        }

        private double GetPriceIncrementPerLabel(double priceDiff)
        {
            int maxPriceLabelCount = YLengthPixel/YLabelPixel;
            int priceDiffInCents = Convert.ToInt32(Math.Ceiling(priceDiff*100));
            double priceIncrementPerLabel = (priceDiffInCents/(maxPriceLabelCount -2) + 1)/100d;
            if (priceIncrementPerLabel < 0.02)
                return priceIncrementPerLabel;
            if (priceIncrementPerLabel <= 0.05)
                return 0.05;
            if (priceIncrementPerLabel <= 0.1)
                return 0.1;
            if (priceIncrementPerLabel <= 0.2)
                return 0.2;
            if (priceIncrementPerLabel <= 0.5)
                return 0.5;
            if (priceIncrementPerLabel <= 1)
                return 1;
            if (priceIncrementPerLabel <= 2)
                return 2;
            if (priceIncrementPerLabel <= 5)
                return 5;
            if (priceIncrementPerLabel <= 10)
                return 10;
            if (priceIncrementPerLabel <= 20)
                return 20;
            if (priceIncrementPerLabel <= 50)
                return 50;
            if (priceIncrementPerLabel <= 100)
                return 100;
            return Math.Pow(10, Math.Ceiling(Math.Log10(priceIncrementPerLabel)));
        }

        private double GetStartPriceLabel(double minPrice, double priceIncrementPerLabel)
        {
            if (priceIncrementPerLabel < 0.02)
                return minPrice;
            if (priceIncrementPerLabel <= 0.05)
                return Math.Floor(minPrice * 20)/20 ;
            if (priceIncrementPerLabel <= 0.1)
                return Math.Floor(minPrice * 10) / 10;
            if (priceIncrementPerLabel <= 0.2)
                return Math.Floor(minPrice * 5) / 5;
            if (priceIncrementPerLabel <= 0.5)
                return Math.Floor(minPrice * 2) / 2;
            if (priceIncrementPerLabel <= 1)
                return Math.Floor(minPrice);
            if (priceIncrementPerLabel <= 2)
                return Math.Floor(minPrice /2) * 2; 
            if (priceIncrementPerLabel <= 5)
                return Math.Floor(minPrice /5) *5;
            if (priceIncrementPerLabel <= 10)
                return Math.Floor(minPrice/10)*10;
            if (priceIncrementPerLabel <= 20)
                return Math.Floor(minPrice / 20) * 20;
            if (priceIncrementPerLabel <= 50)
                return Math.Floor(minPrice / 50) * 50;
            return Math.Pow(10, Math.Floor(Math.Log10(minPrice)));
        }

        private double GetYForPrice(double price)
        {
            var priceIncrementPerPixel = priceIncrementPerLabel/YLabelPixel;
            return DateLine.Y1 - (price - startPrice)/priceIncrementPerPixel - YLabelPixel;
        }

        private double GetYForMacd(double value)
        {
            var macdIncrementPerPixel = macdIncrementPerLabel/MacdLabelPixel;
            return ZeroLine.Y1 - value/macdIncrementPerPixel;
        }

        private double GetYForVolume(double value)
        {
            return ZeroLine.Y1 + 110 - value/volumePerPixel;
        }

        private double GetXForDateTime(DateTime timeStamp)
        {
            double previousX = 0;
            foreach (var pair in xPixelDictionary)
            {
                if (pair.Key == timeStamp)
                    return pair.Value;
                if (pair.Key > timeStamp)
                    return previousX;
                previousX = pair.Value;
            }
            return previousX;
        }

        private void Days200TrendingCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            if (StockComboBox.SelectedValue == null)
                return;

            if (StartDateComboBox.SelectedValue == null)
                return;
            var stockKey = stockContext.Stocks.First(s => s.Id == StockComboBox.SelectedValue).Key;
            var startDate = Convert.ToDateTime(StartDateComboBox.SelectedValue);
            var endDate = stockContext.TransactionData.Where(t => t.StockKey == stockKey).Max(t => t.TimeStamp);
            if (string.IsNullOrEmpty(EndDateComboBox.SelectedValue.ToString()) == false)
                endDate = Convert.ToDateTime(EndDateComboBox.SelectedValue);
            foreach (var channel in stockContext.Channels.Where(c => c.StockKey == stockKey && c.StartDate >= startDate && c.EndDate <= endDate && c.Length == 200))
            {
                DrawChannelSupportLine(channel, days200DrawingElements);
                DrawChannelResistanceLine(channel, days200DrawingElements);
            }
            
        }

        private void Days100TrendingCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            if (StockComboBox.SelectedValue == null)
                return;

            if (StartDateComboBox.SelectedValue == null)
                return;
            var stockKey = stockContext.Stocks.First(s => s.Id == StockComboBox.SelectedValue).Key;
            var startDate = Convert.ToDateTime(StartDateComboBox.SelectedValue);
            var endDate = stockContext.TransactionData.Where(t => t.StockKey == stockKey).Max(t => t.TimeStamp);
            if (string.IsNullOrEmpty(EndDateComboBox.SelectedValue.ToString()) == false)
                endDate = Convert.ToDateTime(EndDateComboBox.SelectedValue);
            foreach (var channel in stockContext.Channels.Where(c => c.StockKey == stockKey && c.StartDate >= startDate && c.EndDate <= endDate && c.Length == 100))
            {
                DrawChannelSupportLine(channel, days100DrawingElements);
                DrawChannelResistanceLine(channel, days100DrawingElements);
            }
            
        }

        private void DrawChannelSupportLine(Channel channel, IList<UIElement> drawingElements)
        {
            var x1 = GetXForDateTime(channel.StartDate);
            var y1 = GetYForPrice(channel.SupportStartPrice);
            var x2 = GetXForDateTime(channel.EndDate);
            var y2 = GetYForPrice(channel.SupportStartPrice + channel.Length*channel.SupportChannelRatio);
            Line line = new Line();
            if (channel.ChannelTrend == 0)
                line.Stroke = new SolidColorBrush(Colors.Blue);
            else if (channel.SupportChannelRatio > 0)
                line.Stroke = new SolidColorBrush(Colors.Green);
            else
                line.Stroke = new SolidColorBrush(Colors.Firebrick);
            line.X1 = x1;
            line.X2 = x2;
            line.Y1 = y1;
            line.Y2 = y2;
            ChartCanvas.Children.Add(line);
            drawingElements.Add(line);
        }

        private void DrawChannelResistanceLine(Channel channel, IList<UIElement> drawingElements)
        {
            var x1 = GetXForDateTime(channel.StartDate);
            var y1 = GetYForPrice(channel.ResistanceStartPrice);
            var x2 = GetXForDateTime(channel.EndDate);
            var y2 = GetYForPrice(channel.ResistanceStartPrice + channel.Length*channel.ResistanceChannelRatio);
            Line line = new Line();
            if (channel.ChannelTrend == 0)
                line.Stroke = new SolidColorBrush(Colors.Purple);
            else if (channel.ResistanceChannelRatio > 0)
                line.Stroke = new SolidColorBrush(Colors.Orange);
            else
                line.Stroke = new SolidColorBrush(Colors.Red);
            line.X1 = x1;
            line.X2 = x2;
            line.Y1 = y1;
            line.Y2 = y2;
            ChartCanvas.Children.Add(line);
            drawingElements.Add(line);
        }

        private void Days200TrendingCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            foreach (var drawingElement in days200DrawingElements)
            {
                ChartCanvas.Children.Remove(drawingElement);
            }
            days200DrawingElements.Clear();
        }

        private void Days100TrendingCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            foreach (var drawingElement in days100DrawingElements)
            {
                ChartCanvas.Children.Remove(drawingElement);
            }
            days100DrawingElements.Clear();
        }

        private void Days50TrendingCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            if (StockComboBox.SelectedValue == null)
                return;

            if (StartDateComboBox.SelectedValue == null)
                return;
            var stockKey = stockContext.Stocks.First(s => s.Id == StockComboBox.SelectedValue).Key;
            var startDate = Convert.ToDateTime(StartDateComboBox.SelectedValue);
            var endDate = stockContext.TransactionData.Where(t => t.StockKey == stockKey).Max(t => t.TimeStamp);
            if (string.IsNullOrEmpty(EndDateComboBox.SelectedValue.ToString()) == false)
                endDate = Convert.ToDateTime(EndDateComboBox.SelectedValue);
            foreach (var channel in stockContext.Channels.Where(c => c.StockKey == stockKey && c.StartDate >= startDate && c.EndDate <= endDate && c.Length == 50))
            {
                DrawChannelSupportLine(channel, days50DrawingElements);
                DrawChannelResistanceLine(channel, days50DrawingElements);
            }
        }

        private void Days50TrendingCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            foreach (var drawingElement in days50DrawingElements)
            {
                ChartCanvas.Children.Remove(drawingElement);
            }
            days50DrawingElements.Clear();
        }

        private void Days20TrendingCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            if (StockComboBox.SelectedValue == null)
                return;

            if (StartDateComboBox.SelectedValue == null)
                return;
            var stockKey = stockContext.Stocks.First(s => s.Id == StockComboBox.SelectedValue).Key;
            var startDate = Convert.ToDateTime(StartDateComboBox.SelectedValue);
            var endDate = stockContext.TransactionData.Where(t => t.StockKey == stockKey).Max(t => t.TimeStamp);
            if (string.IsNullOrEmpty(EndDateComboBox.SelectedValue.ToString()) == false)
                endDate = Convert.ToDateTime(EndDateComboBox.SelectedValue);
            foreach (var channel in stockContext.Channels.Where(c => c.StockKey == stockKey && c.StartDate >= startDate && c.EndDate <= endDate && c.Length == 20))
            {
                DrawChannelSupportLine(channel, days20DrawingElements);
                DrawChannelResistanceLine(channel, days20DrawingElements);
            }
        }

        private void Days20TrendingCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            foreach (var drawingElement in days20DrawingElements)
            {
                ChartCanvas.Children.Remove(drawingElement);
            }
            days20DrawingElements.Clear();
        }
    }
}
