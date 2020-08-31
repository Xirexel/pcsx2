using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Golden_Phi.Tools.Converters
{
    class WidthConverter : IValueConverter
    {
        public double Offset { get; set; }

        public double Scale { get; set; }

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            var l_width = (double)value;

            l_width *= Scale;

            l_width += Offset;

            return l_width;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
