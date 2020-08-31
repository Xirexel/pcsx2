using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golden_Phi.Tools.Savestate
{
    interface IBaseSavestateEntry
    {
        string GetFilename();
        void FreezeIn(ILoadStateBase reader);
        void FreezeOut(ISaveStateBase writer);
    }
}
