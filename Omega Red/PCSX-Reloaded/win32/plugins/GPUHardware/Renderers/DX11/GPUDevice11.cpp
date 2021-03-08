#include "stdafx.h"
#include "GPUDevice11.h"
#include <DirectXMath.h>
#include "./VertexShader.h"
#include "./PixelShader.h"
#include "./PixelShaderSharpen.h"
#include "./PixelShaderNoneTextured.h"


#pragma comment(lib, "d3d11.lib")
#pragma comment(lib, "dxgi.lib")

extern "C" void OutputLog(const char *szFormat, ...);

// {85A27140-AC18-4F27-AAFF-779FBF3E78E3}
static const GUID VERTICAL_FLIP_IID =
    {0x85a27140, 0xac18, 0x4f27, {0xaa, 0xff, 0x77, 0x9f, 0xbf, 0x3e, 0x78, 0xe3}};

extern "C" int iResX;
extern "C" int iResY;


typedef struct _VERTEX
{
    DirectX::XMFLOAT4 Pos;
    DirectX::XMFLOAT2 TexCoord;
    UINT32 Color;
} VERTEX;

typedef struct _PROJECTION_BUFFER
{
    DirectX::XMMATRIX mProj;
} PROJECTION_BUFFER;

typedef struct _ALPHA_FUNC_BUFFER
{
    UINT32 mFunc;
    FLOAT mValue;
    FLOAT padding1;
    FLOAT padding2;
} ALPHA_FUNC_BUFFER;

#define NUMVERTICES 4

VERTEX g_vertexes[NUMVERTICES];

PROJECTION_BUFFER g_projection_buffer;

const size_t g_quad_vertextSize = sizeof(VERTEX) * 4;

const size_t g_triangle_vertextSize = sizeof(VERTEX) * 3;


GPUDevice11::GPUDevice11()
{
    m_mipmap = 1;
    m_upscale_multiplier = 1;
    m_IsTextured = FALSE;
    m_fxaa = FALSE;
}

GPUDevice11::~GPUDevice11()
{
}

