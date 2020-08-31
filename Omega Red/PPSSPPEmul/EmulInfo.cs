using PPSSPPEmul.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PPSSPPEmul
{
    public class EmulInfo
    {
        public static string getGameDiscInfo(string aFilePath)
        {
            string l_result = "";

            using (var l_PPSSPPNative = new PPSSPPNative())
            {
                string l_title;

                string l_id;

                l_PPSSPPNative.getGameInfo(aFilePath, out l_title, out l_id);

                l_result = l_title + "|" + l_id;
            }
            
            return l_result;
        }
    }
}
