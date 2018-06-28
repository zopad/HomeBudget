using HomeBudget.Model;
using HomeBudget.Model.Assets;
using LiteDB;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace HomeBudget.View
{
    /// <summary>
    /// Interaction logic for PriorityWindow.xaml
    /// </summary>
    public partial class PriorityWindow : Window
    {
        
        public PriorityWindow()
        {
            InitializeComponent();
            fillPropLists();
          //  propListMust.Clear(); propListImportant.Clear(); propListExtra.Clear(); propListNotUsed.Clear();
          //  fillDefaultValues();
            saveListsToDb();
            fillCustomList();
            fillWarningsMessages();

            DataContext = this;
        }

        #region fill listboxes with default values
        private void fillPropLists()
        {
            using (var db = new LiteDatabase(@"AdatBazis.db"))
            {
                var col = db.GetCollection<PropListItem>("propListMust");
                var results = col.Find(Query.All());

                foreach(var result in results)
                {
                    propListMust.Add(result);
                }

                var col_imp = db.GetCollection<PropListItem>("propListImportant");
                var results_imp = col_imp.Find(Query.All());

                foreach (var result in results_imp)
                {
                    propListImportant.Add(result);
                }

                var col_extra = db.GetCollection<PropListItem>("propListExtra");
                var results_extra = col_extra.Find(Query.All());

                foreach (var result in results_extra)
                {
                    propListExtra.Add(result);
                }

                var col_notUsed = db.GetCollection<PropListItem>("propListNotUsed");
                var results_notUsed = col_notUsed.Find(Query.All());

                foreach (var result in results_notUsed)
                {
                    propListNotUsed.Add(result);
                }

                var col_custom = db.GetCollection<PropListItem>("propListCustom");
                var results_custom = col_custom.Find(Query.All());

                foreach (var result in results_custom)
                {
                    propListCustom.Add(result);
                }

                if ((propListMust.Count + propListImportant.Count + propListExtra.Count + propListNotUsed.Count) == 0)
                {
                    fillDefaultValues();
                }
                
            }
        }

        public ObservableCollection<PropListItem> propListMust { get; set; } = new ObservableCollection<PropListItem>();
        public ObservableCollection<PropListItem> propListImportant { get; set; } = new ObservableCollection<PropListItem>();
        public ObservableCollection<PropListItem> propListExtra { get; set; } = new ObservableCollection<PropListItem>();
        public ObservableCollection<PropListItem> propListNotUsed { get; set; } = new ObservableCollection<PropListItem>();
        public ObservableCollection<PropListItem> propListCustom { get; set; } = new ObservableCollection<PropListItem>();

        public ObservableCollection<string> warningsList { get; set; } = new ObservableCollection<string>();
        
        private void fillDefaultValues()
        {
            propListMust.Add(new PropListItem("Gázszámla"));
            propListMust.Add(new PropListItem("Villanyszámla"));
            propListMust.Add(new PropListItem("Vízszámla"));
            propListMust.Add(new PropListItem("Lakbér / Lakáshitel"));
            propListMust.Add(new PropListItem("Alapvető élelmiszer"));
            propListMust.Add(new PropListItem("Telefon"));
            propListMust.Add(new PropListItem("Internet"));

            propListImportant.Add(new PropListItem("TV"));
            propListImportant.Add(new PropListItem("Ruházkodás"));
            propListImportant.Add(new PropListItem("Hitelkártya"));

            propListExtra.Add(new PropListItem("Étterem"));
            propListExtra.Add(new PropListItem("Káros szenvedélyek"));
            propListExtra.Add(new PropListItem("Kirándulás"));
            propListExtra.Add(new PropListItem("Gyorsételek"));
            propListExtra.Add(new PropListItem("Vésztartalék"));

            propListNotUsed.Add(new PropListItem("Üzemanyag"));
            propListNotUsed.Add(new PropListItem("Iskoláztatás"));
            propListNotUsed.Add(new PropListItem("Megtakarítások"));
            propListNotUsed.Add(new PropListItem("Életbiztosítás"));
            propListNotUsed.Add(new PropListItem("Lakásbiztosítás"));
        }

        #endregion

        public static IEnumerable<PropListItem> getAllPropsList(bool needImportant)
        {
            using (var db = new LiteDatabase(@"AdatBazis.db"))
            {
                var col_custom = db.GetCollection<PropListItem>("propListCustom");
                var results_custom = col_custom.Find(Query.All());

                var col = db.GetCollection<PropListItem>("propListMust");
                var results = col.Find(Query.All());

                var col_imp = db.GetCollection<PropListItem>("propListImportant");
                var results_imp = col_imp.Find(Query.All());

                var col_extra = db.GetCollection<PropListItem>("propListExtra");
                var results_extra = col_extra.Find(Query.All());

                var col_notUsed = db.GetCollection<PropListItem>("propListNotUsed");
                var results_notUsed = col_notUsed.Find(Query.All());

                var fullList = results_custom.Union(results_imp).Union(results_extra).Union(results_notUsed);
                if (needImportant)
                {
                    fullList.Union(results);
                }
                return fullList;
            }
        }

        private void fillCustomList()
        {
            using (var db = new LiteDatabase(@"AdatBazis.db"))
            {
                var col = db.GetCollection<CustomPropAggregator>("customList");
                var results = col.Find(Query.All());
                var titles = new List<string>();


                var results_custom = getAllPropsList(true);
                

                foreach (var result in results)
                {
                    titles.Add(result.Title);
                }

                foreach (var title in titles.Distinct())
                {
                    bool seen = false;
                    foreach (var result in results_custom)
                    {
                        if (result.Name == title)
                        {
                            seen = true;
                        }
                    }
                    if(!seen) propListCustom.Add(new PropListItem(title));

                }
            }
        }

        private void saveListsToDb()
        {
            using (var db = new LiteDatabase(@"AdatBazis.db"))
            {
                var col = db.GetCollection<PropListItem>("propListMust");
                col.Delete(Query.All());
                foreach (var prop in propListMust)
                {
                    col.Insert(prop);
                }

                var col_imp = db.GetCollection<PropListItem>("propListImportant");
                col_imp.Delete(Query.All());
                foreach (var prop in propListImportant)
                {
                    col_imp.Insert(prop);
                }

                var col_extra = db.GetCollection<PropListItem>("propListExtra");
                col_extra.Delete(Query.All());
                foreach (var prop in propListExtra)
                {
                    col_extra.Insert(prop);
                }

                var col_notUsed = db.GetCollection<PropListItem>("propListNotUsed");
                col_notUsed.Delete(Query.All());
                foreach (var prop in propListNotUsed)
                {
                    col_notUsed.Insert(prop);
                }

                var col_custom = db.GetCollection<PropListItem>("propListCustom");
                col_custom.Delete(Query.All());
                foreach (var prop in propListCustom)
                {
                    col_custom.Insert(prop);
                }
            }
        }

        private void fillWarningsMessages()
        {
            using (var db = new LiteDatabase(@"AdatBazis.db"))
            {
                var col_warnings = db.GetCollection<PropListItem>("warnings");
                var col = col_warnings.Find(Query.All());
                foreach(var item in col)
                {
                    warningsList.Add("Kerettúllépés: " + item.Name + " átlépte a " + item.Limit + " Ft keretet!");
                }

                var col_spikes = db.GetCollection<CustomPropAggregator>("spikes");
                var spikes = col_spikes.Find(Query.All());
                int toSkip = 0;
                string savingTip = "";
                foreach (var item in spikes)
                {
                    if(propListExtra.Skip(toSkip).First() != null){
                        savingTip = "Ajánlott spórolás: " + propListExtra.Skip(toSkip).First().Name + " kategóriából.";
                    }
                    else if(propListImportant.Skip(toSkip).First() != null)
                    {
                        savingTip = "Ajánlott spórolás: " + propListImportant.Skip(toSkip).First().Name + " kategóriából.";
                    }
                    
                    warningsList.Add("Kiugró érték: " + item.Title + " kategóriára " + item.Spending + " forintot költött!" + '\n' +
                        savingTip);
                    toSkip++;
                    if (toSkip >= propListExtra.Count) toSkip = 0;
                }
            }
        }

        public void tryDelete(string propName)
        {
            int index = 0;
            bool shouldDelete = false;

            using (var db = new LiteDatabase(@"AdatBazis.db"))
            {
                var col_custom = db.GetCollection<PropListItem>("propListCustom");
                foreach (var prop in propListCustom)
                {
                    if (prop.Name == propName)
                    {
                        col_custom.Delete(prop.Name);
                    }
                }
            }
            if (shouldDelete)
            {
                propListCustom.RemoveAt(index-1);
            }
        }
        

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            saveListsToDb();
            StartWindow sw = new StartWindow();
            sw.Show();
            this.Close();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (propListCustom.Count > 0)
            {
                this.WarningTextBlock.Text = "Helyezd el a saját kategóriáidat valamelyik prioritási oszlopban, hogy kaphass spórolási tippeket!";
                WarningTextBlock.Foreground = Brushes.Red;
            }
            else
            {
                this.WarningTextBlock.Text = "Az összes saját kategóriádat elhelyezted, nincs több tennivalód.";
                WarningTextBlock.Foreground = Brushes.Green;
            }
        }

        private void collButton_Click(object sender, RoutedEventArgs e)
        {
             Xceed.Wpf.Toolkit.CollectionControlDialog ccd = new Xceed.Wpf.Toolkit.CollectionControlDialog
             {
                 ItemsSource = propListMust.Union(propListCustom).Union(propListExtra).Union(propListImportant).Union(propListNotUsed),
                 Title = "Megjegyzések, limitek megadása"
             };
            
             ccd.ShowDialog();
            if(ccd.DialogResult == true)
            {
                saveListsToDb();

            }

        }
    }
}
