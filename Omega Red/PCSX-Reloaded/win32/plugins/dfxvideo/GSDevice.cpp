#include "GSDevice.h"

#pragma comment(lib, "DXGI.lib")
#pragma comment(lib, "D3D11.lib")



extern "C" {

#include "externals.h"
#include "gpu.h"

extern "C" int iFPSEInterface;
extern "C" PSXDisplay_t PreviousPSXDisplay;
extern "C" PSXDisplay_t PSXDisplay;
extern "C" unsigned short *psxVuw;

void (*BlitScreen)(D3D11_MAPPED_SUBRESOURCE *resource, long x, long y); // fill DDSRender surface
}

#include <DirectXMath.h>
#include "PixelShader.h"
#include "SmoothPixelShader.h"
#include "VertexShader.h"

typedef struct _VERTEX
{
    DirectX::XMFLOAT3 Pos;
    DirectX::XMFLOAT2 TexCoord;
} VERTEX;

#define NUMVERTICES 4
#define WIDTH 800.0f
#define HEIGHT 600.0f

VERTEX g_vertexes[NUMVERTICES];



PSXPoint_t m_CurrentDisplayMode;

bool operator!=(PSXPoint_t a, PSXPoint_t b)
{
    return a.x != b.x || a.y != b.y;
}

GSDevice g_GSDevice;


GSDevice::GSDevice() {}

GSDevice ::~GSDevice() {}

