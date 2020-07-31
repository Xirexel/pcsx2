using CaptureManagerToCSharpProxy;
using CaptureManagerToCSharpProxy.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace MediaStream
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WSAData
    {
        public short version;
        public short highVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 257)]
        public string description;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 129)]
        public string systemStatus;
        public short maxSockets;
        public short maxUdpDg;
        public IntPtr vendorInfo;
    }

    public static class NativeMethods
    {
        [DllImport("Ws2_32.dll")]
        public static extern Int32 WSAStartup(short wVersionRequested, ref WSAData wsaData);

        [DllImport("Ws2_32.dll")]
        public static extern Int32 WSACleanup();
    }


    public class Stream
    {
        private bool m_socketAccessable = false;

        public static CaptureManager mCaptureManager = null;




        Guid MFMediaType_Video = new Guid(0x73646976, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

        Guid MFVideoFormat_H264 = new Guid("34363248-0000-0010-8000-00AA00389B71");

        Guid MFMediaType_Audio = new Guid(0x73647561, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

        Guid MFAudioFormat_AAC = new Guid(0x1610, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        Guid StreamingCBR = new Guid("8F6FF1B6-534E-49C0-B2A8-16D534EAF135");


        private string m_Address = "";

        private string m_StreamName = "";

        private RtmpClient m_StreamingClient = null;

        int m_currentmillisecond = 0;

        private uint m_VideoMixerInputMaxCount = 0;

        private uint m_AudioMixerInputMaxCount = 0;

        private List<object> m_VideoTopologyInputMixerNodes = new List<object>();

        private List<object> m_AudioTopologyInputMixerNodes = new List<object>();

        private ISession m_ISession = null;

        private Action<Action<IntPtr, uint>> m_RegisterAction = null;

        private Dictionary<string, ISession> m_SourceSessions = new Dictionary<string, ISession>();

        private Dictionary<string, object> m_VideoMixerNodes = new Dictionary<string, object>();

        private Dictionary<string, object> m_AudioMixerNodes = new Dictionary<string, object>();



        private Tuple<uint, uint> m_AudioBitRates = null;

        private Tuple<uint, uint> m_VideoBitRates = null;


        private uint m_VideoBitRate = 0;

        private uint m_AudioBitRate = 0;




        private static Stream m_Instance = null;
        
        public static Stream Instance { get { if (m_Instance == null) m_Instance = new Stream(); return m_Instance; } }
        
        private Stream()
        {
            try
            {
                mCaptureManager = new CaptureManager("CaptureManager.dll");
            }
            catch (Exception)
            {
                mCaptureManager = new CaptureManager();
            }

            WSAData dummy = new WSAData();

            m_socketAccessable = NativeMethods.WSAStartup(0x0202, ref dummy) == 0;
        }

        ~Stream()
        {
            NativeMethods.WSACleanup();
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

                if (mCaptureManager.getVersionControl().getXMLStringVersion(out aPtrPtrXMLstring))
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
                    checkMixers();

                    checkEncoders(aPtrPtrXMLstring);

                    aStringBuilderXMLstring.Append(aPtrPtrXMLstring);
                }

            } while (false);
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

            GC.Collect();

            GC.WaitForFullGCComplete();
        }

        public void setPosition(string a_SymbolicLink, float aLeft, float aRight, float aTop, float aBottom)
        {
            do
            {

                if (!m_VideoMixerNodes.ContainsKey(a_SymbolicLink))
                    break;

                var lVideoMixerControl = mCaptureManager.createVideoMixerControl();

                if (lVideoMixerControl != null)
                    lVideoMixerControl.setPosition(m_VideoMixerNodes[a_SymbolicLink], aLeft, aRight, aTop, aBottom);

            } while (false);
        }

        public void setOpacity(string a_SymbolicLink, float a_value)
        {
            do
            {

                if (!m_VideoMixerNodes.ContainsKey(a_SymbolicLink))
                    break;

                var lVideoMixerControl = mCaptureManager.createVideoMixerControl();

                if (lVideoMixerControl != null)
                    lVideoMixerControl.setOpacity(m_VideoMixerNodes[a_SymbolicLink], a_value);

            } while (false);
        }

        public void setRelativeVolume(string a_SymbolicLink, float a_value)
        {
            do
            {
                if (!m_AudioMixerNodes.ContainsKey(a_SymbolicLink))
                    break;

                var lAudioMixerControl = mCaptureManager.createAudioMixerControl();

                if (lAudioMixerControl != null)
                    lAudioMixerControl.setRelativeVolume(m_AudioMixerNodes[a_SymbolicLink], a_value);
            } while (false);
        }

        private void startServer(string a_streamsXml, Action<bool> a_isConnected)
        {
            if (m_StreamingClient != null)
                m_StreamingClient.disconnect();

            m_StreamingClient = RtmpClient.createInstance(a_streamsXml, m_Address, a_isConnected);
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

        public string start(
            string a_PtrDirectX11Source,
            Action<Action<IntPtr, uint>> a_RegisterAction,
            Action<bool> a_isConnected)
        {
            string l_resultMessage = "";

            object l_VideoMediaSource = null;

            object l_AudioMediaSource = null;

            object lAudioSourceOutputMediaType = null;

            object l_VideoSourceMediaType = null;
            
            List<object> lSourceMediaNodeList = new List<object>();

            List<object> lOutputNodes = new List<object>();

            do
            {
                if (mCaptureManager == null)
                    break;

                if (!m_socketAccessable)
                    break;

                m_currentmillisecond = 0;

                int l_IndexCount = 0;

                var l_ISourceControl = mCaptureManager.createSourceControl();

                if (l_ISourceControl == null)
                    break;

                var l_EncoderControl = mCaptureManager.createEncoderControl();

                if (l_EncoderControl == null)
                    break;

                var l_StreamControl = mCaptureManager.createStreamControl();

                if (l_StreamControl == null)
                    break;

                ISpreaderNodeFactory l_SpreaderNodeFactory = null;

                l_StreamControl.createStreamControlNodeFactory(ref l_SpreaderNodeFactory);

                if (l_SpreaderNodeFactory == null)
                    break;

                var l_SinkControl = mCaptureManager.createSinkControl();

                if (l_SinkControl == null)
                    break;
                
                if (!string.IsNullOrEmpty(a_PtrDirectX11Source))
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

                var l_VideoEncoderNode = doc.SelectSingleNode("EncoderFactories/Group[@GUID='{73646976-0000-0010-8000-00AA00389B71}']/EncoderFactory[@IsStreaming='TRUE'][1]/@CLSID");

                if (l_VideoEncoderNode == null)
                    break;

                Guid lCLSIDVideoEncoder;

                if (!Guid.TryParse(l_VideoEncoderNode.Value, out lCLSIDVideoEncoder))
                    break;

                Guid lCLSIDAudioEncoder = new Guid("93AF0C51-2275-45d2-A35B-F2BA21CAED00");
                                               
                lxmldoc = "";

                mCaptureManager.getCollectionOfSinks(ref lxmldoc);

                doc = new XmlDocument();

                doc.LoadXml(lxmldoc);

                var lSinkNode = doc.SelectSingleNode("SinkFactories/SinkFactory[@GUID='{3D64C48E-EDA4-4EE1-8436-58B64DD7CF13}']");

                if (lSinkNode == null)
                    break;

                var lContainerNode = lSinkNode.SelectSingleNode("Value.ValueParts/ValuePart[1]");

                if (lContainerNode == null)
                    break;

                var lReadMode = setContainerFormat(lContainerNode);

                var lSinkControl = mCaptureManager.createSinkControl();

                ISampleGrabberCallbackSinkFactory lSampleGrabberCallbackSinkFactory = null;

                lSinkControl.createSinkFactory(
                lReadMode,
                out lSampleGrabberCallbackSinkFactory);
                
                if (lSampleGrabberCallbackSinkFactory == null)
                    break;

                ISampleGrabberCallback lH264SampleGrabberCallback;

                lSampleGrabberCallbackSinkFactory.createOutputNode(
                    MFMediaType_Video,
                    MFVideoFormat_H264,
                    out lH264SampleGrabberCallback);

                if (lH264SampleGrabberCallback == null)
                    break;


                lH264SampleGrabberCallback.mUpdateNativeFullEvent += delegate
                    (uint aSampleFlags, long aSampleTime, long aSampleDuration, IntPtr aData, uint aSize)
                {
                    if (m_StreamingClient != null)
                    {
                        lock (m_StreamingClient)
                        {
                            if (aSampleFlags == ((uint)1 << 31) && aSampleTime > 0)
                                return;
                            
                            m_currentmillisecond += 1;

                            m_StreamingClient.sendVideoData(m_currentmillisecond, aData, (int)aSize, aSampleFlags, l_IndexCount);

                            m_currentmillisecond += 33;
                        }
                    }
                };

                lOutputNodes.Add(lH264SampleGrabberCallback.getTopologyNode());





                ISampleGrabberCallback lAACSampleGrabberCallback;

                lSampleGrabberCallbackSinkFactory.createOutputNode(
                    MFMediaType_Audio,
                    MFAudioFormat_AAC,
                    out lAACSampleGrabberCallback);

                if (lAACSampleGrabberCallback == null)
                    break;


                lAACSampleGrabberCallback.mUpdateNativeFullEvent += delegate
                    (uint aSampleFlags, long aSampleTime, long aSampleDuration, IntPtr aData, uint aSize)
                {
                    if (m_StreamingClient != null)
                    {
                        lock (m_StreamingClient)
                        {
                            m_currentmillisecond = (int)(aSampleTime / (long)10000);

                            m_StreamingClient.sendAudioData(m_currentmillisecond, aData, (int)aSize, aSampleFlags, l_IndexCount);
                        }
                    }
                };

                lOutputNodes.Add(lAACSampleGrabberCallback.getTopologyNode());




                if (l_VideoMediaSource != null)
                    if (!l_ISourceControl.getSourceOutputMediaTypeFromMediaSource(
                        l_VideoMediaSource,
                        0,
                        0,
                        out l_VideoSourceMediaType))
                        break;
                                                                              
                                                               
                int l_index = 0;

                string lVideoCompressedMediaTypeXmlstring = "";

                if (l_VideoMediaSource != null)
                {
                   var lTupleEncoder  = getEncoderNode(
                        l_ISourceControl,
                        l_EncoderControl,
                        l_VideoSourceMediaType,
                        lCLSIDVideoEncoder,
                        StreamingCBR,
                        m_VideoBitRate,
                        0,
                        lOutputNodes[l_index++]);

                    object l_encoderNode = lTupleEncoder.Item1;

                    if (l_encoderNode == null)
                        break;

                    lVideoCompressedMediaTypeXmlstring = lTupleEncoder.Item2;

                    if (string.IsNullOrWhiteSpace(lVideoCompressedMediaTypeXmlstring))
                        break;

                    if (m_VideoMixerInputMaxCount > 1)
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

                string lAudioCompressedMediaTypeXmlstring = "";

                if (!string.IsNullOrEmpty(lAudioLoopBack))
                {
                    var l_AudioSourceTuple = getSourceNode(
                        l_ISourceControl,
                        l_EncoderControl,
                        l_SpreaderNodeFactory,
                        lAudioLoopBack,
                        lAudioSourceOutputMediaType,
                        lCLSIDAudioEncoder,
                        StreamingCBR,
                        m_AudioBitRate,
                        0,
                        null,
                        lOutputNodes[l_index++]);

                    lAudioCompressedMediaTypeXmlstring = l_AudioSourceTuple.Item2;

                    if (string.IsNullOrWhiteSpace(lAudioCompressedMediaTypeXmlstring))
                        break;

                    lSourceMediaNodeList.Add(l_AudioSourceTuple.Item1);
                }
                else
                {
                    if (l_AudioMediaSource != null)
                    {
                        var lTupleEncoder = getEncoderNode(
                            l_ISourceControl,
                            l_EncoderControl,
                            lAudioSourceOutputMediaType,
                            lCLSIDAudioEncoder,
                            StreamingCBR,
                            m_AudioBitRate,
                            0,
                            lOutputNodes[l_index++]);


                        object l_encoderNode = lTupleEncoder.Item1;

                        if (l_encoderNode == null)
                            break;

                        lAudioCompressedMediaTypeXmlstring = lTupleEncoder.Item2;

                        if (string.IsNullOrWhiteSpace(lAudioCompressedMediaTypeXmlstring))
                            break;

                        if (m_AudioMixerInputMaxCount > 1)
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


                XmlDocument l_streamMediaTypesXml = new XmlDocument();

                XmlNode ldocNode = l_streamMediaTypesXml.CreateXmlDeclaration("1.0", "UTF-8", null);

                l_streamMediaTypesXml.AppendChild(ldocNode);

                XmlElement rootNode = l_streamMediaTypesXml.CreateElement("MediaTypes");

                l_streamMediaTypesXml.AppendChild(rootNode);


                var lAttr = l_streamMediaTypesXml.CreateAttribute("StreamName");

                lAttr.Value = m_StreamName;

                rootNode.Attributes.Append(lAttr);


                doc = new XmlDocument();

                doc.LoadXml(lVideoCompressedMediaTypeXmlstring);

                var lMediaType = doc.SelectSingleNode("MediaType");

                if (lMediaType != null)
                {
                    rootNode.AppendChild(l_streamMediaTypesXml.ImportNode(lMediaType, true));
                }


                doc = new XmlDocument();

                doc.LoadXml(lAudioCompressedMediaTypeXmlstring);

                lMediaType = doc.SelectSingleNode("MediaType");

                if (lMediaType != null)
                {
                    rootNode.AppendChild(l_streamMediaTypesXml.ImportNode(lMediaType, true));
                }

                var lSessionControl = mCaptureManager.createSessionControl();

                if (lSessionControl == null)
                    break;

                m_ISession = lSessionControl.createSession(
                    lSourceMediaNodeList.ToArray());

                if (m_ISession == null)
                    break;

                startServer(l_streamMediaTypesXml.InnerXml, a_isConnected);

                if(m_StreamingClient == null || m_StreamingClient.Handler == -1)
                {

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

                    break;
                }
                
                m_ISession.registerUpdateStateDelegate(UpdateStateDelegate);

                m_ISession.startSession(0, Guid.Empty);

                l_resultMessage = "StreamingIsStarted";

            } while (false);


            foreach (var item in lOutputNodes)
            {
                Marshal.ReleaseComObject(item);
            }            

            foreach (var item in lSourceMediaNodeList)
            {
                Marshal.ReleaseComObject(item);
            }
            
            if (l_VideoSourceMediaType != null)
                Marshal.ReleaseComObject(l_VideoSourceMediaType);

            if (lAudioSourceOutputMediaType != null)
                Marshal.ReleaseComObject(lAudioSourceOutputMediaType);

            if (l_AudioMediaSource != null)
                Marshal.ReleaseComObject(l_AudioMediaSource);

            if (l_VideoMediaSource != null)
                Marshal.ReleaseComObject(l_VideoMediaSource);

            return l_resultMessage;
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
                    {
                        if (m_StreamingClient != null)
                            m_StreamingClient.disconnect();

                        m_StreamingClient = null;

                        m_ISession = null;
                    }
                    break;
                case SessionCallbackEventCode.VideoCaptureDeviceRemoved:
                    break;
                default:
                    break;
            }
        }

        public void stop(bool a_is_explicitly)
        {
            if (m_ISession == null)
                return;

            m_ISession.stopSession();

            m_ISession.closeSession();

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

            if (a_is_explicitly)
            {
                Thread.Sleep(500);

                if (m_StreamingClient != null)
                    m_StreamingClient.disconnect();

                m_ISession = null;

                m_StreamingClient = null;
            }
        }

        private Guid setContainerFormat(XmlNode aXmlNode)
        {

            Guid lContainerFormatGuid = Guid.Empty;

            do
            {
                if (aXmlNode == null)
                    break;

                var lAttrNode = aXmlNode.SelectSingleNode("@Value");

                if (lAttrNode == null)
                    break;

                lAttrNode = aXmlNode.SelectSingleNode("@GUID");

                if (lAttrNode == null)
                    break;

                if (Guid.TryParse(lAttrNode.Value, out lContainerFormatGuid))
                {
                }

            } while (false);

            return lContainerFormatGuid;
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

        private Tuple<object, string> getEncoderNode(
            ISourceControl aSourceControl,
            IEncoderControl aEncoderControl,
            object aSourceMediaType,
            Guid aCLSIDEncoder,
            Guid aCLSIDEncoderMode,
            uint a_CompressionQuality,
            int aCompressedMediaTypeIndex,
            object aOutputNode)
        {
            object lresult = null;

            string lMediaType = "";

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

                lEncoderNodeFactory.createCompressedMediaType(
                    aSourceMediaType,
                    aCLSIDEncoderMode,
                    a_CompressionQuality,
                    (uint)aCompressedMediaTypeIndex,
                    out lCompressedMediaType);

                if (lCompressedMediaType == null)
                    break;

                mCaptureManager.parseMediaType(lCompressedMediaType, out lMediaType);
                
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

            return Tuple.Create<object, string>(lresult, lMediaType);
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
        
        private Tuple<object, string> getSourceNode(
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

            string lMediaType = "";

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

                lEncoderNodeFactory.createCompressedMediaType(
                    aSourceMediaType,
                    aCLSIDEncoderMode,
                    a_CompressionQuality,
                    (uint)aCompressedMediaTypeIndex,
                    out lCompressedMediaType);

                if (lCompressedMediaType == null)
                    break;

                mCaptureManager.parseMediaType(lCompressedMediaType, out lMediaType);

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

            return Tuple.Create<object, string>(lresult, lMediaType);
        }
        
        private void checkEncoders(string a_XMLstring)
        {

            do
            {
                if (mCaptureManager == null)
                    break;
                
                XmlDocument doc = new XmlDocument();

                doc.LoadXml(a_XMLstring);

                if (doc.DocumentElement != null)
                {





                    var lSourceNode = doc.DocumentElement.SelectSingleNode("//*[Source.Attributes/Attribute[@Name='MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_SYMBOLIC_LINK']/SingleValue[@Value='CaptureManager///Software///Sources///AudioEndpointCapture///AudioLoopBack']]");
                    
                    if (lSourceNode != null)
                    {
                        var lAttr = lSourceNode.SelectSingleNode("Source.Attributes/Attribute[@Name='MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_SYMBOLIC_LINK']/SingleValue/@Value");

                        if (lAttr != null)
                        {
                            Guid lCLSIDAudioEncoder = new Guid("93AF0C51-2275-45d2-A35B-F2BA21CAED00");

                            m_AudioBitRates = checkEncoder(lAttr.Value, lCLSIDAudioEncoder);
                        }
                    }

                    lSourceNode = doc.DocumentElement.SelectSingleNode("//*[Source.Attributes/Attribute[@Name='MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_HW_SOURCE']/SingleValue[@Value='Software device']]");

                    if (lSourceNode != null)
                    {
                        var lAttr = lSourceNode.SelectSingleNode("Source.Attributes/Attribute[@Name='MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_SYMBOLIC_LINK']/SingleValue/@Value");

                        if (lAttr != null)
                        {
                            string lxmldoc = "";

                            XmlDocument lEncodersDoc = new XmlDocument();

                            mCaptureManager.getCollectionOfEncoders(ref lxmldoc);

                            lEncodersDoc.LoadXml(lxmldoc);

                            var l_VideoEncoderNode = lEncodersDoc.SelectSingleNode("EncoderFactories/Group[@GUID='{73646976-0000-0010-8000-00AA00389B71}']/EncoderFactory[@IsStreaming='TRUE'][1]/@CLSID");

                            if (l_VideoEncoderNode == null)
                                break;

                            Guid lCLSIDVideoEncoder;

                            if (!Guid.TryParse(l_VideoEncoderNode.Value, out lCLSIDVideoEncoder))
                                break;

                            m_VideoBitRates = checkEncoder(lAttr.Value, lCLSIDVideoEncoder);
                        }
                    }
                }
            } while (false);
        }

        private Tuple<uint, uint> checkEncoder(string a_SymbolicLink, Guid lCLSIDEncoder)
        {

            uint lMaxBitRate = 0;

            uint lMinBitRate = 0;

            do
            {
                if (mCaptureManager == null)
                    break;

                var l_SourceControl = mCaptureManager.createSourceControl();

                if (l_SourceControl == null)
                    break;

                object l_SourceOutputMediaType = null;

                if (! l_SourceControl.getSourceOutputMediaType(
                    a_SymbolicLink,
                    0,
                    0,
                    out l_SourceOutputMediaType))
                    break;

                if (l_SourceOutputMediaType == null)
                    break;

                var l_EncoderControl = mCaptureManager.createEncoderControl();

                if (l_EncoderControl == null)
                    break;

                string lxmlDoc;

                l_EncoderControl.getMediaTypeCollectionOfEncoder(
                        l_SourceOutputMediaType,
                        lCLSIDEncoder,
                        out lxmlDoc);

                var doc = new System.Xml.XmlDocument();

                doc.LoadXml(lxmlDoc);

                var lGroup = doc.SelectSingleNode("EncoderMediaTypes/Group[@GUID='{8F6FF1B6-534E-49C0-B2A8-16D534EAF135}']");
                
                if (lGroup != null)
                {
                    var lAttr = lGroup.SelectSingleNode("@MaxBitRate");

                    if (lAttr != null)
                    {
                        uint.TryParse(lAttr.Value, out lMaxBitRate);
                    }

                    lAttr = lGroup.SelectSingleNode("@MinBitRate");

                    if (lAttr != null)
                    {
                        uint.TryParse(lAttr.Value, out lMinBitRate);
                    }
                }

            } while (false);

            return Tuple.Create<uint, uint>(lMaxBitRate, lMinBitRate);
        }

        public Tuple<uint, uint> getAudioBitRates()
        {
            return m_AudioBitRates;
        }

        public Tuple<uint, uint> getVideoBitRates()
        {
            return m_VideoBitRates;
        }

        public int currentAvalableVideoMixers()
        {
            return m_VideoTopologyInputMixerNodes.Count;
        }

        public int currentAvalableAudioMixers()
        {
            return m_AudioTopologyInputMixerNodes.Count;
        }

        public void setConnectionToken(string a_Address, string a_StreamName)
        {
            m_Address = a_Address;

            m_StreamName = a_StreamName;
        }

        public void setConnectionFunc(Func<string, string, int> a_Func)
        {

            RtmpClient.Connect = a_Func;
        }

        public void setDisconnectFunc(Action<int> a_Action)
        {
            RtmpClient.Disconnect = a_Action;
        }

        public void setWriteFunc(Action<int, int, IntPtr, int, uint, int, int> a_Action)
        {
            RtmpClient.Write = a_Action;
        }

        public void setIsConnectedFunc(Func<int, bool> a_Func)
        {
            RtmpClient.ConnectedFunc = a_Func;
        }



        public void setVideoBitRate(uint a_bitRate)
        {
            m_VideoBitRate = a_bitRate;
        }

        public void setAudioBitRate(uint a_bitRate)
        {
            m_AudioBitRate = a_bitRate;
        }        
    }
}
