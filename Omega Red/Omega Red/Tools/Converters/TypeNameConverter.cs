using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Omega_Red.Tools.Converters
{
    [ValueConversion(typeof(object), typeof(string))]
    class TypeNameConverter : ConverterMarkupExtension<TypeNameConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var l_typeName = value.GetType().Name;

                return l_typeName;
            }

            return "";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
