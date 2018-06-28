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
    public class Housing : INotifyPropertyChanged
    {

        private decimal rent;
        [DisplayName("Lakbér"), RefreshProperties(RefreshProperties.All), Description("")]
        public decimal Rent
        {
            get { return rent; }
            set
            {
                rent = value;
                TotalHousing = Rent + CommonCost + HomeLoan;
            }
        }

        private decimal commonCost;
        [DisplayName("Közös költség"), RefreshProperties(RefreshProperties.All), Description("")]
        public decimal CommonCost
        {
            get { return commonCost; }
            set
            {
                commonCost = value;
                TotalHousing = Rent + CommonCost + HomeLoan;
            }
        }

        private decimal homeLoan;
        [DisplayName("Lakáshitel"), RefreshProperties(RefreshProperties.All)]
        public decimal HomeLoan
        {
            get { return homeLoan; }
            set
            {
                homeLoan = value;
                TotalHousing = Rent + CommonCost + HomeLoan;
            }
        }

        private decimal totalHousing;
        [ReadOnlyAttribute(true), Browsable(false), DisplayName("Lakhatás összesen")]
        public decimal TotalHousing
        {
            get { return totalHousing; }
            set
            {
                totalHousing = value;
                TotalValues.Collection.Single(x => x.Name == "Housing").TotalValue = value;   //care for Capitalized setters
            }
        }

        public override string ToString()
        {
            return totalHousing.ToString("c2");
        }

        void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;


    }
}