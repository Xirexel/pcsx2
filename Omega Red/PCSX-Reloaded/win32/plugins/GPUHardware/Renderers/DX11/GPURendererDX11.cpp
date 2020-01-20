#include "GPURendererDX11.h"
#include "ComPtrCustom.h"


extern "C" void OutputLog(const char* szFormat, ...);


GPURendererDX11::GPURendererDX11()
{
}


GPURendererDX11::~GPURendererDX11()
{
}

void GPURendererDX11::addTexture(UINT32 aTexName, INT32 aInternalformat, INT32 aWidth, INT32 aHeight, INT32 aBorder, DWORD aFormat, INT32 aType, const void *aPtrPixels)
{
	CComPtrCustom<IUnknown> lGPUTexture;

	CComPtrCustom<IUnknown> lShaderResourceView;
		
	m_dev->createTexture(aInternalformat, aWidth, aHeight, aBorder, aFormat, aType, aPtrPixels, &lGPUTexture, &lShaderResourceView);

	if (lGPUTexture && lShaderResourceView)
	{
		auto l_found = m_textures.find(aTexName);

		if (l_found != m_textures.end())
			return;

		m_textures[aTexName] = std::make_tuple(lGPUTexture, lShaderResourceView);
	}
	else
		return;
}

void GPURendererDX11::updateTexture(UINT32 aTexName, INT32 aXoffset, INT32 aYoffset, INT32 aWidth, INT32 aHeight, DWORD aFormat, DWORD aType, const void *aPtrPixels)
{
	auto l_find = m_textures.find(aTexName);

	if (l_find == m_textures.end())
		return;

	m_dev->updateTexture(std::get<0>((*l_find).second), aXoffset, aYoffset, aWidth, aHeight, aFormat, aType, aPtrPixels);
}

void GPURendererDX11::copyTexSubImage2D(UINT32 aTexName, INT32 aXoffset, INT32 aYoffset, INT32 aX, INT32 aY, INT32 aWidth, INT32 aHeight)
{
	auto l_find = m_textures.find(aTexName);

	if (l_find == m_textures.end())
		return;

	m_dev->copyTexSubImage2D(std::get<0>((*l_find).second), aXoffset, aYoffset, aX, aY, aWidth, aHeight);
}

void GPURendererDX11::setTexture(UINT32 aTexName)
{
	if (aTexName == 0)
	{
		m_currentTexName = aTexName;
		
		m_dev->setTexture(nullptr, nullptr);

		return;
	}


	auto l_find = m_textures.find(aTexName);

	if (l_find == m_textures.end())
		return;

	if (m_currentTexName == aTexName)
		return;

	m_currentTexName = aTexName;

	m_dev->setTexture(std::get<0>((*l_find).second), std::get<1>((*l_find).second));
}

void GPURendererDX11::deleteTexture(UINT32 aTexName)
{
	auto l_find = m_textures.find(aTexName);

	if (l_find == m_textures.end())
		return;

	m_textures.erase(l_find);
}