bool GSDevice::Create(void *sharedhandle, void *capturehandle, void *directXDeviceNative)
{
    bool l_result = false;

    do {
        bool nvidia_vendor = false;

        HRESULT hr = E_FAIL;

        D3D11_BUFFER_DESC bd;
        D3D11_SAMPLER_DESC sd;
        D3D11_DEPTH_STENCIL_DESC dsd;
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

        uint32 flags = D3D11_CREATE_DEVICE_BGRA_SUPPORT;

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


        // Create shared texture

        CComPtrCustom<ID3D11Resource> l_Resource;

        hr = m_dev->OpenSharedResource(sharedhandle, IID_PPV_ARGS(&l_Resource));

        if (FAILED(hr))
            return false;

        hr = l_Resource->QueryInterface(IID_PPV_ARGS(&m_SharedTexture));

        if (FAILED(hr))
            return false;

        D3D11_TEXTURE2D_DESC l_Desc;

        m_SharedTexture->GetDesc(&l_Desc);

        UINT l_Height = l_Desc.Height;

        UINT l_Width = (l_Height * 4) / 3;

        UINT l_XOffset = (l_Desc.Width - l_Width) >> 1;


        m_RawTargetTexture.Release();

        l_Desc.Width = WIDTH;

        l_Desc.Height = HEIGHT;

        l_Desc.ArraySize = 1;

        l_Desc.BindFlags = 0;

        l_Desc.MiscFlags = 0;

        l_Desc.SampleDesc.Count = 1;

        l_Desc.SampleDesc.Quality = 0;

        l_Desc.MipLevels = 1;

        l_Desc.CPUAccessFlags = D3D11_CPU_ACCESS_READ | D3D11_CPU_ACCESS_WRITE;

        l_Desc.Usage = D3D11_USAGE_STAGING;

        hr = m_dev->CreateTexture2D(&l_Desc, nullptr, &m_RawTargetTexture);

        if (FAILED(hr))
            return false;


        D3D11_TEXTURE2D_DESC l_ShaderTextureDesc;

        l_ShaderTextureDesc = l_Desc;

        l_ShaderTextureDesc.CPUAccessFlags = 0;

        l_ShaderTextureDesc.Usage = D3D11_USAGE_DEFAULT;

        l_ShaderTextureDesc.BindFlags = D3D11_BIND_SHADER_RESOURCE;

        hr = m_dev->CreateTexture2D(&l_ShaderTextureDesc, nullptr, &m_ShaderTexture);

        if (FAILED(hr))
            return false;

        m_RenderTargetTexture.Release();

        m_SharedTexture->GetDesc(&l_Desc);

        hr = m_dev->CreateTexture2D(&l_Desc, nullptr, &m_RenderTargetTexture);

        if (FAILED(hr))
            return false;

        hr = m_dev->CreateRenderTargetView(m_RenderTargetTexture, nullptr, &m_RTV);

        if (FAILED(hr))
            break;

        l_Resource.Release();

        hr = m_dev->OpenSharedResource(capturehandle, IID_PPV_ARGS(&l_Resource));

        if (FAILED(hr))
            return false;

        hr = l_Resource->QueryInterface(IID_PPV_ARGS(&m_CaptureTexture));

        if (FAILED(hr))
            return false;



        CComPtrCustom<ID3D11VertexShader> l_VertexShader;
        CComPtrCustom<ID3D11InputLayout> l_InputLayout;
        CComPtrCustom<ID3D11SamplerState> l_SamplerState;



        // VERTEX shader
        UINT Size = ARRAYSIZE(g_VS);
        hr = m_dev->CreateVertexShader(g_VS, Size, nullptr, &l_VertexShader);

        if (FAILED(hr))
            break;

        // Input layout
        D3D11_INPUT_ELEMENT_DESC Layout[] =
            {
                {"POSITION", 0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 0, D3D11_INPUT_PER_VERTEX_DATA, 0},
                {"TEXCOORD", 0, DXGI_FORMAT_R32G32_FLOAT, 0, 12, D3D11_INPUT_PER_VERTEX_DATA, 0}};

        UINT NumElements = ARRAYSIZE(Layout);

        hr = m_dev->CreateInputLayout(Layout, NumElements, g_VS, Size, &l_InputLayout);

        if (FAILED(hr))
            break;

        m_ctx->IASetInputLayout(l_InputLayout);

        // Simple pixel shader
        Size = ARRAYSIZE(g_PS);

        hr = m_dev->CreatePixelShader(g_PS, Size, nullptr, &m_SimplePixelShader);

        if (FAILED(hr))
            break;

        // Smooth pixel shader
        Size = ARRAYSIZE(g_SmoothPS);

        hr = m_dev->CreatePixelShader(g_SmoothPS, Size, nullptr, &m_SmoothPixelShader);

        if (FAILED(hr))
            break;



        // Set up sampler
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
            break;

        g_vertexes[0].Pos.x = -1;
        g_vertexes[0].Pos.y = -1;
        g_vertexes[0].Pos.z = 0;

        g_vertexes[1].Pos.x = -1;
        g_vertexes[1].Pos.y = 1;
        g_vertexes[1].Pos.z = 0;

        g_vertexes[2].Pos.x = 1;
        g_vertexes[2].Pos.y = -1;
        g_vertexes[2].Pos.z = 0;

        g_vertexes[3].Pos.x = 1;
        g_vertexes[3].Pos.y = 1;
        g_vertexes[3].Pos.z = 0;



        g_vertexes[0].TexCoord.x = 0;
        g_vertexes[0].TexCoord.y = 1;

        g_vertexes[1].TexCoord.x = 0;
        g_vertexes[1].TexCoord.y = 0;

        g_vertexes[2].TexCoord.x = 1;
        g_vertexes[2].TexCoord.y = 1;

        g_vertexes[3].TexCoord.x = 1;
        g_vertexes[3].TexCoord.y = 0;



        D3D11_SHADER_RESOURCE_VIEW_DESC ShaderDesc;
        ShaderDesc.Format = l_ShaderTextureDesc.Format;
        ShaderDesc.ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2D;
        ShaderDesc.Texture2D.MostDetailedMip = l_ShaderTextureDesc.MipLevels - 1;
        ShaderDesc.Texture2D.MipLevels = l_ShaderTextureDesc.MipLevels;

        // Create new shader resource view
        CComPtrCustom<ID3D11ShaderResourceView> ShaderResource = nullptr;
        hr = m_dev->CreateShaderResourceView(m_ShaderTexture, &ShaderDesc, &ShaderResource);

        if (FAILED(hr))
            break;

        FLOAT BlendFactor[4] = {0.f, 0.f, 0.f, 0.f};
        m_ctx->OMSetBlendState(nullptr, BlendFactor, 0xFFFFFFFF);
        m_ctx->OMSetRenderTargets(1, &m_RTV, nullptr);
        m_ctx->VSSetShader(l_VertexShader, nullptr, 0);
		if (m_fxaa) {
            m_ctx->PSSetShader(m_SmoothPixelShader, nullptr, 0);
		} else {
            m_ctx->PSSetShader(m_SimplePixelShader, nullptr, 0);
        }
        m_ctx->PSSetShaderResources(0, 1, &ShaderResource);
        m_ctx->PSSetSamplers(0, 1, &l_SamplerState);
        m_ctx->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP);



        // Create space for vertices for the dirty rects if the current space isn't large enough
        UINT BytesNeeded = sizeof(VERTEX) * NUMVERTICES;


        // Create vertex buffer
        D3D11_BUFFER_DESC BufferDesc;
        RtlZeroMemory(&BufferDesc, sizeof(BufferDesc));
        BufferDesc.Usage = D3D11_USAGE_DEFAULT;
        BufferDesc.ByteWidth = BytesNeeded;
        BufferDesc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
        BufferDesc.CPUAccessFlags = 0;
        D3D11_SUBRESOURCE_DATA InitData;
        RtlZeroMemory(&InitData, sizeof(InitData));
        InitData.pSysMem = g_vertexes;

        CComPtrCustom<ID3D11Buffer> VertBuf;
        hr = m_dev->CreateBuffer(&BufferDesc, &InitData, &VertBuf);

        if (FAILED(hr))
            break;

        UINT Stride = sizeof(VERTEX);
        UINT Offset = 0;
        m_ctx->IASetVertexBuffers(0, 1, &VertBuf, &Stride, &Offset);



        D3D11_VIEWPORT VP;

        VP.Width = static_cast<FLOAT>(l_Width);
        VP.Height = static_cast<FLOAT>(l_Height);

        VP.MinDepth = 0.0f;
        VP.MaxDepth = 1.0f;
        VP.TopLeftX = static_cast<FLOAT>(l_XOffset);
        VP.TopLeftY = 0.0f;
        m_ctx->RSSetViewports(1, &VP);


    } while (false);

    return l_result;
}

