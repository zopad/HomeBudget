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
    public class PublicUtils : INotifyPropertyChanged
    {

        private decimal heating;
        [DisplayName("Gáz/Fűtés"), RefreshProperties(RefreshProperties.All), Description("")]
        public decimal Heating
        {
            get { return heating; }
            set
            {
                heating = value;
                TotalPublicUtils = Heating + Electricity + Water + CommunalTax;
            }
        }

        private decimal electricity;
        [DisplayName("Áram"), RefreshProperties(RefreshProperties.All), Description("")]
        public decimal Electricity
        {
            get { return electricity; }
            set
            {
                electricity = value;
                TotalPublicUtils = Heating + Electricity + Water + CommunalTax;
            }
        }

        private decimal water;
        [DisplayName("Víz"), RefreshProperties(RefreshProperties.All)]
        public decimal Water
        {
            get { return water; }
            set
            {
                water = value;
                TotalPublicUtils = Heating + Electricity + Water + CommunalTax;
            }
        }

        private decimal communalTax;
        [DisplayName("Kommunális adó"), RefreshProperties(RefreshProperties.All), Description("És egyéb közművek")]
        public decimal CommunalTax
        {
            get { return communalTax; }
            set
            {
                communalTax = value;
                TotalPublicUtils = Heating + Electricity + Water + CommunalTax;
            }
        }

        private decimal totalPublicUtils;
        [ReadOnlyAttribute(true), Browsable(false), DisplayName("Közművek összesen")]
        public decimal TotalPublicUtils
        {
            get { return totalPublicUtils; }
            set
            {
                totalPublicUtils = value;
                TotalValues.Collection.Single(x => x.Name == "PublicUtils").TotalValue = value;   //care for Capitalized setters
            }
        }

        public override string ToString()
        {
            return totalPublicUtils.ToString("c2");
        }

        void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;


    }
}

