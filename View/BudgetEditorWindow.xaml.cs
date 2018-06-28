using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using HomeBudget.Model;
using HomeBudget.Model.Assets;
using LiteDB;
using LiveCharts;
using LiveCharts.Defaults;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace HomeBudget.View
{
    /// <summary>
    /// Interaction logic for BudgetEditorWindow.xaml
    /// </summary>
    public partial class BudgetEditorWindow : Window, IDisposable
    {
        PropertyGrid propGrid = new PropertyGrid();
        properties props = new properties();
        string openFileName;
        CustomClass myProperties = new CustomClass();
        PropertyGrid propGridCustom = new PropertyGrid();
        public SeriesCollection columnCollection { get; set; }
        public ChartValues<ObservableValue> MyValues { get; set; }

        public BudgetEditorWindow()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(ex.InnerException);
            }

            
            openFileName = createSaveFileName() + ".budget";
            propGrid.SelectedObject = props;
            propGrid.BackColor = System.Drawing.ColorTranslator.FromHtml("#5e35b1"); //Deep Purple 500
            propGrid.ExpandAllGridItems();
            propGrid.LineColor = System.Drawing.Color.DarkGray;
            propGrid.PropertySort = PropertySort.Categorized;
            wfHost.Child = propGrid;
            propGrid.PropertyValueChanged += PropGrid_PropertyValueChanged;
            propGridCustom.PropertyValueChanged += PropGrid_PropertyValueChanged;



            propGridCustom.SelectedObject = myProperties;
            wfHost2.Child = propGridCustom;

            foreach (System.Windows.Forms.Control c in propGridCustom.ActiveControl.Controls)
                c.MouseClick += new System.Windows.Forms.MouseEventHandler(propGridCustom_MouseClick);

            fillTotalValues(props);

            this.DataContext = TotalValues.Collection;

            this.Title = string.Format("{0} - Otthoni költségvetéskezelő", openFileName);

            tryToLoadLastSaved();

        }

        private void PropGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            updateLabels();
        }

        private void fillTotalValues(properties props)
        {
            TotalValues.Collection.Clear(); //don't want duplicate elements
            #region TotalValues members
            var tv1 = new TotalValues("Leisure", props.leisure.totalLeisure, "Szórakozás");
         //   var tv2 = new TotalValues("Income", props.totalIncome, "Bevétel");
            var tv3 = new TotalValues("Media", props.media.TotalMedia, "Média");
            var tv4 = new TotalValues("Housing", props.housing.TotalHousing, "Lakhatás");
            var tv5 = new TotalValues("PublicUtils", props.publicUtils.TotalPublicUtils, "Közművek");
            var tv6 = new TotalValues("Transportation", props.transportation.TotalTransportation, "Közlekedés");
            var tv7 = new TotalValues("Household", props.household.TotalHousehold, "Háztartás");
            var tv8 = new TotalValues("Foods", props.food.TotalFoods, "Élelmiszer");
            var tv9 = new TotalValues("Children", props.children.TotalChildren, "Gyermekek");
            var tv10 = new TotalValues("Savings", props.savings.TotalSavings, "Megtakarítások");
            var tv11 = new TotalValues("Insurances", props.insurances.TotalInsurances, "Biztosítások");
            var tv12 = new TotalValues("Others", props.others.TotalOthers, "Egyéb kiadások");
 
            propGrid.Refresh();
            propGridCustom.Refresh();
            var customCollection = new TotalValues("Custom", myProperties.TotalValue, "Saját kiadások");

            TotalValues.Collection.Add(tv1);
            TotalValues.Collection.Add(customCollection);   //tv2
            TotalValues.Collection.Add(tv3);
            TotalValues.Collection.Add(tv4);
            TotalValues.Collection.Add(tv5);
            TotalValues.Collection.Add(tv6);
            TotalValues.Collection.Add(tv7);
            TotalValues.Collection.Add(tv8);
            TotalValues.Collection.Add(tv9);
            TotalValues.Collection.Add(tv10);
            TotalValues.Collection.Add(tv11);
            TotalValues.Collection.Add(tv12);

            #endregion

            updateLabels();

        }

        public void saveToDb()
        {
            decimal income = props.totalIncome;
            decimal spending = getTotalSpending();
            DateTime timeStamp;
            if (DateTimerPicker.Value != null)
            {
                timeStamp = (DateTime)DateTimerPicker.Value;
            }
            else
            {
                timeStamp = DateTime.Now;
            }
            DataAggregator da = new DataAggregator(income, spending, timeStamp);
            var caList = new List<CustomPropAggregator>();
            
            foreach (CustomProperty prop in myProperties.GetAllProps())
            {
                CustomPropAggregator ca = new CustomPropAggregator(timeStamp, prop.Value, prop.Name);
                caList.Add(ca);
            }
            
            saveDatabase(da, caList);
            searchForOverLimit();
        }

        public void saveDatabase(DataAggregator da, List<CustomPropAggregator> caList)
        {
            using (var db = new LiteDatabase(@"AdatBazis.db"))
            {
                var col = db.GetCollection<DataAggregator>("data");
                var results = col.Find(Query.All());
                bool updated = false;
                foreach (var result in results)
                {
                    if((result.TimeStamp.Month == da.TimeStamp.Month) && (result.TimeStamp.Year == da.TimeStamp.Year))    //Minden honapban csak egy bejegyzes legyen.
                    {
                        result.Income = da.Income;
                        result.Spending = da.Spending;
                        result.TimeStamp = da.TimeStamp;
                        col.Update(result);
                        updated = true;
                    }
                }
                if(!updated) col.Insert(da);
                
                var list = db.GetCollection<CustomPropAggregator>("customList");
                //ha a név ugyanaz ÉS a hónap ugyanaz, le kell frissíteni az értéket.
                var customResults = list.Find(Query.All());
                bool shouldInsert;

                foreach (var member in caList)
                {
                    shouldInsert = true;
                    foreach(var result in customResults)
                    {
                        if(member.TimeStamp.Month == result.TimeStamp.Month)
                        {
                            if (member.Title == result.Title)
                            {
                                result.Spending = member.Spending; //update
                                list.Update(result);
                                shouldInsert = false;
                            }
                        }
                    }
                    if(shouldInsert) list.Insert(member);
                }
            }
        }
        
        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog
            {
                Filter = "Költségvetés fájlok (*.budget)|*.budget",
                FilterIndex = 0,
                Title = "Költségvetés elmentése",
                InitialDirectory = System.IO.Path.Combine((Environment.CurrentDirectory), "Saved Budgets"),
            //System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Saved Budgets"),
            AddExtension = true,
                FileName = openFileName
            };

            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                props.removeEventHandlers();
                trySave(sfd.FileName);
                props.addEventHandlers();
            }
        }

        private void trySave(string filename)
        {
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Create);
            formatter.Serialize(fs, props);
            formatter.Serialize(fs, myProperties);
            fs.Close();
            openFileName = System.IO.Path.GetFileName(filename);
        }

        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog
            {
                Filter = "Költségvetés fájlok (*.budget)|*.budget",
                FilterIndex = 0,
                Title = "Válassz költségvetést",
                InitialDirectory = System.IO.Path.Combine((Environment.CurrentDirectory), "Saved Budgets")
            //System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Saved Budgets")
        };

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                openBudget(ofd.FileName);
            }
        }

        private void openBudget(string fileName)
        {
            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open);
            props = (properties)formatter.Deserialize(fs);
            myProperties = (CustomClass)formatter.Deserialize(fs);
            fs.Close();
            openFileName = System.IO.Path.GetFileName(fileName);
            propGrid.SelectedObject = props;
            propGridCustom.SelectedObject = myProperties;

            props.removeEventHandlers();
            props.addEventHandlers();

            fillTotalValues(props);
            this.Title = string.Format("{0} - Otthoni költségvetéskezelő", openFileName);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            //string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Saved Budgets");
            string path = System.IO.Path.Combine((Environment.CurrentDirectory), "Saved Budgets");
            string fullPath = System.IO.Path.Combine(path, openFileName);
            trySave(fullPath);
            saveToDb();
            StartWindow sw = new StartWindow();
            sw.Show();
            this.Close();
        }

        private string createSaveFileName()
        {
            String Month, Year, fullName;
            DateTime timeStamp;
            if (DateTimerPicker.Value == null)
            {
                timeStamp = DateTime.Now;
            }
            else
            {
                timeStamp = (DateTime) DateTimerPicker.Value;
            }
            Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(timeStamp.Month);
            Year = timeStamp.Year.ToString();
            fullName = Year + " " + Month;
            return fullName;
        }

        private void tryToLoadLastSaved()
        {
            //string dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Saved Budgets");
            string dir = System.IO.Path.Combine((Environment.CurrentDirectory), "Saved Budgets");
            string filename = createSaveFileName() + ".budget";
            string fullName = String.Concat(dir + "\\", filename);
            if (File.Exists(fullName))
            {
                openBudget(fullName);
                saveToDb();
            }
            propGrid.ExpandAllGridItems();
        }

            public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ((IDisposable)propGrid).Dispose();
                ((IDisposable)wfHost).Dispose();
            }
        }

        private decimal getTotalSpending()
        {
            return props.sumProps() + myProperties.TotalValue - props.totalIncome;
        }

        private void updateLabels()
        {
            decimal remaining = props.totalIncome - getTotalSpending();
            sumIncomeLabel.Content = props.totalIncome.ToString() + " Ft";
            sumSpendingLabel.Content = getTotalSpending().ToString() + " Ft";
            sumRemainingLabel.Content = (remaining).ToString() + " Ft";
            if(remaining < 0)
            {
                sumRemainingLabel.Foreground = new SolidColorBrush(Colors.DarkRed);
            }
            else
            {
                sumRemainingLabel.Foreground = new SolidColorBrush(Colors.ForestGreen);
            }
        }

        public void searchForOverLimit()
        {
            List<PropListItem> overLimitItems = new List<PropListItem>();
            List<CustomPropAggregator> spikeItems = new List<CustomPropAggregator>();
            using (var db = new LiteDatabase(@"AdatBazis.db"))
            {

                var fullCol = PriorityWindow.getAllPropsList(false);
                foreach(var item in fullCol)
                {
                    if (item.Limit > 0 && item.Name=="TV" && props.media.Television > item.Limit) overLimitItems.Add(item);
                    if (item.Limit > 0 && item.Name == "Ruházkodás" && props.household.Clothing > item.Limit) overLimitItems.Add(item);
                    if (item.Limit > 0 && item.Name == "Hitelkártya" && props.others.CreditCard > item.Limit) overLimitItems.Add(item);
                    if (item.Limit > 0 && item.Name == "Étterem" && props.leisure.eatingOut > item.Limit) overLimitItems.Add(item);
                    if (item.Limit > 0 && item.Name == "Káros szenvedélyek" && props.leisure.cigarettesAndAlcohol > item.Limit) overLimitItems.Add(item);
                }

                var col_custom = db.GetCollection<CustomPropAggregator>("customList");
                var listCustom = db.GetCollection<PropListItem>("propListCustom");
                var listCol = listCustom.Find(Query.All());
                var customCol = col_custom.Find(Query.All("Title"));
                foreach(var item in listCol)
                {
                    foreach(var custom in customCol)
                    if(item.Name == custom.Title)
                        {
                            if(item.Limit > 0 && item.Limit < custom.Spending)
                            {
                                if(!overLimitItems.Contains(item)) overLimitItems.Add(item);
                            }
                        }
                }


                var col_warnings = db.GetCollection<PropListItem>("warnings");
                col_warnings.Delete(Query.All());
                foreach(var item in overLimitItems)
                {
                    col_warnings.Insert(item);
                }

                foreach(var custom in customCol)
                {
                    foreach (var other in customCol)
                    {
                        if (other.Title == custom.Title && (other.TimeStamp.Month == custom.TimeStamp.Month+1) 
                            && (other.Spending > (custom.Spending * new decimal(1.5))))
                        {
                            spikeItems.Add(other);
                        }
                    }
                }

                var col_spikes = db.GetCollection<CustomPropAggregator>("spikes");
                col_spikes.Delete(Query.All());
                foreach (var item in spikeItems)
                {
                    col_spikes.Insert(item);
                }

            }
            
        }


        ~BudgetEditorWindow()
        {
            Dispose(false);
        }

        private void addCustomPropButton_Click(object sender, RoutedEventArgs e)
        {
            if(customPropNameTextBox.Text == "")
            {
                customPropNameTextBox.Text = "Névtelen";
            }
            CustomProperty myProp = new CustomProperty(customPropNameTextBox.Text, 0, typeof(decimal), myProperties);
            myProperties.Add(myProp);
            propGridCustom.Refresh();
        }

        private void delCustomPropButton_Click(object sender, RoutedEventArgs e)
        {
            myProperties.Remove(customPropNameTextBox.Text);
            myProperties.TotalValue += 0;

            using (var db = new LiteDatabase(@"AdatBazis.db"))
            {
                var list = db.GetCollection<CustomPropAggregator>("customList");
                var results = list.Delete(Query.EQ("Title", customPropNameTextBox.Text));
            }
            propGridCustom.Refresh();

            PriorityWindow pw = new PriorityWindow();
            pw.tryDelete(customPropNameTextBox.Text);
        }

        private void ExportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog
            {
                Filter = "Excel táblázat (*.xlsx)|*.xlsx",
                FilterIndex = 0,
                Title = "Exportálás Excel táblázatba",
                InitialDirectory = System.IO.Path.Combine((Environment.CurrentDirectory), "Saved Budgets"),
            //System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Saved Budgets"),
                FileName = "Költségvetés " + createSaveFileName()
            };

            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                exportToExcel(sfd.FileName);
            }
        }

        private void exportToExcel(string fileName)
        {
            //Exportálás előtt mentsük az esetleges változásokat
            // string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Saved Budgets");
            string path = System.IO.Path.Combine((Environment.CurrentDirectory), "Saved Budgets");
            string fullPath = System.IO.Path.Combine(path, openFileName);
            trySave(fullPath);
            saveToDb();

            using (var package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Költségvetés " + createSaveFileName());
                worksheet.Cells[1, 1].Value = "Név";
                worksheet.Cells[1, 2].Value = "Érték";


                worksheet.Cells[2, 1].Value = "Jövedelem";
                worksheet.Cells[2, 2].Value = props.net;
                worksheet.Cells[3, 1].Value = "Nyugdíj";
                worksheet.Cells[3, 2].Value = props.pension;
                worksheet.Cells[4, 1].Value = "Családi pótlék, segélyek";
                worksheet.Cells[4, 2].Value = props.aids;
                worksheet.Cells[5, 1].Value = "Megtakarítások, befektetések";
                worksheet.Cells[5, 2].Value = props.otherIncome1;
                worksheet.Cells[6, 1].Value = "Egyéb jövedelem";
                worksheet.Cells[6, 2].Value = props.otherIncome2;

                int index = 7;

                foreach(var tv in TotalValues.Collection)
                {
                    worksheet.Cells[index, 1].Value = tv.DisplayName;
                    worksheet.Cells[index, 2].Value = tv.TotalValue;
                    index++;
                }

                worksheet.Cells[index, 1].Value = "Saját kiadások összesen";
                worksheet.Cells[index++, 2].Value = myProperties.TotalValue;
                

                using (var db = new LiteDatabase(@"AdatBazis.db"))
                {
                    var col = db.GetCollection<CustomPropAggregator>("customList");
                    var results = col.Find(Query.All());

                    foreach (var result in results)
                    {
                        worksheet.Cells[index, 1].Value = result.Title;
                        worksheet.Cells[index, 2].Value = result.Spending;
                        index++;
                    }
                }


                worksheet.Cells["D2"].Value = "Bevétel összesen: ";
                worksheet.Cells["D3"].Value = "Kiadás összesen: ";
                worksheet.Cells["D4"].Value = "Summázva: ";

                worksheet.Cells["E2"].Formula = "sum(B2:B6)";
                worksheet.Cells["E3"].Formula = "sum(B7:B500)";
                worksheet.Cells["E4"].Formula = "E2 - E3";

                using (var range = worksheet.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                worksheet.Calculate();

                worksheet.Cells.AutoFitColumns(0);

                package.Workbook.Properties.Title = "Költségvetés ";
                var fileInf = new FileInfo(fileName);
                package.SaveAs(fileInf);
            }
        }

        private void QuitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //  string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Saved Budgets");
            string path = System.IO.Path.Combine((Environment.CurrentDirectory), "Saved Budgets");
            string fullPath = System.IO.Path.Combine(path, openFileName);
            trySave(fullPath);
            saveToDb();

            MessageBoxResult result = System.Windows.MessageBox.Show("Biztosan kilép?" + '\n' + "A felvitt adatok automatikusan mentésre kerültek.", "Kilépés megerősítése", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void propGridCustom_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            string name = propGridCustom.SelectedGridItem.Label;
            customPropNameTextBox.Text = name;
        }
    }
}
