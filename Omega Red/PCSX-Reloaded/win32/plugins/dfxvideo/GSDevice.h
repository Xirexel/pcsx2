#include "CommonCPP.h"
#include <d3d11.h>
#include <d3d9.h>
#include <string>
#include "ComPtrCustom.h"

class GSDevice
{
public:
    GSDevice();
    virtual ~GSDevice();

    bool Create(void *sharedhandle, void *capturehandle, void *directXDeviceNative);

    void DoBufferSwap();

	void ClearScreen();

	void setFXAA(BOOL a_value);

private:
    CComPtrCustom<ID3D11Device> m_dev;
    CComPtrCustom<ID3D11DeviceContext> m_ctx;
	
    CComPtrCustom<ID3D11RenderTargetView> m_RTV;

    CComPtrCustom<ID3D11Texture2D> m_SharedTexture;

    CComPtrCustom<ID3D11Texture2D> m_RawTargetTexture;

    CComPtrCustom<ID3D11Texture2D> m_RenderTargetTexture;

    CComPtrCustom<ID3D11Texture2D> m_ShaderTexture;	

    CComPtrCustom<ID3D11Texture2D> m_CaptureTexture;

    CComPtrCustom<ID3D11PixelShader> m_SimplePixelShader;

    CComPtrCustom<ID3D11PixelShader> m_SmoothPixelShader;

    bool m_fxaa;

	


    void Flip();

    void Draw();

	void UpdateVertexBuffer();

};