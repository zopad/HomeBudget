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
    public class Transportation : INotifyPropertyChanged
    {

        private decimal fuel;
        [DisplayName("Üzemanyag"), RefreshProperties(RefreshProperties.All), Description("")]
        public decimal Fuel
        {
            get { return fuel; }
            set
            {
                fuel = value;
                TotalTransportation = Fuel + Maintenance + Pass;
            }
        }

        private decimal maintenance;
        [DisplayName("Jármű fenntartási költségek"), RefreshProperties(RefreshProperties.All), Description("")]
        public decimal Maintenance
        {
            get { return maintenance; }
            set
            {
                maintenance = value;
                TotalTransportation = Fuel + Maintenance + Pass;
            }
        }

        private decimal pass;
        [DisplayName("Tömegközlekedési bérlet/jegy"), RefreshProperties(RefreshProperties.All)]
        public decimal Pass
        {
            get { return pass; }
            set
            {
                pass = value;
                TotalTransportation = Fuel + Maintenance + Pass;
            }
        }

        private decimal totalTransportation;
        [ReadOnlyAttribute(true), Browsable(false), DisplayName("Közlekedés összesen")]
        public decimal TotalTransportation
        {
            get { return totalTransportation; }
            set
            {
                totalTransportation = value;
                TotalValues.Collection.Single(x => x.Name == "Transportation").TotalValue = value;   //care for Capitalized setters
            }
        }

        public override string ToString()
        {
            return totalTransportation.ToString("c2");
        }

        void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;


    }
}
