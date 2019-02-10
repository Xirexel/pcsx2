using Omega_Red.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.Capture
{
    class OffScreenStream
    {

        private StringBuilder m_Version_XMLstring = new StringBuilder();

        private Assembly m_MediaStreamAssembly = null;

        private object m_StreamObj = null;

        private MethodInfo m_Start = null;

        private MethodInfo m_Stop = null;

        private MethodInfo m_StartServer = null;

        private MethodInfo m_StopServer = null;

        private MethodInfo m_GetVersion = null;

        public object UpdateCallbackDelegate { get { return mUpdateCallbackDelegate; } }

        private object mUpdateCallbackDelegate = null;

        private static OffScreenStream m_Instance = null;

        public static OffScreenStream Instance { get { if (m_Instance == null) m_Instance = new OffScreenStream(); return m_Instance; } }

        private OffScreenStream()
        {

            try
            {
                var l_ExecutingAssembly = System.Reflection.Assembly.GetExecutingAssembly();

                AppDomain.CurrentDomain.AssemblyResolve += (object sender, ResolveEventArgs args) =>
                {
                    using (var lStreamManagerToCSharpProxyStream = l_ExecutingAssembly.GetManifestResourceStream("Omega_Red.Modules.x86.CaptureManagerToCSharpProxy.dll"))
                    {
                        if (lStreamManagerToCSharpProxyStream == null)
                            return null;

                        byte[] lStreamManagerToCSharpProxybuffer = new byte[(int)lStreamManagerToCSharpProxyStream.Length];

                        lStreamManagerToCSharpProxyStream.Read(lStreamManagerToCSharpProxybuffer, 0, lStreamManagerToCSharpProxybuffer.Length);

                        var lStreamManagerToCSharpProxyAssembly = Assembly.Load(lStreamManagerToCSharpProxybuffer);

                        return lStreamManagerToCSharpProxyAssembly;
                    }
                };

                using (var lStream = l_ExecutingAssembly.GetManifestResourceStream("Omega_Red.Modules.x86.MediaStream.dll"))
                {
                    if (lStream == null)
                        return;

                    byte[] buffer = new byte[(int)lStream.Length];

                    lStream.Read(buffer, 0, buffer.Length);

                    m_MediaStreamAssembly = AppDomain.CurrentDomain.Load(buffer);

                    if (m_MediaStreamAssembly != null)
                    {
                        Type l_StreamType = m_MediaStreamAssembly.GetType("MediaStream.OffScreenStream");

                        if (l_StreamType != null)
                        {
                            var lProperites = l_StreamType.GetProperties();

                            foreach (var item in lProperites)
                            {
                                if (item.Name.Contains("Instance"))
                                {
                                    m_StreamObj = item.GetMethod.Invoke(null, null);

                                    break;
                                }
                            }

                            m_Start = l_StreamType.GetMethod("start");

                            m_Stop = l_StreamType.GetMethod("stop");

                            m_StartServer = l_StreamType.GetMethod("startServer");

                            m_StopServer = l_StreamType.GetMethod("stopServer");

                            m_GetVersion = l_StreamType.GetMethod("getVersion");

                            var l_UpdateCallbackDelegateProp = l_StreamType.GetProperty("UpdateCallbackDelegate");

                            if (l_UpdateCallbackDelegateProp != null)
                                mUpdateCallbackDelegate = l_UpdateCallbackDelegateProp.GetMethod.Invoke(m_StreamObj, null);

                            m_GetVersion.Invoke(m_StreamObj, new object[] { m_Version_XMLstring });
                        }
                    }
                }
            }
            catch (System.Exception exc)
            {
                var f = exc.Message;
            }
        }

        public string CaptureManagerVersion { get { return m_Version_XMLstring.ToString(); } }

        private string mFileExtention = "";

        public string FileExtention
        {
            get { return mFileExtention; }
        }

        public bool start()
        {
            bool l_result = false;

            do
            {
                if (m_StreamObj == null)
                    break;

                if (m_Start == null)
                    break;

                mFileExtention = m_Start.Invoke(m_StreamObj, new object[] {
                    Omega_Red.Tools.ModuleControl.getRenderingTexture(),
                    Omega_Red.Tools.ModuleControl.getAudioCaptureProcessor(),
                    "",
                    Omega_Red.Properties.Settings.Default.CompressionQuality}) as string;

                l_result = true;

            } while (false);

            return l_result;
        }

        public bool stop()
        {
            bool l_result = false;

            do
            {

                if (m_StreamObj == null)
                    break;

                if (m_Stop == null)
                    break;

                m_Stop.Invoke(m_StreamObj, null);

                l_result = true;

            } while (false);

            return l_result;
        }

        public bool startServer()
        {
            bool l_result = false;

            do
            {
                if (m_StreamObj == null)
                    break;

                if (m_StartServer == null)
                    break;

                m_StartServer.Invoke(m_StreamObj, new object[] {
                    Settings.Default.OffScreenStreamerPort});

                l_result = true;

            } while (false);

            return l_result;
        }

        public bool stopServer()
        {
            bool l_result = false;

            do
            {

                if (m_StreamObj == null)
                    break;

                if (m_StopServer == null)
                    break;

                m_StopServer.Invoke(m_StreamObj, null);

                l_result = true;

            } while (false);

            return l_result;
        }
    }
}
