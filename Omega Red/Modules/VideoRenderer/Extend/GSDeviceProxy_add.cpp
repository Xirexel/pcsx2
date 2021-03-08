/*
 *	Copyright (C) 2007-2009 Gabest
 *	http://www.gabest.org
 *
 *  This Program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2, or (at your option)
 *  any later version.
 *
 *  This Program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with GNU Make; see the file COPYING.  If not, write to
 *  the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA USA.
 *  http://www.gnu.org/copyleft/gpl.html
 *
 */

#include "stdafx.h"
#include "GSdx.h"
#include "GSDeviceProxy.h"
#include "GSUtil.h"
#include "resource.h"
#include <fstream>
#include "resource.h"
#include "GSTables.h"

#pragma comment(lib, "DXGI.lib")
#pragma comment(lib, "D3D11.lib")
#pragma comment(lib, "d3dcompiler.lib")
#pragma comment(lib, "Comctl32.lib")

bool GSDeviceProxy::CreateTextureFX()
{
	HRESULT hr;

	D3D11_BUFFER_DESC bd;

	memset(&bd, 0, sizeof(bd));

	bd.ByteWidth = sizeof(VSConstantBuffer);
	bd.Usage = D3D11_USAGE_DEFAULT;
	bd.BindFlags = D3D11_BIND_CONSTANT_BUFFER;

	hr = m_dev->CreateBuffer(&bd, NULL, &m_vs_cb);

	if (FAILED(hr)) return false;

	memset(&bd, 0, sizeof(bd));

	bd.ByteWidth = sizeof(GSConstantBuffer);
	bd.Usage = D3D11_USAGE_DEFAULT;
	bd.BindFlags = D3D11_BIND_CONSTANT_BUFFER;

	hr = m_dev->CreateBuffer(&bd, NULL, &m_gs_cb);

	if (FAILED(hr)) return false;

	memset(&bd, 0, sizeof(bd));

	bd.ByteWidth = sizeof(PSConstantBuffer);
	bd.Usage = D3D11_USAGE_DEFAULT;
	bd.BindFlags = D3D11_BIND_CONSTANT_BUFFER;

	hr = m_dev->CreateBuffer(&bd, NULL, &m_ps_cb);

	if (FAILED(hr)) return false;

	D3D11_SAMPLER_DESC sd;

	memset(&sd, 0, sizeof(sd));

	sd.Filter = D3D11_FILTER_MIN_MAG_MIP_POINT;
	sd.AddressU = D3D11_TEXTURE_ADDRESS_CLAMP;
	sd.AddressV = D3D11_TEXTURE_ADDRESS_CLAMP;
	sd.AddressW = D3D11_TEXTURE_ADDRESS_CLAMP;
	sd.MinLOD = -FLT_MAX;
	sd.MaxLOD = FLT_MAX;
	sd.MaxAnisotropy = D3D11_MIN_MAXANISOTROPY;
	sd.ComparisonFunc = D3D11_COMPARISON_NEVER;

	hr = m_dev->CreateSamplerState(&sd, &m_palette_ss);

	if (FAILED(hr)) return false;

	// create layout

	VSSelector sel;
	VSConstantBuffer cb;

	SetupVS(sel, &cb);

	GSConstantBuffer gcb;

	SetupGS(GSSelector(1), &gcb);

	//

	return true;
}


#include "resource1.h"


void GSDeviceProxy::CompileShader(const char *source, size_t size, const char *fn, ID3DInclude *include, const char *entry, D3D_SHADER_MACRO *macro, ID3D11DomainShader **ds)
{
	HRESULT hr;

	//std::vector<D3D_SHADER_MACRO> m;

	//PrepareShaderMacro(m, macro);

	//CComPtr<ID3DBlob> shader, error;

	//hr = s_pD3DCompile(source, size, fn, &m[0], s_old_d3d_compiler_dll ? nullptr : include, entry, "ds_5_0", 0, 0, &shader, &error);

	//if (error) {
	//	printf("%s\n", (const char *)error->GetBufferPointer());
	//}

	//if (FAILED(hr)) {
	//	throw GSDXRecoverableError();
	//}

	//hr = m_dev->CreateDomainShader((void *)shader->GetBufferPointer(), shader->GetBufferSize(), NULL, ds);

	//if (FAILED(hr)) {
	//	throw GSDXRecoverableError();
	//}
}

