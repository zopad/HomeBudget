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
    public class Foods : INotifyPropertyChanged
    {

        private decimal basic;
        [DisplayName("Alapvető"), RefreshProperties(RefreshProperties.All), Description("")]
        public decimal Basic
        {
            get { return basic; }
            set
            {
                basic = value;
                TotalFoods = Basic + Extra;
            }
        }

        private decimal extra;
        [DisplayName("Extra"), RefreshProperties(RefreshProperties.All), Description("Chips, édességek, stb.")]
        public decimal Extra
        {
            get { return extra; }
            set
            {
                extra = value;
                TotalFoods = Basic + Extra;
            }
        }

        private decimal totalFoods;
        [ReadOnlyAttribute(true), Browsable(false), DisplayName("Élelmiszer összesen")]
        public decimal TotalFoods
        {
            get { return totalFoods; }
            set
            {
                totalFoods = value;
                TotalValues.Collection.Single(x => x.Name == "Foods").TotalValue = value;   //care for Capitalized setters
            }
        }

        public override string ToString()
        {
            return totalFoods.ToString("c2");
        }

        void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;


    }
}

