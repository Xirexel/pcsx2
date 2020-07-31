using Omega_Red.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Omega_Red.Tools
{
    class PPSSPPControl
    {               
        private IntPtr m_VideoPanelHandler = IntPtr.Zero;

        private double m_Audiolevel = 1.0;

        private bool m_is_muted = false;

        private bool m_is_launched = false;

        private bool m_is_paused = false;

        private static PPSSPPControl m_Instance = null;

        public static PPSSPPControl Instance { get { if (m_Instance == null) m_Instance = new PPSSPPControl(); return m_Instance; } }

        private PPSSPPControl() { }

        public void setVideoPanelHandler(IntPtr a_VideoPanelHandler)
        {
            m_VideoPanelHandler = a_VideoPanelHandler;
        }

        public void Launch(string a_filepath, string szStickDirectory, Action a_callBackDel)
        {
            if (m_is_launched)
            {
                resumeGame();

                return;
            }
                       
            ThreadStart innerCallStart = new ThreadStart(()=> {

                PPSSPPNative.Instance.launch(
                    a_filepath,
                    Direct3D11Device.Instance.Native,
                    m_VideoPanelHandler, 
                    Capture.TargetTexture.Instance.TargetHandler,
                    Tools.PadInput.Instance.TouchPadCallback,
                    Capture.AudioCaptureTarget.Instance.DataCallback,
                    szStickDirectory);

                m_is_launched = true;

                setSoundLevel(m_Audiolevel);

                setIsMuted(m_is_muted);

                if (a_callBackDel != null)
                    a_callBackDel();

            });

            Thread innerCallThread = new Thread(innerCallStart);

            innerCallThread.Start();
        }

        public void close()
        {
            if (m_is_launched)
                PPSSPPNative.Instance.shutdown();

            m_is_launched = false;
        }

        public void pauseGame()
        {
            if (m_is_launched)
                PPSSPPNative.Instance.pause();

            m_is_paused = true;
        }

        public void resumeGame()
        {
            if (m_is_launched && m_is_paused)
            {
                setSoundLevel(m_Audiolevel);

                PPSSPPNative.Instance.resume();

                m_is_paused = false;
            }
        }

        public void saveState(String a_filename)
        {
            if (m_is_launched)
            {
                PPSSPPNative.Instance.save(a_filename);

                m_is_launched = true;

                setSoundLevel(m_Audiolevel);

                setIsMuted(m_is_muted);
            }
        }

        public void loadState(String a_filename, Action a_callBackDel)
        {
            if (m_is_launched)
            {
                ThreadStart innerCallStart = new ThreadStart(() => {

                    PPSSPPNative.Instance.load(a_filename);

                    if (a_callBackDel != null)
                        a_callBackDel();

                });

                Thread innerCallThread = new Thread(innerCallStart);

                innerCallThread.Start();
            }
        }

        public Tuple<string, string> getGameInfo(string a_filename)
        {

            string l_title;

            string l_id;

            PPSSPPNative.Instance.getGameInfo(a_filename, out l_title, out l_id);

            return Tuple.Create<string, string>(l_title, l_id);
        }

        public void setSoundLevel(double a_level)
        {
            if (m_is_launched)
                if (m_is_muted)
                    PPSSPPNative.Instance.setAudioVolume(0.0f);
                else
                    PPSSPPNative.Instance.setAudioVolume((float)a_level);
          
            m_Audiolevel = a_level;
        }

        public void setIsMuted(bool a_state)
        {
            m_is_muted = a_state;

            if (m_is_launched)
                if (m_is_muted)
                    setSoundLevel(0.0f);
                else
                    setSoundLevel(m_Audiolevel);
        }

        public void setLimitFrame(bool a_state)
        {
            PPSSPPNative.Instance.setLimitFrame(a_state);
        }
    }
}
