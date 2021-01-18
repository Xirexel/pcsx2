using Omega_Red.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.Emulators
{
    enum EmulStartState
    {
        OK,
        Failed,
        Postpone
    }


    public enum AspectRatio
    {
        None = 0,
        Ratio_4_3 = 1,
        Ratio_16_9 = 2
    }

    interface IEmul
    {
        EmulStartState start(IsoInfo a_IsoInfo, IntPtr a_SharedHandle);

        bool pause();

        bool resume();
        
        bool stop();

        void loadState(SaveStateInfo a_SaveStateInfo);

        void saveState(SaveStateInfo a_SaveStateInfo, string aDate, double aDurationInSeconds, byte[] aScreenshot);

        void setLimitFrame(bool a_state);

        void setAudioVolume(float a_level);

        void setMemoryCard(string a_file_path);

        void setVideoAspectRatio(AspectRatio a_AspectRatio);

        GameType GameType { get; }

        string DiscSerial { get; }

        string BiosCheckSum { get; }

        
    }
}
