#include "Pcsx2Defs.h"
#include "VideoRenderer.h"
//#include "Extend\GSDeviceProxy.h"
//#include "Extend\GSWndStub.h"
#include "pugixml.hpp"

//#include "Renderers/SW/GSRendererSW.h"


VideoRenderer g_VideoRenderer;

VideoRenderer::VideoRenderer():
	m_AspectRatio(1),
	m_is_wired(FALSE), 
	m_is_tessellated(FALSE)
{
}

VideoRenderer::~VideoRenderer()
{
}

void VideoRenderer::execute(const wchar_t* a_command, wchar_t** a_result)
{
	using namespace pugi;

	xml_document l_xmlDoc;

	auto l_XMLRes = l_xmlDoc.load_string(a_command);

	if (l_XMLRes.status == xml_parse_status::status_ok)
	{
		auto l_document = l_xmlDoc.document_element();

		if (l_document.empty())
			return;

		if (std::wstring(l_document.name()) == L"Config")
		{
			auto l_ChildNode = l_document.first_child();

			while (!l_ChildNode.empty())
			{
				if (std::wstring(l_ChildNode.name()) == L"Init")
				{
					void* l_SharedHandle = nullptr;
					
					auto l_Attribute = l_ChildNode.attribute(L"ShareHandler");

//					if (!l_Attribute.empty() && !m_VideoRenderer)
//					{
//						auto l_value = l_Attribute.as_llong();
//
//						if (l_value != 0)
//						{
//							try
//							{
//
//								l_SharedHandle = (void*)l_value;
//
//							}
//							catch (...)
//							{
//
//							}
//
//						}
//					}
					
					void *l_CaptureHandler = nullptr;

                    l_Attribute = l_ChildNode.attribute(L"CaptureHandler");

//                    if (!l_Attribute.empty() && !m_VideoRenderer) {
//                        auto l_value = l_Attribute.as_llong();
//
//                        if (l_value != 0) {
//                            try {
//
//                                l_CaptureHandler = (void *)l_value;
//
//                            } catch (...) {
//                            }
//                        }
//                    }
					
                    void *l_DirectXDeviceNative = nullptr;

                    l_Attribute = l_ChildNode.attribute(L"DirectXDeviceNative");

//                    if (!l_Attribute.empty() && !m_VideoRenderer) {
//                        auto l_value = l_Attribute.as_llong();
//
//                        if (l_value != 0) {
//                            try {
//
//                                l_DirectXDeviceNative = (void *)l_value;
//
//                            } catch (...) {
//                            }
//                        }
//                    }

                    if (l_SharedHandle != nullptr)
                        init(l_SharedHandle, l_CaptureHandler, l_DirectXDeviceNative);				

				}
				else if (std::wstring(l_ChildNode.name()) == L"Shutdown")
				{
					shutdown();
				}
				else if (std::wstring(l_ChildNode.name()) == L"AspectRatio")
				{					
					auto l_Attribute = l_ChildNode.attribute(L"Value");

					if (!l_Attribute.empty())
					{
						m_AspectRatio = l_Attribute.as_int(1);

//						if (m_VideoRenderer)
//						{
//							m_VideoRenderer->SetAspectRatio(m_AspectRatio);
//						}
					}					
				}
				else if (std::wstring(l_ChildNode.name()) == L"DoFreeze")
				{

//					GSFreezeData* l_GSFreezeDataHandle = nullptr;

					auto l_Attribute = l_ChildNode.attribute(L"FreezeData");

//					if (!l_Attribute.empty() && m_VideoRenderer)
//					{
//						auto l_value = l_Attribute.as_llong();
//
//						if (l_value != 0)
//						{
//							try
//							{
//
//								l_GSFreezeDataHandle = (GSFreezeData*)l_value;
//
//							}
//							catch (...)
//							{
//
//							}
//
//						}
//					}

					int l_mode = -1;

					l_Attribute = l_ChildNode.attribute(L"Mode");

//					if (!l_Attribute.empty() && m_VideoRenderer)
//					{
//						auto l_value = l_Attribute.as_int(-1);
//
//						if (l_value != -1)
//						{
//							l_mode = l_value;
//						}
//					}
//
//					if (l_GSFreezeDataHandle != nullptr)
//					{
//						if (l_mode == FREEZE_SAVE)
//						{
//							m_VideoRenderer->Freeze(l_GSFreezeDataHandle, false);
//						}
//						else if (l_mode == FREEZE_SIZE)
//						{
//							m_VideoRenderer->Freeze(l_GSFreezeDataHandle, true);
//						}
//						else if (l_mode == FREEZE_LOAD)
//						{
//							m_VideoRenderer->Defrost(l_GSFreezeDataHandle);
//						}
//
//					}
                } 
				else if (std::wstring(l_ChildNode.name()) == L"GameCRC") {

                    auto l_Attribute = l_ChildNode.attribute(L"Value");

                    if (!l_Attribute.empty()) {
//                        if (m_VideoRenderer) {
//                            m_VideoRenderer->SetGameCRC(l_Attribute.as_uint(0), 0);
//                        }
                    }	
                } else if (std::wstring(l_ChildNode.name()) == L"IsWired") {

                    auto l_Attribute = l_ChildNode.attribute(L"Value");

                    if (!l_Attribute.empty()) {

						m_is_wired = l_Attribute.as_uint(0);

//                        if (m_VideoRenderer) {
//                            m_VideoRenderer->setIsWired(m_is_wired);
//                        }
                    }
                } else if (std::wstring(l_ChildNode.name()) == L"IsTessellated") {

                    auto l_Attribute = l_ChildNode.attribute(L"Value");

					m_is_tessellated = l_Attribute.as_uint(0);

                    if (!l_Attribute.empty()) {
//                        if (m_VideoRenderer) {
//                            m_VideoRenderer->setIsTessellated(m_is_tessellated);
//                        }
                    }
                } else if (std::wstring(l_ChildNode.name()) == L"IsFXAA") {

                    auto l_Attribute = l_ChildNode.attribute(L"Value");

                    m_is_fxaa = l_Attribute.as_uint(0);

                    if (!l_Attribute.empty()) {
//                        if (m_VideoRenderer) {
//                            m_VideoRenderer->setFXAA(m_is_fxaa);
//                        }
                    }
                }
				
				l_ChildNode = l_ChildNode.next_sibling();
			}
		}
	}
}

