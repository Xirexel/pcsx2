using PPSSPPEmul.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PPSSPPEmul
{
    public class EmulInstance
    {
        private PPSSPPNative m_PPSSPPNative = null;

        private bool m_is_paused = false;

        internal static EmulInstance InternalInstance = null;

        public static EmulInstance Instance { get { InternalInstance = new EmulInstance(); return InternalInstance; } }

        private EmulInstance()
        { }
        
        public bool start(
            string szCmdLine, 
            IntPtr a_VideoPanelHandler,
            IntPtr a_getTouchPadCallback,
            IntPtr a_VideoTargetHandler,
            IntPtr a_AudioCaptureTargetHandler,
            string szStickDirectory)
        {
            var l_result = false;

            do
            {
                m_PPSSPPNative = new PPSSPPNative();

                m_PPSSPPNative.launch(
                    szCmdLine,
                    a_VideoPanelHandler,
                    a_VideoTargetHandler,
                    a_getTouchPadCallback,
                    a_AudioCaptureTargetHandler,
                    szStickDirectory);

                l_result = true;

            } while (false);

            return l_result;
        }

        public bool pause()
        {
            if(m_PPSSPPNative != null)
            {
                m_PPSSPPNative.pause();

                m_is_paused = true;
            }

            return true;
        }

        public bool resume()
        {
            if (m_PPSSPPNative != null)
            {
                m_PPSSPPNative.resume();
            }

            m_is_paused = false;

            return true;
        }

        public bool stop()
        {
            var l_result = false;

            do
            {
                if (m_PPSSPPNative != null)
                {
                    m_PPSSPPNative.shutdown();

                    m_PPSSPPNative.Dispose();

                    m_PPSSPPNative = null;
                }

                l_result = true;

            } while (false);

            return l_result;
        }

        public void setLimitFrame(bool a_state)
        {
            if (m_PPSSPPNative != null)
            {
                m_PPSSPPNative.setLimitFrame(a_state);
            }
        }

        public void loadState(string a_sstate_filepath)
        {
            var l_is_paused = m_is_paused;

            if (!l_is_paused)
                pause();

            if (System.IO.File.Exists(a_sstate_filepath))
            {
                if (m_PPSSPPNative != null)
                {
                    var l_file_path = Path.GetTempPath() + "_temp";

                    if (System.IO.File.Exists(l_file_path))
                        File.Delete(l_file_path);

                    Tools.Savestate.SStates.Instance.LoadPPSSPP(a_sstate_filepath, l_file_path);

                    m_PPSSPPNative.load(l_file_path);
                }
            }

            if (!l_is_paused)
                resume();
        }

        public void saveState(string a_sstate_filepath, string aDate, double aDurationInSeconds, byte[] aScreenshot)
        {
            var l_file_path = Path.GetTempPath() + "_temp";

            if (System.IO.File.Exists(l_file_path))
                File.Delete(l_file_path);

            Tools.Savestate.SStates.Screenshot = aScreenshot;

            try
            {
                if (m_PPSSPPNative != null)
                {
                    File.Delete(l_file_path);
                    
                    string l_tempFile = Path.GetTempPath() + "temp";

                    m_PPSSPPNative.save(l_tempFile);

                    if (File.Exists(l_tempFile))
                    {
                        Tools.Savestate.SStates.Instance.SavePPSSPP(l_file_path, l_tempFile, aDate, aDurationInSeconds);

                        File.Delete(l_tempFile);
                    }
                    
                    File.Delete(a_sstate_filepath);

                    File.Move(l_file_path, a_sstate_filepath);
                }
            }
            catch (Exception)
            {
            }
        }

        public void setAudioVolume(float a_value)
        {
            if (m_PPSSPPNative != null)
            {
                m_PPSSPPNative.setAudioVolume(a_value);
            }
        }
    }
}
