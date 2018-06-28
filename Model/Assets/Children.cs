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
    public class Children : INotifyPropertyChanged
    {

        private decimal school;
        [DisplayName("Iskoláztatás, tankönyvek"), RefreshProperties(RefreshProperties.All), Description("Óvoda, bölcsöde is")]
        public decimal School
        {
            get { return school; }
            set
            {
                school = value;
                TotalChildren = School + Meals + Hobbies;
            }
        }

        private decimal meals;
        [DisplayName("Étkeztetés"), RefreshProperties(RefreshProperties.All), Description("")]
        public decimal Meals
        {
            get { return meals; }
            set
            {
                meals = value;
                TotalChildren = School + Meals + Hobbies;
            }
        }

        private decimal hobbies;
        [DisplayName("Hobbik, szakkörök"), RefreshProperties(RefreshProperties.All), Description("")]
        public decimal Hobbies
        {
            get { return hobbies; }
            set
            {
                hobbies = value;
                TotalChildren = School + Meals + Hobbies;
            }
        }

        private decimal totalChildren;
        [ReadOnlyAttribute(true), Browsable(false), DisplayName("Gyermekek összesen")]
        public decimal TotalChildren
        {
            get { return totalChildren; }
            set
            {
                totalChildren = value;
                TotalValues.Collection.Single(x => x.Name == "Children").TotalValue = value;   //care for Capitalized setters
            }
        }

        public override string ToString()
        {
            return totalChildren.ToString("c2");
        }

        void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;


    }
}
