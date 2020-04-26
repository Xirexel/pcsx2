#pragma once

#include <memory>
#include "../../Window/GSWnd.h"
#include "GPUDevice.h"

class GPURenderer
{
public:
	GPURenderer();
	virtual ~GPURenderer();

public:
	std::shared_ptr<GSWnd> m_wnd;
	GPUDevice* m_dev;


	virtual bool CreateDevice(GPUDevice *dev, void *sharedhandle = nullptr, void *capturehandle = nullptr, void *directXDeviceNative = nullptr);
    virtual void VSync(int field);
    virtual void ClearTargets();
	virtual void ClearRenderTarget();
	virtual void ClearDepth();
	virtual void setClearColor(float aRed, float aGreen, float aBlue, float aAlpha);
	virtual void setOrtho(double aLeft, double aRight, double aBottom, double aTop, double azNear, double azFar);	
	virtual void draw(DWORD aDrawMode, void* aPtrVertexes, BOOL aIsTextured);
    virtual void addTexture(UINT32 aTexName, INT32 aInternalformat, INT32 aWidth, INT32 aHeight, INT32 aBorder, DWORD aFormat, INT32 aType, const void *aPtrPixels) {}
	virtual void setAlphaFunc(UINT32 aFunc, FLOAT aValue);
	virtual void updateTexture(UINT32 aTexName, INT32 aXoffset, INT32 aYoffset, INT32 aWidth, INT32 aHeight, DWORD aFormat, DWORD aType, const void *aPtrPixels){}
	virtual void setTexture(UINT32 aTexName) {}
	virtual void deleteTexture(UINT32 aTexName) {}
	virtual void enableBlend(BOOL aEnable);
	virtual void setBlendFunc(DWORD aSfactor, DWORD aDfactor);
	virtual void setBlendEquation(DWORD aOPcode);
	virtual void copyTexSubImage2D(UINT32 aTexName, INT32 aXoffset, INT32 aYoffset, INT32 aX, INT32 aY, INT32 aWidth, INT32 aHeight) {}
	virtual void setDepthFunc(UINT32 aFunc);
	virtual void enableScissor(BOOL aEnable);
	virtual void setScissor(INT32 aX, INT32 aY, INT32 aWidth, INT32 aHeight);


	
	void setTexturePacksMode(UINT32 a_TexturePackMode);
    void setTexturePacksPath(const std::wstring &a_RefTexturePackPath);

    void setFXAA(BOOL a_value);

protected:
    int m_vsync;
    BOOL m_fxaa;
};

