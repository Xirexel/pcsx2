using Omega_Red.Util.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.Capture
{
    class TargetTexture
    {
        private D3D11Texture2D m_target_texture = null;

        private DXGIResource m_resource = null;

        private IntPtr m_shared_handler = IntPtr.Zero;

        public IntPtr TargetHandler { get { return m_shared_handler; } }

        public IntPtr TargetNative { get { return m_target_texture.Native; } }
        

        private static TargetTexture m_Instance = null;

        public static TargetTexture Instance { get { if (m_Instance == null) m_Instance = new TargetTexture(); return m_Instance; } }

        private TargetTexture()
        {
        }

        ~TargetTexture()
        {
            if (m_target_texture != null)
                m_target_texture.Dispose();

            if (m_resource != null)
                m_resource.Dispose();

        }

        public void init()
        {
            try
            {
                Managers.ConfigManager.Instance.ResolutionEvent += (a_resolution) => {

                    var l_Width = (a_resolution * 16) / 9;

                    var l_Height = a_resolution;

                    init(l_Width, l_Height);
                };
            }
            catch (Exception)
            {
            }
        }

        private void init(uint a_Width, uint a_Height)
        {
            
            NativeStructs.D3D11_TEXTURE2D_DESC lTextureDesc = new NativeStructs.D3D11_TEXTURE2D_DESC();
                       

            lTextureDesc.Width = a_Width;

            lTextureDesc.Height = a_Height;

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

            if (m_target_texture != null)
                m_target_texture.Dispose();

            m_target_texture = null;

            m_target_texture = Util.Direct3D11Device.Instance.Device.CreateTexture2D(lTextureDesc);

            m_resource = m_target_texture.getDXGIResource();

            m_shared_handler = m_resource.GetSharedHandle();
        }
    }
}
