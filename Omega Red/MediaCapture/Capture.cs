/*  MediaCapture - Video capture for Omega Red PS2 Emulator for PCs
*
*  MediaCapture is free software: you can redistribute it and/or modify it under the terms
*  of the GNU Lesser General Public License as published by the Free Software Found-
*  ation, either version 3 of the License, or (at your option) any later version.
*
*  MediaCapture is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
*  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
*  PURPOSE.  See the GNU General Public License for more details.
*
*  You should have received a copy of the GNU General Public License along with MediaCapture.
*  If not, see <http://www.gnu.org/licenses/>.
*/

using CaptureManagerToCSharpProxy;
using CaptureManagerToCSharpProxy.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace MediaCapture
{
    public class Capture
    {
        public static CaptureManager mCaptureManager = null;

        private uint m_VideoMixerInputMaxCount = 0;

        private uint m_AudioMixerInputMaxCount = 0;

        private List<object> m_VideoTopologyInputMixerNodes = new List<object>();

        private List<object> m_AudioTopologyInputMixerNodes = new List<object>();

        private ISession m_ISession = null;

        private Action<Action<IntPtr, uint>> m_RegisterAction = null;

        private Dictionary<string, ISession> m_SourceSessions = new Dictionary<string, ISession>();

        private Dictionary<string, object> m_VideoMixerNodes = new Dictionary<string, object>();

        private Dictionary<string, object> m_AudioMixerNodes = new Dictionary<string, object>();

        private static Capture m_Instance = null;
                
        public static Capture Instance { get { if (m_Instance == null) m_Instance = new Capture(); return m_Instance; } }

        private Capture()
        {
            try
            {
                mCaptureManager = new CaptureManager("CaptureManager.dll");
            }
            catch (Exception)
            {
                mCaptureManager = new CaptureManager();
            }
        }

        public void getVersion(StringBuilder aStringBuilderXMLstring)
        {
            do
            {
                if (aStringBuilderXMLstring == null)
                    break;

                if (mCaptureManager == null)
                    break;

                string aPtrPtrXMLstring = "";

                if(mCaptureManager.getVersionControl().getXMLStringVersion(out aPtrPtrXMLstring))
                {
                    aStringBuilderXMLstring.Append(aPtrPtrXMLstring);
                }
                
            } while (false);
        }
        
        public void getCollectionOfSources(StringBuilder aStringBuilderXMLstring)
        {
            do
            {
                if (aStringBuilderXMLstring == null)
                    break;

                if (mCaptureManager == null)
                    break;

                string aPtrPtrXMLstring = "";
                
                if (mCaptureManager.getCollectionOfSources(ref aPtrPtrXMLstring))
                {
                    aStringBuilderXMLstring.Append(aPtrPtrXMLstring);

                    checkMixers();
                }

            } while (false);
        }


        private void checkMixers()
        {
            if (mCaptureManager != null)
            {
                var lStreamControl = mCaptureManager.createStreamControl();

                if (lStreamControl != null)
                {
                    string lXmlString = "";

                    if (lStreamControl.getCollectionOfStreamControlNodeFactories(ref lXmlString))
                    {

                        XmlDocument doc = new XmlDocument();

                        doc.LoadXml(lXmlString);

                        var l_VideoMixerInputMaxCount = doc.SelectSingleNode(
                            "StreamControlNodeFactories/Group[@GUID='{A080FA3C-4870-48E2-96DB-522EEB94FD0D}']/StreamControlNodeFactory[@MajorType ='MFMediaType_Video']/Value.ValueParts/ValuePart[1]/@Value");

                        if (l_VideoMixerInputMaxCount == null)
                            return;

                        uint l_value = 0;

                        if (uint.TryParse(l_VideoMixerInputMaxCount.Value, out l_value))
                        {
                            m_VideoMixerInputMaxCount = l_value;
                        }

                        var l_AudioMixerInputMaxCount = doc.SelectSingleNode(
                            "StreamControlNodeFactories/Group[@GUID='{A080FA3C-4870-48E2-96DB-522EEB94FD0D}']/StreamControlNodeFactory[@MajorType ='MFMediaType_Audio']/Value.ValueParts/ValuePart[1]/@Value");

                        if (l_AudioMixerInputMaxCount == null)
                            return;

                        l_value = 0;

                        if (uint.TryParse(l_AudioMixerInputMaxCount.Value, out l_value))
                        {
                            m_AudioMixerInputMaxCount = l_value;
                        }
                    }
                }
            }
        }

        public bool addSource(string a_SymbolicLink, uint a_MediaTypeIndex, IntPtr a_RenderTarget)
        {
            bool lresult = false;
            
            ISourceControl l_ISourceControl = null;

            ISinkControl l_SinkControl = null;



            IEVRSinkFactory l_EVRSinkFactory = null;

            do
            {
                if (m_SourceSessions.ContainsKey(a_SymbolicLink))
                    break;

                l_ISourceControl = mCaptureManager.createSourceControl();

                if (l_ISourceControl == null)
                    break;
                
                object l_TargetNode = null;
                
                if (a_RenderTarget != IntPtr.Zero)
                {
                    if (m_VideoTopologyInputMixerNodes.Count == 0)
                        break;

                    l_SinkControl = mCaptureManager.createSinkControl();

                    if (l_SinkControl == null)
                        break;

                    l_SinkControl.createSinkFactory(Guid.Empty, out l_EVRSinkFactory);

                    if (l_EVRSinkFactory == null)
                        break;
                    
                    List<object> lRenderOutputNodesList = new List<object>();

                    object l_RenderTargetNode = null;

                    l_EVRSinkFactory.createOutputNode(
                            a_RenderTarget,
                            out l_RenderTargetNode);
                    
                    if (l_RenderTargetNode == null)
                        break;

                    var l_StreamControl = mCaptureManager.createStreamControl();

                    if (l_StreamControl == null)
                        break;

                    ISpreaderNodeFactory l_SpreaderNodeFactory = null;

                    l_StreamControl.createStreamControlNodeFactory(ref l_SpreaderNodeFactory);

                    if (l_SpreaderNodeFactory == null)
                        break;

                    List<object> lOutputNodeList = new List<object>();
                    
                    lOutputNodeList.Add(m_VideoTopologyInputMixerNodes[0]);

                    lOutputNodeList.Add(l_RenderTargetNode);

                    l_SpreaderNodeFactory.createSpreaderNode(
                        lOutputNodeList,
                        out l_TargetNode);

                    if (l_TargetNode == null)
                        break;

                    m_VideoMixerNodes.Add(a_SymbolicLink, m_VideoTopologyInputMixerNodes[0]);

                    m_VideoTopologyInputMixerNodes.RemoveAt(0);
                }
                else
                {
                    if (m_AudioTopologyInputMixerNodes.Count == 0)
                        break;

                    l_TargetNode = m_AudioTopologyInputMixerNodes[0];

                    m_AudioMixerNodes.Add(a_SymbolicLink, m_AudioTopologyInputMixerNodes[0]);

                    m_AudioTopologyInputMixerNodes.RemoveAt(0);
                }

                if (l_TargetNode == null)
                    break;

                List<object> lSourceNodes = new List<object>();

                object lSourceNode;

                if (File.Exists(a_SymbolicLink))
                {
                    var lICaptureProcessor = ImageCaptureProcessor.createCaptureProcessor(a_SymbolicLink);

                    if (lICaptureProcessor == null)
                        break;

                    object lImageSourceSource = null;

                    l_ISourceControl.createSourceFromCaptureProcessor(
                        lICaptureProcessor,
                        out lImageSourceSource);
                    
                    l_ISourceControl.createSourceNodeFromExternalSourceWithDownStreamConnection(
                        lImageSourceSource,
                        0,
                        0,
                        l_TargetNode,
                        out lSourceNode);
                }
                else
                {
                    if (!l_ISourceControl.createSourceNode(
                        a_SymbolicLink,
                        0,
                        a_MediaTypeIndex,
                        l_TargetNode,
                        out lSourceNode))
                        break;
                }

                if (lSourceNode == null)
                    break;

                if (lSourceNodes != null)
                    lSourceNodes.Add(lSourceNode);

                var lSessionControl = mCaptureManager.createSessionControl();

                if (lSessionControl == null)
                    break;

                var l_ISession = lSessionControl.createSession(lSourceNodes.ToArray());

                if (l_ISession == null)
                    break;

                if (l_ISession.startSession(0, Guid.Empty))
                {
                    m_SourceSessions.Add(a_SymbolicLink, l_ISession);

                    lresult = true;
                }

            } while (false);

            return lresult;
        }

        public void removeSource(string a_SymbolicLink)
        {
            if (!m_SourceSessions.ContainsKey(a_SymbolicLink))
                return;

            m_SourceSessions[a_SymbolicLink].closeSession();
            
            m_SourceSessions.Remove(a_SymbolicLink);

            GC.Collect();

            GC.WaitForFullGCComplete();

            if (m_VideoMixerNodes.ContainsKey(a_SymbolicLink))
            {
                m_VideoTopologyInputMixerNodes.Add(m_VideoMixerNodes[a_SymbolicLink]);

                m_VideoMixerNodes.Remove(a_SymbolicLink);
            }
            else if (m_AudioMixerNodes.ContainsKey(a_SymbolicLink))
            {
                m_AudioTopologyInputMixerNodes.Add(m_AudioMixerNodes[a_SymbolicLink]);

                m_AudioMixerNodes.Remove(a_SymbolicLink);
            }
        }

        public void setPosition(string a_SymbolicLink, float aLeft, float aRight, float aTop, float aBottom)
        {
            do
            {
#if EXTEND_CM
                if (!m_VideoMixerNodes.ContainsKey(a_SymbolicLink))
                    break;

                var lVideoMixerControl = mCaptureManager.createVideoMixerControl();

                if (lVideoMixerControl != null)
                    lVideoMixerControl.setPosition(m_VideoMixerNodes[a_SymbolicLink], aLeft, aRight, aTop, aBottom);
#endif

            } while (false);
        }

        public void setOpacity(string a_SymbolicLink, float a_value)
        {
            do
            {
#if EXTEND_CM
                if (!m_VideoMixerNodes.ContainsKey(a_SymbolicLink))
                    break;

                var lVideoMixerControl = mCaptureManager.createVideoMixerControl();

                if (lVideoMixerControl != null)
                    lVideoMixerControl.setOpacity(m_VideoMixerNodes[a_SymbolicLink], a_value);
#endif
            } while (false);
        }

        public void setRelativeVolume(string a_SymbolicLink, float a_value)
        {
            do
            {
#if EXTEND_CM
                if (!m_AudioMixerNodes.ContainsKey(a_SymbolicLink))
                    break;

                var lAudioMixerControl = mCaptureManager.createAudioMixerControl();

                if (lAudioMixerControl != null)
                    lAudioMixerControl.setRelativeVolume(m_AudioMixerNodes[a_SymbolicLink], a_value);
#endif
            } while (false);
        }

        public string start(
            string a_PtrDirectX11Source,
            Action<Action<IntPtr, uint>> a_RegisterAction, 
            string a_FilePath, 
            uint a_CompressionQuality)
        {

            string l_FileExtention = "";

            ISourceControl l_ISourceControl = null;

            IEncoderControl l_EncoderControl = null;

            IStreamControl l_StreamControl = null;

            ISpreaderNodeFactory l_SpreaderNodeFactory = null;

            ISinkControl l_SinkControl = null;

            object l_VideoMediaSource = null;

            object lAudioSourceOutputMediaType = null;

            object l_AudioMediaSource = null;

            object l_VideoSourceMediaType = null;

            List<object> lCompressedMediaTypeList = new List<object>();

            List<object> lOutputNodes = null;

            List<object> lSourceMediaNodeList = new List<object>();

            do
            {
                if (mCaptureManager == null)
                    break;

                l_ISourceControl = mCaptureManager.createSourceControl();

                if (l_ISourceControl == null)
                    break;

                l_EncoderControl = mCaptureManager.createEncoderControl();

                if (l_EncoderControl == null)
                    break;

                l_StreamControl = mCaptureManager.createStreamControl();

                if (l_StreamControl == null)
                    break;

                l_StreamControl.createStreamControlNodeFactory(ref l_SpreaderNodeFactory);

                if (l_SpreaderNodeFactory == null)
                    break;

                l_SinkControl = mCaptureManager.createSinkControl();

                if (l_SinkControl == null)
                    break;
                
                if(!string.IsNullOrEmpty(a_PtrDirectX11Source))
                {
                    var l_VideoCaptureProcessor = VideoTextureCaptureProcessor.createCaptureProcessor(a_PtrDirectX11Source);

                    if (l_VideoCaptureProcessor == null)
                        break;

                    l_ISourceControl.createSourceFromCaptureProcessor(
                        l_VideoCaptureProcessor,
                        out l_VideoMediaSource);

                    if (l_VideoMediaSource == null)
                        break;
                }

                if (m_RegisterAction == null)
                    m_RegisterAction = a_RegisterAction;

                object l_AudioCaptureProcessor = AudioCaptureProcessor.createCaptureProcessor(m_RegisterAction);
                
                // Audio Source

                string lAudioLoopBack = null;

                uint lAudioSourceIndexStream = 0;

                uint lAudioSourceIndexMediaType = 0;

                if (l_AudioCaptureProcessor != null)
                {
                    l_ISourceControl.createSourceFromCaptureProcessor(
                        l_AudioCaptureProcessor,
                        out l_AudioMediaSource);

                    if (l_AudioMediaSource == null)
                        break;

                    l_ISourceControl.getSourceOutputMediaTypeFromMediaSource(
                        l_AudioMediaSource,
                        lAudioSourceIndexStream,
                        lAudioSourceIndexMediaType, 
                        out lAudioSourceOutputMediaType);

                }
                else
                {
                    lAudioLoopBack = "CaptureManager///Software///Sources///AudioEndpointCapture///AudioLoopBack";
                    
                    l_ISourceControl.getSourceOutputMediaType(
                        lAudioLoopBack,
                        lAudioSourceIndexStream,
                        lAudioSourceIndexMediaType, out lAudioSourceOutputMediaType);

                    l_ISourceControl.createSourceNode(
                        lAudioLoopBack,
                        lAudioSourceIndexStream,
                        lAudioSourceIndexMediaType,
                        out l_AudioMediaSource);

                }







                string lxmldoc = "";

                XmlDocument doc = new XmlDocument();

                mCaptureManager.getCollectionOfEncoders(ref lxmldoc);

                doc.LoadXml(lxmldoc);

                var l_VideoEncoderNode = doc.SelectSingleNode("EncoderFactories/Group[@GUID='{73646976-0000-0010-8000-00AA00389B71}']/EncoderFactory[1]/@CLSID");

                if (l_VideoEncoderNode == null)
                    break;

                Guid lCLSIDVideoEncoder;

                if (!Guid.TryParse(l_VideoEncoderNode.Value, out lCLSIDVideoEncoder))
                    break;

                var l_AudioEncoderNode = doc.SelectSingleNode("EncoderFactories/Group[@GUID='{73647561-0000-0010-8000-00AA00389B71}']/EncoderFactory[1]/@CLSID");

                if (l_AudioEncoderNode == null)
                    break;

                Guid lCLSIDAudioEncoder;

                if (!Guid.TryParse(l_AudioEncoderNode.Value, out lCLSIDAudioEncoder))
                    break;




                if(l_VideoMediaSource != null)
                if (!l_ISourceControl.getSourceOutputMediaTypeFromMediaSource(
                    l_VideoMediaSource,
                    0,
                    0,
                    out l_VideoSourceMediaType))
                    break;

                Guid lGUIDVideoEncoderMode = Guid.Empty;

                if (l_VideoSourceMediaType != null)
                {

                    l_EncoderControl.getMediaTypeCollectionOfEncoder(
                        l_VideoSourceMediaType,
                        lCLSIDVideoEncoder,
                        out lxmldoc);

                    doc.LoadXml(lxmldoc);

                    var lGUIDEncoderModeNode = doc.SelectSingleNode("EncoderMediaTypes/Group[1]/@GUID");

                    if (lGUIDEncoderModeNode == null)
                        break;


                    if (!Guid.TryParse(lGUIDEncoderModeNode.Value, out lGUIDVideoEncoderMode))
                        break;
                    
                    var l_VideoCompressedMediaType = getCompressedMediaType(
                        l_EncoderControl,
                        l_VideoSourceMediaType,
                        lCLSIDVideoEncoder,
                        lGUIDVideoEncoderMode,
                        a_CompressionQuality,
                        0);

                    if (l_VideoCompressedMediaType == null)
                        break;

                    lCompressedMediaTypeList.Add(l_VideoCompressedMediaType);
                }


                Guid lGUIDAudioEncoderMode = Guid.Empty;

                if (lAudioSourceOutputMediaType != null)
                {
                    l_EncoderControl.getMediaTypeCollectionOfEncoder(
                        lAudioSourceOutputMediaType,
                        lCLSIDAudioEncoder,
                        out lxmldoc);

                    doc.LoadXml(lxmldoc);

                    var lGUIDEncoderModeNode = doc.SelectSingleNode("EncoderMediaTypes/Group[1]/@GUID");

                    if (lGUIDEncoderModeNode == null)
                        break;

                    if (!Guid.TryParse(lGUIDEncoderModeNode.Value, out lGUIDAudioEncoderMode))
                        break;

                    var l_AudioCompressedMediaType = getCompressedMediaType(
                        l_EncoderControl,
                        lAudioSourceOutputMediaType,
                        lCLSIDAudioEncoder,
                        lGUIDAudioEncoderMode,
                        a_CompressionQuality,
                        0);

                    if (l_AudioCompressedMediaType == null)
                        break;

                    lCompressedMediaTypeList.Add(l_AudioCompressedMediaType);
                }

                

                doc = new XmlDocument();

                mCaptureManager.getCollectionOfSinks(ref lxmldoc);

                doc.LoadXml(lxmldoc);

                var l_FileContainerNode = doc.SelectSingleNode("SinkFactories/SinkFactory[@GUID='{D6E342E3-7DDD-4858-AB91-4253643864C2}']/Value.ValueParts/ValuePart[1]");

                if (l_FileContainerNode == null)
                    break;

                var lSelectedAttr = l_FileContainerNode.Attributes["Value"];

                if (lSelectedAttr == null)
                    break;

                string l_fileExtention = lSelectedAttr.Value;

                if (l_fileExtention == null)
                    break;

                l_FileExtention = l_fileExtention.ToLower();

                lSelectedAttr = l_FileContainerNode.Attributes["GUID"];

                if (lSelectedAttr == null)
                    break;


                IFileSinkFactory l_FileSinkFactory = null;

                l_SinkControl.createSinkFactory(
                Guid.Parse(lSelectedAttr.Value),
                out l_FileSinkFactory);

                if (l_FileSinkFactory == null)
                    break;

               
                lOutputNodes = getOutputNodes(
                    lCompressedMediaTypeList,
                    l_FileSinkFactory,
                    a_FilePath);

                if (lOutputNodes == null || lOutputNodes.Count == 0)
                    break;





                var lSinkNode = doc.SelectSingleNode("SinkFactories/SinkFactory[@GUID='{2F34AF87-D349-45AA-A5F1-E4104D5C458E}']");

                if (lSinkNode == null)
                    break;

                var lContainerNode = lSinkNode.SelectSingleNode("Value.ValueParts/ValuePart[1]");

                if (lContainerNode == null)
                    break;



                IEVRSinkFactory lSinkFactory;

                var lSinkControl = mCaptureManager.createSinkControl();

                lSinkControl.createSinkFactory(
                Guid.Empty,
                out lSinkFactory);

                int l_index = 0;

                if(l_VideoMediaSource != null)
                {
                    var l_encoderNode = getEncoderNode(
                        l_ISourceControl,
                        l_EncoderControl,
                        l_SpreaderNodeFactory,
                        l_VideoSourceMediaType,
                        lCLSIDVideoEncoder,
                        lGUIDVideoEncoderMode,
                        a_CompressionQuality,
                        0,
                        lOutputNodes[l_index++]);

                    if (l_encoderNode == null)
                        break;
#if EXTEND_CM
                    if(m_VideoMixerInputMaxCount > 1)
                    {
                        IMixerNodeFactory lMixerNodeFactory = null;

                        l_StreamControl.createStreamControlNodeFactory(ref lMixerNodeFactory);

                        List<object> lVideoTopologyInputMixerNodes;

                        lMixerNodeFactory.createMixerNodes(
                            l_encoderNode,
                            m_VideoMixerInputMaxCount,
                            out lVideoTopologyInputMixerNodes);

                        if (lVideoTopologyInputMixerNodes.Count == 0)
                            break;

                        l_encoderNode = lVideoTopologyInputMixerNodes[0];

                        for (int i = 1; i < lVideoTopologyInputMixerNodes.Count; i++)
                        {
                            m_VideoTopologyInputMixerNodes.Add(lVideoTopologyInputMixerNodes[i]);
                        }
                    }
#endif
                    object l_VideoSourceNode = null;

                    if (!l_ISourceControl.createSourceNodeFromExternalSourceWithDownStreamConnection(
                        l_VideoMediaSource,
                        0,
                        0,
                        l_encoderNode,
                        out l_VideoSourceNode))
                        break;

                    lSourceMediaNodeList.Add(l_VideoSourceNode);
                }

                if(!string.IsNullOrEmpty(lAudioLoopBack))
                {
                    var l_AudioSourceNode = getSourceNode(
                        l_ISourceControl,
                        l_EncoderControl,
                        l_SpreaderNodeFactory,
                        lAudioLoopBack,
                        lAudioSourceOutputMediaType,
                        lCLSIDAudioEncoder,
                        lGUIDAudioEncoderMode,
                        a_CompressionQuality,
                        0,
                        null,
                        lOutputNodes[l_index++]);

                    lSourceMediaNodeList.Add(l_AudioSourceNode);
                }
                else
                {
                    if(l_AudioMediaSource != null)
                    {
                        var l_encoderNode = getEncoderNode(
                            l_ISourceControl,
                            l_EncoderControl,
                            l_SpreaderNodeFactory,
                            lAudioSourceOutputMediaType,
                            lCLSIDAudioEncoder,
                            lGUIDAudioEncoderMode,
                            a_CompressionQuality,
                            0,
                            lOutputNodes[l_index++]);

                        if (l_encoderNode == null)
                            break;
#if EXTEND_CM
                        if(m_AudioMixerInputMaxCount > 1)
                        {
                            IMixerNodeFactory lMixerNodeFactory = null;

                            l_StreamControl.createStreamControlNodeFactory(ref lMixerNodeFactory);

                            List<object> lAudioTopologyInputMixerNodes;

                            lMixerNodeFactory.createMixerNodes(
                                l_encoderNode,
                                m_AudioMixerInputMaxCount,
                                out lAudioTopologyInputMixerNodes);

                            if (lAudioTopologyInputMixerNodes.Count == 0)
                                break;

                            l_encoderNode = lAudioTopologyInputMixerNodes[0];

                            for (int i = 1; i < lAudioTopologyInputMixerNodes.Count; i++)
                            {
                                m_AudioTopologyInputMixerNodes.Add(lAudioTopologyInputMixerNodes[i]);
                            }
                        }
#endif
                        object l_AudioSourceNode = null;

                        if (!l_ISourceControl.createSourceNodeFromExternalSourceWithDownStreamConnection(
                            l_AudioMediaSource,
                            0,
                            0,
                            l_encoderNode,
                            out l_AudioSourceNode))
                            break;

                        lSourceMediaNodeList.Add(l_AudioSourceNode);
                    }
                }

                var lSessionControl = mCaptureManager.createSessionControl();

                if (lSessionControl == null)
                    break;

                m_ISession = lSessionControl.createSession(
                    lSourceMediaNodeList.ToArray());

                if (m_ISession == null)
                    break;

                m_ISession.registerUpdateStateDelegate(UpdateStateDelegate);

                m_ISession.startSession(0, Guid.Empty);
                
            } while (false);

            if (lOutputNodes != null)
                foreach (var item in lOutputNodes)
                {
                    Marshal.ReleaseComObject(item);
                }

            if (lSourceMediaNodeList != null)
                foreach (var item in lSourceMediaNodeList)
                {
                    Marshal.ReleaseComObject(item);
                }


            if (lAudioSourceOutputMediaType != null)
                Marshal.ReleaseComObject(lAudioSourceOutputMediaType);

            if (l_AudioMediaSource != null)
                Marshal.ReleaseComObject(l_AudioMediaSource);

            if (l_VideoSourceMediaType != null)
                Marshal.ReleaseComObject(l_VideoSourceMediaType);

            foreach (var item in lCompressedMediaTypeList)
            {
                Marshal.ReleaseComObject(item);
            }
            
            if (l_VideoMediaSource != null)
                Marshal.ReleaseComObject(l_VideoMediaSource);

            return l_FileExtention;
        }

        private void UpdateStateDelegate(uint aCallbackEventCode, uint aSessionDescriptor)
        {
            SessionCallbackEventCode k = (SessionCallbackEventCode)aCallbackEventCode;

            switch (k)
            {
                case SessionCallbackEventCode.Unknown:
                    break;
                case SessionCallbackEventCode.Error:
                    break;
                case SessionCallbackEventCode.Status_Error:
                    break;
                case SessionCallbackEventCode.Execution_Error:
                    break;
                case SessionCallbackEventCode.ItIsReadyToStart:
                    break;
                case SessionCallbackEventCode.ItIsStarted:
                    break;
                case SessionCallbackEventCode.ItIsPaused:
                    break;
                case SessionCallbackEventCode.ItIsStopped:
                    break;
                case SessionCallbackEventCode.ItIsEnded:
                    break;
                case SessionCallbackEventCode.ItIsClosed:
                    break;
                case SessionCallbackEventCode.VideoCaptureDeviceRemoved:
                    {


                        //Dispatcher.Invoke(
                        //DispatcherPriority.Normal,
                        //new Action(() => mLaunchButton_Click(null, null)));

                    }
                    break;
                default:
                    break;
            }
        }

        public void stop()
        {
            if (m_ISession == null)
                return;

            m_ISession.stopSession();

            m_ISession.closeSession();

            m_ISession = null;

            foreach (var item in m_VideoTopologyInputMixerNodes)
            {
                Marshal.ReleaseComObject(item);
            }

            m_VideoTopologyInputMixerNodes.Clear();

            foreach (var item in m_AudioTopologyInputMixerNodes)
            {
                Marshal.ReleaseComObject(item);
            }

            m_AudioTopologyInputMixerNodes.Clear();
        }

        private object getCompressedMediaType(
            IEncoderControl aEncoderControl,
            object aSourceMediaType,
            Guid aCLSIDEncoder,
            Guid aCLSIDEncoderMode,
            uint a_CompressionQuality,
            int aCompressedMediaTypeIndex)
        {
            object lresult = null;

            do
            {
                if (aCompressedMediaTypeIndex < 0)
                    break;

                if (aEncoderControl == null)
                    break;

                if (aSourceMediaType == null)
                    break;

                IEncoderNodeFactory lEncoderNodeFactory;

                if (!aEncoderControl.createEncoderNodeFactory(
                    aCLSIDEncoder,
                    out lEncoderNodeFactory))
                    break;

                if (lEncoderNodeFactory == null)
                    break;

                object lCompressedMediaType;

                if (!lEncoderNodeFactory.createCompressedMediaType(
                    aSourceMediaType,
                    aCLSIDEncoderMode,
                    a_CompressionQuality,
                    (uint)aCompressedMediaTypeIndex,
                    out lCompressedMediaType))
                    break;

                lresult = lCompressedMediaType;

            } while (false);

            return lresult;
        }

        private List<object> getOutputNodes(
            List<object> aCompressedMediaTypeList,
            IFileSinkFactory aFileSinkFactory,
            string aFilename)
        {
            List<object> lresult = new List<object>();

            do
            {
                if (aCompressedMediaTypeList == null)
                    break;

                if (aCompressedMediaTypeList.Count == 0)
                    break;

                if (aFileSinkFactory == null)
                    break;

                if (string.IsNullOrEmpty(aFilename))
                    break;

                aFileSinkFactory.createOutputNodes(
                    aCompressedMediaTypeList,
                    aFilename,
                    out lresult);

            } while (false);

            return lresult;
        }

        private object getEncoderNode(
            ISourceControl aSourceControl,
            IEncoderControl aEncoderControl,
            ISpreaderNodeFactory aSpreaderNodeFactory,
            object aSourceMediaType,
            Guid aCLSIDEncoder,
            Guid aCLSIDEncoderMode,
            uint a_CompressionQuality,
            int aCompressedMediaTypeIndex,
            object aOutputNode)
        {
            object lresult = null;

            do
            {
                if (aCompressedMediaTypeIndex < 0)
                    break;

                if (aEncoderControl == null)
                    break;

                if (aSourceMediaType == null)
                    break;

                IEncoderNodeFactory lEncoderNodeFactory;

                if (!aEncoderControl.createEncoderNodeFactory(
                    aCLSIDEncoder,
                    out lEncoderNodeFactory))
                    break;

                if (lEncoderNodeFactory == null)
                    break;

                object lEncoderNode;

                if (!lEncoderNodeFactory.createEncoderNode(
                    aSourceMediaType,
                    aCLSIDEncoderMode,
                    a_CompressionQuality,
                    (uint)aCompressedMediaTypeIndex,
                    aOutputNode,
                    out lEncoderNode))
                    break;

                lresult = lEncoderNode;

            } while (false);

            return lresult;
        }


        private object getSourceNode(
            ISourceControl aSourceControl,
            IEncoderControl aEncoderControl,
            ISpreaderNodeFactory aSpreaderNodeFactory,
            object aMediaSource,
            object aSourceMediaType,
            Guid aCLSIDEncoder,
            Guid aCLSIDEncoderMode,
            uint a_CompressionQuality,
            int aCompressedMediaTypeIndex,
            object PreviewRenderNode,
            object aOutputNode)
        {
            object lresult = null;

            do
            {
                if (aCompressedMediaTypeIndex < 0)
                    break;

                if (aEncoderControl == null)
                    break;

                if (aSourceMediaType == null)
                    break;

                IEncoderNodeFactory lEncoderNodeFactory;

                if (!aEncoderControl.createEncoderNodeFactory(
                    aCLSIDEncoder,
                    out lEncoderNodeFactory))
                    break;

                if (lEncoderNodeFactory == null)
                    break;

                object lEncoderNode;

                if (!lEncoderNodeFactory.createEncoderNode(
                    aSourceMediaType,
                    aCLSIDEncoderMode,
                    a_CompressionQuality,
                    (uint)aCompressedMediaTypeIndex,
                    aOutputNode,
                    out lEncoderNode))
                    break;


                object SpreaderNode = lEncoderNode;

                if (PreviewRenderNode != null)
                {

                    List<object> lOutputNodeList = new List<object>();

                    lOutputNodeList.Add(PreviewRenderNode);

                    lOutputNodeList.Add(lEncoderNode);

                    aSpreaderNodeFactory.createSpreaderNode(
                        lOutputNodeList,
                        out SpreaderNode);

                }                

                object lSourceNode;

                if (!aSourceControl.createSourceNodeFromExternalSourceWithDownStreamConnection(
                    aMediaSource,
                    0,
                    0,
                    SpreaderNode,
                    out lSourceNode))
                    break;

                lresult = lSourceNode;

            } while (false);

            return lresult;
        }


        private object getSourceNode(
            ISourceControl aSourceControl,
            IEncoderControl aEncoderControl,
            ISpreaderNodeFactory aSpreaderNodeFactory,
            string aSymbolicLink,
            object aSourceMediaType,
            Guid aCLSIDEncoder,
            Guid aCLSIDEncoderMode,
            uint a_CompressionQuality,
            int aCompressedMediaTypeIndex,
            object PreviewRenderNode,
            object aOutputNode)
        {
            object lresult = null;

            do
            {
                if (aCompressedMediaTypeIndex < 0)
                    break;

                if (aEncoderControl == null)
                    break;

                if (aSourceMediaType == null)
                    break;

                IEncoderNodeFactory lEncoderNodeFactory;

                if (!aEncoderControl.createEncoderNodeFactory(
                    aCLSIDEncoder,
                    out lEncoderNodeFactory))
                    break;

                if (lEncoderNodeFactory == null)
                    break;

                object lEncoderNode;

                if (!lEncoderNodeFactory.createEncoderNode(
                    aSourceMediaType,
                    aCLSIDEncoderMode,
                    a_CompressionQuality,
                    (uint)aCompressedMediaTypeIndex,
                    aOutputNode,
                    out lEncoderNode))
                    break;


                object SpreaderNode = lEncoderNode;

                if (PreviewRenderNode != null)
                {

                    List<object> lOutputNodeList = new List<object>();

                    lOutputNodeList.Add(PreviewRenderNode);

                    lOutputNodeList.Add(lEncoderNode);

                    aSpreaderNodeFactory.createSpreaderNode(
                        lOutputNodeList,
                        out SpreaderNode);

                }

                object lSourceNode;

                if (!aSourceControl.createSourceNode(
                    aSymbolicLink,
                    0,
                    0,
                    SpreaderNode,
                    out lSourceNode))
                    break;

                lresult = lSourceNode;

            } while (false);

            return lresult;
        }

        public int currentAvalableVideoMixers()
        {
            return m_VideoTopologyInputMixerNodes.Count;
        }

        public int currentAvalableAudioMixers()
        {
            return m_AudioTopologyInputMixerNodes.Count;
        }
    }
}