bool GPUDevice11::Create(const std::shared_ptr<GSWnd> &wnd, void *sharedhandle, void *capturehandle, void *directXDeviceNative)
{
    bool nvidia_vendor = false;

    if (!__super::Create(wnd)) {
        return false;
    }

    HRESULT hr = E_FAIL;



    D3D11_BUFFER_DESC bd;
    D3D11_RASTERIZER_DESC rd;
    D3D11_BLEND_DESC bsd;

    CComPtrCustom<IDXGIAdapter1> adapter;
    D3D_DRIVER_TYPE driver_type = D3D_DRIVER_TYPE_HARDWARE;

    CComPtrCustom<IDXGIFactory1> dxgi_factory;
    CreateDXGIFactory1(__uuidof(IDXGIFactory1), (void **)&dxgi_factory);
    if (dxgi_factory)
        for (int i = 0;; i++) {
            CComPtrCustom<IDXGIAdapter1> enum_adapter;
            if (S_OK != dxgi_factory->EnumAdapters1(i, &enum_adapter))
                break;
            DXGI_ADAPTER_DESC1 desc;
            hr = enum_adapter->GetDesc1(&desc);
            if (S_OK == hr) {
                if (desc.VendorId == 0x10DE)
                    nvidia_vendor = true;

                adapter = enum_adapter;
                driver_type = D3D_DRIVER_TYPE_UNKNOWN;
                break;
            }
        }

    // NOTE : D3D11_CREATE_DEVICE_SINGLETHREADED
    //   This flag is safe as long as the DXGI's internal message pump is disabled or is on the
    //   same thread as the GS window (which the emulator makes sure of, if it utilizes a
    //   multithreaded GS).  Setting the flag is a nice and easy 5% speedup on GS-intensive scenes.

    uint32 flags = D3D11_CREATE_DEVICE_SINGLETHREADED;

    flags |= D3D11_CREATE_DEVICE_BGRA_SUPPORT;

#ifdef DEBUG
    flags |= D3D11_CREATE_DEVICE_DEBUG;
#endif

    D3D_FEATURE_LEVEL level;

    const D3D_FEATURE_LEVEL levels[] =
        {
            D3D_FEATURE_LEVEL_11_0,
            D3D_FEATURE_LEVEL_10_1,
            D3D_FEATURE_LEVEL_10_0,
        };

    hr = D3D11CreateDevice(adapter, driver_type, NULL, flags, levels, countof(levels), D3D11_SDK_VERSION, &m_dev, &level, &m_ctx);

    if (FAILED(hr))
        return false;

    // Set maximum texture size limit based on supported feature level.
    if (level >= D3D_FEATURE_LEVEL_11_0)
        m_d3d_texsize = D3D11_REQ_TEXTURE2D_U_OR_V_DIMENSION;
    else
        m_d3d_texsize = D3D10_REQ_TEXTURE2D_U_OR_V_DIMENSION;

    { // HACK: check nVIDIA
        // Note: It can cause issues on several games such as SOTC, Fatal Frame, plus it adds border offset.
        bool disable_safe_features = 0; // theApp.GetConfigB("UserHacks") && theApp.GetConfigB("UserHacks_Disable_Safe_Features");
        m_hack_topleft_offset = (m_upscale_multiplier != 1 && nvidia_vendor && !disable_safe_features) ? -0.01f : 0.0f;
    }


    // Create shared texture

    CComPtr<ID3D11Resource> l_Resource;

    hr = m_dev->OpenSharedResource(sharedhandle, IID_PPV_ARGS(&l_Resource));

    if (FAILED(hr))
        return false;

    hr = l_Resource->QueryInterface(IID_PPV_ARGS(&m_SharedTexture));

    if (FAILED(hr))
        return false;

    D3D11_TEXTURE2D_DESC l_Desc;

    m_SharedTexture->GetDesc(&l_Desc);

    m_Height = l_Desc.Height;

    m_Width = (float)m_Height * 4.0f / 3.0f;

    m_Xoffset = (l_Desc.Width - m_Width) >> 1;

	iResX = m_Width;

    iResY = m_Height; 

    l_Desc.Width = m_Width;

    m_RenderTargetTexture.Release();

    hr = m_dev->CreateTexture2D(&l_Desc, nullptr, &m_RenderTargetTexture);

    if (FAILED(hr))
        return false;

    l_Resource.Release();

    hr = m_dev->OpenSharedResource(capturehandle, IID_PPV_ARGS(&l_Resource));

    if (FAILED(hr))
        return false;

    hr = l_Resource->QueryInterface(IID_PPV_ARGS(&m_CaptureTexture));

    if (FAILED(hr))
        return false;


    CComPtrCustom<ID3D11RenderTargetView> l_RTV;

    m_dev->CreateRenderTargetView(m_SharedTexture, nullptr, &l_RTV);

    FLOAT l_clearColor[4] = {0.0f, 0.0f, 0.0f, 1.0f};

    m_ctx->ClearRenderTargetView(l_RTV, l_clearColor);

    l_RTV.Release();

    //m_dev->CreateRenderTargetView(m_CaptureTexture, nullptr, &l_RTV);

    //m_ctx->ClearRenderTargetView(l_RTV, l_clearColor);



    // VERTEX shader
    UINT Size = ARRAYSIZE(g_VS);
    hr = m_dev->CreateVertexShader(g_VS, Size, nullptr, &m_VertexShader);

    if (FAILED(hr))
        return false;

    // Input layout
    D3D11_INPUT_ELEMENT_DESC Layout[] =
        {
            {"POSITION", 0, DXGI_FORMAT_R32G32B32A32_FLOAT, 0, 0, D3D11_INPUT_PER_VERTEX_DATA, 0},
            {"TEXCOORD", 0, DXGI_FORMAT_R32G32_FLOAT, 0, 16, D3D11_INPUT_PER_VERTEX_DATA, 0},
            {"COLOR", 0, DXGI_FORMAT_R8G8B8A8_UNORM, 0, 24, D3D11_INPUT_PER_VERTEX_DATA, 0}};

    UINT NumElements = ARRAYSIZE(Layout);

    hr = m_dev->CreateInputLayout(Layout, NumElements, g_VS, Size, &m_InputLayout);

    if (FAILED(hr))
        return false;

    m_ctx->IASetInputLayout(m_InputLayout);

    // Simple pixel shader
    Size = ARRAYSIZE(g_PS);

    hr = m_dev->CreatePixelShader(g_PS, Size, nullptr, &m_SimplePixelShader);

    if (FAILED(hr))
        return false;

	

    // Simple pixel shader
    Size = ARRAYSIZE(g_PS_sharpen);

    hr = m_dev->CreatePixelShader(g_PS_sharpen, Size, nullptr, &m_SimplePixelShader_sharpen);

    if (FAILED(hr))
        return false;
	



    // Simple pixel shader
    Size = ARRAYSIZE(g_PS_none_textured);

    hr = m_dev->CreatePixelShader(g_PS_none_textured, Size, nullptr, &m_SimplePixelShader_none_textured);

    if (FAILED(hr))
        return false;

	m_SimplePixelShader_textured = m_SimplePixelShader;

    // Set up sampler
    CComPtrCustom<ID3D11SamplerState> l_SamplerState;
    D3D11_SAMPLER_DESC SampDesc;	   
    RtlZeroMemory(&SampDesc, sizeof(SampDesc));
    SampDesc.Filter = D3D11_FILTER_MIN_MAG_MIP_LINEAR;
    SampDesc.AddressU = D3D11_TEXTURE_ADDRESS_CLAMP;
    SampDesc.AddressV = D3D11_TEXTURE_ADDRESS_CLAMP;
    SampDesc.AddressW = D3D11_TEXTURE_ADDRESS_CLAMP;
    SampDesc.ComparisonFunc = D3D11_COMPARISON_NEVER;
    SampDesc.MinLOD = 0;
    SampDesc.MaxLOD = D3D11_FLOAT32_MAX;

    hr = m_dev->CreateSamplerState(&SampDesc, &l_SamplerState);

    if (FAILED(hr))
        return false;

    m_ctx->PSSetSamplers(0, 1, &l_SamplerState);

    l_SamplerState.Release();




    RtlZeroMemory(&SampDesc, sizeof(SampDesc));
    SampDesc.Filter = D3D11_FILTER_MIN_MAG_MIP_POINT;
    SampDesc.AddressU = D3D11_TEXTURE_ADDRESS_CLAMP;
    SampDesc.AddressV = D3D11_TEXTURE_ADDRESS_CLAMP;
    SampDesc.AddressW = D3D11_TEXTURE_ADDRESS_CLAMP;
    SampDesc.ComparisonFunc = D3D11_COMPARISON_NEVER;
    SampDesc.MinLOD = 0;
    SampDesc.MaxLOD = D3D11_FLOAT32_MAX;

    hr = m_dev->CreateSamplerState(&SampDesc, &l_SamplerState);

    if (FAILED(hr))
        return false;

    m_ctx->PSSetSamplers(1, 1, &l_SamplerState);

	l_SamplerState.Release();



    memset(&rd, 0, sizeof(rd));

    rd.FillMode = D3D11_FILL_SOLID;
    rd.CullMode = D3D11_CULL_NONE;

    rd.ScissorEnable = TRUE;

    hr = m_dev->CreateRasterizerState(&rd, &m_RasterizerState);

    m_ctx->RSSetState(m_RasterizerState);



    g_vertexes[0].Pos.x = -205;
    g_vertexes[0].Pos.y = -205;
    g_vertexes[0].Pos.z = 0;

    g_vertexes[1].Pos.x = -205;
    g_vertexes[1].Pos.y = 205;
    g_vertexes[1].Pos.z = 0;

    g_vertexes[2].Pos.x = 205;
    g_vertexes[2].Pos.y = -205;
    g_vertexes[2].Pos.z = 0;

    g_vertexes[3].Pos.x = 205;
    g_vertexes[3].Pos.y = 205;
    g_vertexes[3].Pos.z = 0;



    g_vertexes[0].Pos.w = 1;

    g_vertexes[1].Pos.w = 1;

    g_vertexes[2].Pos.w = 1;

    g_vertexes[3].Pos.w = 1;



    m_color_blend_op = D3D11_BLEND_OP::D3D11_BLEND_OP_ADD;
    m_alpha_blend_op = D3D11_BLEND_OP::D3D11_BLEND_OP_ADD;



    m_ctx->VSSetShader(m_VertexShader, nullptr, 0);
    m_ctx->PSSetShader(m_SimplePixelShader_textured, nullptr, 0);




    // Create space for vertices for the dirty rects if the current space isn't large enough
    UINT BytesNeeded = sizeof(VERTEX) * NUMVERTICES;


    // Create vertex buffer
    D3D11_BUFFER_DESC BufferDesc;
    RtlZeroMemory(&BufferDesc, sizeof(BufferDesc));
    BufferDesc.ByteWidth = BytesNeeded;
    BufferDesc.Usage = D3D11_USAGE_DEFAULT;
    BufferDesc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
    BufferDesc.CPUAccessFlags = 0;



    D3D11_SUBRESOURCE_DATA InitData;
    RtlZeroMemory(&InitData, sizeof(InitData));
    InitData.pSysMem = g_vertexes;

    hr = m_dev->CreateBuffer(&BufferDesc, &InitData, &m_DrawVertexBuffer);


    if (FAILED(hr))
        return false;

    UINT Stride = sizeof(VERTEX);
    UINT Offset = 0;
    m_ctx->IASetVertexBuffers(0, 1, &m_DrawVertexBuffer, &Stride, &Offset);



    m_VP.Width = static_cast<FLOAT>(m_Width);
    m_VP.Height = static_cast<FLOAT>(m_Height);

    m_VP.MinDepth = 0.0f;
    m_VP.MaxDepth = 1.0f;
    m_VP.TopLeftX = static_cast<FLOAT>(0);
    m_VP.TopLeftY = 0.0f;
    m_ctx->RSSetViewports(1, &m_VP);

    Reset(m_Width, m_Height);


    D3D11_BLEND_DESC blendDesc;
    ZeroMemory(&blendDesc, sizeof(blendDesc));

    D3D11_RENDER_TARGET_BLEND_DESC rtbd;
    ZeroMemory(&rtbd, sizeof(rtbd));

    rtbd.BlendEnable = FALSE;
    rtbd.SrcBlend = D3D11_BLEND::D3D11_BLEND_ONE;
    rtbd.DestBlend = D3D11_BLEND::D3D11_BLEND_ZERO;
    rtbd.BlendOp = D3D11_BLEND_OP::D3D11_BLEND_OP_ADD;
    rtbd.SrcBlendAlpha = D3D11_BLEND::D3D11_BLEND_ONE;
    rtbd.DestBlendAlpha = D3D11_BLEND::D3D11_BLEND_ZERO;
    rtbd.BlendOpAlpha = D3D11_BLEND_OP::D3D11_BLEND_OP_ADD;
    rtbd.RenderTargetWriteMask = D3D11_COLOR_WRITE_ENABLE::D3D11_COLOR_WRITE_ENABLE_ALL;

    blendDesc.RenderTarget[0] = rtbd;

    m_disableBlending.Release();

    m_dev->CreateBlendState(&blendDesc, &m_disableBlending);

    float l_BlendFactor[4] = {1.0f, 1.0f, 1.0f, 1.0f};
    m_ctx->OMSetBlendState(m_disableBlending, l_BlendFactor, 0xffffffff);



    ALPHA_FUNC_BUFFER l_ALPHA_FUNC_BUFFER;

    l_ALPHA_FUNC_BUFFER.mFunc = 0.0f;

    l_ALPHA_FUNC_BUFFER.mValue = 0.0f;

    l_ALPHA_FUNC_BUFFER.padding1 = 0.0f;

    l_ALPHA_FUNC_BUFFER.padding2 = 0.0f;


    // Fill in a buffer description.
    D3D11_BUFFER_DESC cbDesc;
    cbDesc.ByteWidth = sizeof(ALPHA_FUNC_BUFFER);
    cbDesc.Usage = D3D11_USAGE_DEFAULT;
    cbDesc.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
    cbDesc.CPUAccessFlags = 0;
    cbDesc.MiscFlags = 0;
    cbDesc.StructureByteStride = 0;

    RtlZeroMemory(&InitData, sizeof(InitData));
    InitData.pSysMem = &l_ALPHA_FUNC_BUFFER;


    m_alpha_fun_buffer.Release();

    // Create the buffer.
    m_dev->CreateBuffer(&cbDesc, &InitData,
                        &m_alpha_fun_buffer);

    m_ctx->PSSetConstantBuffers(0, 1, &m_alpha_fun_buffer);
	
    return true;
}