void GSDevice::Flip()
{
    if (m_ctx) {

        m_ctx->Flush();

        m_ctx->CopyResource(m_CaptureTexture, m_RenderTargetTexture);

        m_ctx->CopyResource(m_SharedTexture, m_CaptureTexture);
    }
}
void GSDevice::ClearScreen()
{
    if (m_ctx && m_RenderTargetTexture) {


        D3D11_TEXTURE2D_DESC l_Desc;

        m_RenderTargetTexture->GetDesc(&l_Desc);

        l_Desc.ArraySize = 1;

        l_Desc.BindFlags = 0;

        l_Desc.MiscFlags = 0;

        l_Desc.SampleDesc.Count = 1;

        l_Desc.SampleDesc.Quality = 0;

        l_Desc.MipLevels = 1;

        l_Desc.CPUAccessFlags = D3D11_CPU_ACCESS_READ | D3D11_CPU_ACCESS_WRITE;

        l_Desc.Usage = D3D11_USAGE_STAGING;

        CComPtrCustom<ID3D11Texture2D> l_RawTargetTexture;

        HRESULT hr = m_dev->CreateTexture2D(&l_Desc, nullptr, &l_RawTargetTexture);

        if (FAILED(hr))
            return;

        D3D11_MAPPED_SUBRESOURCE resource;
        UINT subresource = D3D11CalcSubresource(0, 0, 0);
        m_ctx->Map(l_RawTargetTexture, subresource, D3D11_MAP_READ_WRITE, 0, &resource);

        ZeroMemory(resource.pData, resource.RowPitch * l_Desc.Height);

        m_ctx->Unmap(l_RawTargetTexture, subresource);

        m_ctx->CopyResource(m_CaptureTexture, l_RawTargetTexture);

        m_ctx->CopyResource(m_SharedTexture, l_RawTargetTexture);
    }
}

void GSDevice::DoBufferSwap()
{
    if (m_ctx) {

        if (m_fxaa) {
            m_ctx->PSSetShader(m_SmoothPixelShader, nullptr, 0);
        } else {
            m_ctx->PSSetShader(m_SimplePixelShader, nullptr, 0);
        }

        long x, y;

        x = PSXDisplay.DisplayPosition.x;
        y = PSXDisplay.DisplayPosition.y;

        D3D11_MAPPED_SUBRESOURCE resource;
        UINT subresource = D3D11CalcSubresource(0, 0, 0);
        m_ctx->Map(m_RawTargetTexture, subresource, D3D11_MAP_READ_WRITE, 0, &resource);

        BlitScreen(&resource, x, 0);

        m_ctx->Unmap(m_RawTargetTexture, subresource);

        m_ctx->Flush();

        m_ctx->CopyResource(m_ShaderTexture, m_RawTargetTexture);

        if (m_CurrentDisplayMode != PSXDisplay.DisplayMode)
            UpdateVertexBuffer();

        Draw();

        Flip();
    }
}

void GSDevice::Draw()
{
    if (!m_ctx)
        return;

    FLOAT l_clearColor[] = {0.0f, 0.0f, 0.0f, 0.0f};

    m_ctx->ClearRenderTargetView(m_RTV, l_clearColor);

    m_ctx->Draw(NUMVERTICES, 0);
}

