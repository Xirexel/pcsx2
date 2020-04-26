#pragma once

#include "../../Window/GSWnd.h"

class GPUDevice
{
public:

    GPUDevice();
    virtual ~GPUDevice();


    virtual bool Create(const std::shared_ptr<GSWnd> &wnd, void *sharedhandle = nullptr, void *capturehandle = nullptr, void *directXDeviceNative = nullptr);

    virtual void Flip() {}
    virtual void SetVSync(int vsync) { m_vsync = vsync; }

    virtual void Present() {}
    virtual void ClearTargets() {}
    virtual void ClearRenderTarget() {}
    virtual void ClearDepth() {}
    virtual void setClearColor(float aRed, float aGreen, float aBlue, float aAlpha) {}
    virtual void setOrtho(double aLeft, double aRight, double aBottom, double aTop, double azNear, double azFar) {}
    virtual void draw(DWORD aDrawMode, void *aPtrVertexes, BOOL aIsTextured) {}
    virtual void enableBlend(BOOL aEnable) {}
    virtual void setBlendFunc(DWORD aSfactor, DWORD aDfactor) {}
    virtual void setBlendEquation(DWORD aOPcode) {}
    virtual void createTexture(INT32 aInternalformat, INT32 aWidth, INT32 aHeight, INT32 aBorder, DWORD aFormat, DWORD aType, const void *aPtrPixels, IUnknown **aPtrPtrGPUTexture, IUnknown **aPtrPtrUnkShaderResourceView) {}
    virtual void updateTexture(IUnknown *aPtrGPUUnkTexture, INT32 aXoffset, INT32 aYoffset, INT32 aWidth, INT32 aHeight, DWORD aFormat, DWORD aType, const void *aPtrPixels) {}
    virtual void setTexture(IUnknown *aPtrUnkTexture, IUnknown *aPtrPtrUnkShaderResourceView) {}
    virtual void setAlphaFunc(UINT32 aFunc, FLOAT aValue) {}
    virtual void copyTexSubImage2D(IUnknown *aPtrUnkTexture, INT32 aXoffset, INT32 aYoffset, INT32 aX, INT32 aY, INT32 aWidth, INT32 aHeight) {}
    virtual void setDepthFunc(UINT32 aFunc) {}
    virtual void enableScissor(BOOL aEnable) {}
    virtual void setScissor(INT32 aX, INT32 aY, INT32 aWidth, INT32 aHeight) {}
    virtual void setTexturePackMode(UINT32 a_TexturePackMode);
    virtual void setTexturePacksPath(const std::wstring &a_RefTexturePackPath);
    virtual void setFXAA(BOOL a_value) {}

    static GPUDevice *s_instance;
    virtual void setRawTexture(void *a_PtrMemory, const char *a_StringIDs) = 0;

protected:

    enum TexturePackMode {
        NONE = 0,
        LOAD = NONE + 1,
        SAVE = LOAD + 1
    };

    std::shared_ptr<GSWnd> m_wnd;
    int m_vsync;
    TexturePackMode m_TexturePackMode;
    std::wstring m_TexturePacksPath;
    int m_ResolitionX;
    int m_ResolitionY;

    virtual void Reset(int w, int h) {}
};