void GPUDevice11::Flip()
{
    LOG_CHECK_PTR_MEMORY(m_ctx);

    m_ctx->Flush();

    m_ctx->CopySubresourceRegion(m_SharedTexture,
                                 0,
                                 m_Xoffset, 0,
                                 0,
                                 m_RenderTargetTexture,
                                 0,
                                 nullptr);

    m_ctx->CopySubresourceRegion(m_CaptureTexture,
                                 0,
                                 m_Xoffset, 0,
                                 0,
                                 m_RenderTargetTexture,
                                 0,
                                 nullptr);
}

void GPUDevice11::ClearTargets()
{
    LOG_CHECK_PTR_MEMORY(m_ctx);

    ClearRenderTarget();

    Flip();

    m_ctx->Flush();
}

void GPUDevice11::ClearRenderTarget()
{
    LOG_CHECK_PTR_MEMORY(m_DSTV);

    LOG_CHECK_PTR_MEMORY(m_ctx);

#ifdef DEBUG
    FLOAT l_clearColor[4] = {1.0f, 0.0f, 1.0f, 1.0f};

    m_ctx->ClearRenderTargetView(m_RTV, l_clearColor);
#else
    m_ctx->ClearRenderTargetView(m_RTV, m_clearColor);
#endif // DEBUG
}

