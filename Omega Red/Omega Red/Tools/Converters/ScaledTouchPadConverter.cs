using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Omega_Red.Tools.Converters
{
    class ScaledTouchPadConverter : IValueConverter
    {
        public double Offset { get; set; }

        public double Scale { get; set; }

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            UInt32 l_Scale = (UInt32)value;

            Double l_width = (Double)parameter;

            l_width = l_width * (Double)l_Scale / 100.0;

            return l_width;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
