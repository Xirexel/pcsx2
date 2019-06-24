using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Omega_Red.Tools.Converters
{
    class PrivacyStatusConvertor : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            int lresult = -1;

            do
            {
                var lstring = value as string;

                if (string.IsNullOrWhiteSpace(lstring))
                    break;

                if (lstring == "private")
                    lresult = 0;
                else
                if (lstring == "public")
                    lresult = 1;
                else
                if (lstring == "unlisted")
                    lresult = 2;
            

            } while (false);
                                 
            return lresult;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            string lresult = "";

            do
            {
                var lvalue = (int)value;

                if (lvalue == 0)
                    lresult = "private";
                else
                if (lvalue == 1)
                    lresult = "public";
                else
                if (lvalue == 2)
                    lresult = "unlisted";           

            } while (false);


            return lresult;
        }
        #endregion
    }
}