void GSDeviceProxy::CompileShader(const char *source, size_t size, const char *fn, ID3DInclude *include, const char *entry, D3D_SHADER_MACRO *macro, ID3D11HullShader **hs)
{
	HRESULT hr;

	std::vector<D3D_SHADER_MACRO> m;

	//PrepareShaderMacro(m, macro);

	//CComPtr<ID3DBlob> shader, error;

	//hr = s_pD3DCompile(source, size, fn, &m[0], s_old_d3d_compiler_dll ? nullptr : include, entry, "hs_5_0", 0, 0, &shader, &error);

	//if (error) {
	//	printf("%s\n", (const char *)error->GetBufferPointer());
	//}

	//if (FAILED(hr)) {
	//	throw GSDXRecoverableError();
	//}

	//hr = m_dev->CreateHullShader((void *)shader->GetBufferPointer(), shader->GetBufferSize(), NULL, hs);

	//if (FAILED(hr)) {
	//	throw GSDXRecoverableError();
	//}
}

void GSDeviceProxy::SetupVS(VSSelector sel, const VSConstantBuffer* cb)
{
	auto i = std::as_const(m_vs).find(sel);

	if (i == m_vs.end())
	{
		ShaderMacro sm(m_shader.model);

		sm.AddMacro("VS_TME", sel.tme);
		sm.AddMacro("VS_FST", sel.fst);

		D3D11_INPUT_ELEMENT_DESC layout[] =
		{
			{"TEXCOORD", 0, DXGI_FORMAT_R32G32_FLOAT, 0, 0, D3D11_INPUT_PER_VERTEX_DATA, 0},
			{"COLOR", 0, DXGI_FORMAT_R8G8B8A8_UINT, 0, 8, D3D11_INPUT_PER_VERTEX_DATA, 0},
			{"TEXCOORD", 1, DXGI_FORMAT_R32_FLOAT, 0, 12, D3D11_INPUT_PER_VERTEX_DATA, 0},
			{"POSITION", 0, DXGI_FORMAT_R16G16_UINT, 0, 16, D3D11_INPUT_PER_VERTEX_DATA, 0},
			{"POSITION", 1, DXGI_FORMAT_R32_UINT, 0, 20, D3D11_INPUT_PER_VERTEX_DATA, 0},
			{"TEXCOORD", 2, DXGI_FORMAT_R16G16_UINT, 0, 24, D3D11_INPUT_PER_VERTEX_DATA, 0},
			{"COLOR", 1, DXGI_FORMAT_R8G8B8A8_UNORM, 0, 28, D3D11_INPUT_PER_VERTEX_DATA, 0},
		};

		GSVertexShader11Proxy vs;

		std::vector<char> shader;
		theApp.LoadResource(IDR_TFX_FX, shader);
		CreateShader(shader, "tfx.fx", nullptr, "vs_main", sm.GetPtr(), &vs.vs, layout, countof(layout), &vs.il);

		m_vs[sel] = vs;

		i = m_vs.find(sel);
	}

	if (m_vs_cb_cache.Update(cb))
	{
		ID3D11DeviceContext* ctx = m_ctx;

		ctx->UpdateSubresource(m_vs_cb, 0, NULL, cb, 0, 0);
	}

	VSSetShader(i->second.vs, m_vs_cb);

	IASetInputLayout(i->second.il);
}

void GSDeviceProxy::SetupGS(GSSelector sel, const GSConstantBuffer* cb)
{
	CComPtr<ID3D11GeometryShader> gs;

	const bool unscale_pt_ln = (sel.point == 1 || sel.line == 1);
	// Geometry shader is disabled if sprite conversion is done on the cpu (sel.cpu_sprite).
	if ((sel.prim > 0 && sel.cpu_sprite == 0 && (sel.iip == 0 || sel.prim == 3)) || unscale_pt_ln)
	{
		const auto i = std::as_const(m_gs).find(sel);

		if (i != m_gs.end())
		{
			gs = i->second;
		}
		else
		{
			ShaderMacro sm(m_shader.model);

			sm.AddMacro("GS_IIP", sel.iip);
			sm.AddMacro("GS_PRIM", sel.prim);
			sm.AddMacro("GS_POINT", sel.point);
			sm.AddMacro("GS_LINE", sel.line);

			std::vector<char> shader;
			theApp.LoadResource(IDR_TFX_FX, shader);
			CreateShader(shader, "tfx.fx", nullptr, "gs_main", sm.GetPtr(), &gs);

			m_gs[sel] = gs;
		}
	}


	if (m_gs_cb_cache.Update(cb))
	{
		ID3D11DeviceContext* ctx = m_ctx;

		ctx->UpdateSubresource(m_gs_cb, 0, NULL, cb, 0, 0);
	}

	GSSetShader(gs, m_gs_cb);
}