void GPUDevice11::ClearDepth()
{
    LOG_CHECK_PTR_MEMORY(m_DSTV);

    LOG_CHECK_PTR_MEMORY(m_ctx);

    m_ctx->ClearDepthStencilView(m_DSTV, D3D11_CLEAR_DEPTH, 0.0f, 0);
}

void GPUDevice11::Reset(int w, int h)
{
    if (m_RenderTargetTexture) {
        LOG_CHECK_PTR_MEMORY(m_RenderTargetTexture);

        m_RTV.Release();

        m_dev->CreateRenderTargetView(m_RenderTargetTexture, nullptr, &m_RTV);

        LOG_CHECK_PTR_MEMORY(m_RTV);



        D3D11_TEXTURE2D_DESC desc;

        memset(&desc, 0, sizeof(desc));

        // Texture limit for D3D10/11 min 1, max 8192 D3D10, max 16384 D3D11.
        desc.Width = std::max(1, std::min(w, m_d3d_texsize));
        desc.Height = std::max(1, std::min(h, m_d3d_texsize));
        desc.Format = (DXGI_FORMAT)DXGI_FORMAT::DXGI_FORMAT_D24_UNORM_S8_UINT;
        desc.MipLevels = 1;
        desc.ArraySize = 1;
        desc.SampleDesc.Count = 1;
        desc.SampleDesc.Quality = 0;
        desc.Usage = D3D11_USAGE_DEFAULT;
        desc.BindFlags = D3D11_BIND_DEPTH_STENCIL;

        CComPtrCustom<ID3D11Texture2D> texture;

        LOG_INVOKE(m_dev->CreateTexture2D(&desc, NULL, &texture));

        LOG_CHECK_PTR_MEMORY(texture);

        m_DSTV.Release();

        D3D11_DEPTH_STENCIL_VIEW_DESC dsvDesc;
        ZeroMemory(&dsvDesc, sizeof(dsvDesc));
        dsvDesc.Format = desc.Format;
        dsvDesc.ViewDimension = D3D11_DSV_DIMENSION_TEXTURE2DMS;

        m_dev->CreateDepthStencilView(texture, &dsvDesc, &m_DSTV);

        LOG_CHECK_PTR_MEMORY(m_DSTV);


        m_ctx->OMSetRenderTargets(1, &m_RTV, m_DSTV);



        D3D11_DEPTH_STENCIL_DESC dsd;

        ZeroMemory(&dsd, sizeof(D3D11_DEPTH_STENCIL_DESC));

        // DepthDefault

        dsd.DepthEnable = TRUE;

        dsd.DepthFunc = D3D11_COMPARISON_FUNC::D3D11_COMPARISON_ALWAYS;

        dsd.DepthWriteMask = D3D11_DEPTH_WRITE_MASK::D3D11_DEPTH_WRITE_MASK_ALL;

        m_dev->CreateDepthStencilState(&dsd, &m_DepthStencilState);

        LOG_CHECK_PTR_MEMORY(m_DepthStencilState);

        m_ctx->OMSetDepthStencilState(m_DepthStencilState, 0);
    }
}

void GPUDevice11::Present()
{
    Flip();
}

void GPUDevice11::SetExclusive(bool isExcl) {}

void GPUDevice11::setClearColor(float aRed, float aGreen, float aBlue, float aAlpha)
{
    m_clearColor[0] = aRed;
    m_clearColor[1] = aGreen;
    m_clearColor[2] = aBlue;
    m_clearColor[3] = aAlpha;
}

void GPUDevice11::setOrtho(double aLeft, double aRight, double aBottom, double aTop, double azNear, double azFar)
{
    LOG_CHECK_PTR_MEMORY(m_dev);

    LOG_CHECK_PTR_MEMORY(m_ctx);

    if (aBottom == aTop)
        return;

    if (aLeft == aRight)
        return;

    if (azNear == azFar)
        return;

    DirectX::XMMATRIX lProj = DirectX::XMMatrixOrthographicOffCenterLH(aLeft, aRight, aBottom, aTop, azNear, azFar);

    g_projection_buffer.mProj = DirectX::XMMatrixTranspose(lProj);

    // Fill in a buffer description.
    D3D11_BUFFER_DESC cbDesc;
    cbDesc.ByteWidth = sizeof(PROJECTION_BUFFER);
    cbDesc.Usage = D3D11_USAGE_DYNAMIC;
    cbDesc.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
    cbDesc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
    cbDesc.MiscFlags = 0;
    cbDesc.StructureByteStride = 0;

    D3D11_SUBRESOURCE_DATA InitData;
    RtlZeroMemory(&InitData, sizeof(InitData));
    InitData.pSysMem = &g_projection_buffer;


    m_directx_projection_buffer.Release();

    // Create the buffer.
    LOG_INVOKE(m_dev->CreateBuffer(&cbDesc, &InitData,
                                   &m_directx_projection_buffer));

    m_ctx->VSSetConstantBuffers(0, 1, &m_directx_projection_buffer);
}

