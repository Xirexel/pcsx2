using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.Tools.Savestate
{
    interface IBaseSavestateEntry
    {
        string GetFilename();
        void FreezeIn(ILoadStateBase reader);
        void FreezeOut(ISaveStateBase writer);
    }
}
