using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCSXEmul.Tools
{
    interface IModule
    {
        void execute(string a_command, out string a_result);
        void execute(string a_command);
    }
}
