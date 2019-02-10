using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Omega_Red.Tools.Converters
{
    class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            Visibility l_result = Visibility.Visible;

            var l_UIElement = value as UIElement;
            
            if (l_UIElement != null)
            {
                l_result = l_UIElement.Visibility;
            }

            return l_result;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