int innerGSinit()
{
//    if (!GSUtil::CheckSSE()) {
//        return -1;
//    }
//
//    // Vector instructions must be avoided when initialising GSdx since PCSX2
//    // can crash if the CPU does not support the instruction set.
//    // Initialise it here instead - it's not ideal since we have to strip the
//    // const type qualifier from all the affected variables.
//    theApp.Init();
//
//    GSUtil::Init();
//    GSBlock::InitVectors();
//    GSClut::InitVectors();
//#ifdef ENABLE_OPENCL
//    GSRendererCL::InitVectors();
//#endif
//    //GSRendererSW::InitVectors();
//    GSVector4i::InitVectors();
//    GSVector4::InitVectors();
//#if _M_SSE >= 0x500
//    GSVector8::InitVectors();
//#endif
//#if _M_SSE >= 0x501
//    GSVector8i::InitVectors();
//#endif
//    GSVertexTrace::InitVectors();
//
//    //if (g_const == nullptr)
//    //    return -1;
//    //else
//    //    g_const->Init();
//
//#ifdef _WIN32
//
//    //s_hr = ::CoInitializeEx(NULL, COINIT_MULTITHREADED);
//
//    if (!GSDeviceProxy::LoadD3DCompiler()) {
//        return -1;
//    }
//#endif

    return 0;
}

int innerGSshutdown()
{
//    theApp.SetCurrentRendererType(GSRendererType::Undefined);

#ifdef _WIN32
	
    GSDeviceProxy::FreeD3DCompiler();

#endif

	return -1;
}

