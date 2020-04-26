using Omega_Red.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Omega_Red.Tools
{
    class PCSXControl
    {
        private IntPtr m_VideoPanelHandler = IntPtr.Zero;
        
        private bool m_is_launched = false;

        private bool m_is_paused = false;

        private static PCSXControl m_Instance = null;

        public static PCSXControl Instance { get { if (m_Instance == null) m_Instance = new PCSXControl(); return m_Instance; } }

        private PCSXControl() { }

        public void setVideoPanelHandler(IntPtr a_VideoPanelHandler)
        {
            m_VideoPanelHandler = a_VideoPanelHandler;
        }

        public void Launch(string a_filepath, string a_discSerial, Action a_callBackDel)
        {
            init();

            if (m_is_launched)
            {
                resumeGame();

                return;
            }

            PCSXModuleManager.Instance.initGPU();

            ModuleControl.Instance.initPCSX(PCSXModuleManager.Instance.GPU);


            System.IO.Directory.CreateDirectory(Properties.Settings.Default.TexturePacksFolder + @"\Dump\" + a_discSerial);

            ModuleControl.Instance.setDiscSerial(a_discSerial);
            

            PCSXNative.Instance.setModule(PCSXModuleManager.Instance.GPU);

            ThreadStart innerCallStart = new ThreadStart(() => {

                PCSXNative.Instance.launch(
                    a_filepath);

                m_is_launched = true;
                
                if (a_callBackDel != null)
                    a_callBackDel();

            });

            Thread innerCallThread = new Thread(innerCallStart);

            innerCallThread.Start();
        }

        public void close()
        {
            if (m_is_launched)
                PCSXNative.Instance.shutdown();

            PCSXNative.Instance.clearModules();

            PCSXModuleManager.Instance.releaseGPU();

            m_is_launched = false;
        }

        public void pauseGame()
        {
            if (m_is_launched)
                PCSXNative.Instance.pause();

            m_is_paused = true;
        }

        public void resumeGame()
        {
            if (m_is_launched && m_is_paused)
            {
                PCSXNative.Instance.resume();

                m_is_paused = false;
            }
        }

        public void saveState(String a_filename)
        {
            if (m_is_launched)
            {
                PCSXNative.Instance.save(a_filename);

                m_is_launched = true;
            }
        }

        public void loadState(String a_filename, Action a_callBackDel)
        {
            if (m_is_launched)
            {
                ThreadStart innerCallStart = new ThreadStart(() => {

                    PCSXNative.Instance.load(a_filename);

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

            PCSXNative.Instance.getGameInfo(a_filename, out l_title, out l_id);

            return Tuple.Create<string, string>(l_title, l_id);
        }
        
        public void setLimitFrame(bool a_state)
        {
            ModuleControl.Instance.setLimitFrame(a_state);
        }

        private bool m_isinitilized = false;

        private void init()
        {
            if (m_isinitilized)
                return;
            
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
                    if(Omega_Red.Tools.BiosControl.LoadBIOS(a_FirstArg, a_SecondArg, Models.GameType.PS1))
                        l_result = 1;

                } while (false);

                return l_result;
            };
        }
    }
}