void GSDeviceProxy::SetupPS(PSSelector sel, const PSConstantBuffer* cb, PSSamplerSelector ssel)
{
	auto i = std::as_const(m_ps).find(sel);

	if (i == m_ps.end())
	{
		ShaderMacro sm(m_shader.model);

		sm.AddMacro("PS_SCALE_FACTOR", std::max(1, m_upscale_multiplier));
		sm.AddMacro("PS_FST", sel.fst);
		sm.AddMacro("PS_WMS", sel.wms);
		sm.AddMacro("PS_WMT", sel.wmt);
		sm.AddMacro("PS_FMT", sel.fmt);
		sm.AddMacro("PS_AEM", sel.aem);
		sm.AddMacro("PS_TFX", sel.tfx);
		sm.AddMacro("PS_TCC", sel.tcc);
		sm.AddMacro("PS_ATST", sel.atst);
		sm.AddMacro("PS_FOG", sel.fog);
		sm.AddMacro("PS_CLR1", sel.clr1);
		sm.AddMacro("PS_FBA", sel.fba);
		sm.AddMacro("PS_FBMASK", sel.fbmask);
		sm.AddMacro("PS_LTF", sel.ltf);
		sm.AddMacro("PS_TCOFFSETHACK", sel.tcoffsethack);
		sm.AddMacro("PS_POINT_SAMPLER", sel.point_sampler);
		sm.AddMacro("PS_SHUFFLE", sel.shuffle);
		sm.AddMacro("PS_READ_BA", sel.read_ba);
		sm.AddMacro("PS_CHANNEL_FETCH", sel.channel);
		sm.AddMacro("PS_TALES_OF_ABYSS_HLE", sel.tales_of_abyss_hle);
		sm.AddMacro("PS_URBAN_CHAOS_HLE", sel.urban_chaos_hle);
		sm.AddMacro("PS_DFMT", sel.dfmt);
		sm.AddMacro("PS_DEPTH_FMT", sel.depth_fmt);
		sm.AddMacro("PS_PAL_FMT", sel.fmt >> 2);
		sm.AddMacro("PS_INVALID_TEX0", sel.invalid_tex0);
		sm.AddMacro("PS_HDR", sel.hdr);
		sm.AddMacro("PS_COLCLIP", sel.colclip);
		sm.AddMacro("PS_BLEND_A", sel.blend_a);
		sm.AddMacro("PS_BLEND_B", sel.blend_b);
		sm.AddMacro("PS_BLEND_C", sel.blend_c);
		sm.AddMacro("PS_BLEND_D", sel.blend_d);
		sm.AddMacro("PS_DITHER", sel.dither);
		sm.AddMacro("PS_ZCLAMP", sel.zclamp);

		CComPtr<ID3D11PixelShader> ps;

		std::vector<char> shader;
		theApp.LoadResource(IDR_TFX_FX, shader);
		CreateShader(shader, "tfx.fx", nullptr, "ps_main", sm.GetPtr(), &ps);

		m_ps[sel] = ps;

		i = m_ps.find(sel);
	}

	if (m_ps_cb_cache.Update(cb))
	{
		ID3D11DeviceContext* ctx = m_ctx;

		ctx->UpdateSubresource(m_ps_cb, 0, NULL, cb, 0, 0);
	}

	CComPtr<ID3D11SamplerState> ss0, ss1;

	if (sel.tfx != 4)
	{
		if (!(sel.fmt < 3 && sel.wms < 3 && sel.wmt < 3))
		{
			ssel.ltf = 0;
		}

		auto i = std::as_const(m_ps_ss).find(ssel);

		if (i != m_ps_ss.end())
		{
			ss0 = i->second;
		}
		else
		{
			D3D11_SAMPLER_DESC sd, af;

			memset(&sd, 0, sizeof(sd));

			af.Filter = m_aniso_filter ? D3D11_FILTER_ANISOTROPIC : D3D11_FILTER_MIN_MAG_LINEAR_MIP_POINT;
			sd.Filter = ssel.ltf ? af.Filter : D3D11_FILTER_MIN_MAG_MIP_POINT;

			sd.AddressU = ssel.tau ? D3D11_TEXTURE_ADDRESS_WRAP : D3D11_TEXTURE_ADDRESS_CLAMP;
			sd.AddressV = ssel.tav ? D3D11_TEXTURE_ADDRESS_WRAP : D3D11_TEXTURE_ADDRESS_CLAMP;
			sd.AddressW = D3D11_TEXTURE_ADDRESS_CLAMP;
			sd.MinLOD = -FLT_MAX;
			sd.MaxLOD = FLT_MAX;
			sd.MaxAnisotropy = m_aniso_filter;
			sd.ComparisonFunc = D3D11_COMPARISON_NEVER;

			m_dev->CreateSamplerState(&sd, &ss0);

			m_ps_ss[ssel] = ss0;
		}

		if (sel.fmt >= 3)
		{
			ss1 = m_palette_ss;
		}
	}

	PSSetSamplerState(ss0, ss1);

	PSSetShader(i->second, m_ps_cb);
}