int VideoRenderer::init(void *sharedhandle, void *capturehandle, void *directXDeviceNative)
{
//    innerGSinit();
//
//    std::unique_ptr<GSDeviceProxy> l_Device;
//
//	GSRendererType VideoRenderer = GSUtil::CheckDirect3D11Level() >= D3D_FEATURE_LEVEL_10_0 ? GSRendererType::DX1011_HW : GSRendererType::Null;
//
//	GSDeviceProxy::LoadD3DCompiler();
//
//	try
//	{
//		switch (VideoRenderer)
//		{
//		case GSRendererType::DX1011_HW:
//		case GSRendererType::DX1011_SW:
//		case GSRendererType::DX1011_OpenCL:
//            l_Device = std::make_unique<GSDeviceProxy>();
//			break;
//		default:
//			break;
//		}
//
//		if (l_Device == NULL)
//		{
//			return -1;
//		}
//
//		if (m_VideoRenderer == NULL)
//		{
//			switch (VideoRenderer)
//			{
//			case GSRendererType::DX1011_HW:
//                    m_VideoRenderer = std::make_unique<GSRendererProxy>();
//				break;
//			case GSRendererType::OGL_HW:
//			case GSRendererType::DX1011_SW:
//			case GSRendererType::OGL_SW:
//			case GSRendererType::DX1011_OpenCL:
//			case GSRendererType::OGL_OpenCL:
//			default:
//				break;
//			}
//			if (m_VideoRenderer == NULL)
//				return -1;
//		}
//
//		if (m_VideoRenderer->m_wnd == NULL)
//		{
//            m_VideoRenderer->m_wnd = std::make_shared<GSWndStub>();
//		}
//	}
//	catch (std::exception& ex)
//	{
//		// Allowing std exceptions to escape the scope of the plugin callstack could
//		// be problematic, because of differing typeids between DLL and EXE compilations.
//		// ('new' could throw std::alloc)
//
//		printf("GSdx error: Exception caught in GSopen: %s", ex.what());
//
//		return -1;
//	}
//
//	//s_gs->SetFrameLimit(s_framelimit);
//
//	m_VideoRenderer->SetAspectRatio(m_AspectRatio);
//
//	if (!m_VideoRenderer->CreateDevice(l_Device.get(), sharedhandle, capturehandle, directXDeviceNative))
//	{
//		// This probably means the user has DX11 configured with a video card that is only DX9
//		// compliant.  Cound mean drivr issues of some sort also, but to be sure, that's the most
//		// common cause of device creation errors. :)  --air
//
//		//GSclose();
//
//		return -1;
//	}
//
//    m_VideoRenderer->setIsWired(m_is_wired);
//
//    m_VideoRenderer->setIsTessellated(m_is_tessellated);
//
//    m_VideoRenderer->setFXAA(m_is_fxaa);
//
//	l_Device.release();
	
	return 0;
}

void VideoRenderer::shutdown()
{
//	m_VideoRenderer.reset();
//
//	innerGSshutdown();
}

void VideoRenderer::setBaseMem(void * a_ptr)
{
//	if (a_ptr == nullptr)
//		return;
//
//	m_BaseMem = (uint8*)a_ptr;
//
//	if (m_VideoRenderer)
//		m_VideoRenderer->SetRegsMem(m_BaseMem);
}

void VideoRenderer::setIrqCallback(Action a_callback)
{
//	if (a_callback == nullptr)
//		return;
//
//	m_Irq = a_callback;
//
//	if (m_VideoRenderer)
//		m_VideoRenderer->SetIrqCallback(m_Irq);
}

void VideoRenderer::reset()
{
}

void VideoRenderer::setVsync(bool a_state)
{
//	if (m_VideoRenderer)
//		m_VideoRenderer->SetVSync(a_state);
}

void VideoRenderer::setGameCRC(int crc, int options)
{
//	if (m_VideoRenderer)
//		m_VideoRenderer->SetGameCRC(crc, options);
}

void VideoRenderer::setFrameSkip(int skip)
{
//	if (m_VideoRenderer)
//		m_VideoRenderer->SetFrameSkip(skip);
}

void VideoRenderer::vsync(int field)
{
//	if (m_VideoRenderer)
//		m_VideoRenderer->VSync(field);
}

void VideoRenderer::gifTransfer(const uint8* mem, uint32 size)
{
//	if (m_VideoRenderer)
//		m_VideoRenderer->Transfer<3>(mem, size);
}

void VideoRenderer::readFIFO2(uint8 *pMem, int32 qwc)
{
//	if (m_VideoRenderer)
//		m_VideoRenderer->ReadFIFO(pMem, qwc);
}

void VideoRenderer::initReadFIFO2(uint8 *pMem, int32 qwc)
{
//	if (m_VideoRenderer)
//		m_VideoRenderer->InitReadFIFO(pMem, qwc);
}

void VideoRenderer::gifSoftReset(uint32 mask)
{
//	if (m_VideoRenderer)
//		m_VideoRenderer->SoftReset(mask);
}
