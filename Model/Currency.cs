using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Model
{
    public class Currency : IComparable<Currency>
    {
        public Currency(string bankName, string name, decimal buy, decimal sell, string date)
        {
            BankName = bankName;
            Name = name;
            Buy = buy;
            Sell = sell;
            Date = date;
        }

        public string BankName { get; set; }
        public string Name { get; set; }
        public decimal Buy { get; set; }
        public decimal Sell { get; set; }
        public string Date { get; set; }

        public int CompareTo(Currency other)
        {
            return this.Buy.CompareTo(other.Buy);
        }
    }
}
