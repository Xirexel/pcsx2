#pragma once

#include "../Common/GPURenderer.h"
#include "ComPtrCustom.h"
#include <unordered_map>

class GPURendererDX11 : public GPURenderer
{
public:
    GPURendererDX11();
    virtual ~GPURendererDX11();
    virtual void addTexture(UINT32 aTexName, INT32 aInternalformat, INT32 aWidth, INT32 aHeight, INT32 aBorder, DWORD aFormat, INT32 aType, const void *aPtrPixels) override;
    virtual void updateTexture(UINT32 aTexName, INT32 aXoffset, INT32 aYoffset, INT32 aWidth, INT32 aHeight, DWORD aFormat, DWORD aType, const void *aPtrPixels) override;
    virtual void setTexture(UINT32 aTexName) override;
    virtual void deleteTexture(UINT32 aTexName) override;
    virtual void copyTexSubImage2D(UINT32 aTexName, INT32 aXoffset, INT32 aYoffset, INT32 aX, INT32 aY, INT32 aWidth, INT32 aHeight) override;


private:
    std::unordered_map<UINT32, std::tuple<CComPtrCustom<IUnknown>, CComPtrCustom<IUnknown>>> m_textures;
    UINT32 m_currentTexName;
};
