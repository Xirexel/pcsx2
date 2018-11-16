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
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void UpdateCallback();

    public class Stream
    {

        private CaptureManager mCaptureManager = null;

        private ISession mISession = null;

        private static Stream m_Instance = null;

        public UpdateCallback UpdateCallbackDelegate { get { return mUpdateCallbackDelegate; } }

        private UpdateCallback mUpdateCallbackDelegate = null;

        private UpdateCallback mUpdateCallbackDelegateInner = null;

        public static Stream Instance { get { if (m_Instance == null) m_Instance = new Stream(); return m_Instance; } }

        private Stream()
        {
            try
            {
                mCaptureManager = new CaptureManager();

                mUpdateCallbackDelegate =
                () =>
                {
                    //lock (this)
                    //{
                    //    if (mUpdateCallbackDelegateInner != null)
                    //        mUpdateCallbackDelegateInner();
                    //}
                };
            }
            catch (System.Exception)
            {

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

        RtspServer s = null;

        //rtsp://127.0.0.1:8554

        private void startServer(List<Tuple<RtspServer.StreamType, int>> streams)
        {
            int port = 8554;
            string username = "user";      // or use NUL if there is no username
            string password = "password";  // or use NUL if there is no password

            s = new RtspServer(streams, port, null, null);
            s.StartListen();
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



        public string start(string a_PtrDirectX11Source, string a_FilePath, uint a_CompressionQuality)
        {

            string l_FileExtention = "";

            ISourceControl l_ISourceControl = null;

            IEncoderControl l_EncoderControl = null;

            IStreamControl l_StreamControl = null;
            
            ISinkControl l_SinkControl = null;

            object l_VideoMediaSource = null;
            
            object l_VideoSourceMediaType = null;
            
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

                UpdateCallback lUpdateCallbackDelegateInner = null;

                var l_VideoCaptureProcessor = VideoTextureCaptureProcessor.createCaptureProcessor(a_PtrDirectX11Source, ref lUpdateCallbackDelegateInner);

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




                l_EncoderControl.getMediaTypeCollectionOfEncoder(
                    l_VideoSourceMediaType,
                    lCLSIDVideoEncoder,
                    out lxmldoc);

                doc.LoadXml(lxmldoc);

                var lGUIDEncoderModeNode = doc.SelectSingleNode("EncoderMediaTypes/Group[1]/@GUID");

                if (lGUIDEncoderModeNode == null)
                    break;

                Guid lGUIDVideoEncoderMode;

                if (!Guid.TryParse(lGUIDEncoderModeNode.Value, out lGUIDVideoEncoderMode))
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

                int lIndexCount = 120;


                var l_videoStream = createVideoStream(lSampleGrabberCallbackSinkFactory, lIndexCount);


                IEncoderNodeFactory lEncoderNodeFactory;

                if (!l_EncoderControl.createEncoderNodeFactory(
                    lCLSIDVideoEncoder,
                    out lEncoderNodeFactory))
                    break;

                object lEncoderNode;

                if (!lEncoderNodeFactory.createEncoderNode(
                    l_VideoSourceMediaType,
                    lGUIDVideoEncoderMode,
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

                List<Tuple<RtspServer.StreamType, int>> streams = new List<Tuple<RtspServer.StreamType, int>>();

                if (l_videoStream.Item1 != null)
                {
                    streams.Add(Tuple.Create<RtspServer.StreamType, int>(l_videoStream.Item2, l_videoStream.Item3));
                }



                var lSessionControl = mCaptureManager.createSessionControl();

                if (lSessionControl == null)
                    break;

                mISession = lSessionControl.createSession(
                    lSourceMediaNodeList.ToArray());

                if (mISession == null)
                    break;

                mISession.registerUpdateStateDelegate(UpdateStateDelegate);

                mISession.startSession(0, Guid.Empty);

                lock (this)
                {
                    mUpdateCallbackDelegateInner = lUpdateCallbackDelegateInner;
                }
                
                startServer(streams);

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

                Guid MFMediaType_Video = new Guid(0x73646976, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

                Guid MFVideoFormat_H264 = new Guid("34363248-0000-0010-8000-00AA00389B71");

                aISampleGrabberCallbackSinkFactory.createOutputNode(
                    MFMediaType_Video,
                    MFVideoFormat_H264,
                    out lH264SampleGrabberCallback);
                
                if (lH264SampleGrabberCallback != null)
                {
                    uint ltimestamp = 0;

                    lH264SampleGrabberCallback.mUpdateEvent += delegate
                        (byte[] aData, uint aLength)
                    {
                        if (s != null)
                        {
                            ThreadPool.QueueUserWorkItem((object state) =>
                            {
                                s.sendData(aIndexCount, ltimestamp, aData);

                                ltimestamp += 33;
                            });
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
            if (s != null)
            {
                s.StopListen();

                s.Dispose();

                s = null;
            }
            
            if (mISession == null)
                return;

            lock (this)
            {
                mUpdateCallbackDelegateInner = null;
            }


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
