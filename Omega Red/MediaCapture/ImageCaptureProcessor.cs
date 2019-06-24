using CaptureManagerToCSharpProxy.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media.Imaging;

namespace MediaCapture
{
    class ImageCaptureProcessor : ICaptureProcessor
    {
        static Guid MFVideoFormat_ARGB32 = new Guid(
 21, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);


        string mPresentationDescriptor = "";

        class RawDataFrame
        {
            public IntPtr NativeDataFrame { get; private set; }

            public int NativeDataFrameLength { get; private set; }

            public RawDataFrame(byte[] aPixels)
            {
                NativeDataFrame = Marshal.AllocHGlobal(aPixels.Length);

                Marshal.Copy(aPixels, 0, NativeDataFrame, aPixels.Length);

                NativeDataFrameLength = aPixels.Length;
            }

            public void releaseRawData()
            {
                Marshal.FreeHGlobal(NativeDataFrame);
            }
        }


        List<RawDataFrame> mRawDataFrames = new List<RawDataFrame>();

        int mCurrentFrameIndex = 0;

        private ImageCaptureProcessor() { }

        ~ImageCaptureProcessor()
        {

        }

        static public ICaptureProcessor createCaptureProcessor(string a_FilePath)
        {
            if (Capture.mCaptureManager == null)
                return null;

            string lPresentationDescriptor = "<?xml version='1.0' encoding='UTF-8'?>" +
            "<PresentationDescriptor StreamCount='1'>" +
                "<PresentationDescriptor.Attributes Title='Attributes of Presentation'>" +
                    "<Attribute Name='MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_SYMBOLIC_LINK' GUID='{58F0AAD8-22BF-4F8A-BB3D-D2C4978C6E2F}' Title='The symbolic link for a video capture driver.' Description='Contains the unique symbolic link for a video capture driver.'>" +
                        "<SingleValue Value='ImageCaptureProcessor' />" +
                    "</Attribute>" +
                    "<Attribute Name='MF_DEVSOURCE_ATTRIBUTE_FRIENDLY_NAME' GUID='{60D0E559-52F8-4FA2-BBCE-ACDB34A8EC01}' Title='The display name for a device.' Description='The display name is a human-readable string, suitable for display in a user interface.'>" +
                        "<SingleValue Value='Image Capture Processor' />" +
                    "</Attribute>" +
                "</PresentationDescriptor.Attributes>" +
                "<StreamDescriptor Index='0' MajorType='MFMediaType_Video' MajorTypeGUID='{73646976-0000-0010-8000-00AA00389B71}'>" +
                    "<MediaTypes TypeCount='1'>" +
                        "<MediaType Index='0'>" +
                            "<MediaTypeItem Name='MF_MT_FRAME_SIZE' GUID='{1652C33D-D6B2-4012-B834-72030849A37D}' Title='Width and height of the video frame.' Description='Width and height of a video frame, in pixels.'>" +
                                "<Value.ValueParts>" +
                                    "<ValuePart Title='Width' Value='Temp_Width' />" +
                                    "<ValuePart Title='Height' Value='Temp_Height' />" +
                                "</Value.ValueParts>" +
                            "</MediaTypeItem>" +
                            "<MediaTypeItem Name='MF_MT_AVG_BITRATE' GUID='{20332624-FB0D-4D9E-BD0D-CBF6786C102E}' Title='Approximate data rate of the video stream.' Description='Approximate data rate of the video stream, in bits per second, for a video media type.'>" +
                                "<SingleValue  Value='33570816' />" +
                            "</MediaTypeItem>" +
                            "<MediaTypeItem Name='MF_MT_MAJOR_TYPE' GUID='{48EBA18E-F8C9-4687-BF11-0A74C9F96A8F}' Title='Major type GUID for a media type.' Description='The major type defines the overall category of the media data.'>" +
                                "<SingleValue Value='MFMediaType_Video' GUID='{73646976-0000-0010-8000-00AA00389B71}' />" +
                            "</MediaTypeItem>" +
                            "<MediaTypeItem Name='MF_MT_DEFAULT_STRIDE' GUID='{644B4E48-1E02-4516-B0EB-C01CA9D49AC6}' Title='Default surface stride.' Description='Default surface stride, for an uncompressed video media type. Stride is the number of bytes needed to go from one row of pixels to the next.'>" +
                                "<SingleValue Value='Temp_Stride' />" +
                            "</MediaTypeItem>" +
                            "<MediaTypeItem Name='MF_MT_FIXED_SIZE_SAMPLES' GUID='{B8EBEFAF-B718-4E04-B0A9-116775E3321B}' Title='The fixed size of samples in stream.' Description='Specifies for a media type whether the samples have a fixed size.'>" +
                                "<SingleValue Value='True' />" +
                            "</MediaTypeItem>" +
                            "<MediaTypeItem Name='MF_MT_FRAME_RATE' GUID='{C459A2E8-3D2C-4E44-B132-FEE5156C7BB0}' Title='Frame rate.' Description='Frame rate of a video media type, in frames per second.'>" +
                                "<RatioValue Value='10.0'>" +
                                    "<Value.ValueParts>" +
                                        "<ValuePart Title='Numerator'  Value='10' />" +
                                        "<ValuePart Title='Denominator'  Value='1' />" +
                                    "</Value.ValueParts>" +
                                "</RatioValue>" +
                            "</MediaTypeItem>" +
                            "<MediaTypeItem Name='MF_MT_PIXEL_ASPECT_RATIO' GUID='{C6376A1E-8D0A-4027-BE45-6D9A0AD39BB6}' Title='Pixel aspect ratio.' Description='Pixel aspect ratio for a video media type.'>" +
                                "<RatioValue  Value='1'>" +
                                    "<Value.ValueParts>" +
                                        "<ValuePart Title='Numerator'  Value='1' />" +
                                        "<ValuePart Title='Denominator'  Value='1' />" +
                                    "</Value.ValueParts>" +
                                "</RatioValue>" +
                            "</MediaTypeItem>" +
                            "<MediaTypeItem Name='MF_MT_ALL_SAMPLES_INDEPENDENT' GUID='{C9173739-5E56-461C-B713-46FB995CB95F}' Title='Independent of samples.' Description='Specifies for a media type whether each sample is independent of the other samples in the stream.'>" +
                                "<SingleValue Value='True' />" +
                            "</MediaTypeItem>" +
                            "<MediaTypeItem Name='MF_MT_SAMPLE_SIZE' GUID='{DAD3AB78-1990-408B-BCE2-EBA673DACC10}' Title='The fixed size of each sample in stream.' Description='Specifies the size of each sample, in bytes, in a media type.'>" +
                                "<SingleValue Value='Temp_SampleSize' />" +
                            "</MediaTypeItem>" +
                            "<MediaTypeItem Name='MF_MT_INTERLACE_MODE' GUID='{E2724BB8-E676-4806-B4B2-A8D6EFB44CCD}' Title='Describes how the frames are interlaced.' Description='Describes how the frames in a video media type are interlaced.'>" +
                                "<SingleValue Value='MFVideoInterlace_Progressive' />" +
                            "</MediaTypeItem>" +
                            "<MediaTypeItem Name='MF_MT_SUBTYPE' GUID='{F7E34C9A-42E8-4714-B74B-CB29D72C35E5}' Title='Subtype GUID for a media type.' Description='The subtype GUID defines a specific media format type within a major type.'>" +
                                "<SingleValue GUID='{Temp_SubTypeGUID}' />" +
                            "</MediaTypeItem>" +
                        "</MediaType>" +
                    "</MediaTypes>" +
                "</StreamDescriptor>" +
            "</PresentationDescriptor>";

            ImageCaptureProcessor lICaptureProcessor = new ImageCaptureProcessor();


            using (var lImageStream = File.Open(a_FilePath, FileMode.Open))
            {
                if (lImageStream == null)
                    return null;

                var lBitmap = BitmapDecoder.Create(lImageStream, BitmapCreateOptions.None, BitmapCacheOption.None);

                if (lBitmap == null)
                    return null;

                if (lBitmap.Frames == null || lBitmap.Frames.Count == 0)
                    return null;

                BitmapSource bitmapSource = lBitmap.Frames[0];

                int lStride = 0;

                Guid lMFVideoFormat_RGBFormat = MFVideoFormat_ARGB32;

                Capture.mCaptureManager.getStrideForBitmapInfoHeader(lMFVideoFormat_RGBFormat, (uint)bitmapSource.PixelWidth, out lStride);

                int width = bitmapSource.PixelWidth;
                int height = bitmapSource.PixelHeight;
                int stride = (int)Math.Abs(lStride);



                lPresentationDescriptor = lPresentationDescriptor.Replace("Temp_Width", ((uint)bitmapSource.PixelWidth).ToString());

                lPresentationDescriptor = lPresentationDescriptor.Replace("Temp_Height", ((uint)bitmapSource.PixelHeight).ToString());

                lPresentationDescriptor = lPresentationDescriptor.Replace("Temp_Stride", lStride.ToString());

                lPresentationDescriptor = lPresentationDescriptor.Replace("Temp_SampleSize", ((uint)(Math.Abs(lStride) * bitmapSource.PixelHeight * 3)).ToString());

                lPresentationDescriptor = lPresentationDescriptor.Replace("Temp_SubTypeGUID", lMFVideoFormat_RGBFormat.ToString());

                lICaptureProcessor.mPresentationDescriptor = lPresentationDescriptor;


                foreach (var litem in lBitmap.Frames)
                {
                    bitmapSource = litem;

                    if (bitmapSource.Format != System.Windows.Media.PixelFormats.Bgra32)
                    {
                        FormatConvertedBitmap newFormatedBitmapSource = new FormatConvertedBitmap();

                        newFormatedBitmapSource.BeginInit();

                        newFormatedBitmapSource.Source = litem;

                        newFormatedBitmapSource.DestinationFormat = System.Windows.Media.PixelFormats.Bgra32;

                        newFormatedBitmapSource.EndInit();

                        bitmapSource = newFormatedBitmapSource;
                    }

                    byte[] lPixels = new byte[height * stride];

                    bitmapSource.CopyPixels(lPixels, stride, 0);

                    lICaptureProcessor.mRawDataFrames.Add(new RawDataFrame(lPixels));
                }
            }

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
            foreach (var item in mRawDataFrames)
            {
                item.releaseRawData();
            }

            mRawDataFrames.Clear();
        }

        public void sourceRequest(ISourceRequestResult aISourceRequestResult)
        {
            System.Threading.Thread.Sleep(100);

            if (aISourceRequestResult == null)
                return;

            uint lStreamIndex = 0;

            aISourceRequestResult.getStreamIndex(out lStreamIndex);

            if (lStreamIndex == 0)
            {
                aISourceRequestResult.setData(
                    mRawDataFrames[mCurrentFrameIndex].NativeDataFrame,
                    (uint)mRawDataFrames[mCurrentFrameIndex].NativeDataFrameLength, 1);

                ++mCurrentFrameIndex;

                if (mCurrentFrameIndex >= mRawDataFrames.Count)
                    mCurrentFrameIndex = 0;
            }
        }

        public void start(long aStartPositionInHundredNanosecondUnits, ref Guid aGUIDTimeFormat)
        {
            if (mRawDataFrames.Count == 0)
                return;

            mCurrentFrameIndex = 0;
        }

        public void stop()
        {
        }
    }
}
