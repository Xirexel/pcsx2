using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.Managers
{
    interface IStreamingManager: IDisposable
    {
        event Action<bool> m_IsAllowedCofirmEvent;

        object Panel { get;}

        Tuple<string, string> getConnectionToken();
    }
}