void GSDeviceProxy::SetupOM(OMDepthStencilSelector dssel, OMBlendSelector bsel, uint8 afix)
{
	auto i = std::as_const(m_om_dss).find(dssel);

	if (i == m_om_dss.end())
	{
		D3D11_DEPTH_STENCIL_DESC dsd;

		memset(&dsd, 0, sizeof(dsd));

		if (dssel.date)
		{
			dsd.StencilEnable = true;
			dsd.StencilReadMask = 1;
			dsd.StencilWriteMask = 1;
			dsd.FrontFace.StencilFunc = D3D11_COMPARISON_EQUAL;
			dsd.FrontFace.StencilPassOp = dssel.date_one ? D3D11_STENCIL_OP_ZERO : D3D11_STENCIL_OP_KEEP;
			dsd.FrontFace.StencilFailOp = D3D11_STENCIL_OP_KEEP;
			dsd.FrontFace.StencilDepthFailOp = D3D11_STENCIL_OP_KEEP;
			dsd.BackFace.StencilFunc = D3D11_COMPARISON_EQUAL;
			dsd.BackFace.StencilPassOp = dssel.date_one ? D3D11_STENCIL_OP_ZERO : D3D11_STENCIL_OP_KEEP;
			dsd.BackFace.StencilFailOp = D3D11_STENCIL_OP_KEEP;
			dsd.BackFace.StencilDepthFailOp = D3D11_STENCIL_OP_KEEP;
		}

		if (dssel.ztst != ZTST_ALWAYS || dssel.zwe)
		{
			static const D3D11_COMPARISON_FUNC ztst[] =
			{
				D3D11_COMPARISON_NEVER,
				D3D11_COMPARISON_ALWAYS,
				D3D11_COMPARISON_GREATER_EQUAL,
				D3D11_COMPARISON_GREATER
			};

			dsd.DepthEnable = true;
			dsd.DepthWriteMask = dssel.zwe ? D3D11_DEPTH_WRITE_MASK_ALL : D3D11_DEPTH_WRITE_MASK_ZERO;
			dsd.DepthFunc = ztst[dssel.ztst];
		}

		CComPtr<ID3D11DepthStencilState> dss;

		m_dev->CreateDepthStencilState(&dsd, &dss);

		m_om_dss[dssel] = dss;

		i = m_om_dss.find(dssel);
	}

	OMSetDepthStencilState(i->second, 1);

	auto j = std::as_const(m_om_bs).find(bsel);

	if (j == m_om_bs.end())
	{
		D3D11_BLEND_DESC bd;

		memset(&bd, 0, sizeof(bd));

		bd.RenderTarget[0].BlendEnable = bsel.abe;

		if (bsel.abe)
		{
			int i = ((bsel.a * 3 + bsel.b) * 3 + bsel.c) * 3 + bsel.d;

			HWBlend blend = GetBlend(i);
			bd.RenderTarget[0].BlendOp = (D3D11_BLEND_OP)blend.op;
			bd.RenderTarget[0].SrcBlend = (D3D11_BLEND)blend.src;
			bd.RenderTarget[0].DestBlend = (D3D11_BLEND)blend.dst;
			bd.RenderTarget[0].BlendOpAlpha = D3D11_BLEND_OP_ADD;
			bd.RenderTarget[0].SrcBlendAlpha = D3D11_BLEND_ONE;
			bd.RenderTarget[0].DestBlendAlpha = D3D11_BLEND_ZERO;

			if (bsel.accu_blend)
			{
				bd.RenderTarget[0].SrcBlend = D3D11_BLEND_ONE;
				bd.RenderTarget[0].DestBlend = D3D11_BLEND_ONE;
			}
		}

		if (bsel.wr) bd.RenderTarget[0].RenderTargetWriteMask |= D3D11_COLOR_WRITE_ENABLE_RED;
		if (bsel.wg) bd.RenderTarget[0].RenderTargetWriteMask |= D3D11_COLOR_WRITE_ENABLE_GREEN;
		if (bsel.wb) bd.RenderTarget[0].RenderTargetWriteMask |= D3D11_COLOR_WRITE_ENABLE_BLUE;
		if (bsel.wa) bd.RenderTarget[0].RenderTargetWriteMask |= D3D11_COLOR_WRITE_ENABLE_ALPHA;

		CComPtr<ID3D11BlendState> bs;

		m_dev->CreateBlendState(&bd, &bs);

		m_om_bs[bsel] = bs;

		j = m_om_bs.find(bsel);
	}

	OMSetBlendState(j->second, (float)(int)afix / 0x80);
}

