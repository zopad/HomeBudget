using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Model.Assets
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [System.Serializable()]
    public class Savings : INotifyPropertyChanged
    {

        private decimal privatePension;
        [DisplayName("Magánnyugdíj"), RefreshProperties(RefreshProperties.All), Description("")]
        public decimal PrivatePension
        {
            get { return privatePension; }
            set
            {
                privatePension = value;
                TotalSavings = PrivatePension + Investment;
            }
        }

        private decimal investment;
        [DisplayName("Megtakarítások, befektetések"), RefreshProperties(RefreshProperties.All), Description("")]
        public decimal Investment
        {
            get { return investment; }
            set
            {
                investment = value;
                TotalSavings = PrivatePension + Investment;
            }
        }

        private decimal totalSavings;
        [ReadOnlyAttribute(true), Browsable(false), DisplayName("Megtakarítások összesen")]
        public decimal TotalSavings
        {
            get { return totalSavings; }
            set
            {
                totalSavings = value;
                TotalValues.Collection.Single(x => x.Name == "Savings").TotalValue = value;   //care for Capitalized setters
            }
        }

        public override string ToString()
        {
            return totalSavings.ToString("c2");
        }

        void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;


    }
}

