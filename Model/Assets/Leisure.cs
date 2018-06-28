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
    public class Leisure : INotifyPropertyChanged
    {

        private decimal _eatingOut;
        [DisplayName("Étterem"), RefreshProperties(RefreshProperties.All), Description("Gyorsételek is ide tartozhatnak")]
        public decimal eatingOut
        {
            get { return _eatingOut; }
            set
            {
                _eatingOut = value;
                totalLeisure = eatingOut + cinema + holidays + sports + cigarettesAndAlcohol;
            }
        }

        private decimal _cinema;
        [DisplayName("Mozi"), RefreshProperties(RefreshProperties.All), Description("Mozi")]
        public decimal cinema
        {
            get { return _cinema; }
            set
            {
                _cinema = value;
                totalLeisure = eatingOut + cinema + holidays + sports + cigarettesAndAlcohol;
            }
        }

        private decimal _holidays;
        [DisplayName("Nyaralás"), RefreshProperties(RefreshProperties.All)]
        public decimal holidays
        {
            get { return _holidays; }
            set
            {
                _holidays = value;
                totalLeisure = eatingOut + cinema + holidays + sports + cigarettesAndAlcohol;
            }
        }

        private decimal _sports;
        [DisplayName("Sport"), RefreshProperties(RefreshProperties.All)]
        public decimal sports
        {
            get { return _sports; }
            set
            {
                _sports = value;
                totalLeisure = eatingOut + cinema + holidays + sports + cigarettesAndAlcohol;
            }
        }

        private decimal _cigarettesAndAlcohol;
        [DisplayName("Káros szenvedélyek"), RefreshProperties(RefreshProperties.All), Description("Alkohol, cigaretta, stb")]
        public decimal cigarettesAndAlcohol
        {
            get { return _cigarettesAndAlcohol; }
            set
            {
                _cigarettesAndAlcohol = value;
                totalLeisure = eatingOut + cinema + holidays + sports + cigarettesAndAlcohol;
            }
        }

        private decimal _totalLeisure;
        [ReadOnlyAttribute(true), Browsable(false), DisplayName("Szórakozás összesen")]
        public decimal totalLeisure
        {
            get { return _totalLeisure; }
            set
            {
                _totalLeisure = value;
                TotalValues.Collection.Single(x => x.Name == "Leisure").TotalValue = value;
            }
        }

        public override string ToString()
        {
            return totalLeisure.ToString("c2");
        }

        void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;


    }
}
