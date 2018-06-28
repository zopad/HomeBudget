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
    public class Household : INotifyPropertyChanged
    {

        private decimal clothing;
        [DisplayName("Ruházkodás"), RefreshProperties(RefreshProperties.All), Description("")]
        public decimal Clothing
        {
            get { return clothing; }
            set
            {
                clothing = value;
                TotalHousehold = Clothing + Cleaning;
            }
        }

        private decimal cleaning;
        [DisplayName("Háztartás és takarítás"), RefreshProperties(RefreshProperties.All), Description("")]
        public decimal Cleaning
        {
            get { return cleaning; }
            set
            {
                cleaning = value;
                TotalHousehold = Clothing + Cleaning;
            }
        }

        private decimal totalHousehold;
        [ReadOnlyAttribute(true), Browsable(false), DisplayName("Háztartás összesen")]
        public decimal TotalHousehold
        {
            get { return totalHousehold; }
            set
            {
                totalHousehold = value;
                TotalValues.Collection.Single(x => x.Name == "Household").TotalValue = value;   //care for Capitalized setters
            }
        }

        public override string ToString()
        {
            return totalHousehold.ToString("c2");
        }

        void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;


    }
}