void GPUDevice11::draw(DWORD aDrawMode, void *aPtrVertexes, BOOL aIsTextured)
{
    LOG_CHECK_PTR_MEMORY(m_ctx);

    if (m_IsTextured != aIsTextured) {
        if (aIsTextured == FALSE)
            m_ctx->PSSetShader(m_SimplePixelShader_none_textured, nullptr, 0);
        else
            m_ctx->PSSetShader(m_SimplePixelShader_textured, nullptr, 0);
    }


    m_IsTextured = aIsTextured;

    size_t l_vertextSize = 0;
    size_t l_vertextCount = 0;

    switch (aDrawMode) {
        case GL_TRIANGLE_STRIP:
        case GL_QUADS: {
            l_vertextSize = g_quad_vertextSize;

            l_vertextCount = 4;

            m_ctx->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP);
        } break;
        case GL_TRIANGLES: {
            l_vertextSize = g_triangle_vertextSize;

            l_vertextCount = 3;

            m_ctx->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
        } break;


        default:
            return;
    }

    VERTEX *l_PtrVERTEX = (VERTEX *)aPtrVertexes;

    if (m_vertical_flip_state != FALSE) {
        if (l_vertextCount == 4) {
            auto l_temp_TexCoord_y = l_PtrVERTEX[0].TexCoord.y;

            l_PtrVERTEX[0].TexCoord.y = l_PtrVERTEX[2].TexCoord.y;

            l_PtrVERTEX[2].TexCoord.y = l_temp_TexCoord_y;



            l_temp_TexCoord_y = l_PtrVERTEX[1].TexCoord.y;

            l_PtrVERTEX[1].TexCoord.y = l_PtrVERTEX[3].TexCoord.y;

            l_PtrVERTEX[3].TexCoord.y = l_temp_TexCoord_y;

        } else if (l_vertextCount == 3) {
            auto l_temp_TexCoord = l_PtrVERTEX[0].TexCoord;

            l_PtrVERTEX[0].TexCoord = l_PtrVERTEX[1].TexCoord;

            l_PtrVERTEX[1].TexCoord = l_temp_TexCoord;
        }
    }
	   
    INT32 l_sourceRowPitch = l_vertextCount * sizeof(VERTEX);

    m_ctx->UpdateSubresource(
        m_DrawVertexBuffer,
        0,
        nullptr,
        aPtrVertexes,
        l_sourceRowPitch,
        0);

    m_ctx->Draw(l_vertextCount, 0);
}

void GPUDevice11::setBlendState(ID3D11BlendState *aPtrBlendState)
{
    LOG_CHECK_PTR_MEMORY(m_ctx);

    float l_BlendFactor[4] = {1.0f, 1.0f, 1.0f, 1.0f};

    m_ctx->OMSetBlendState(aPtrBlendState, l_BlendFactor, 0xffffffff);
}

void GPUDevice11::enableBlend(BOOL aEnable)
{
    LOG_CHECK_PTR_MEMORY(m_ctx);

    if (aEnable == FALSE) {
        setBlendState(m_disableBlending);
    } else {
        setBlendState(m_enableBlending);
    }

    m_blend_enabled = aEnable;
}