void GSDeviceProxy::Flip()
{
    m_ctx->Flush();

    m_ctx->CopyResource(m_SharedTexture, m_RenderTargetTexture);

    m_ctx->CopyResource(m_CaptureTexture, m_RenderTargetTexture);
}

void GSDeviceProxy::BeforeDraw()
{
	// DX can't read from the FB
	// So let's copy it and send that to the shader instead

	auto bits = m_state.ps_sr_bitfield;
	m_state.ps_sr_bitfield = 0;

	unsigned long i;
	while (_BitScanForward(&i, bits))
	{
		GSTexture11* tex = m_state.ps_sr_texture[i];

		if (tex->Equal(m_state.rt_texture) || tex->Equal(m_state.rt_ds))
		{
#ifdef _DEBUG
			OutputDebugString(format("WARNING: FB read detected on slot %i, copying...", i).c_str());
#endif
			GSTexture* cp = nullptr;

			CloneTexture(tex, &cp);

			PSSetShaderResource(i, cp);
		}

		bits ^= 1u << i;
	}

	PSUpdateShaderState();
}

void GSDeviceProxy::AfterDraw()
{
	unsigned long i;
	while (_BitScanForward(&i, m_state.ps_sr_bitfield))
	{
#ifdef _DEBUG
		OutputDebugString(format("WARNING: Cleaning up copied texture on slot %i", i).c_str());
#endif
		Recycle(m_state.ps_sr_texture[i]);
		PSSetShaderResource(i, NULL);
	}
}

