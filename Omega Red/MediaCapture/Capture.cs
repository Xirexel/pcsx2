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
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MediaCapture
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void UpdateCallback();

    public class Capture
    {
        
        private CaptureManager mCaptureManager = null;

        private ISession mISession = null;

        private static Capture m_Instance = null;

        public UpdateCallback UpdateCallbackDelegate { get { return mUpdateCallbackDelegate; } }

        private UpdateCallback mUpdateCallbackDelegate = null;

        private UpdateCallback mUpdateCallbackDelegateInner = null;
        
        public static Capture Instance { get { if (m_Instance == null) m_Instance = new Capture(); return m_Instance; } }

        private Capture()
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

                if(mCaptureManager.getVersionControl().getXMLStringVersion(out aPtrPtrXMLstring))
                {
                    aStringBuilderXMLstring.Append(aPtrPtrXMLstring);
                }
                
            } while (false);

        }

        public string start(string a_PtrDirectX11Source, string a_FilePath, uint a_CompressionQuality)
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

                UpdateCallback lUpdateCallbackDelegateInner = null;

                var l_VideoCaptureProcessor = VideoTextureCaptureProcessor.createCaptureProcessor(a_PtrDirectX11Source, ref lUpdateCallbackDelegateInner);

                if (l_VideoCaptureProcessor == null)
                    break;

                l_ISourceControl.createSourceFromCaptureProcessor(
                    l_VideoCaptureProcessor,
                    out l_VideoMediaSource);

                if (l_VideoMediaSource == null)
                    break;

                

                


                // Audio Source


                string lAudioLoopBack = "CaptureManager///Software///Sources///AudioEndpointCapture///AudioLoopBack";

                uint lAudioSourceIndexStream = 0;

                uint lAudioSourceIndexMediaType = 0;

                l_ISourceControl.getSourceOutputMediaType(
                    lAudioLoopBack,
                    lAudioSourceIndexStream,
                    lAudioSourceIndexMediaType, out lAudioSourceOutputMediaType);

                l_ISourceControl.createSourceNode(
                    lAudioLoopBack,
                    lAudioSourceIndexStream,
                    lAudioSourceIndexMediaType,
                    out l_AudioMediaSource);

                if (l_AudioMediaSource == null)
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

                var l_AudioEncoderNode = doc.SelectSingleNode("EncoderFactories/Group[@GUID='{73647561-0000-0010-8000-00AA00389B71}']/EncoderFactory[1]/@CLSID");

                if (l_AudioEncoderNode == null)
                    break;

                Guid lCLSIDAudioEncoder;

                if (!Guid.TryParse(l_AudioEncoderNode.Value, out lCLSIDAudioEncoder))
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



                l_EncoderControl.getMediaTypeCollectionOfEncoder(
                    lAudioSourceOutputMediaType,
                    lCLSIDAudioEncoder,
                    out lxmldoc);

                doc.LoadXml(lxmldoc);

                lGUIDEncoderModeNode = doc.SelectSingleNode("EncoderMediaTypes/Group[1]/@GUID");

                if (lGUIDEncoderModeNode == null)
                    break;

                Guid lGUIDAudioEncoderMode;

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

                var l_VideoSourceNode = getSourceNode(
                    l_ISourceControl,
                    l_EncoderControl,
                    l_SpreaderNodeFactory,
                    l_VideoMediaSource,
                    l_VideoSourceMediaType,
                    lCLSIDVideoEncoder,
                    lGUIDVideoEncoderMode,
                    a_CompressionQuality,
                    0,
                    null,
                    lOutputNodes[0]);


                lSourceMediaNodeList.Add(l_VideoSourceNode);


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
                    lOutputNodes[1]);

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

                lock (this)
                {
                    mUpdateCallbackDelegateInner = lUpdateCallbackDelegateInner;
                }

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