void GPUDevice11::setBlendFunc(DWORD aSfactor, DWORD aDfactor)
{
    LOG_CHECK_PTR_MEMORY(m_ctx);

    D3D11_BLEND_DESC blendDesc;
    ZeroMemory(&blendDesc, sizeof(blendDesc));

    D3D11_RENDER_TARGET_BLEND_DESC rtbd;
    ZeroMemory(&rtbd, sizeof(rtbd));

    rtbd.BlendEnable = TRUE;
    rtbd.SrcBlend = D3D11_BLEND::D3D11_BLEND_SRC_ALPHA;
    rtbd.DestBlend = D3D11_BLEND::D3D11_BLEND_INV_SRC_ALPHA;
    rtbd.BlendOp = m_color_blend_op;
    rtbd.SrcBlendAlpha = D3D11_BLEND::D3D11_BLEND_ZERO;
    rtbd.DestBlendAlpha = D3D11_BLEND::D3D11_BLEND_ZERO;
    rtbd.BlendOpAlpha = m_alpha_blend_op;
    rtbd.RenderTargetWriteMask = D3D11_COLOR_WRITE_ENABLE::D3D11_COLOR_WRITE_ENABLE_ALL;



    if (aSfactor == GL_ZERO) {
        rtbd.SrcBlend = D3D11_BLEND::D3D11_BLEND_ZERO;
        rtbd.SrcBlendAlpha = D3D11_BLEND::D3D11_BLEND_ZERO;
    } else if (aSfactor == GL_ONE) {
        rtbd.SrcBlend = D3D11_BLEND::D3D11_BLEND_ONE;
        rtbd.SrcBlendAlpha = D3D11_BLEND::D3D11_BLEND_ONE;
    } else if (aSfactor == GL_SRC_ALPHA) {
        rtbd.SrcBlend = D3D11_BLEND::D3D11_BLEND_SRC_ALPHA;
        rtbd.SrcBlendAlpha = D3D11_BLEND::D3D11_BLEND_SRC_ALPHA;
    } else if (aSfactor == GL_ONE_MINUS_SRC_ALPHA) {
        rtbd.SrcBlend = D3D11_BLEND::D3D11_BLEND_INV_SRC_ALPHA;
        rtbd.SrcBlendAlpha = D3D11_BLEND::D3D11_BLEND_INV_SRC_ALPHA;
    } else if (aSfactor == GL_ONE_MINUS_SRC_COLOR) {
        rtbd.SrcBlend = D3D11_BLEND::D3D11_BLEND_INV_SRC_COLOR;
        rtbd.SrcBlendAlpha = D3D11_BLEND::D3D11_BLEND_INV_SRC_COLOR;
    } else {
        rtbd.SrcBlend = D3D11_BLEND::D3D11_BLEND_SRC_ALPHA;
        rtbd.SrcBlendAlpha = D3D11_BLEND::D3D11_BLEND_ZERO;
    }



    if (aDfactor == GL_ZERO) {
        rtbd.DestBlend = D3D11_BLEND::D3D11_BLEND_ZERO;
        rtbd.DestBlendAlpha = D3D11_BLEND::D3D11_BLEND_ZERO;
    } else if (aDfactor == GL_ONE) {
        rtbd.DestBlend = D3D11_BLEND::D3D11_BLEND_ONE;
        rtbd.DestBlendAlpha = D3D11_BLEND::D3D11_BLEND_ZERO;
    } else if (aDfactor == GL_SRC_ALPHA) {
        rtbd.DestBlend = D3D11_BLEND::D3D11_BLEND_SRC_ALPHA;
        rtbd.DestBlendAlpha = D3D11_BLEND::D3D11_BLEND_ZERO;
    } else if (aDfactor == GL_ONE_MINUS_SRC_ALPHA) {
        rtbd.DestBlend = D3D11_BLEND::D3D11_BLEND_INV_SRC_ALPHA;
        rtbd.DestBlendAlpha = D3D11_BLEND::D3D11_BLEND_ZERO;
    } else if (aDfactor == GL_ONE_MINUS_SRC_COLOR) {
        rtbd.DestBlend = D3D11_BLEND::D3D11_BLEND_INV_SRC_COLOR;
        rtbd.DestBlendAlpha = D3D11_BLEND::D3D11_BLEND_ZERO;
    } else {
        rtbd.DestBlend = D3D11_BLEND::D3D11_BLEND_INV_SRC_ALPHA;
        rtbd.DestBlendAlpha = D3D11_BLEND::D3D11_BLEND_ZERO;
    }



    blendDesc.AlphaToCoverageEnable = false;
    blendDesc.RenderTarget[0] = rtbd;

    m_enableBlending.Release();

    LOG_INVOKE(m_dev->CreateBlendState(&blendDesc, &m_enableBlending));

    LOG_CHECK_PTR_MEMORY(m_enableBlending);

    enableBlend(m_blend_enabled);
}

void GPUDevice11::setBlendEquation(DWORD aOPcode)
{
    if (aOPcode == FUNC_REVERSE_SUBTRACT_EXT) {
        m_color_blend_op = D3D11_BLEND_OP::D3D11_BLEND_OP_REV_SUBTRACT;

        m_alpha_blend_op = D3D11_BLEND_OP::D3D11_BLEND_OP_REV_SUBTRACT;
    }

    if (aOPcode == FUNC_ADD_EXT) {
        m_color_blend_op = D3D11_BLEND_OP::D3D11_BLEND_OP_ADD;

        m_alpha_blend_op = D3D11_BLEND_OP::D3D11_BLEND_OP_ADD;
    }
}

