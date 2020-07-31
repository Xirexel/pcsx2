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
        enum FileFormat
        {
            ASF,
            MP4,
            NONE
        }


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


        ISwitcherControl m_SwitcherControl = null;

        object m_VideoSwitcherNode = null;

        object m_AudioSwitcherNode = null;


        Guid m_FileFormatGUID = Guid.Empty;


        Guid m_CLSIDAudioEncoder = new Guid("93AF0C51-2275-45d2-A35B-F2BA21CAED00");
        
        Guid m_GUIDAudioEncoderMode = Guid.Empty;

        Guid m_GUIDVideoEncoderMode = Guid.Empty;


        List<object> m_CompressedMediaTypeList = new List<object>();

        List<object> m_UncompressedMediaTypeList = new List<object>();


        private Tuple<uint, uint, string> m_AudioBitRates = null;

        private List<Tuple<Tuple<uint, uint, string>, Tuple<string, string>>> m_VideoBitRates = new List<Tuple<Tuple<uint, uint, string>, Tuple<string, string>>>();


        private uint m_VideoBitRate = 0;

        private uint m_CurrentVideoBitRate = 0;

        private Guid m_VideoEncoderCLSID = Guid.Empty;

        private uint m_AudioBitRate = 0;

        private uint m_CurrentAudioBitRate = 0;


        private FileFormat m_FileFormat = FileFormat.NONE;


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

            if(mCaptureManager != null)
            {
                m_SwitcherControl = mCaptureManager.createSwitcherControl();

                if (m_SwitcherControl == null)
                    return;
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

                checkFileFormat();

                string aPtrPtrXMLstring = "";
                
                if (mCaptureManager.getCollectionOfSources(ref aPtrPtrXMLstring))
                {
                    aStringBuilderXMLstring.Append(aPtrPtrXMLstring);

                    checkMixers();

                    checkEncoders(aPtrPtrXMLstring);
                }

            } while (false);
        }
        
        private void checkEncoders(string a_XMLstring)
        {

            do
            {
                if (mCaptureManager == null)
                    break;

                if (m_FileFormat == FileFormat.NONE)
                    break;

                List<Tuple<string, string>> l_encoderCLSIDs = new List<Tuple<string, string>>();

                l_encoderCLSIDs.Add(Tuple.Create<string, string>("{6CA50344-051A-4DED-9779-A43305165E35}", "H264 Microsoft Encoder"));

                l_encoderCLSIDs.Add(Tuple.Create<string, string>("{0257849E-1A7F-4F57-AF39-D6ADC3B42483}", "H264 Hardware Encoder"));
                
                switch (m_FileFormat)
                {
                    case FileFormat.ASF:
                        l_encoderCLSIDs.Add(Tuple.Create<string, string>("{7E320092-596A-41B2-BBEB-175D10504EB6}", "WMVideo8 Encoder"));
                        break;
                    case FileFormat.MP4:
                        break;
                    case FileFormat.NONE:
                    default:
                        break;
                }

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
                            var l_Constant_bit_rate_Mode = "{CA37E2BE-BEC0-4B17-946D-44FBC1B3DF55}"; // Constant bit rate Mode

                            var l_Streaming_Constant_bit_rate_Mode = "{8F6FF1B6-534E-49C0-B2A8-16D534EAF135}"; // Streaming Constant bit rate Mode

                            var l_rang = checkEncoder(lAttr.Value, m_CLSIDAudioEncoder, m_FileFormat == FileFormat.MP4? l_Constant_bit_rate_Mode: l_Streaming_Constant_bit_rate_Mode);

                            if(l_rang != null)
                                m_AudioBitRates = l_rang[0];
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

                            foreach (var l_encoderCLSID in l_encoderCLSIDs)
                            {             
                                Guid lCLSIDVideoEncoder;

                                if (!Guid.TryParse(l_encoderCLSID.Item1, out lCLSIDVideoEncoder))
                                    break;

                                var l_Range = checkEncoder(lAttr.Value, lCLSIDVideoEncoder);

                                if(l_Range != null)
                                {
                                    foreach (var item in l_Range)
                                    {
                                        m_VideoBitRates.Add(Tuple.Create<Tuple<uint, uint, string>, Tuple<string, string>>(item, l_encoderCLSID));
                                    }
                                }
                            }
                        }
                    }
                }
            } while (false);
        }
        
        private List<Tuple<uint, uint, string>> checkEncoder(string a_SymbolicLink, Guid a_CLSIDEncoder, string a_ConstantBitRateMode = "")
        {
            List<Tuple<uint, uint, string>> l_result = new List<Tuple<uint, uint, string>>();

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

                if (!l_SourceControl.getSourceOutputMediaType(
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
                        a_CLSIDEncoder,
                        out lxmlDoc);

                var doc = new System.Xml.XmlDocument();

                doc.LoadXml(lxmlDoc);

                var lGroup = doc.SelectSingleNode("EncoderMediaTypes/Group[@GUID='" + a_ConstantBitRateMode + "']");

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

                    l_result.Add(Tuple.Create<uint, uint, string>(lMaxBitRate, lMinBitRate, a_ConstantBitRateMode));
                }

                lGroup = doc.SelectSingleNode("EncoderMediaTypes/Group[@GUID='{EE8C3745-F45B-42B3-A8CC-C7A696440955}']");

                if (lGroup != null)
                {
                    lMaxBitRate = 0;

                    lMinBitRate = 0;

                    l_result.Add(Tuple.Create<uint, uint, string>(lMaxBitRate, lMinBitRate, "{EE8C3745-F45B-42B3-A8CC-C7A696440955}"));
                }
                                
            } while (false);

            return l_result;
        }

        private void checkFileFormat()
        {
            string lxmldoc = "";

            XmlDocument doc = new XmlDocument();

            mCaptureManager.getCollectionOfSinks(ref lxmldoc);

            doc.LoadXml(lxmldoc);

            var l_FileContainerNodes = doc.SelectNodes("SinkFactories/SinkFactory[@GUID='{D6E342E3-7DDD-4858-AB91-4253643864C2}']/Value.ValueParts/ValuePart");

            bool l_is_mp4_format = false;

            bool l_is_asf_format = false;

            foreach (var item in l_FileContainerNodes)
            {
                var l_FileContainerNode = item as XmlNode;

                if(l_FileContainerNode != null)
                {

                    var lSelectedAttr = l_FileContainerNode.Attributes["Value"];

                    if (lSelectedAttr == null)
                        continue;

                    string l_fileExtention = lSelectedAttr.Value;

                    if (l_fileExtention == null)
                        continue;

                    if(string.Compare(l_fileExtention, "mp4", true) == 0)
                    {
                        l_is_mp4_format = true;
                    }

                    if (string.Compare(l_fileExtention, "asf", true) == 0)
                    {
                        l_is_asf_format = true;
                    }

                }
            }

            if (l_is_mp4_format)
                m_FileFormat = FileFormat.MP4;
            else if (l_is_asf_format)
                m_FileFormat = FileFormat.ASF;                
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

        public string start(
            string a_PtrDirectX11Source,
            Action<Action<IntPtr, uint>> a_RegisterAction, 
            string a_FilePath, 
            uint a_CompressionQuality)
        {
            if (m_VideoBitRate == 0)
                m_CurrentVideoBitRate = a_CompressionQuality;
            else
                m_CurrentVideoBitRate = m_VideoBitRate;


            if (m_AudioBitRate == 0)
                m_CurrentAudioBitRate = a_CompressionQuality;
            else
                m_CurrentAudioBitRate = m_AudioBitRate;


            foreach (var item in m_CompressedMediaTypeList)
            {
                Marshal.ReleaseComObject(item);
            }

            m_CompressedMediaTypeList.Clear();


            foreach (var item in m_UncompressedMediaTypeList)
            {
                Marshal.ReleaseComObject(item);
            }

            m_UncompressedMediaTypeList.Clear();


            string l_FileExtention = "";

            string l_resultFileExtention = "";

            ISourceControl l_ISourceControl = null;

            IEncoderControl l_EncoderControl = null;

            IStreamControl l_StreamControl = null;

            ISpreaderNodeFactory l_SpreaderNodeFactory = null;

            ISinkControl l_SinkControl = null;

            object l_VideoMediaSource = null;

            object lAudioSourceOutputMediaType = null;

            object l_AudioMediaSource = null;

            object l_VideoSourceMediaType = null;

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

                string l_ext = "";
                               
                switch (m_FileFormat)
                {
                    case FileFormat.ASF:
                        l_ext = "ASF";
                        break;
                    case FileFormat.MP4:
                        l_ext = "MP4";
                        break;
                    case FileFormat.NONE:
                    default:
                        break;
                }

                if (string.IsNullOrWhiteSpace(l_ext))
                    break;


                string lxmldoc = "";

                XmlDocument l_doc = new XmlDocument();

                mCaptureManager.getCollectionOfSinks(ref lxmldoc);

                l_doc.LoadXml(lxmldoc);

                var l_FileContainerNode = l_doc.SelectSingleNode("SinkFactories/SinkFactory[@GUID='{D6E342E3-7DDD-4858-AB91-4253643864C2}']/Value.ValueParts/ValuePart[@Value='" + l_ext + "']");

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

                if(!Guid.TryParse(lSelectedAttr.Value, out m_FileFormatGUID))
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







                lxmldoc = "";

                XmlDocument doc = new XmlDocument();
                                                
                Guid lCLSIDVideoEncoder = m_VideoEncoderCLSID;              


                if(l_VideoMediaSource != null)
                if (!l_ISourceControl.getSourceOutputMediaTypeFromMediaSource(
                    l_VideoMediaSource,
                    0,
                    0,
                    out l_VideoSourceMediaType))
                    break;

                if (l_VideoSourceMediaType != null)
                {                                        
                    var l_VideoCompressedMediaType = getCompressedMediaType(
                        l_EncoderControl,
                        l_VideoSourceMediaType,
                        lCLSIDVideoEncoder,
                        m_GUIDVideoEncoderMode,
                        m_CurrentVideoBitRate,
                        0);

                    if (l_VideoCompressedMediaType == null)
                        break;

                    m_CompressedMediaTypeList.Add(l_VideoCompressedMediaType);

                    m_UncompressedMediaTypeList.Add(l_VideoSourceMediaType);
                }



                if (lAudioSourceOutputMediaType != null)
                {
                    var l_AudioCompressedMediaType = getCompressedMediaType(
                        l_EncoderControl,
                        lAudioSourceOutputMediaType,
                        m_CLSIDAudioEncoder,
                        m_GUIDAudioEncoderMode,
                        m_CurrentAudioBitRate,
                        0);

                    if (l_AudioCompressedMediaType == null)
                        break;

                    m_CompressedMediaTypeList.Add(l_AudioCompressedMediaType);

                    m_UncompressedMediaTypeList.Add(lAudioSourceOutputMediaType);
                }


                var l_Encoders = createEncoderNodes(a_FilePath);

                if (l_Encoders == null || l_Encoders.Count != 2)
                    break;
                
                int l_index = 0;

                if(l_VideoMediaSource != null)
                {
                    var l_encoderNode = l_Encoders[l_index++];

                    if (l_encoderNode == null)
                        break;

                    m_VideoSwitcherNode = createSwitcher(l_encoderNode);

                    if (m_VideoSwitcherNode == null)
                        break;

                    l_encoderNode = m_VideoSwitcherNode;

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
                    var l_encoderNode = l_Encoders[l_index++];

                    if (l_encoderNode == null)
                        break;

                    m_AudioSwitcherNode = createSwitcher(l_encoderNode);

                    if (m_AudioSwitcherNode == null)
                        break;

                    l_encoderNode = m_AudioSwitcherNode;

                    var l_AudioSourceNode = getSourceNode(
                        l_ISourceControl,
                        lAudioLoopBack,
                        l_encoderNode);

                    lSourceMediaNodeList.Add(l_AudioSourceNode);
                }
                else
                {
                    if(l_AudioMediaSource != null)
                    {
                        var l_encoderNode = l_Encoders[l_index++];

                        if (l_encoderNode == null)
                            break;

                        m_AudioSwitcherNode = createSwitcher(l_encoderNode);

                        if (m_AudioSwitcherNode == null)
                            break;

                        l_encoderNode = m_AudioSwitcherNode;

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

                l_resultFileExtention = l_FileExtention;

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

            
            if (l_AudioMediaSource != null)
                Marshal.ReleaseComObject(l_AudioMediaSource);
                                   
            if (l_VideoMediaSource != null)
                Marshal.ReleaseComObject(l_VideoMediaSource);

            return l_resultFileExtention;
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
                    break;
                default:
                    break;
            }
        }

        public void stop()
        {


            foreach (var item in m_CompressedMediaTypeList)
            {
                Marshal.ReleaseComObject(item);
            }

            m_CompressedMediaTypeList.Clear();





            foreach (var item in m_UncompressedMediaTypeList)
            {
                Marshal.ReleaseComObject(item);
            }

            m_UncompressedMediaTypeList.Clear();



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

            try
            {
                Marshal.FinalReleaseComObject(m_VideoSwitcherNode);
            }
            catch (Exception)
            {
            }

            try
            {
                Marshal.FinalReleaseComObject(m_AudioSwitcherNode);
            }
            catch (Exception)
            {
            }                       
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

        private object getSourceNode(
            ISourceControl aSourceControl,
            string aSymbolicLink,
            object aOutputNode)
        {
            object lresult = null;

            do
            {
                
                object lSourceNode;

                if (!aSourceControl.createSourceNode(
                    aSymbolicLink,
                    0,
                    0,
                    aOutputNode,
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




        public Tuple<uint, uint, string> getAudioBitRates()
        {
            return m_AudioBitRates;
        }

        public List<Tuple<Tuple<uint, uint, string>, Tuple<string, string>>> getVideoBitRates()
        {
            return m_VideoBitRates;
        }



        public void setVideoBitRate(uint a_bitRate, string a_encoderCLSID, string a_encoderModeCLSID)
        {
            m_VideoBitRate = a_bitRate;

            Guid lCLSIDVideoEncoder;

            if (Guid.TryParse(a_encoderCLSID, out lCLSIDVideoEncoder))
                m_VideoEncoderCLSID = lCLSIDVideoEncoder;

            Guid lCLSIDEncoderMode;

            if (Guid.TryParse(a_encoderModeCLSID, out lCLSIDEncoderMode))
                m_GUIDVideoEncoderMode = lCLSIDEncoderMode;
        }

        public void setAudioBitRate(uint a_bitRate, string a_encoderModeCLSID)
        {
            m_AudioBitRate = a_bitRate;

            Guid lCLSIDEncoderMode;

            if (Guid.TryParse(a_encoderModeCLSID, out lCLSIDEncoderMode))
                m_GUIDAudioEncoderMode = lCLSIDEncoderMode;
        }

        public bool restart(string a_FilePath)
        {
            bool l_result = false;

            do
            {
                detachRecorder();

                l_result = attachRecorder(a_FilePath);

            } while (false);

            return l_result;
        }
        
        private void detachRecorder()
        {
            m_SwitcherControl.detachSwitchers(m_ISession);
        }

        bool attachRecorder(string a_FilePath)
        {
            bool l_result = false;

            do
            {
                List<object> lEncoderNodes = createEncoderNodes(a_FilePath);

                if (lEncoderNodes == null)
                    break;

                int lencoderCount = 0;

                if (m_VideoSwitcherNode != null)
                    m_SwitcherControl.atttachSwitcher(m_VideoSwitcherNode, lEncoderNodes[lencoderCount++]);

                if (m_AudioSwitcherNode != null)
                    m_SwitcherControl.atttachSwitcher(m_AudioSwitcherNode, lEncoderNodes[lencoderCount++]);

                m_SwitcherControl.resumeSwitchers(m_ISession);

                l_result = true;

            } while (false);

            return l_result;
        }

        private List<object> createEncoderNodes(string a_FilePath)
        {

            List<object> lEncoderNodes = new List<object>();

            do
            {

                ISourceControl l_ISourceControl = null;

                IEncoderControl l_EncoderControl = null;

                IStreamControl l_StreamControl = null;

                ISpreaderNodeFactory l_SpreaderNodeFactory = null;

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


                ISinkControl l_SinkControl = null;

                l_SinkControl = mCaptureManager.createSinkControl();

                if (l_SinkControl == null)
                    break;

                IFileSinkFactory l_FileSinkFactory = null;

                l_SinkControl.createSinkFactory(
                m_FileFormatGUID,
                out l_FileSinkFactory);

                if (l_FileSinkFactory == null)
                    break;


                List<object> lOutputNodes = getOutputNodes(
                    m_CompressedMediaTypeList,
                    l_FileSinkFactory,
                    a_FilePath);

                if (lOutputNodes == null || lOutputNodes.Count == 0)
                    break;


                int lOutputIndex = 0;

                object lEncoderNode = getEncoderNode(
                        l_ISourceControl,
                        l_EncoderControl,
                        m_UncompressedMediaTypeList[lOutputIndex],
                        m_VideoEncoderCLSID,
                        m_GUIDVideoEncoderMode,
                        m_CurrentVideoBitRate,
                        0,
                        lOutputNodes[lOutputIndex++]);

                if (lEncoderNode == null)
                    break;

                lEncoderNodes.Add(lEncoderNode);

                lEncoderNode = null;

                lEncoderNode = getEncoderNode(
                        l_ISourceControl,
                        l_EncoderControl,
                        m_UncompressedMediaTypeList[lOutputIndex],
                        m_CLSIDAudioEncoder,
                        m_GUIDAudioEncoderMode,
                        m_CurrentAudioBitRate,
                        0,
                        lOutputNodes[lOutputIndex++]);

                if (lEncoderNode != null)
                    lEncoderNodes.Add(lEncoderNode);

            } while (false);
                                                  
            return lEncoderNodes;
        }
        private object createSwitcher(object aEncoderNode)
        {
            object lresult = null;

            do
            {

                IStreamControl l_StreamControl = null;

                l_StreamControl = mCaptureManager.createStreamControl();

                if (l_StreamControl == null)
                    break;

                ISwitcherNodeFactory lSwitcherNodeFactory = null;

                if (!l_StreamControl.createStreamControlNodeFactory(ref lSwitcherNodeFactory))
                    break;

                if (lSwitcherNodeFactory == null)
                    break;

                lSwitcherNodeFactory.createSwitcherNode(aEncoderNode, out lresult);

            } while (false);

            return lresult;
        }
    }
}
