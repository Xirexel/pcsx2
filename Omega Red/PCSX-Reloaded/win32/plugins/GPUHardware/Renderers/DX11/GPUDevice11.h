#pragma once

#include "../Common/GPUDevice.h"
#include "ComPtrCustom.h"
#include <unordered_map>

class GPUDevice11:
	public GPUDevice
{
public:
	GPUDevice11();
	virtual ~GPUDevice11();


	bool Create(const std::shared_ptr<GSWnd> &wnd, void *sharedhandle, void *capturehandle, void *directXDeviceNative);

	virtual void Present();
    virtual void ClearTargets();
	virtual void ClearRenderTarget();
	virtual void ClearDepth();
	virtual void setClearColor(float aRed, float aGreen, float aBlue, float aAlpha);
	virtual void setOrtho(double aLeft, double aRight, double aBottom, double aTop, double azNear, double azFar);
	virtual void draw(DWORD aDrawMode, void* aPtrVertexes, BOOL aIsTextured);
	virtual void enableBlend(BOOL aEnable)override;
	virtual void setBlendFunc(DWORD aSfactor, DWORD aDfactor)override;
	virtual void setBlendEquation(DWORD aOPcode)override;
	virtual void createTexture(INT32 aInternalformat, INT32 aWidth, INT32 aHeight, INT32 aBorder, DWORD aFormat, DWORD aType, const void * aPtrPixels, IUnknown** aPtrPtrGPUTexture, IUnknown** aPtrPtrUnkShaderResourceView) override;
	virtual void updateTexture(IUnknown* aPtrGPUUnkTexture, INT32 aXoffset, INT32 aYoffset, INT32 aWidth, INT32 aHeight, DWORD aFormat, DWORD aType, const void *aPtrPixels) override;
	virtual void setTexture(IUnknown* aPtrUnkTexture, IUnknown* aPtrPtrUnkShaderResourceView)override;
	virtual void setAlphaFunc(UINT32 aFunc, FLOAT aValue)override;
	virtual void copyTexSubImage2D(IUnknown* aPtrUnkTexture, INT32 aXoffset, INT32 aYoffset, INT32 aX, INT32 aY, INT32 aWidth, INT32 aHeight)override;
	virtual void setDepthFunc(UINT32 aFunc)override;
	virtual void enableScissor(BOOL aEnable)override;
	virtual void setScissor(INT32 aX, INT32 aY, INT32 aWidth, INT32 aHeight)override;
    virtual void setFXAA(BOOL a_value)override;

protected:
    virtual void setRawTexture(void *a_PtrMemory, const char *a_StringIDs) override;

private:
	float m_hack_topleft_offset;
	int m_upscale_multiplier;
	int m_mipmap;
	int m_d3d_texsize;
	FLOAT m_clearColor[4] = { 1.0f, 0.0f, 1.0f, 0.0f };
	BOOL m_blend_enabled = FALSE;
	D3D11_BLEND_OP m_color_blend_op;
	D3D11_BLEND_OP m_alpha_blend_op;
    BOOL m_IsTextured;
    BOOL m_fxaa;
	BOOL m_vertical_flip_state = FALSE;
	UINT m_Height;
    UINT m_Width;
    UINT m_Xoffset;


	CComPtrCustom<ID3D11Device> m_dev;
	CComPtrCustom<ID3D11DeviceContext> m_ctx;
    CComPtr<ID3D11Texture2D> m_SharedTexture;
    CComPtr<ID3D11Texture2D> m_RenderTargetTexture;
    CComPtr<ID3D11Texture2D> m_CaptureTexture;
	CComPtrCustom<ID3D11InputLayout> m_InputLayout;
	CComPtrCustom<ID3D11RasterizerState> m_RasterizerState;
	CComPtrCustom<ID3D11VertexShader> m_VertexShader;
	CComPtrCustom<ID3D11DepthStencilState> m_DepthStencilState;
	CComPtrCustom<ID3D11Buffer> m_directx_projection_buffer;
	CComPtrCustom<ID3D11Buffer> m_alpha_fun_buffer;
	CComPtrCustom<ID3D11ShaderResourceView> m_CurrentTextureShaderResource;
	D3D11_VIEWPORT m_VP;

	CComPtrCustom<ID3D11RenderTargetView> m_RTV;
	
	CComPtrCustom<ID3D11DepthStencilView> m_DSTV;

    CComPtrCustom<ID3D11PixelShader> m_SimplePixelShader_textured;
	CComPtrCustom<ID3D11PixelShader> m_SimplePixelShader;
    CComPtrCustom<ID3D11PixelShader> m_SimplePixelShader_sharpen;
	CComPtrCustom<ID3D11PixelShader> m_SimplePixelShader_none_textured;
	CComPtrCustom<ID3D11Buffer> m_DrawVertexBuffer;

	CComPtrCustom<ID3D11BlendState> m_disableBlending;
	CComPtrCustom<ID3D11BlendState> m_enableBlending;

    std::unordered_map<UINT64, CComPtrCustom<IUnknown>> m_textures;


	void Reset(int w, int h);
	void SetExclusive(bool isExcl);
	void Flip();

	void setBlendState(ID3D11BlendState* aPtrBlendState);


};