void GPUDevice11::createTexture(
    INT32 aInternalformat,
    INT32 aWidth,
    INT32 aHeight,
    INT32 aBorder,
    DWORD aFormat,
    DWORD aType,
    const void *aPtrPixels,
    IUnknown **aPtrPtrGPUTexture,
    IUnknown **aPtrPtrUnkShaderResourceView)
{
    LOG_CHECK_PTR_MEMORY(m_dev);

    LOG_CHECK_PTR_MEMORY(aPtrPtrGPUTexture);


    if (m_TexturePackMode == TexturePackMode::LOAD) {
        if (aFormat == GL_BGRA_EXT)
            aFormat = GL_RGBA;

        if (aFormat == GL_BGR_EXT)
            aFormat = GL_RGB;
    }


    UINT32 l_source_pixel_bytes = 4;

    DXGI_FORMAT l_format = DXGI_FORMAT::DXGI_FORMAT_R8G8B8A8_UNORM;

    if (aInternalformat == 3) {
        if (aFormat == GL_RGB)
            l_format = DXGI_FORMAT::DXGI_FORMAT_R8G8B8A8_UNORM;
        if (aFormat == GL_BGR_EXT)
            l_format = DXGI_FORMAT::DXGI_FORMAT_B8G8R8A8_UNORM;

    } else if (aInternalformat == 4) {
        if (aFormat == GL_RGBA)
            l_format = DXGI_FORMAT::DXGI_FORMAT_R8G8B8A8_UNORM;
        if (aFormat == GL_BGRA_EXT)
            l_format = DXGI_FORMAT::DXGI_FORMAT_B8G8R8A8_UNORM;
    }


    if (aInternalformat == GL_RGB5_A1) {
        l_format = DXGI_FORMAT::DXGI_FORMAT_B5G5R5A1_UNORM;

        l_source_pixel_bytes = 2;
    } else if (aInternalformat == GL_RGB)
        l_format = DXGI_FORMAT::DXGI_FORMAT_B8G8R8X8_UNORM;
    else if (aInternalformat == GL_RGBA8) {
        if (aFormat == GL_BGR_EXT)
            l_format = DXGI_FORMAT::DXGI_FORMAT_B8G8R8X8_UNORM;
        else if (aFormat == GL_BGRA_EXT)
            l_format = DXGI_FORMAT::DXGI_FORMAT_B8G8R8A8_UNORM;
        else
            l_format = DXGI_FORMAT::DXGI_FORMAT_R8G8B8A8_UNORM;
    }



    D3D11_TEXTURE2D_DESC l_Desc;


    l_Desc.Width = aWidth;
    l_Desc.Height = aHeight;
    l_Desc.MipLevels = 1;
    l_Desc.ArraySize = 1;
    l_Desc.Format = l_format;
    l_Desc.SampleDesc.Count = 1;
    l_Desc.SampleDesc.Quality = 0;
    l_Desc.Usage = D3D11_USAGE_DEFAULT;
    l_Desc.BindFlags = D3D11_BIND_SHADER_RESOURCE;
    l_Desc.CPUAccessFlags = 0;
    l_Desc.MiscFlags = 0;

    CComPtrCustom<ID3D11Texture2D> l_Texture;

    if (aPtrPixels == nullptr) {
        LOG_INVOKE(m_dev->CreateTexture2D(&l_Desc, nullptr, &l_Texture));
    } else {

        D3D11_SUBRESOURCE_DATA l_subresourceResource;
        ZeroMemory(&l_subresourceResource, sizeof(D3D11_SUBRESOURCE_DATA));

        l_subresourceResource.pSysMem = aPtrPixels;
        l_subresourceResource.SysMemPitch = aWidth * l_source_pixel_bytes;

        LOG_INVOKE(m_dev->CreateTexture2D(&l_Desc, &l_subresourceResource, &l_Texture));
    }

    LOG_INVOKE_QUERY_INTERFACE(l_Texture, aPtrPtrGPUTexture);


    if (aPtrPtrUnkShaderResourceView != nullptr) {

        CComPtrCustom<ID3D11ShaderResourceView> l_CurrentTextureShaderResource;


        D3D11_SHADER_RESOURCE_VIEW_DESC ShaderDesc;
        ShaderDesc.Format = l_Desc.Format;
        ShaderDesc.ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2D;
        ShaderDesc.Texture2D.MostDetailedMip = l_Desc.MipLevels - 1;
        ShaderDesc.Texture2D.MipLevels = l_Desc.MipLevels;

        LOG_INVOKE(m_dev->CreateShaderResourceView(l_Texture, &ShaderDesc, &l_CurrentTextureShaderResource));

        LOG_INVOKE_QUERY_INTERFACE(l_CurrentTextureShaderResource, aPtrPtrUnkShaderResourceView);
    }
}

void GPUDevice11::updateTexture(IUnknown *aPtrGPUUnkTexture, INT32 aXoffset, INT32 aYoffset, INT32 aWidth, INT32 aHeight, DWORD aFormat, DWORD aType, const void *aPtrPixels)
{
    LOG_CHECK_PTR_MEMORY(m_dev);

    LOG_CHECK_PTR_MEMORY(m_ctx);

    LOG_CHECK_PTR_MEMORY(aPtrGPUUnkTexture);

    CComPtrCustom<ID3D11Texture2D> l_GPUTexture;

    LOG_INVOKE_QUERY_INTERFACE(aPtrGPUUnkTexture, &l_GPUTexture);

    LOG_CHECK_PTR_MEMORY(l_GPUTexture);


	UINT64 l_ID = 0;


    UINT32 l_source_pixel_bytes = 4;

    if ((aFormat == GL_RGB) || aFormat == GL_BGR_EXT)
        l_source_pixel_bytes = 3;

    if (aType == GL_UNSIGNED_SHORT_5_5_5_1_EXT)
        l_source_pixel_bytes = 2;
	

    D3D11_TEXTURE2D_DESC l_Desc;

    l_GPUTexture->GetDesc(&l_Desc);



    UINT32 l_dest_pixel_bytes = 4;

    if (l_Desc.Format == DXGI_FORMAT::DXGI_FORMAT_B5G5R5A1_UNORM)
        l_dest_pixel_bytes = 2;

    D3D11_BOX l_D3D11_BOX;

    l_D3D11_BOX.left = aXoffset;

    l_D3D11_BOX.right = aXoffset + aWidth;

    l_D3D11_BOX.top = aYoffset;

    l_D3D11_BOX.bottom = aYoffset + aHeight;

    l_D3D11_BOX.front = 0;

    l_D3D11_BOX.back = 1;

    INT32 l_sourceRowPitch = aWidth * l_source_pixel_bytes;

    m_ctx->UpdateSubresource(
        l_GPUTexture,
        0,
        &l_D3D11_BOX,
        aPtrPixels,
        l_sourceRowPitch,
        0);
}

void GPUDevice11::copyTexSubImage2D(IUnknown *aPtrUnkTexture, INT32 aXoffset, INT32 aYoffset, INT32 aX, INT32 aY, INT32 aWidth, INT32 aHeight)
{
    LOG_CHECK_PTR_MEMORY(m_ctx);

    LOG_CHECK_PTR_MEMORY(aPtrUnkTexture);

    CComPtrCustom<ID3D11Texture2D> l_Texture;

    LOG_INVOKE_QUERY_INTERFACE(aPtrUnkTexture, &l_Texture);

    LOG_CHECK_PTR_MEMORY(l_Texture);

    D3D11_TEXTURE2D_DESC l_Desc;

    l_Texture->GetDesc(&l_Desc);

    BOOL l_vertical_flip_state = TRUE;

    LOG_INVOKE(l_Texture->SetPrivateData(VERTICAL_FLIP_IID, sizeof(l_vertical_flip_state), &l_vertical_flip_state));



    D3D11_BOX l_D3D11_BOX;

    l_D3D11_BOX.left = aX;

    l_D3D11_BOX.right = aX + aWidth;

    l_D3D11_BOX.top = aY;

    l_D3D11_BOX.bottom = aY + aHeight;

    l_D3D11_BOX.front = 0;

    l_D3D11_BOX.back = 1;


    m_ctx->CopySubresourceRegion(l_Texture,
                                 0,
                                 aXoffset, aYoffset,
                                 0,
                                 m_RenderTargetTexture,
                                 0,
                                 &l_D3D11_BOX);
}

