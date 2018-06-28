using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.ViewModel.Util
{
    class CurrencyConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                decimal amount;
                if (decimal.TryParse(value as string, NumberStyles.Currency, null, out amount)) return amount;
                throw new FormatException("Not a valid currency amount");
            }
            return base.ConvertFrom(context, culture, value);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is decimal)
            {
                return ((decimal)value).ToString("C");
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
