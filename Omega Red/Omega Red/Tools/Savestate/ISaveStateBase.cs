using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.Tools.Savestate
{
    interface ISaveStateBase
    {
        void FreezeTag(string a_tag);
        void Freeze(uint a_value);
        void Freeze(int a_value);
        void Freeze(byte[] a_value);
    }
}
