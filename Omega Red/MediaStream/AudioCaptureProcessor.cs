using CaptureManagerToCSharpProxy.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaStream
{
    class AudioCaptureProcessor : ICaptureProcessor
    {
        private string mPresentationDescriptor = "";

        private Action<Action<IntPtr, uint>> m_RegisterAction;

        private ISourceRequestResult m_ISourceRequestResult = null;

        private AudioCaptureProcessor() { }

        static public ICaptureProcessor createCaptureProcessor(Action<Action<IntPtr, uint>> a_RegisterAction)
        {                       
            string lPresentationDescriptor = "<?xml version='1.0' encoding='UTF-8'?>" +
            "<PresentationDescriptor StreamCount='1'>" +
                "<PresentationDescriptor.Attributes Title='Attributes of Presentation'>" +
                    "<Attribute Name='MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_SYMBOLIC_LINK' GUID='{58F0AAD8-22BF-4F8A-BB3D-D2C4978C6E2F}' Title='The symbolic link for a audio capture driver.' Description='Contains the unique symbolic link for a audio capture driver.'>" +
                        "<SingleValue Value='ImageCaptureProcessor' />" +
                    "</Attribute>" +
                    "<Attribute Name='MF_DEVSOURCE_ATTRIBUTE_FRIENDLY_NAME' GUID='{60D0E559-52F8-4FA2-BBCE-ACDB34A8EC01}' Title='The display name for a device.' Description='The display name is a human-readable string, suitable for display in a user interface.'>" +
                        "<SingleValue Value='Audio Capture Processor' />" +
                    "</Attribute>" +
                "</PresentationDescriptor.Attributes>" +
                "<StreamDescriptor Index='0' MajorType='MFMediaType_Audio' MajorTypeGUID='{73647561-0000-0010-8000-00AA00389B71}'>" +
                    "<Attribute Name='MF_SD_STREAM_NAME' GUID='{4F1B099D-D314-41E5-A781-7FEFAA4C501F}' Title='The name of a stream.' Description='Contains the name of a stream.'>" +
                        "<SingleValue Value='Audio Capture Processor' />" +
                    "</Attribute>" +
                    "<MediaTypes TypeCount='1'>" +
                        "<MediaType Index='0'>" +
                            "<MediaTypeItem Name='MF_MT_MAJOR_TYPE' GUID='{48EBA18E-F8C9-4687-BF11-0A74C9F96A8F}' Title='Major type GUID for a media type.' Description='The major type defines the overall category of the media data.'>" +
                                "<SingleValue Value='MFMediaType_Audio' GUID='{73647561-0000-0010-8000-00AA00389B71}' />" +
                            "</MediaTypeItem>" +
                            "<MediaTypeItem Name='MF_MT_SUBTYPE' GUID='{F7E34C9A-42E8-4714-B74B-CB29D72C35E5}' Title='Subtype GUID for a media type.' Description='The subtype GUID defines a specific media format type within a major type.'>" +
                                "<SingleValue Value='MFAudioFormat_PCM' GUID='{00000001-0000-0010-8000-00AA00389B71}' />" +
                            "</MediaTypeItem>" +
                            "<MediaTypeItem Name='MF_MT_AUDIO_NUM_CHANNELS' GUID='{37E48BF5-645E-4C5B-89DE-ADA9E29B696A}' Title='Number of audio channels.' Description='Number of audio channels in an audio media type.'>" +
                                "<SingleValue Value='2' />" +
                            "</MediaTypeItem>" +
                            "<MediaTypeItem Name='MF_MT_AUDIO_SAMPLES_PER_SECOND' GUID='{5fAEEAE7-0290-4C31-9E8A-C534F68D9DBA}' Title='Number of audio samples per second (integer value).' Description='Number of audio samples per second in an audio media type.'>" +
                                "<SingleValue Value='48000' />" +
                            "</MediaTypeItem>" +
                            "<MediaTypeItem Name='MF_MT_AUDIO_BLOCK_ALIGNMENT' GUID='{322DE230-9EEB-43BD-AB7A-FF412251541D}' Title='Block alignment, in bytes.' Description='Block alignment, in bytes, for an audio media type. The block alignment is the minimum atomic unit of data for the audio format.'>" +
                                "<SingleValue Value='4' />" +
                            "</MediaTypeItem>" +
                            "<MediaTypeItem Name='MF_MT_AUDIO_AVG_BYTES_PER_SECOND' GUID='{1AAB75C8-CFEF-451C-AB95-AC034B8E1731}' Title='Average number of bytes per second.' Description='Average number of bytes per second in an audio media type.'>" +
                                "<SingleValue Value='192000' />" +
                            "</MediaTypeItem>" +
                            "<MediaTypeItem Name='MF_MT_AUDIO_BITS_PER_SAMPLE' GUID='{F2DEB57F-40FA-4764-AA33-ED4F2D1FF669}' Title='Number of bits per audio sample.' Description='Number of bits per audio sample in an audio media type.'>" +
                                "<SingleValue Value='16' />" +
                            "</MediaTypeItem>" +
                            "<MediaTypeItem Name='MF_MT_ALL_SAMPLES_INDEPENDENT' GUID='{C9173739-5E56-461C-B713-46FB995CB95F}' Title='Independent of samples.' Description='Specifies for a media type whether each sample is independent of the other samples in the stream.'>" +
                                "<SingleValue Value='True' />" +
                            "</MediaTypeItem>" +
                        "</MediaType>" +
                    "</MediaTypes>" +
                "</StreamDescriptor>" +
            "</PresentationDescriptor>";

            AudioCaptureProcessor lICaptureProcessor = new AudioCaptureProcessor();
            
            lICaptureProcessor.mPresentationDescriptor = lPresentationDescriptor;

            lICaptureProcessor.m_RegisterAction = a_RegisterAction;

            return lICaptureProcessor;
        }

        public void initilaize(IInitilaizeCaptureSource IInitilaizeCaptureSource)
        {
            if (IInitilaizeCaptureSource != null)
            {
                IInitilaizeCaptureSource.setPresentationDescriptor(mPresentationDescriptor);
            }
        }

        public void pause()
        {
        }

        public void setCurrentMediaType(ICurrentMediaType aICurrentMediaType)
        {
            if (aICurrentMediaType == null)
                throw new NotImplementedException();

            uint lStreamIndex = 0;

            uint lMediaTypeIndex = 0;

            aICurrentMediaType.getStreamIndex(out lStreamIndex);

            aICurrentMediaType.getMediaTypeIndex(out lMediaTypeIndex);

            if (lStreamIndex != 0 || lMediaTypeIndex != 0)
                throw new NotImplementedException();
        }

        public void shutdown()
        {
            m_RegisterAction(null);

            m_ISourceRequestResult = null;
        }

        public void sourceRequest(ISourceRequestResult aISourceRequestResult)
        {
            if (m_ISourceRequestResult == null)
            {
                m_ISourceRequestResult = aISourceRequestResult;

                m_RegisterAction((aPtrData, aByteSize) => 
                {
                    m_ISourceRequestResult.setData(aPtrData, aByteSize, 1);
                });
            }
        }

        public void start(long aStartPositionInHundredNanosecondUnits, ref Guid aGUIDTimeFormat)
        {
        }

        public void stop()
        {
            m_RegisterAction(null);

            m_ISourceRequestResult = null;
        }
    }
}
