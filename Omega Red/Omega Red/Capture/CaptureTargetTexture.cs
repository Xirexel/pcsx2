using Omega_Red.Capture.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.Capture
{
    class CaptureTargetTexture
    {
        private Direct3D11 m_device = null;

        private D3D11Texture2D m_target_texture = null;

        private DXGIResource m_resource = null;

        private IntPtr m_shared_handler = IntPtr.Zero;

        public IntPtr CaptureHandler { get { return m_shared_handler; } }

        public IntPtr CaptureNative { get { return m_target_texture.Native; } }


        private static CaptureTargetTexture m_Instance = null;

        public static CaptureTargetTexture Instance { get { if (m_Instance == null) m_Instance = new CaptureTargetTexture(); return m_Instance; } }

        private CaptureTargetTexture()
        {
            try
            {
                init();
            }
            catch (Exception)
            {
            }
        }

        ~CaptureTargetTexture()
        {
            if (m_device != null)
                m_device.Dispose();

            if (m_target_texture != null)
                m_target_texture.Dispose();

            if (m_resource != null)
                m_resource.Dispose();            
        }

        private void init()
        {

            m_device = Interop.Direct3D11.Create();

            NativeStructs.D3D11_TEXTURE2D_DESC lTextureDesc = new NativeStructs.D3D11_TEXTURE2D_DESC();
                       

            lTextureDesc.Width = Panels.VideoPanel.WIDTH;

            lTextureDesc.Height = Panels.VideoPanel.HEIGHT;

            lTextureDesc.Format = NativeStructs.DXGI_FORMAT_B8G8R8A8_UNORM;

            lTextureDesc.ArraySize = 1;

            lTextureDesc.BindFlags = NativeStructs.D3D11_BIND_RENDER_TARGET;

            lTextureDesc.MiscFlags = NativeStructs.D3D11_RESOURCE_MISC_SHARED;

            lTextureDesc.SampleDesc = new NativeStructs.DXGI_SAMPLE_DESC();

            lTextureDesc.SampleDesc.Count = 1;

            lTextureDesc.SampleDesc.Quality = 0;

            lTextureDesc.MipLevels = 1;

            lTextureDesc.CPUAccessFlags = 0;

            lTextureDesc.Usage = NativeStructs.D3D11_USAGE_DEFAULT;

            m_target_texture = m_device.CreateTexture2D(lTextureDesc);

            m_resource = m_target_texture.getDXGIResource();

            m_shared_handler = m_resource.GetSharedHandle();
        }
    }
}
