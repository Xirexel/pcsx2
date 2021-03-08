using PCSXEmul.Tools;
using PCSXEmul.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PCSXEmul
{
    public class EmulInstance
    {
        public uint CheckSum { get; private set; }

        public string DiscSerial { get; private set; } = "";
                        
        private bool m_is_paused = false;

        internal static EmulInstance InternalInstance = null;

        public static EmulInstance Instance { get { InternalInstance = new EmulInstance(); return InternalInstance; } }

        private EmulInstance()
        { }
        
        public bool start(
            IntPtr a_VideoPanelHandler,
            IntPtr a_TouchPadCallbackHandler,
            IntPtr a_VideoTargetHandler,
            IntPtr a_AudioCaptureTargetHandler,
            IntPtr a_WindowHandler,
            string a_iso_file,
            string a_discSerial,
            string a_bios_file)
        {
            var l_result = false;

            do
            {

                ModuleControl.Instance.setVideoPanelHandler(a_VideoPanelHandler);

                ModuleControl.Instance.setTouchPadCallbackHandler(a_TouchPadCallbackHandler);

                ModuleControl.Instance.setVideoTargetHandler(a_VideoTargetHandler);

                ModuleControl.Instance.setAudioCaptureTargetHandler(a_AudioCaptureTargetHandler);

                ModuleControl.Instance.setWindowHandler(a_WindowHandler);

                BiosControl.FilePath = a_bios_file;

                if (!PCSXModuleManager.Instance.isInit)
                {
                    break;
                }

                if (!PCSXNative.Instance.isInit)
                {
                    break;
                }

                foreach (var l_Module in PCSXModuleManager.Instance.Modules)
                {
                    PCSXNative.Instance.setModule(l_Module);
                }

                init();

                PCSXNative.Instance.launch(a_iso_file);

                l_result = true;

            } while (false);

            return l_result;
        }

        private bool m_isinitilized = false;
        
        private void init()
        {
            if (m_isinitilized)
                return;

            ModuleControl.Instance.initPCSX();

            Bind();

            m_isinitilized = true;
        }

        private void Bind()
        {
            PCSXNative.Instance.setPluginsOpenCallback = delegate ()
            {
                ModuleControl.Instance.openPad();
            };

            PCSXNative.Instance.setPluginsCloseCallback = delegate ()
            {
                ModuleControl.Instance.closePad();
            };

            PCSXNative.Instance.setBIOSMemoryCallback = delegate (IntPtr a_FirstArg, Int32 a_SecondArg)
            {
                Int32 l_result = 0;

                do
                {
                    if (Tools.BiosControl.LoadBIOS(a_FirstArg, a_SecondArg))
                        l_result = 1;

                } while (false);

                return l_result;
            };
        }


        public bool pause()
        {
            PCSXNative.Instance.pause();

            m_is_paused = true;

            return true;
        }

        public bool resume()
        {
            PCSXNative.Instance.resume();

            m_is_paused = false;

            return true;
        }

        public bool stop()
        {
            var l_result = false;

            do
            {

                PCSXNative.Instance.shutdown();

                l_result = true;

            } while (false);

            return l_result;
        }

        public void setLimitFrame(bool a_state)
        {
            do
            {
                ModuleControl.Instance.setLimitFrame(a_state);

            } while (false);
        }

        public void loadState(string a_sstate_filepath)
        {
            var l_is_paused = m_is_paused;

            if (!l_is_paused)
                pause();

            
            var l_file_path = Path.GetTempPath() + "_temp";

            if (System.IO.File.Exists(a_sstate_filepath))
            {
                if (System.IO.File.Exists(l_file_path))
                    File.Delete(l_file_path);

                Tools.Savestate.SStates.Instance.LoadPCSX(a_sstate_filepath, l_file_path);

                PCSXNative.Instance.load(l_file_path);
            }

            if (!l_is_paused)
                resume();
        }

        public void saveState(string a_sstate_filepath, string aDate, double aDurationInSeconds, byte[] aScreenshot)
        {
            var l_file_path = Path.GetTempPath() + "_temp";

            Tools.Savestate.SStates.Screenshot = aScreenshot;

            try
            {
                File.Delete(l_file_path);

                PCSXNative.Instance.save(l_file_path);

                Tools.Savestate.SStates.Instance.SavePCSX(a_sstate_filepath, l_file_path, aDate, aDurationInSeconds);

                File.Delete(l_file_path);
            }
            catch (Exception)
            {
            }
        }

        public void setAudioVolume(float a_value)
        {
            ModuleControl.Instance.setSoundLevel(a_value);
        }

        public void setMemoryCard(string a_file_path)
        {
            ModuleControl.Instance.setMemoryCard(a_file_path);
        }
    }
}
