#include "GPURenderer.h"



GPURenderer::GPURenderer():
	m_dev(nullptr)
{
}

GPURenderer::~GPURenderer()
{
	if(m_dev != nullptr)
		delete m_dev;
}

bool GPURenderer::CreateDevice(GPUDevice *dev, void *sharedhandle, void *capturehandle, void *directXDeviceNative)
{
    if (m_dev)
        return false;

	if (!dev->Create(m_wnd, sharedhandle, capturehandle, directXDeviceNative))
	{
		return false;
	}

	m_dev = dev;
	m_dev->SetVSync(m_vsync);

	return true;
}

void GPURenderer::VSync(int field)
{
	m_dev->Present();
}
void GPURenderer::ClearTargets()
{
    m_dev->ClearTargets();
}
void GPURenderer::ClearRenderTarget()
{
	m_dev->ClearRenderTarget();
}
void GPURenderer::ClearDepth()
{
	m_dev->ClearDepth();
}

void GPURenderer::setClearColor(float aRed, float aGreen, float aBlue, float aAlpha)
{
	m_dev->setClearColor(aRed, aGreen, aBlue, aAlpha);
}

void GPURenderer::setOrtho(double aLeft, double aRight, double aBottom, double aTop, double azNear, double azFar)
{
	m_dev->setOrtho(aLeft, aRight, aBottom, aTop, azNear, azFar);
}

void GPURenderer::draw(DWORD aDrawMode, void* aPtrVertexes, BOOL aIsTextured)
{
    m_dev->setFXAA(m_fxaa);
	m_dev->draw(aDrawMode, aPtrVertexes, aIsTextured);
}

void GPURenderer::enableBlend(BOOL aEnable)
{
	m_dev->enableBlend(aEnable);
}

void GPURenderer::setBlendFunc(DWORD aSfactor, DWORD aDfactor)
{
	m_dev->setBlendFunc(aSfactor, aDfactor);
}

void GPURenderer::setBlendEquation(DWORD aOPcode)
{
	m_dev->setBlendEquation(aOPcode);
}

void GPURenderer::setAlphaFunc(UINT32 aFunc, FLOAT aValue)
{
	m_dev->setAlphaFunc(aFunc, aValue);
}

void GPURenderer::setDepthFunc(UINT32 aFunc)
{
	m_dev->setDepthFunc(aFunc);
}

void GPURenderer::enableScissor(BOOL aEnable)
{
	m_dev->enableScissor(aEnable);
}

void GPURenderer::setScissor(INT32 aX, INT32 aY, INT32 aWidth, INT32 aHeight)
{
	m_dev->setScissor(aX, aY, aWidth, aHeight);
}

void GPURenderer::setTexturePacksMode(UINT32 a_TexturePackMode)
{
    m_dev->setTexturePackMode(a_TexturePackMode);
}

void GPURenderer::setTexturePacksPath(const std::wstring &a_RefTexturePacksPath)
{
    m_dev->setTexturePacksPath(a_RefTexturePacksPath);
}

void GPURenderer::setFXAA(BOOL a_value)
{
    if (a_value == FALSE)
        m_fxaa = false;
    else
        m_fxaa = true;
}
