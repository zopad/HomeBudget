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
    public class Insurances : INotifyPropertyChanged
    {

        private decimal life;
        [DisplayName("Életbiztosítás"), RefreshProperties(RefreshProperties.All), Description("")]
        public decimal Life
        {
            get { return life; }
            set
            {
                life = value;
                TotalInsurances = Life + House + Car;
            }
        }

        private decimal house;
        [DisplayName("Lakásbiztosítás"), RefreshProperties(RefreshProperties.All), Description("")]
        public decimal House
        {
            get { return house; }
            set
            {
                house = value;
                TotalInsurances = Life + House + Car;
            }
        }

        private decimal car;
        [DisplayName("Járműbiztosítás"), RefreshProperties(RefreshProperties.All), Description("Pl Casco")]
        public decimal Car
        {
            get { return car; }
            set
            {
                car = value;
                TotalInsurances = Life + House + Car;
            }
        }

        private decimal totalInsurances;
        [ReadOnlyAttribute(true), Browsable(false), DisplayName("Biztosítások összesen")]
        public decimal TotalInsurances
        {
            get { return totalInsurances; }
            set
            {
                totalInsurances = value;
                TotalValues.Collection.Single(x => x.Name == "Insurances").TotalValue = value;   //care for Capitalized setters
            }
        }

        public override string ToString()
        {
            return totalInsurances.ToString("c2");
        }

        void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;


    }
}
