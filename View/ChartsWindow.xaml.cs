using HomeBudget.Model;
using HomeBudget.Model.Assets;
using LiteDB;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace HomeBudget.View
{
    /// <summary>
    /// Interaction logic for ChartsWindow.xaml
    /// </summary>
    public partial class ChartsWindow : Window
    {
        bool yearlyMode = false;
        int earliestYear = DateTime.Now.Year;
        List<string> my_labels = new List<string>();
        

        ChartValues<decimal> cvIncome = new ChartValues<decimal>();
        ChartValues<decimal> cvSpending = new ChartValues<decimal>();

        public ChartsWindow()
        {
            InitializeComponent();
            createLineChart();
            createStackedAreaChart();
            ZoomingMode = ZoomingOptions.Xy;
            DataContext = this;
        }

        private void createLineChart()
        {
            decimal sumIncome = 0, sumSpending = 0;
            

            using (var db = new LiteDatabase(@"AdatBazis.db"))
            {
                var col = db.GetCollection<DataAggregator>("data");

                var results = col.Find(Query.All("TimeStamp"));
                results = results.OrderBy(x => x.TimeStamp).ToList();

                earliestYear = results.First().TimeStamp.Year;
                var lastYear = earliestYear;

                foreach (var result in results)
                {
                    if (yearlyMode == false)
                    {
                        cvIncome.Add(result.Income);
                        cvSpending.Add(result.Spending);
                        
                        my_labels.Add(result.TimeStamp.Year.ToString() + " " + result.TimeStamp.ToString("MMMM", CultureInfo.CurrentCulture));
                    }
                    else
                    {
                        if(result.TimeStamp.Year != lastYear)
                        {
                            cvIncome.Add(sumIncome);
                            cvSpending.Add(sumSpending);
                            sumIncome = 0;
                            sumSpending = 0;
                            lastYear = result.TimeStamp.Year;
                        }
                        sumIncome += result.Income;
                        sumSpending += result.Spending;
                    }
                }
                if (sumIncome != 0) cvIncome.Add(sumIncome);
                if (sumSpending != 0) cvSpending.Add(sumSpending);
            }
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Bevétel",
                    Values = cvIncome,
                    DataLabels = true,
                    LabelPoint = point => Math.Truncate(point.Y/1000) + " ezer"
                },
                new LineSeries
                {
                    Title = "Kiadás",
                    Values = cvSpending,
                    DataLabels = true,
                    LabelPoint = point => Math.Truncate(point.Y/1000) + " ezer"
                },

            };

            if (yearlyMode == false)
            {
                Labels = my_labels.ToArray();
            }
            else
            {
                Labels = generateLabels(earliestYear).ToArray();
            }
        }

        private List<string> generateLabels(int yearFrom)
        {
            var ret = new List<string>();
            for(int i = yearFrom; i <= DateTime.Now.Year; i++)
            {
                ret.Add(i.ToString());
            }
            return ret;
        }

        private void createStackedAreaChart()
        {
            AreaCollection = new SeriesCollection();
            using (var db = new LiteDatabase(@"AdatBazis.db"))
            {
                var col = db.GetCollection<CustomPropAggregator>("customList");

                var results = col.Find(Query.All("TimeStamp"));
                results = results.OrderBy(x => x.TimeStamp).ToList();
                var titles = new List<string>();

                foreach (var result in results)
                {
                    titles.Add(result.Title);
                }
                titles.Sort();

                foreach (var title in titles.Distinct())
                {
                    var cv = new ChartValues<DateTimePoint>();
                    foreach (var result in results)
                    {
                        
                        if (result.Title == title)
                        {
                            cv.Add(new DateTimePoint(new DateTime(result.TimeStamp.Year, result.TimeStamp.Month, 1), (double)result.Spending));
                            
                        }
                    }

                    var sas = new StackedAreaSeries
                    {
                        Title = title,
                        Values = cv,
                        LineSmoothness = 0.2
                    };
                    AreaCollection.Add(sas);

                    System.Windows.Controls.Panel.SetZIndex(sas, 0);
                }
            };
            if (!yearlyMode)
            {
                XFormatter = val => new DateTime((long)val).ToString("MMMM");
            }
            else
            {
                XFormatter = val => new DateTime((long)val).ToString("YYYY");
            }
            DataContext = this;
        }

        public SeriesCollection SeriesCollection { get; set; }
        public SeriesCollection AreaCollection { get; set; }
        public string[] Labels { get; set; }
        public ZoomingOptions ZoomingMode { get; private set; }
        public Func<double, string> YFormatter { get; set; }
        public Func<double, string> XFormatter { get; set; }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            StartWindow sw = new StartWindow();
            sw.Show();
            this.Close();
        }

        private void modeButton_Click(object sender, RoutedEventArgs e)
        {
            Array.Clear(Labels, 0, Labels.Length);
            if (yearlyMode)
            {
                modeButton.Content = "Váltás éves nézetre";
                topChart.AxisX.Clear();
                topChart.AxisX.Add(new Axis
                {
                    Labels = my_labels
                });
                
            }
            else
            {
                modeButton.Content = "Váltás havi nézetre";
                topChart.AxisX.Clear();
                topChart.AxisX.Add(new Axis
                {
                    Labels = generateLabels(earliestYear).ToArray()
            });

            }
            yearlyMode = !yearlyMode;
            cvIncome.Clear();
            cvSpending.Clear();
            createLineChart();
            createStackedAreaChart();
        }
    }
}