void GSDevice::UpdateVertexBuffer()
{
    do {

        float l_text_x = (float)PSXDisplay.DisplayMode.x / WIDTH;

        float l_text_y = (float)PSXDisplay.DisplayMode.y / HEIGHT;


        g_vertexes[0].TexCoord.x = 0;
        g_vertexes[0].TexCoord.y = l_text_y;

        g_vertexes[2].TexCoord.x = l_text_x;
        g_vertexes[2].TexCoord.y = l_text_y;

        g_vertexes[3].TexCoord.x = l_text_x;
        g_vertexes[3].TexCoord.y = 0;



        // Create space for vertices for the dirty rects if the current space isn't large enough
        UINT BytesNeeded = sizeof(VERTEX) * NUMVERTICES;


        // Create vertex buffer
        D3D11_BUFFER_DESC BufferDesc;
        RtlZeroMemory(&BufferDesc, sizeof(BufferDesc));
        BufferDesc.Usage = D3D11_USAGE_DEFAULT;
        BufferDesc.ByteWidth = BytesNeeded;
        BufferDesc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
        BufferDesc.CPUAccessFlags = 0;
        D3D11_SUBRESOURCE_DATA InitData;
        RtlZeroMemory(&InitData, sizeof(InitData));
        InitData.pSysMem = g_vertexes;

        CComPtrCustom<ID3D11Buffer> VertBuf;
        HRESULT hr = m_dev->CreateBuffer(&BufferDesc, &InitData, &VertBuf);

        if (FAILED(hr))
            break;

        UINT Stride = sizeof(VERTEX);
        UINT Offset = 0;
        m_ctx->IASetVertexBuffers(0, 1, &VertBuf, &Stride, &Offset);

        m_CurrentDisplayMode = PSXDisplay.DisplayMode;

    } while (false);
}

void GSDevice::setFXAA(BOOL a_value)
{
    if (a_value == FALSE)
        m_fxaa = false;
    else
        m_fxaa = true;
}


extern "C" void DestroyPic(void) {}

////////////////////////////////////////////////////////////////////////

extern "C" void BlitScreen32(D3D11_MAPPED_SUBRESOURCE *resource, long x, long y) // BLIT IN 32bit COLOR MODE
{
    unsigned char *pD;
    unsigned long lu;
    unsigned short s;
    unsigned int startxy;
    short row, column;
    short dx = (short)PreviousPSXDisplay.Range.x1;
    short dy = (short)PreviousPSXDisplay.DisplayMode.y;


    unsigned char *surf = (unsigned char *)resource->pData;


    if (PreviousPSXDisplay.Range.y0) // centering needed?
    {
        surf += PreviousPSXDisplay.Range.y0 * resource->RowPitch;
        dy -= PreviousPSXDisplay.Range.y0;
    }

    surf += PreviousPSXDisplay.Range.x0 << 2;

    if (PSXDisplay.RGB24) {
        if (iFPSEInterface) {
            for (column = 0; column < dy; column++) {
                startxy = ((1024) * (column + y)) + x;
                pD = (unsigned char *)&psxVuw[startxy];

                for (row = 0; row < dx; row++) {
                    lu = *((unsigned long *)pD);
                    *((unsigned long *)((surf) + (column * resource->RowPitch) + row * 4)) =
                        0xff000000 | (BLUE(lu) << 16) | (GREEN(lu) << 8) | (RED(lu));
                    pD += 3;
                }
            }
        } else {
            for (column = 0; column < dy; column++) {
                startxy = ((1024) * (column + y)) + x;
                pD = (unsigned char *)&psxVuw[startxy];

                for (row = 0; row < dx; row++) {
                    lu = *((unsigned long *)pD);
                    *((unsigned long *)((surf) + (column * resource->RowPitch) + row * 4)) =
                        0xff000000 | (RED(lu) << 16) | (GREEN(lu) << 8) | (BLUE(lu));
                    pD += 3;
                }
            }
        }
    } else {
        for (column = 0; column < dy; column++) {
            startxy = ((1024) * (column + y)) + x;
            for (row = 0; row < dx; row++) {
                s = psxVuw[startxy++];
                *((unsigned long *)((surf) + (column * resource->RowPitch) + row * 4)) =
                    ((((s << 19) & 0xf80000) | ((s << 6) & 0xf800) | ((s >> 7) & 0xf8)) & 0xffffff) | 0xff000000;
            }
        }
    }
}


extern "C" void DoClearScreenBuffer(void) {}

extern "C" void ShowGpuPic(void) {}

extern "C" void MoveScanLineArea(HWND hwnd) {}

extern "C" void DoBufferSwap(void)
{
    g_GSDevice.DoBufferSwap();
}

extern "C" void DoClearFrontBuffer(void) {}

////////////////////////////////////////////////////////////////////////

int DXinitialize()
{
    BlitScreen = BlitScreen32;

    return 0;
}



extern "C" unsigned long ulInitDisplay(void)
{
    DXinitialize();

    return 1;
}

extern "C" void CloseDisplay(void)
{
    g_GSDevice.ClearScreen();
}

extern "C" void CreatePic(unsigned char *pMem) {}

extern "C" void ShowTextGpuPic(void) {}