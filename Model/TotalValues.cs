using LiveCharts;
using LiveCharts.Wpf;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace HomeBudget.Model
{
    public class TotalValues : INotifyPropertyChanged
    {
        public static ObservableCollection<TotalValues> Collection = new ObservableCollection<TotalValues>();

        private string name;    
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        private string displayName;
        public string DisplayName
        {
            get { return displayName; }
            set
            {
                displayName = value;
                RaisePropertyChanged("DisplayName");
            }
        }

        private decimal totalValue;
        public decimal TotalValue
        {
            get { return totalValue; }
            set
            {
                if (totalValue != value)
                {
                    totalValue = value;
                    RaisePropertyChanged("TotalValue");
                }
            }
        }

        public TotalValues(string name, decimal value, string displayName)
        {
            this.name = name;
            totalValue = value;
            this.displayName = displayName;
        }

        void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;

    }
}
