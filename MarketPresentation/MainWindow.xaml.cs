using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

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
        private const int XLengPixel = 500;
        private const int YLengPixel = 500;
        private const int XLabelPixel = 50;
        private const int YLabelPixel = 50;
        private const int TransactionLinePixel = 10;

        private IList<UIElement> drawingElements = new List<UIElement>();
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
            foreach (var transactionData in stockContext.TransactionDatas.Where(t => t.StockKey == stockKey).OrderBy(t => t.TimeStamp))
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
            foreach (var transactionData in stockContext.TransactionDatas.Where(t => t.StockKey == stockKey).OrderBy(t => t.TimeStamp))
            {
                if (transactionData.TimeStamp > startDate)
                    EndDateComboBox.Items.Add(transactionData.TimeStamp);
            }
        }

        private void SearchButton_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var drawingElement in drawingElements)
            {
                ChartCanvas.Children.Remove(drawingElement);
            }
            drawingElements.Clear();
            if (StockComboBox.SelectedValue == null)
                return;

            if (StartDateComboBox.SelectedValue == null)
                return;
            var stockKey = stockContext.Stocks.First(s => s.Id == StockComboBox.SelectedValue).Key;
            var startDate = Convert.ToDateTime(StartDateComboBox.SelectedValue);
            var endDate = stockContext.TransactionDatas.Where(t => t.StockKey == stockKey).Max(t => t.TimeStamp);
            if (string.IsNullOrEmpty(EndDateComboBox.SelectedValue.ToString()) == false)
                endDate = Convert.ToDateTime(EndDateComboBox.SelectedValue);
            var orderedTransactionList =
                stockContext.TransactionDatas.Where(
                    t => t.StockKey == stockKey && t.TimeStamp >= startDate && t.TimeStamp <= endDate)
                    .OrderBy(t => t.TimeStamp).ToList();
            int count = orderedTransactionList.Count();
            var transactionsPerLine = GetTransactionPerLine(count);
            int dateTimeLabelIncrement = GetDateTimeLabelIncrement(count, transactionsPerLine);
            var dateTimeLabels = AddDateTimeLabel(orderedTransactionList, dateTimeLabelIncrement);
            var maxPrice = orderedTransactionList.Max(t => t.High);
            var minPrice = orderedTransactionList.Min(t => t.Low);
            TotalTransactionLabel.Content = count;
            LowLabel.Content = minPrice;
            HighLabel.Content = maxPrice;
            var diff = maxPrice - minPrice;
            var priceIncriment = GetPriceIncrementPerLabel(diff);
            var startPrice = GetStartPriceLabel(minPrice, priceIncriment);
            AddPriceLabel(startPrice, priceIncriment);
            var transactionIntervalPixel = TransactionLinePixel;
            if (transactionsPerLine == 1)
            {
                transactionIntervalPixel = XLabelPixel / dateTimeLabelIncrement;
            }
            int i = 0;
            int j = 0;
            int k = 0;
            while (i < orderedTransactionList.Count)
            {
                var x = (j + 1) * XLabelPixel + k * transactionIntervalPixel + X0Pixel;
                var open = orderedTransactionList[i].Open;
                var close = orderedTransactionList[i + transactionsPerLine - 1].Close;
                var high = orderedTransactionList[i].High;
                var low = orderedTransactionList[i].Low;
                for (int n = 0; n < transactionsPerLine; n++)
                {
                    if (orderedTransactionList[i + n].High > high)
                        high = orderedTransactionList[i + n].High;
                    if (orderedTransactionList[i + n].Low < low)
                        low = orderedTransactionList[i + n].Low;
                }
                Line line = new Line();
                line.Stroke = new SolidColorBrush(Colors.Black);
                line.X1 = x;
                line.X2 = x;
                line.Y1 = GetYForPrice(startPrice, priceIncriment, high);
                line.Y2 = GetYForPrice(startPrice, priceIncriment, low);
                ChartCanvas.Children.Add(line);
                drawingElements.Add(line);
                line = new Line();
                line.Stroke = new SolidColorBrush(Colors.Black);
                line.X1 = x - 3;
                line.X2 = x;
                line.Y1 = GetYForPrice(startPrice, priceIncriment, open);
                line.Y2 = GetYForPrice(startPrice, priceIncriment, open);
                ChartCanvas.Children.Add(line);
                drawingElements.Add(line);
                line = new Line();
                line.Stroke = new SolidColorBrush(Colors.Black);
                line.X1 = x;
                line.X2 = x + 3;
                line.Y1 = GetYForPrice(startPrice, priceIncriment, close);
                line.Y2 = GetYForPrice(startPrice, priceIncriment, close);
                ChartCanvas.Children.Add(line);
                drawingElements.Add(line);
                i += transactionsPerLine;
                k++;
                if (k == 5)
                {
                    k = 0;
                    j++;
                }
            }
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
                textBox.Text = orderedTransactionList[i].TimeStamp.ToString("MM-dd");
                textBox.FontSize = 10;
                textBox.BorderThickness = new Thickness(0);
                var x = (j + 1)*XLabelPixel + X0Pixel;
                Canvas.SetLeft(textBox, x - 10);
                Canvas.SetBottom(textBox, 0);
                ChartCanvas.Children.Add(textBox);
                drawingElements.Add(textBox);
                Line line = new Line();
                line.Stroke = new SolidColorBrush(Colors.Black);
                line.X1 = x;
                line.X2 = x;
                line.Y1 = DateLine.Y1 - 3;
                line.Y2 = DateLine.Y1 + 3;
                ChartCanvas.Children.Add(line);
                drawingElements.Add(line);
                i += dateTimeLabelIncrement;
                j++;
            }
            return dateTimeLabels.ToArray();
        }

        private int GetTransactionPerLine(int count)
        {
            int maxTransactionCount = XLengPixel/TransactionLinePixel;
            return count/maxTransactionCount + 1;
        }

        private int GetDateTimeLabelIncrement(int count, int transactionPerLine)
        {
            if (transactionPerLine == 1)
            {
                int maxDateTimeCount = XLengPixel/XLabelPixel;
                return count/maxDateTimeCount + 1;
            }
            return transactionPerLine*5;
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
                Canvas.SetBottom(textBox, y);
                ChartCanvas.Children.Add(textBox);
                drawingElements.Add(textBox);
                Line line = new Line();
                line.Stroke = new SolidColorBrush(Colors.Aquamarine);
                line.X1 = PriceLine.X1 - 3;
                line.X2 = DateLine.X2;
                line.Y1 = DateLine.Y1 - y;
                line.Y2 = DateLine.Y1 - y;
                ChartCanvas.Children.Add(line);
                drawingElements.Add(line);
                price += priceIncrementPerLabel;
                y += YLabelPixel;
            }
        }

        private double GetPriceIncrementPerLabel(double priceDiff)
        {
            int maxPriceLabelCount = YLengPixel/YLabelPixel;
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

        private double GetYForPrice(double startPrice, double priceIncrementPerLabel, double price)
        {
            var priceIncrementPerPixel = priceIncrementPerLabel/YLabelPixel;
            return DateLine.Y1 - (price - startPrice)/priceIncrementPerPixel - YLabelPixel;
        }
    }
}
