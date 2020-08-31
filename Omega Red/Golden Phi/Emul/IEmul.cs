using Golden_Phi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golden_Phi.Emul
{
    enum EmulStartState
    {
        OK,
        Failed,
        Postpone
    }

    interface IEmul
    {
        EmulStartState start(IsoInfo a_IsoInfo, IntPtr a_SharedHandle);

        bool pause();
        
        bool stop();

        void loadState(SaveStateInfo a_SaveStateInfo);

        void saveState(SaveStateInfo a_SaveStateInfo, string aDate, double aDurationInSeconds, byte[] aScreenshot);

        void setLimitFrame(bool a_state);

        void setAudioVolume(float a_level);

        GameType GameType { get; }

        string DiscSerial { get; }

        string BiosCheckSum { get; }

        
    }
}
