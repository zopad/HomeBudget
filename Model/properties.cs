using HomeBudget.Model.Assets;
using HomeBudget.ViewModel.TypeEditors;
using HomeBudget.ViewModel.Util;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace HomeBudget.Model
{
    [System.Serializable()]
    public class properties : INotifyPropertyChanged
    {
        public event propertiesChangedEventHandler propertiesChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public delegate void propertiesChangedEventHandler(PropertiesChangedEventArgs e);

        public properties()
        {
            removeEventHandlers();
            addEventHandlers();
        }

        public void addEventHandlers()
        {
            // _leisure.propertiesChanged += expandable_propertiesChanged;
        }

        public void removeEventHandlers()
        {
            //  _leisure.propertiesChanged -= expandable_propertiesChanged;
        }

        [Browsable(false)]
        public decimal sumProps()   //month parameter ?
        {
            decimal ret = 0;
            ret = Convert.ToDecimal(totalIncome) + leisure.totalLeisure + media.TotalMedia + housing.TotalHousing + publicUtils.TotalPublicUtils
                + transportation.TotalTransportation + household.TotalHousehold + food.TotalFoods + children.TotalChildren +
                savings.TotalSavings + insurances.TotalInsurances + others.TotalOthers;
            return ret;
        }

        private decimal _net = 100000;
        [CategoryAttribute("(Bevételek)"), RefreshProperties(RefreshProperties.All), DisplayName("Nettó havi fizetés")]
        public decimal net
        {
            get { return _net; }
            set
            {
                _net = value;
                if (_net < 0) { net = 0; }
                totalIncome = (net + pension + otherIncome1 + otherIncome2 + aids); //.ToString("c2");
                
            }
        }

        private decimal _pension;
        [CategoryAttribute("(Bevételek)"), RefreshProperties(RefreshProperties.All), DisplayName("Nyugdíj")]
        public decimal pension
        {
            get { return _pension; }
            set
            {
                _pension = value;
                if (_pension < 0) { pension = 0; }
                totalIncome = (net + pension + otherIncome1 + otherIncome2 + aids);
            }
        }

        private decimal _aids;
        [CategoryAttribute("(Bevételek)"), RefreshProperties(RefreshProperties.All), DisplayName("Családi pótlék, segélyek")]
        public decimal aids
        {
            get { return _aids; }
            set
            {
                _aids = value;
                if (_aids < 0) { aids = 0; }
                totalIncome = (net + pension + otherIncome1 + otherIncome2 + aids);
            }
        }

        private decimal _otherIncome1;
        [CategoryAttribute("(Bevételek)"), RefreshProperties(RefreshProperties.All), DisplayName("Megtakarítások, befektetések")]
        public decimal otherIncome1
        {
            get { return _otherIncome1; }
            set
            {
                _otherIncome1 = value;
                if (_otherIncome1 < 0) { otherIncome1 = 0; }
                totalIncome = (net + pension + otherIncome1 + otherIncome2 + aids);
            }
        }

        private decimal _otherIncome2;
        [CategoryAttribute("(Bevételek)"), RefreshProperties(RefreshProperties.All), DisplayName("Egyéb jövedelem")]
        public decimal otherIncome2
        {
            get { return _otherIncome2; }
            set
            {
                _otherIncome2 = value;
                if (_otherIncome2 < 0) { otherIncome2 = 0; }
                totalIncome = (net + pension + otherIncome1 + otherIncome2 + aids);
            }
        }

        // private string _totalIncome = "0,00 Ft";
        private decimal _totalIncome = 0;
        [TypeConverter(typeof(CurrencyConverter))]
        [CategoryAttribute("(Bevételek)"), RefreshProperties(RefreshProperties.All), ReadOnlyAttribute(true), DisplayName("Teljes Jövedelem"), Editor(typeof(TypeEditor1), typeof(UITypeEditor)),]
        public decimal totalIncome
        {
            get { return _totalIncome; }
            set
            {
                _totalIncome = value;
              //  decimal decValue = default(decimal);
              //  decimal.TryParse(value, NumberStyles.Currency, CultureInfo.CurrentCulture, out decValue);

               // TotalValues.Collection.Single(x => x.Name == "Income").TotalValue = _totalIncome;

            }
        }

        #region Assets constructors

        private Children _children = new Children();
        [CategoryAttribute("(Kiadások)"), DisplayName("Gyermekek"), RefreshProperties(RefreshProperties.All), Editor(typeof(TypeEditor8), typeof(UITypeEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Children children
        {
            get { return _children; }
            set
            {
                _children = value;
            }
        }


        private Leisure _leisure = new Leisure();
        [CategoryAttribute("(Kiadások)"), DisplayName("Szórakozás"), RefreshProperties(RefreshProperties.All), Editor(typeof(TypeEditor0), typeof(UITypeEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Leisure leisure
        {
            get { return _leisure; }
            set
            {
                _leisure = value;
            }
        }

        private Media _media = new Media();
        [CategoryAttribute("(Kiadások)"), DisplayName("Média"), RefreshProperties(RefreshProperties.All), Editor(typeof(TypeEditor2), typeof(UITypeEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Media media
        {
            get { return _media; }
            set
            {
                _media = value;
            }
        }

        private Housing _housing = new Housing();
        [CategoryAttribute("(Kiadások)"), DisplayName("Lakhatás"), RefreshProperties(RefreshProperties.All), Editor(typeof(TypeEditor3), typeof(UITypeEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Housing housing
        {
            get { return _housing; }
            set
            {
                _housing = value;
            }
        }

        private PublicUtils _publicUtils = new PublicUtils();
        [CategoryAttribute("(Kiadások)"), DisplayName("Közművek"), RefreshProperties(RefreshProperties.All), Editor(typeof(TypeEditor4), typeof(UITypeEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public PublicUtils publicUtils
        {
            get { return _publicUtils; }
            set
            {
                _publicUtils = value;
            }
        }

        private Transportation _transportation = new Transportation();
        [CategoryAttribute("(Kiadások)"), DisplayName("Közlekedés"), RefreshProperties(RefreshProperties.All), Editor(typeof(TypeEditor5), typeof(UITypeEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Transportation transportation
        {
            get { return _transportation; }
            set
            {
                _transportation = value;
            }
        }

        private Household _household = new Household();
        [CategoryAttribute("(Kiadások)"), DisplayName("Háztartás"), RefreshProperties(RefreshProperties.All), Editor(typeof(TypeEditor6), typeof(UITypeEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Household household
        {
            get { return _household; }
            set
            {
                _household = value;
            }
        }

        private Foods _food = new Foods();
        [CategoryAttribute("(Kiadások)"), DisplayName("Élelmiszer"), RefreshProperties(RefreshProperties.All), Editor(typeof(TypeEditor7), typeof(UITypeEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Foods food
        {
            get { return _food; }
            set
            {
                _food = value;
            }
        }

        private Savings _savings = new Savings();
        [CategoryAttribute("(Kiadások)"), DisplayName("Megtakarítások"), RefreshProperties(RefreshProperties.All), Editor(typeof(TypeEditor9), typeof(UITypeEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Savings savings
        {
            get { return _savings; }
            set
            {
                _savings = value;
            }
        }

        private Insurances _insurances = new Insurances();
        [CategoryAttribute("(Kiadások)"), DisplayName("Biztosítások"), RefreshProperties(RefreshProperties.All), Editor(typeof(TypeEditor10), typeof(UITypeEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Insurances insurances
        {
            get { return _insurances; }
            set
            {
                _insurances = value;
            }
        }

        private Others _others = new Others();
        [CategoryAttribute("(Kiadások)"), DisplayName("Egyéb kiadások"), RefreshProperties(RefreshProperties.All), Editor(typeof(TypeEditor11), typeof(UITypeEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Others others
        {
            get { return _others; }
            set
            {
                _others = value;
            }
        }

        

        #endregion


        private void expandable_propertiesChanged(PropertiesChangedEventArgs e)
        {
            if (propertiesChanged != null)
            {
                propertiesChanged(new PropertiesChangedEventArgs
                {
                    propName = e.propName,
                    newValue = e.newValue
                });
            }
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(e.propName));
            }
        }
        
        /*public void Add(CustomProperty Value)
        {
            base.List.Add(Value);
        }*/
    }
}