void GPUDevice11::setTexture(IUnknown *aPtrUnkTexture, IUnknown *aPtrPtrUnkShaderResourceView)
{
    LOG_CHECK_PTR_MEMORY(m_dev);

    LOG_CHECK_PTR_MEMORY(m_ctx);

    if (m_CurrentTextureShaderResource)
        m_CurrentTextureShaderResource.Release();

    if (aPtrUnkTexture == nullptr) {
        m_ctx->PSSetShaderResources(0, 0, nullptr);

        return;
    }

    LOG_CHECK_PTR_MEMORY(aPtrPtrUnkShaderResourceView);

    CComPtrCustom<ID3D11Texture2D> l_Texture;

    LOG_INVOKE_QUERY_INTERFACE(aPtrUnkTexture, &l_Texture);

    LOG_CHECK_PTR_MEMORY(l_Texture);

    LOG_INVOKE_QUERY_INTERFACE(aPtrPtrUnkShaderResourceView, &m_CurrentTextureShaderResource);

    LOG_CHECK_PTR_MEMORY(m_CurrentTextureShaderResource);

    m_ctx->PSSetShaderResources(0, 1, &m_CurrentTextureShaderResource);

    UINT32 l_state_size;

    m_vertical_flip_state = FALSE;

    auto lhr = l_Texture->GetPrivateData(VERTICAL_FLIP_IID, &l_state_size, nullptr);

    if (SUCCEEDED(lhr) && l_state_size == sizeof(UINT32)) {
        l_Texture->GetPrivateData(VERTICAL_FLIP_IID, &l_state_size, &m_vertical_flip_state);
    }
}

void GPUDevice11::setAlphaFunc(UINT32 aFunc, FLOAT aValue)
{
    LOG_CHECK_PTR_MEMORY(m_ctx);

    LOG_CHECK_PTR_MEMORY(m_alpha_fun_buffer);

    ALPHA_FUNC_BUFFER l_ALPHA_FUNC_BUFFER;

    l_ALPHA_FUNC_BUFFER.mFunc = aFunc;

    l_ALPHA_FUNC_BUFFER.mValue = aValue;

    l_ALPHA_FUNC_BUFFER.padding1 = 0.0f;

    l_ALPHA_FUNC_BUFFER.padding2 = 0.0f;


    INT32 l_sourceRowPitch = sizeof(ALPHA_FUNC_BUFFER);

    m_ctx->UpdateSubresource(
        m_alpha_fun_buffer,
        0,
        nullptr,
        &l_ALPHA_FUNC_BUFFER,
        l_sourceRowPitch,
        0);
}

void GPUDevice11::setDepthFunc(UINT32 aFunc)
{
    LOG_CHECK_PTR_MEMORY(m_dev);

    LOG_CHECK_PTR_MEMORY(m_ctx);

    LOG_CHECK_PTR_MEMORY(m_DepthStencilState);

    D3D11_DEPTH_STENCIL_DESC dsd;

    m_DepthStencilState->GetDesc(&dsd);

    if (aFunc == GL_LESS) {
        dsd.DepthFunc = D3D11_COMPARISON_FUNC::D3D11_COMPARISON_GREATER;
    } else if (aFunc == GL_ALWAYS) {
        dsd.DepthFunc = D3D11_COMPARISON_FUNC::D3D11_COMPARISON_ALWAYS;
    }

    m_DepthStencilState.Release();

    m_dev->CreateDepthStencilState(&dsd, &m_DepthStencilState);

    LOG_CHECK_PTR_MEMORY(m_DepthStencilState);

    m_ctx->OMSetDepthStencilState(m_DepthStencilState, 0);
}

void GPUDevice11::enableScissor(BOOL aEnable)
{
    LOG_CHECK_PTR_MEMORY(m_ctx);

    LOG_CHECK_PTR_MEMORY(m_RasterizerState);

    D3D11_RASTERIZER_DESC l_Desc;

    m_RasterizerState->GetDesc(&l_Desc);

    l_Desc.ScissorEnable = aEnable;

    m_RasterizerState.Release();

    m_dev->CreateRasterizerState(&l_Desc, &m_RasterizerState);

    LOG_CHECK_PTR_MEMORY(m_RasterizerState);

    m_ctx->RSSetState(m_RasterizerState);
}

void GPUDevice11::setScissor(INT32 aX, INT32 aY, INT32 aWidth, INT32 aHeight)
{
    LOG_CHECK_PTR_MEMORY(m_ctx);

    D3D11_RECT l_Scissor;

    l_Scissor.left = aX;

    l_Scissor.top = m_Height - aY - aHeight;

    l_Scissor.right = aX + aWidth;

    l_Scissor.bottom = m_Height - aY;

    m_ctx->RSSetScissorRects(1, &l_Scissor);
}

void GPUDevice11::setFXAA(BOOL a_value)
{
    if (m_fxaa != a_value)
	{
		if (a_value == FALSE)
		{
            m_SimplePixelShader_textured = m_SimplePixelShader;
		}
		else {
            m_SimplePixelShader_textured = m_SimplePixelShader_sharpen;
		}

		m_IsTextured = FALSE;
	}

	m_fxaa = a_value;
}

void GPUDevice11::setRawTexture(void *a_PtrMemory, const char *a_StringIDs){}