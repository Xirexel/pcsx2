/*  Omega Red - Client PS2 Emulator for PCs
*
*  Omega Red is free software: you can redistribute it and/or modify it under the terms
*  of the GNU Lesser General Public License as published by the Free Software Found-
*  ation, either version 3 of the License, or (at your option) any later version.
*
*  Omega Red is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
*  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
*  PURPOSE.  See the GNU General Public License for more details.
*
*  You should have received a copy of the GNU General Public License along with Omega Red.
*  If not, see <http://www.gnu.org/licenses/>.
*/

using Omega_Red.Util.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Omega_Red.Capture
{
    class MediaCapture
    {
        private StringBuilder m_Version_XMLstring = new StringBuilder();

        private Assembly m_MediaCaptureAssembly = null;

        private object m_CaptureObj = null;

        private MethodInfo m_Start = null;

        private MethodInfo m_Stop = null;

        private MethodInfo m_GetVersion = null;

        private MethodInfo m_GetCollectionOfSources = null;

        private MethodInfo m_AddSource = null;

        private MethodInfo m_RemoveSource = null;

        private MethodInfo m_SetPosition = null;

        private MethodInfo m_SetOpacity = null;

        private MethodInfo m_SetRelativeVolume = null;

        private MethodInfo m_CurrentAvalableVideoMixers = null;

        private MethodInfo m_CurrentAvalableAudioMixers = null;

        private string m_TempFileName = "";
                
        private static MediaCapture m_Instance = null;

        public static MediaCapture Instance { get { if (m_Instance == null) m_Instance = new MediaCapture(); return m_Instance; } }

        private MediaCapture()
        {

            try
            {
                var l_ExecutingAssembly = System.Reflection.Assembly.GetExecutingAssembly();

                AppDomain.CurrentDomain.AssemblyResolve += (object sender, ResolveEventArgs args)=>
                {
                    using (var lCaptureManagerToCSharpProxyStream = l_ExecutingAssembly.GetManifestResourceStream("Omega_Red.Modules.x86.CaptureManagerToCSharpProxy.dll"))
                    {
                        if (lCaptureManagerToCSharpProxyStream == null)
                            return null;

                        byte[] lCaptureManagerToCSharpProxybuffer = new byte[(int)lCaptureManagerToCSharpProxyStream.Length];

                        lCaptureManagerToCSharpProxyStream.Read(lCaptureManagerToCSharpProxybuffer, 0, lCaptureManagerToCSharpProxybuffer.Length);
                        
                        var lCaptureManagerToCSharpProxyAssembly = Assembly.Load(lCaptureManagerToCSharpProxybuffer);

                        return lCaptureManagerToCSharpProxyAssembly;
                    }
                };

                using (var lStream = l_ExecutingAssembly.GetManifestResourceStream("Omega_Red.Modules.x86.MediaCapture.dll"))
                {
                    if (lStream == null)
                        return;

                    byte[] buffer = new byte[(int)lStream.Length];

                    lStream.Read(buffer, 0, buffer.Length);

                    m_MediaCaptureAssembly = AppDomain.CurrentDomain.Load(buffer);

                    if (m_MediaCaptureAssembly != null)
                    {
                        Type l_CaptureType = m_MediaCaptureAssembly.GetType("MediaCapture.Capture");
                        
                        if (l_CaptureType != null)
                        {
                            var lProperites = l_CaptureType.GetProperties();

                            foreach (var item in lProperites)
                            {
                                if (item.Name.Contains("Instance"))
                                {
                                    m_CaptureObj = item.GetMethod.Invoke(null, null);

                                    break;
                                }
                            }                            

                            m_Start = l_CaptureType.GetMethod("start");

                            m_Stop = l_CaptureType.GetMethod("stop");

                            m_GetVersion = l_CaptureType.GetMethod("getVersion");

                            m_GetCollectionOfSources = l_CaptureType.GetMethod("getCollectionOfSources");

                            m_AddSource = l_CaptureType.GetMethod("addSource");

                            m_RemoveSource = l_CaptureType.GetMethod("removeSource");

                            m_SetPosition = l_CaptureType.GetMethod("setPosition");

                            m_SetOpacity = l_CaptureType.GetMethod("setOpacity");

                            m_SetRelativeVolume = l_CaptureType.GetMethod("setRelativeVolume");


                            m_CurrentAvalableVideoMixers = l_CaptureType.GetMethod("currentAvalableVideoMixers");

                            m_CurrentAvalableAudioMixers = l_CaptureType.GetMethod("currentAvalableAudioMixers");

                            

                            m_GetVersion.Invoke(m_CaptureObj, new object[] { m_Version_XMLstring });
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

                m_TempFileName = System.IO.Path.GetTempFileName();

                if (string.IsNullOrEmpty(m_TempFileName))
                    break;

                if (m_CaptureObj == null)
                    break;

                if (m_Start == null)
                    break;

                mFileExtention = m_Start.Invoke(m_CaptureObj, new object[] {
                    CaptureTargetTexture.Instance.CaptureNative.ToString(),
                    AudioCaptureTarget.Instance.RegisterAction,
                    m_TempFileName,
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
                
                if (string.IsNullOrEmpty(m_TempFileName))
                    break;

                if (m_CaptureObj == null)
                    break;

                if (m_Stop == null)
                    break;

                m_Stop.Invoke(m_CaptureObj, null);

                if (!string.IsNullOrEmpty(m_TempFileName))
                {
                    Thread.Sleep(1000);

                    Omega_Red.Managers.MediaRecorderManager.Instance.createItem(m_TempFileName);

                    m_TempFileName = "";
                }

                l_result = true;

            } while (false);

            return l_result;
        }

        public string getCollectionOfSources()
        {
            string l_result = "";

            do
            {
                
                if (m_CaptureObj == null)
                    break;

                if (m_GetCollectionOfSources == null)
                    break;
                               
                StringBuilder l_Sources_XMLstring = new StringBuilder();

                m_GetCollectionOfSources.Invoke(m_CaptureObj, new object[] { l_Sources_XMLstring });

                if (l_Sources_XMLstring.Length > 0)
                    l_result = l_Sources_XMLstring.ToString();

            } while (false);

            return l_result;
        }

        public bool addSource(string a_SymbolicLink, uint a_MediaTypeIndex, IntPtr a_RenderTarget)
        {
            bool lresult = false;

            do
            {

                if (m_CaptureObj == null)
                    break;

                if (m_AddSource == null)
                    break;

                lresult = (bool)m_AddSource.Invoke(m_CaptureObj, new object[] { a_SymbolicLink, a_MediaTypeIndex, a_RenderTarget });
                
            } while (false);

            return lresult;
        }

        public void removeSource(string a_SymbolicLink)
        {
            do
            {

                if (m_CaptureObj == null)
                    break;

                if (m_RemoveSource == null)
                    break;

                m_RemoveSource.Invoke(m_CaptureObj, new object[] { a_SymbolicLink });
                
            } while (false);
        }

        public void setPosition(string a_SymbolicLink, float aLeft, float aRight, float aTop, float aBottom)
        {
            do
            {

                if (m_CaptureObj == null)
                    break;

                if (m_SetPosition == null)
                    break;

                m_SetPosition.Invoke(m_CaptureObj, new object[] { a_SymbolicLink, aLeft, aRight, aTop, aBottom });

            } while (false);
        }

        public int currentAvalableVideoMixers()
        {
            int lresult = 0;

            do
            {

                if (m_CaptureObj == null)
                    break;

                if (m_CurrentAvalableVideoMixers == null)
                    break;

                lresult = (int)m_CurrentAvalableVideoMixers.Invoke(m_CaptureObj, new object[] { });

            } while (false);

            return lresult;
        }

        public int currentAvalableAudioMixers()
        {
            int lresult = 0;

            do
            {

                if (m_CaptureObj == null)
                    break;

                if (m_CurrentAvalableAudioMixers == null)
                    break;

                lresult = (int)m_CurrentAvalableAudioMixers.Invoke(m_CaptureObj, new object[] { });

            } while (false);

            return lresult;
        }

        public void setOpacity(string a_SymbolicLink, float a_value)
        {
            do
            {

                if (m_CaptureObj == null)
                    break;

                if (m_SetOpacity == null)
                    break;

                m_SetOpacity.Invoke(m_CaptureObj, new object[] { a_SymbolicLink, a_value });

            } while (false);

        }

        public void setRelativeVolume(string a_SymbolicLink, float a_value)
        {
            do
            {

                if (m_CaptureObj == null)
                    break;

                if (m_SetRelativeVolume == null)
                    break;

                m_SetRelativeVolume.Invoke(m_CaptureObj, new object[] { a_SymbolicLink, a_value });

            } while (false);

        }
    }
}