bool GSDeviceProxy::Create(const std::shared_ptr<GSWnd> &wnd, void *sharedhandle, void *capturehandle, void *directXDeviceNative)
{
    bool nvidia_vendor = false;

    if (!__super::Create(wnd)) {
        return false;
    }

    HRESULT hr = E_FAIL;

    D3D11_BUFFER_DESC bd;
    D3D11_SAMPLER_DESC sd;
    D3D11_DEPTH_STENCIL_DESC dsd;
    D3D11_RASTERIZER_DESC rd;
    D3D11_BLEND_DESC bsd;

    CComPtr<IDXGIAdapter1> adapter;
    D3D_DRIVER_TYPE driver_type = D3D_DRIVER_TYPE_HARDWARE;

    std::string adapter_id = theApp.GetConfigS("Adapter");

    if (adapter_id == "ref") {
        driver_type = D3D_DRIVER_TYPE_REFERENCE;
    } else {
        CComPtr<IDXGIFactory1> dxgi_factory;
        CreateDXGIFactory1(__uuidof(IDXGIFactory1), (void **)&dxgi_factory);
        if (dxgi_factory)
            for (int i = 0;; i++) {
                CComPtr<IDXGIAdapter1> enum_adapter;
                if (S_OK != dxgi_factory->EnumAdapters1(i, &enum_adapter))
                    break;
                DXGI_ADAPTER_DESC1 desc;
                hr = enum_adapter->GetDesc1(&desc);
                if (S_OK == hr && (GSAdapter(desc) == adapter_id || adapter_id == "default")) {
                    if (desc.VendorId == 0x10DE)
                        nvidia_vendor = true;

                    adapter = enum_adapter;
                    driver_type = D3D_DRIVER_TYPE_UNKNOWN;
                    break;
                }
            }
    }
	
    // NOTE : D3D11_CREATE_DEVICE_SINGLETHREADED
    //   This flag is safe as long as the DXGI's internal message pump is disabled or is on the
    //   same thread as the GS window (which the emulator makes sure of, if it utilizes a
    //   multithreaded GS).  Setting the flag is a nice and easy 5% speedup on GS-intensive scenes.

    uint32 flags = D3D11_CREATE_DEVICE_BGRA_SUPPORT; //D3D11_CREATE_DEVICE_SINGLETHREADED;

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

    if (!SetFeatureLevel(level, true)) {
        return false;
    }

    { // HACK: check nVIDIA
        // Note: It can cause issues on several games such as SOTC, Fatal Frame, plus it adds border offset.
        bool disable_safe_features = theApp.GetConfigB("UserHacks") && theApp.GetConfigB("UserHacks_Disable_Safe_Features");
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

    m_RenderTargetTexture.Release();

    hr = m_dev->CreateTexture2D(&l_Desc, nullptr, &m_RenderTargetTexture);

    if (FAILED(hr))
        return false;

    l_Resource.Release();

	if (capturehandle != nullptr) {

        hr = m_dev->OpenSharedResource(capturehandle, IID_PPV_ARGS(&l_Resource));

        if (FAILED(hr))
            return false;

        hr = l_Resource->QueryInterface(IID_PPV_ARGS(&m_CaptureTexture));

        if (FAILED(hr))
            return false;
	}

    if (!wnd->Create("GS", l_Desc.Width, l_Desc.Height)) {
        return -1;
    }

	if (l_Desc.Height > 720) {

        auto l_upscale_multiplier = ceilf(((float)l_Desc.Height) / 720.0f) * 2.0f;

        m_upscale_multiplier = (int)l_upscale_multiplier;

        theApp.SetConfig("upscale_multiplier", m_upscale_multiplier);
	}

    D3D11_FEATURE_DATA_D3D10_X_HARDWARE_OPTIONS options;

    hr = m_dev->CheckFeatureSupport(D3D11_FEATURE_D3D10_X_HARDWARE_OPTIONS, &options, sizeof(D3D11_FEATURE_D3D10_X_HARDWARE_OPTIONS));

    // convert

    D3D11_INPUT_ELEMENT_DESC il_convert[] =
        {
            {"POSITION", 0, DXGI_FORMAT_R32G32B32A32_FLOAT, 0, 0, D3D11_INPUT_PER_VERTEX_DATA, 0},
            {"TEXCOORD", 0, DXGI_FORMAT_R32G32_FLOAT, 0, 16, D3D11_INPUT_PER_VERTEX_DATA, 0},
            {"COLOR", 0, DXGI_FORMAT_R8G8B8A8_UNORM, 0, 28, D3D11_INPUT_PER_VERTEX_DATA, 0},
        };

    ShaderMacro sm_model(m_shader.model);

    std::vector<char> shader;
    theApp.LoadResource(IDR_CONVERT_FX, shader);
    CreateShader(shader, "convert.fx", nullptr, "vs_main", sm_model.GetPtr(), &m_convert.vs, il_convert, countof(il_convert), &m_convert.il);

    ShaderMacro sm_convert(m_shader.model);
    sm_convert.AddMacro("PS_SCALE_FACTOR", std::max(1, m_upscale_multiplier));

    D3D_SHADER_MACRO *sm_convert_ptr = sm_convert.GetPtr();

    for (size_t i = 0; i < countof(m_convert.ps); i++) {
        CreateShader(shader, "convert.fx", nullptr, format("ps_main%d", i).c_str(), sm_convert_ptr, &m_convert.ps[i]);
    }

    memset(&dsd, 0, sizeof(dsd));

    hr = m_dev->CreateDepthStencilState(&dsd, &m_convert.dss);

    dsd.DepthEnable = true;
    dsd.DepthWriteMask = D3D11_DEPTH_WRITE_MASK_ALL;
    dsd.DepthFunc = D3D11_COMPARISON_ALWAYS;

    hr = m_dev->CreateDepthStencilState(&dsd, &m_convert.dss_write);

    memset(&bsd, 0, sizeof(bsd));

    bsd.RenderTarget[0].RenderTargetWriteMask = D3D11_COLOR_WRITE_ENABLE_ALL;

    hr = m_dev->CreateBlendState(&bsd, &m_convert.bs);

    // merge

    memset(&bd, 0, sizeof(bd));

    bd.ByteWidth = sizeof(MergeConstantBuffer);
    bd.Usage = D3D11_USAGE_DEFAULT;
    bd.BindFlags = D3D11_BIND_CONSTANT_BUFFER;

    hr = m_dev->CreateBuffer(&bd, NULL, &m_merge.cb);

    theApp.LoadResource(IDR_MERGE_FX, shader);
    for (size_t i = 0; i < countof(m_merge.ps); i++) {
        CreateShader(shader, "merge.fx", nullptr, format("ps_main%d", i).c_str(), sm_model.GetPtr(), &m_merge.ps[i]);
    }

    memset(&bsd, 0, sizeof(bsd));

    bsd.RenderTarget[0].BlendEnable = true;
    bsd.RenderTarget[0].BlendOp = D3D11_BLEND_OP_ADD;
    bsd.RenderTarget[0].SrcBlend = D3D11_BLEND_SRC_ALPHA;
    bsd.RenderTarget[0].DestBlend = D3D11_BLEND_INV_SRC_ALPHA;
    bsd.RenderTarget[0].BlendOpAlpha = D3D11_BLEND_OP_ADD;
    bsd.RenderTarget[0].SrcBlendAlpha = D3D11_BLEND_ONE;
    bsd.RenderTarget[0].DestBlendAlpha = D3D11_BLEND_ZERO;
    bsd.RenderTarget[0].RenderTargetWriteMask = D3D11_COLOR_WRITE_ENABLE_ALL;

    hr = m_dev->CreateBlendState(&bsd, &m_merge.bs);

    // interlace

    memset(&bd, 0, sizeof(bd));

    bd.ByteWidth = sizeof(InterlaceConstantBuffer);
    bd.Usage = D3D11_USAGE_DEFAULT;
    bd.BindFlags = D3D11_BIND_CONSTANT_BUFFER;

    hr = m_dev->CreateBuffer(&bd, NULL, &m_interlace.cb);

    theApp.LoadResource(IDR_INTERLACE_FX, shader);
    for (size_t i = 0; i < countof(m_interlace.ps); i++) {
        CreateShader(shader, "interlace.fx", nullptr, format("ps_main%d", i).c_str(), sm_model.GetPtr(), &m_interlace.ps[i]);
    }

    // Shade Boost

    ShaderMacro sm_sboost(m_shader.model);

    sm_sboost.AddMacro("SB_SATURATION", theApp.GetConfigI("ShadeBoost_Saturation"));
    sm_sboost.AddMacro("SB_BRIGHTNESS", theApp.GetConfigI("ShadeBoost_Brightness"));
    sm_sboost.AddMacro("SB_CONTRAST", theApp.GetConfigI("ShadeBoost_Contrast"));

    memset(&bd, 0, sizeof(bd));

    bd.ByteWidth = sizeof(ShadeBoostConstantBuffer);
    bd.Usage = D3D11_USAGE_DEFAULT;
    bd.BindFlags = D3D11_BIND_CONSTANT_BUFFER;

    hr = m_dev->CreateBuffer(&bd, NULL, &m_shadeboost.cb);

    theApp.LoadResource(IDR_SHADEBOOST_FX, shader);
    CreateShader(shader, "shadeboost.fx", nullptr, "ps_main", sm_sboost.GetPtr(), &m_shadeboost.ps);

    // External fx shader

    memset(&bd, 0, sizeof(bd));

    bd.ByteWidth = sizeof(ExternalFXConstantBuffer);
    bd.Usage = D3D11_USAGE_DEFAULT;
    bd.BindFlags = D3D11_BIND_CONSTANT_BUFFER;

    hr = m_dev->CreateBuffer(&bd, NULL, &m_shaderfx.cb);

    // Fxaa

    memset(&bd, 0, sizeof(bd));

    bd.ByteWidth = sizeof(FXAAConstantBuffer);
    bd.Usage = D3D11_USAGE_DEFAULT;
    bd.BindFlags = D3D11_BIND_CONSTANT_BUFFER;

    hr = m_dev->CreateBuffer(&bd, NULL, &m_fxaa.cb);

    //

    memset(&rd, 0, sizeof(rd));

    rd.FillMode = D3D11_FILL_SOLID;
    rd.CullMode = D3D11_CULL_NONE;
    rd.FrontCounterClockwise = false;
    rd.DepthBias = false;
    rd.DepthBiasClamp = 0;
    rd.SlopeScaledDepthBias = 0;
    rd.DepthClipEnable = false; // ???
    rd.ScissorEnable = true;
    rd.MultisampleEnable = true;
    rd.AntialiasedLineEnable = false;

    hr = m_dev->CreateRasterizerState(&rd, &m_solid_rs);

    m_ctx->RSSetState(m_solid_rs);

    rd.FillMode = D3D11_FILL_WIREFRAME;

    hr = m_dev->CreateRasterizerState(&rd, &m_wired_rs);

    //

    memset(&rd, 0, sizeof(rd));

    rd.FillMode = D3D11_FILL_SOLID;
    rd.CullMode = D3D11_CULL_NONE;
    rd.FrontCounterClockwise = false;
    rd.DepthBias = false;
    rd.DepthBiasClamp = 0;
    rd.SlopeScaledDepthBias = 0;
    rd.DepthClipEnable = false; // ???
    rd.ScissorEnable = true;
    rd.MultisampleEnable = true;
    rd.AntialiasedLineEnable = false;

    hr = m_dev->CreateRasterizerState(&rd, &m_rs);

    m_ctx->RSSetState(m_rs);

    //

    memset(&sd, 0, sizeof(sd));

    sd.Filter = m_aniso_filter ? D3D11_FILTER_ANISOTROPIC : D3D11_FILTER_MIN_MAG_MIP_LINEAR;
    sd.AddressU = D3D11_TEXTURE_ADDRESS_CLAMP;
    sd.AddressV = D3D11_TEXTURE_ADDRESS_CLAMP;
    sd.AddressW = D3D11_TEXTURE_ADDRESS_CLAMP;
    sd.MinLOD = -FLT_MAX;
    sd.MaxLOD = FLT_MAX;
    sd.MaxAnisotropy = m_aniso_filter;
    sd.ComparisonFunc = D3D11_COMPARISON_NEVER;

    hr = m_dev->CreateSamplerState(&sd, &m_convert.ln);

    sd.Filter = m_aniso_filter ? D3D11_FILTER_ANISOTROPIC : D3D11_FILTER_MIN_MAG_MIP_POINT;

    hr = m_dev->CreateSamplerState(&sd, &m_convert.pt);

    //

    Reset(1, 1);

    //

    CreateTextureFX();

    //

    memset(&dsd, 0, sizeof(dsd));

    dsd.DepthEnable = false;
    dsd.StencilEnable = true;
    dsd.StencilReadMask = 1;
    dsd.StencilWriteMask = 1;
    dsd.FrontFace.StencilFunc = D3D11_COMPARISON_ALWAYS;
    dsd.FrontFace.StencilPassOp = D3D11_STENCIL_OP_REPLACE;
    dsd.FrontFace.StencilFailOp = D3D11_STENCIL_OP_KEEP;
    dsd.FrontFace.StencilDepthFailOp = D3D11_STENCIL_OP_KEEP;
    dsd.BackFace.StencilFunc = D3D11_COMPARISON_ALWAYS;
    dsd.BackFace.StencilPassOp = D3D11_STENCIL_OP_REPLACE;
    dsd.BackFace.StencilFailOp = D3D11_STENCIL_OP_KEEP;
    dsd.BackFace.StencilDepthFailOp = D3D11_STENCIL_OP_KEEP;

    m_dev->CreateDepthStencilState(&dsd, &m_date.dss);

    D3D11_BLEND_DESC blend;

    memset(&blend, 0, sizeof(blend));

    m_dev->CreateBlendState(&blend, &m_date.bs);

    // Exclusive/Fullscreen flip, issued for legacy (managed) windows only.  GSopen2 style
    // emulators will issue the flip themselves later on.
		
    return true;
}

bool GSDeviceProxy::Reset(int w, int h)
{
    if (!__super::Reset(w, h))
        return false;

    if (m_RenderTargetTexture)
        m_backbuffer = new GSTexture11(m_RenderTargetTexture);
    else
        return false;

	ClearRenderTarget(m_backbuffer, 0);

    Flip();

	if (m_ctx)
		m_ctx->Flush();

    return true;
}



void GSDeviceProxy::setIsWired(BOOL a_value)
{
    m_is_wired = a_value;
}

void GSDeviceProxy::setIsTessellated(BOOL a_value)
{
    m_is_tessellated = a_value;
}


