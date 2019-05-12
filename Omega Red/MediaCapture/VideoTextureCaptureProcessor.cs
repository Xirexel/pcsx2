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

using CaptureManagerToCSharpProxy.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaCapture
{
    class VideoTextureCaptureProcessor : ICaptureProcessor
    {
        string mPresentationDescriptor = "";
                
        private VideoTextureCaptureProcessor() { }

        static public ICaptureProcessor createCaptureProcessor(string a_PtrDirectX11Source)
        {
            if (string.IsNullOrEmpty(a_PtrDirectX11Source))
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
                            "<MediaTypeItem Name='MF_MT_FRAME_RATE' GUID='{C459A2E8-3D2C-4E44-B132-FEE5156C7BB0}' Title='Frame rate.' Description='Frame rate of a video media type, in frames per second.'>" + 
                                "<RatioValue Value='30.0'>" + 
                                    "<Value.ValueParts>" + 
                                        "<ValuePart Title='Numerator'  Value='30' />" +  
                                        "<ValuePart Title='Denominator'  Value='1' />" +  
                                    "</Value.ValueParts>" + 
                                "</RatioValue>" + 
                            "</MediaTypeItem>" +
	                        "<MediaTypeItem Name='CM_DirectX11_Capture_Texture' GUID='{179B7A05-496A-4C9F-B8C6-15F04E669595}'>" +
                                "<SingleValue Value='{Temp_Capture_Texture}' />" +
                            "</MediaTypeItem>" +
                        "</MediaType>" +
                    "</MediaTypes>" +
                "</StreamDescriptor>" +
            "</PresentationDescriptor>";

            VideoTextureCaptureProcessor lICaptureProcessor = new VideoTextureCaptureProcessor();
            
            lPresentationDescriptor = lPresentationDescriptor.Replace("{Temp_Capture_Texture}", a_PtrDirectX11Source);                  

            lICaptureProcessor.mPresentationDescriptor = lPresentationDescriptor;
                               
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
        }

        public void sourceRequest(ISourceRequestResult aISourceRequestResult)
        {
        }

        public void start(long aStartPositionInHundredNanosecondUnits, ref Guid aGUIDTimeFormat)
        {                     
        }

        public void stop()
        {
        }
    }
}
