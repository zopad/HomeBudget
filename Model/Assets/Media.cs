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
    public class Media : INotifyPropertyChanged
    {

        private decimal telephone;
        [DisplayName("Telefon"), RefreshProperties(RefreshProperties.All), Description("Mobil előfizetés is")]
        public decimal Telephone
        {
            get { return telephone; }
            set
            {
                telephone = value;
                TotalMedia = Telephone + Television + Internet;
            }
        }

        private decimal television;
        [DisplayName("TV"), RefreshProperties(RefreshProperties.All), Description("Előfizetések is")]
        public decimal Television
        {
            get { return television; }
            set
            {
                television = value;
                TotalMedia = Telephone + Television + Internet;
            }
        }

        private decimal internet;
        [DisplayName("Internet"), RefreshProperties(RefreshProperties.All)]
        public decimal Internet
        {
            get { return internet; }
            set
            {
                internet = value;
                TotalMedia = Telephone + Television + Internet;     //not totalMedia !
            }
        }

        private decimal totalMedia;
        [ReadOnlyAttribute(true), Browsable(false), DisplayName("Média összesen")]
        public decimal TotalMedia
        {
            get { return totalMedia; }
            set
            {
                totalMedia = value;
                TotalValues.Collection.Single(x => x.Name == "Media").TotalValue = value;   //care for Capitalized setters
            }
        }

        public override string ToString()
        {
            return totalMedia.ToString("c2");
        }

        void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;


    }
}
