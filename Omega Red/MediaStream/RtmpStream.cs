using CaptureManagerToCSharpProxy;
using CaptureManagerToCSharpProxy.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace MediaStream
{
    public class RtmpStream
    {

        private CaptureManager mCaptureManager = null;

        private ISession mISession = null;

        private static RtmpStream m_Instance = null;
        
        private Action<Action<IntPtr, uint>> m_RegisterAction = null;

        public static RtmpStream Instance { get { if (m_Instance == null) m_Instance = new RtmpStream(); return m_Instance; } }


        List<Tuple<RtspServer.StreamType, int>> m_streams = new List<Tuple<RtspServer.StreamType, int>>();

        private int m_videoTrackID = 120;

        private int m_audioTrackID = 121;

        Guid MFMediaType_Video = new Guid(
0x73646976, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

        Guid MFVideoFormat_H264 = new Guid("34363248-0000-0010-8000-00AA00389B71");

        Guid MFMediaType_Audio = new Guid(
0x73647561, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

        Guid MFAudioFormat_AAC = new Guid(
0x1610, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        Guid StreamingCBR = new Guid("8F6FF1B6-534E-49C0-B2A8-16D534EAF135");

        private RtmpStream()
        {
            m_streams.Add(Tuple.Create<RtspServer.StreamType, int>(RtspServer.StreamType.Video, m_videoTrackID));

            m_streams.Add(Tuple.Create<RtspServer.StreamType, int>(RtspServer.StreamType.Audio, m_audioTrackID));

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

                if (mCaptureManager.getVersionControl().getXMLStringVersion(out aPtrPtrXMLstring))
                {
                    aStringBuilderXMLstring.Append(aPtrPtrXMLstring);
                }

            } while (false);

        }

        RtspServer m_RtspServer = null;

        //rtsp://127.0.0.1:8554

        private void startServer()
        {
            int port = 8554;
            string username = "user";      // or use NUL if there is no username
            string password = "password";  // or use NUL if there is no password

            m_RtspServer = new RtspServer(m_streams, port, null, null);
            m_RtspServer.StartListen();
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

            ISinkControl l_SinkControl = null;

            object l_VideoMediaSource = null;

            object l_VideoSourceMediaType = null;

            object lAudioSourceOutputMediaType = null;

            object l_AudioMediaSource = null;

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

                l_SinkControl = mCaptureManager.createSinkControl();

                if (l_SinkControl == null)
                    break;
                
                var l_VideoCaptureProcessor = VideoTextureCaptureProcessor.createCaptureProcessor(a_PtrDirectX11Source);

                if (l_VideoCaptureProcessor == null)
                    break;

                l_ISourceControl.createSourceFromCaptureProcessor(
                    l_VideoCaptureProcessor,
                    out l_VideoMediaSource);

                if (l_VideoMediaSource == null)
                    break;







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




                if (!l_ISourceControl.getSourceOutputMediaTypeFromMediaSource(
                    l_VideoMediaSource,
                    0,
                    0,
                    out l_VideoSourceMediaType))
                    break;



                                                          
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



                var l_videoStream = createVideoStream(lSampleGrabberCallbackSinkFactory, m_videoTrackID);


                IEncoderNodeFactory lEncoderNodeFactory;

                if (!l_EncoderControl.createEncoderNodeFactory(
                    lCLSIDVideoEncoder,
                    out lEncoderNodeFactory))
                    break;

                object lEncoderNode;

                if (!lEncoderNodeFactory.createEncoderNode(
                    l_VideoSourceMediaType,
                    StreamingCBR,
                    a_CompressionQuality,
                    (uint)0,
                    l_videoStream.Item1,
                    out lEncoderNode))
                    break;


                object l_VideoSourceNode;

                if (!l_ISourceControl.createSourceNodeFromExternalSourceWithDownStreamConnection(
                    l_VideoMediaSource,
                    0,
                    0,
                    lEncoderNode,
                    out l_VideoSourceNode))
                    break;


                lSourceMediaNodeList.Add(l_VideoSourceNode);






                // Audio Source


                if (m_RegisterAction == null)
                    m_RegisterAction = a_RegisterAction;

                object l_AudioCaptureProcessor = AudioCaptureProcessor.createCaptureProcessor(m_RegisterAction);



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

                var l_audioStream = createAudioStream(lSampleGrabberCallbackSinkFactory, m_audioTrackID);

                lEncoderNodeFactory = null;

                Guid lAACEncoder = new Guid("93AF0C51-2275-45d2-A35B-F2BA21CAED00");

                if (!l_EncoderControl.createEncoderNodeFactory(
                    lAACEncoder,
                    out lEncoderNodeFactory))
                    break;

                lEncoderNode = null;

                if (!lEncoderNodeFactory.createEncoderNode(
                    lAudioSourceOutputMediaType,
                    StreamingCBR,
                    a_CompressionQuality,
                    (uint)0,
                    l_audioStream.Item1,
                    out lEncoderNode))
                    break;


                object l_AudioSourceNode;

                if (!l_ISourceControl.createSourceNodeFromExternalSourceWithDownStreamConnection(
                    l_AudioMediaSource,
                    lAudioSourceIndexStream,
                    lAudioSourceIndexMediaType,
                    lEncoderNode,
                    out l_AudioSourceNode))
                    break;


                lSourceMediaNodeList.Add(l_AudioSourceNode);




                var lSessionControl = mCaptureManager.createSessionControl();

                if (lSessionControl == null)
                    break;

                mISession = lSessionControl.createSession(
                    lSourceMediaNodeList.ToArray());

                if (mISession == null)
                    break;

                mISession.registerUpdateStateDelegate(UpdateStateDelegate);

                mISession.startSession(0, Guid.Empty);

                startServer();
                
            } while (false);

            if (lSourceMediaNodeList != null)
                foreach (var item in lSourceMediaNodeList)
                {
                    Marshal.ReleaseComObject(item);
                }

            if (l_VideoSourceMediaType != null)
                Marshal.ReleaseComObject(l_VideoSourceMediaType);

            if (l_VideoMediaSource != null)
                Marshal.ReleaseComObject(l_VideoMediaSource);

            return l_FileExtention;
        }

        private Tuple<object, RtspServer.StreamType, int> createVideoStream(
            ISampleGrabberCallbackSinkFactory aISampleGrabberCallbackSinkFactory,
            int aIndexCount)
        {
            object result = null;

            RtspServer.StreamType type = RtspServer.StreamType.None;

            int index = 0;

            do
            {
                ISampleGrabberCallback lH264SampleGrabberCallback;

                aISampleGrabberCallbackSinkFactory.createOutputNode(
                    MFMediaType_Video,
                    MFVideoFormat_H264,
                    out lH264SampleGrabberCallback);

                if (lH264SampleGrabberCallback != null)
                {
                    lH264SampleGrabberCallback.mUpdateEvent += delegate
                        (byte[] aData, uint aLength)
                    {
                        if (m_RtspServer != null)
                        {
                            lock (m_RtspServer)
                            {
                                currentmillisecond += 33;

                                m_RtspServer.sendData(aIndexCount, (int)type, currentmillisecond * 90, aData);
                            };
                        }
                    };

                    result = lH264SampleGrabberCallback.getTopologyNode();
                }

                if (result != null)
                {
                    type = RtspServer.StreamType.Video;

                    index = aIndexCount;
                }
            }
            while (false);

            return Tuple.Create<object, RtspServer.StreamType, int>(result, type, index);
        }



        uint currentmillisecond = 0;

        private Tuple<object, RtspServer.StreamType, int> createAudioStream(ISampleGrabberCallbackSinkFactory aISampleGrabberCallbackSinkFactory, int aIndexCount)
        {
            object result = null;

            RtspServer.StreamType type = RtspServer.StreamType.Audio;

            int index = aIndexCount;

            do
            {

                ISampleGrabberCallback lAACSampleGrabberCallback;

                aISampleGrabberCallbackSinkFactory.createOutputNode(
                    MFMediaType_Audio,
                    MFAudioFormat_AAC,
                    out lAACSampleGrabberCallback);

                if (lAACSampleGrabberCallback != null)
                {
                    lAACSampleGrabberCallback.mUpdateFullEvent += delegate
                        (uint aSampleFlags, long aSampleTime, long aSampleDuration, byte[] aData, uint aLength)
                    {
                        if (m_RtspServer != null)
                        {
                            lock (m_RtspServer)
                            {
                                currentmillisecond = (uint)aSampleTime / 10000;

                                m_RtspServer.sendData(aIndexCount, (int)type, currentmillisecond * 90, aData);
                            }
                        }
                    };

                    result = lAACSampleGrabberCallback.getTopologyNode();

                }
            }
            while (false);

            return Tuple.Create<object, RtspServer.StreamType, int>(result, type, index);
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
            if (m_RtspServer != null)
            {
                m_RtspServer.StopListen();

                m_RtspServer.Dispose();

                m_RtspServer = null;
            }
            
            if (mISession == null)
                return;
                       
            mISession.stopSession();

            mISession.closeSession();

            mISession = null;
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
    }
}
