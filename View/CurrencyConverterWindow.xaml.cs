using HomeBudget.Model;
using HomeBudget.ViewModel;
using HomeBudget.ViewModel.CurrencyConverter;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace HomeBudget.View
{
    /// <summary>
    /// Interaction logic for CurrencyConverterWindow.xaml
    /// </summary>
    public partial class CurrencyConverterWindow : Window
    {
        static List<Currency> currencies;
        private Boolean formFullyLoaded = false;

        public CurrencyConverterWindow()
        {
            InitializeComponent();
            currencies = XmlWebApiReader.ReadXml();
            StartBackgroundUpdater(5);      //update currencies from the API every n minutes

            #region controls setup
            updateTimerComboBox.Items.Add("5 percenként");
            updateTimerComboBox.Items.Add("Félóránként");
            updateTimerComboBox.Items.Add("Óránként");
            updateTimerComboBox.SelectedIndex = 0;

            List<string> currList = new List<string>
            { "EUR", "GBP", "AUD", "DKK", "JPY", "CAD", "NOK", "CHF", "SEK", "USD", "CZK", "PLN", "HRK", "RON", "TRY" };
            currList.Sort();
            ComboBoxFrom.ItemsSource = ComboBoxTo.ItemsSource = ComboBoxTopFrom.ItemsSource = ComboBoxAlert.ItemsSource = currList;
            ComboBoxFrom.SelectedValue = ComboBoxTopFrom.SelectedValue =  "EUR";
            ComboBoxTo.SelectedValue = "GBP";

            ComboBoxAlert.SelectedValue = "EUR";
            dudAlert.Value = 330;
            
            DataGridCurrency.ItemsSource = currencies;

            ComboBoxFrom.SelectionChanged += currChanged;
            ComboBoxFrom.DropDownClosed += currChanged;
            ComboBoxTo.SelectionChanged += currChanged;
            ComboBoxTo.DropDownClosed += currChanged;

            formFullyLoaded = true;
            dudFrom.Value =  (decimal)100.10;
            dudTopFrom.Value = (decimal)1.0;
            calcHufRate(ComboBoxTopFrom.Text, "sell");
            calcExchangeRate(ComboBoxTo.Text, ComboBoxFrom.Text);

            createChart();

            #endregion
        }

        private List<Currency> searchCurr(string currName)
        {
            List<Currency> newList = new List<Currency>();
            foreach (Currency curr in currencies)
            {
                if (curr.Name.Equals(currName))
                {
                    newList.Add(curr);
                }
            }
            return newList;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            StartWindow sw = new StartWindow();
            sw.Show();
            this.Close();
        }

        #region event handlers
        private void dudFrom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (formFullyLoaded)
            {
                decimal result = (decimal)dudFrom.Value * calcExchangeRate(ComboBoxTo.Text, ComboBoxFrom.Text);
                dudTo.Value = result;
            }
        }
        
        
        private void currChanged(object sender, EventArgs e)
        {
            if (formFullyLoaded)
            {
                decimal result = (decimal)dudFrom.Value * calcExchangeRate(ComboBoxTo.Text, ComboBoxFrom.Text);
                dudTo.Value = result;
            }
        }

        private void updateTopLeft(object sender, EventArgs e)
        {
            if (formFullyLoaded)
            {
                decimal result = (decimal)dudTopFrom.Value * calcHufRate(ComboBoxTopFrom.Text, "sell");
                dudTopTo.Value = result;
            }
        }

        private void updateTopRight(object sender, EventArgs e)
        {
            if (formFullyLoaded)
            {
                decimal result = (decimal)dudTopTo.Value / calcHufRate(ComboBoxTopFrom.Text, "buy");
                dudTopFrom.Value = result;
            }
        }
        #endregion

        private decimal calcExchangeRate(string to, string from)
        {
            decimal sellLeft, buyRight;

            var buyList = searchCurr(to);
            var sellList = searchCurr(from);

            if (sellList.Count > 0)
            {
                {
                    sellLeft = sellList.Max().Buy;
                    sellBankNameLabel.Content = String.Concat("1 ", ComboBoxFrom.Text, "  ", sellList.Max().BankName.ToUpper(), " árfolyamán: ", sellLeft.ToString(), " Ft.");
                    if ((bool)shouldAlert.IsChecked)
                    {
                        if(sellLeft > dudAlert.Value && from==ComboBoxAlert.SelectedValue.ToString())
                        {
                            FancyBalloon balloon = new FancyBalloon();
                            balloon.BalloonText = "Árfolyamértesítés";
                            balloon.balloonName.Text = from;
                            balloon.balloonValue.Text = dudAlert.Value.ToString();

                            MyNotifyIcon.ShowCustomBalloon(balloon, PopupAnimation.Slide, 4000);
                        }
                    }
                }
            }
            else
            {
                return 1;
                throw new Exception("No exchange found");
            }

            if (buyList.Count > 0)
            {
                { 
                    buyRight = buyList.Min().Sell;
                    buyBankNameLabel.Content = String.Concat("1 ", ComboBoxTo.Text, "  ", buyList.Min().BankName.ToUpper(), " árfolyamán: ", buyRight.ToString(), " Ft.");
                }
                return sellLeft / buyRight;
            }
            else
            {
                return 1;
                throw new Exception("No exchange found");
            }
        
        }

        public SeriesCollection SeriesCollection { get; set; }
        ChartValues<decimal> cvRate = new ChartValues<decimal>();
        List<string> my_labels = new List<string>();
        public string[] Labels { get; set; }

        private void createChart()
        {
            foreach(var elem in currencies)
            {
                if(elem.Name == "EUR" && elem.Date.StartsWith(DateTime.Now.Year.ToString()))
                {
                    cvRate.Add(elem.Buy);
                    if (!my_labels.Contains(elem.Date))
                    {
                        my_labels.Add(elem.Date + ", " + elem.BankName.ToUpper());
                    }
                }
            }

            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "EUR vétel",
                    Values = cvRate
                }
            };
            Labels = my_labels.ToArray();
            DataContext = this;
        }

        private decimal calcHufRate(string from, string mode)
        {
            var sellList = searchCurr(from);
            if(sellList.Count > 0)
            {
                if (mode == "buy")
                {
                    return sellList.Min().Sell;
                }
                else
                {
                    return sellList.Max().Buy;
                }
            }
            else
            {
                return 1;
                throw new Exception("No exchange found");
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            MyNotifyIcon.Dispose();

            base.OnClosing(e);
        }


        private void btnShowCustomBalloon_Click(object sender, RoutedEventArgs e)
        {
            FancyBalloon balloon = new FancyBalloon();
            balloon.BalloonText = "Árfolyamértesítés";
            
            MyNotifyIcon.ShowCustomBalloon(balloon, PopupAnimation.Slide, 4000);
        }

        private void btnCloseCustomBalloon_Click(object sender, RoutedEventArgs e)
        {
            MyNotifyIcon.CloseBalloon();
        }

        private void StartBackgroundUpdater(int waitMinutes)
        {
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromMinutes(waitMinutes);

            var timer = new System.Threading.Timer((e) =>
            {
                currencies = XmlWebApiReader.ReadXml();
            }, null, startTimeSpan, periodTimeSpan);
        }

        private void updateTimerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(updateTimerComboBox.Text == "5 percenként")
            {
                StartBackgroundUpdater(5);
            }
            if (updateTimerComboBox.Text == "Félóránként")
            {
                StartBackgroundUpdater(30);
            }
            if (updateTimerComboBox.Text == "Óránként")
            {
                StartBackgroundUpdater(60);
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            dudTopFrom.IsReadOnly = true;
            dudTopFrom.ValueChanged -= updateTopLeft;
            ComboBoxTopFrom.SelectionChanged -= updateTopLeft;
            ComboBoxTopFrom.DropDownClosed -= updateTopLeft;
            ComboBoxTopFrom.SelectionChanged += updateTopRight;
            ComboBoxTopFrom.DropDownClosed += updateTopRight;
            dudTopTo.IsReadOnly = false;
            dudTopTo.ValueChanged += updateTopRight;
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            dudTopFrom.IsReadOnly = false;
            dudTopFrom.ValueChanged += updateTopLeft;
            ComboBoxTopFrom.SelectionChanged += updateTopLeft;
            ComboBoxTopFrom.DropDownClosed += updateTopLeft;
            ComboBoxTopFrom.SelectionChanged -= updateTopRight;
            ComboBoxTopFrom.DropDownClosed -= updateTopRight;
            dudTopTo.IsReadOnly = true;
            dudTopTo.ValueChanged -= updateTopRight;
        }

        private void ComboBoxAlert_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (formFullyLoaded)
            {
                string from = ComboBoxAlert.SelectedValue.ToString() ?? "EUR";
                var sellList = searchCurr(from);
                decimal sellLeft = sellList.Max().Buy;
                if ((bool)shouldAlert.IsChecked)
                {
                    if (sellLeft > dudAlert.Value && from == ComboBoxAlert.SelectedValue.ToString())
                    {
                        FancyBalloon balloon = new FancyBalloon();
                        balloon.balloonName.Text = from;
                        balloon.balloonValue.Text = dudAlert.Value.ToString();
                        balloon.BalloonText = "Árfolyamértesítés";

                        MyNotifyIcon.ShowCustomBalloon(balloon, PopupAnimation.Slide, 4000);
                    }
                }
            }
        }
    }
}
