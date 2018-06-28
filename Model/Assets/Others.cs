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
    public class Others : INotifyPropertyChanged
    {

        private decimal creditCard;
        [DisplayName("Hitelkártya"), RefreshProperties(RefreshProperties.All), Description("")]
        public decimal CreditCard
        {
            get { return creditCard; }
            set
            {
                creditCard = value;
                TotalOthers = CreditCard + LoanPayment + SuddenExp1 + SuddenExp2 + Other;
            }
        }

        private decimal loanPayment;
        [DisplayName("Hitel törlesztés"), RefreshProperties(RefreshProperties.All), Description("")]
        public decimal LoanPayment
        {
            get { return loanPayment; }
            set
            {
                loanPayment = value;
                TotalOthers = CreditCard + LoanPayment + SuddenExp1 + SuddenExp2 + Other;
            }
        }

        private decimal suddenExp1;
        [DisplayName("Váratlan kiadás - háztartási"), RefreshProperties(RefreshProperties.All), Description("")]
        public decimal SuddenExp1
        {
            get { return suddenExp1; }
            set
            {
                suddenExp1 = value;
                TotalOthers = CreditCard + LoanPayment + SuddenExp1 + SuddenExp2 + Other;
            }
        }

        private decimal suddenExp2;
        [DisplayName("Váratlan kiadás - egyéb"), RefreshProperties(RefreshProperties.All), Description("")]
        public decimal SuddenExp2
        {
            get { return suddenExp2; }
            set
            {
                suddenExp2 = value;
                TotalOthers = CreditCard + LoanPayment + SuddenExp1 + SuddenExp2 + Other;
            }
        }

        private decimal other;
        [DisplayName("Egyéb - saját kategória"), RefreshProperties(RefreshProperties.All), Description("")]
        public decimal Other
        {
            get { return other; }
            set
            {
                other = value;
                TotalOthers = CreditCard + LoanPayment + SuddenExp1 + SuddenExp2 + Other;
            }
        }

        private decimal totalOthers;
        [ReadOnlyAttribute(true), Browsable(false), DisplayName("Egyebek összesen")]
        public decimal TotalOthers
        {
            get { return totalOthers; }
            set
            {
                totalOthers = value;
                TotalValues.Collection.Single(x => x.Name == "Others").TotalValue = value;   //care for Capitalized setters
            }
        }

        public override string ToString()
        {
            return totalOthers.ToString("c2");
        }

        void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;


    }
}

