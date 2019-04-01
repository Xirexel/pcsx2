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

#pragma once

#include "Renderers/HW/GSRendererHW.h"
#include "Renderers/DX11/GSTextureCache11.h"
#include "Renderers/HW/GSVertexHW.h"
#include "GSDeviceProxy.h"

class GSRendererProxy : public GSRendererHW
{
private:
    bool UserHacks_AlphaHack;
    bool UserHacks_AlphaStencil;

private:
    inline void ResetStates();
    inline void SetupIA(const float &sx, const float &sy);
    inline void EmulateAtst(const int pass, const GSTextureCache::Source *tex);
    inline void EmulateZbuffer();
    inline void EmulateTextureShuffleAndFbmask();
    inline void EmulateChannelShuffle(GSTexture **rt, const GSTextureCache::Source *tex);
    inline void EmulateTextureSampler(const GSTextureCache::Source *tex);

    GSDeviceProxy::VSSelector m_vs_sel;
    GSDeviceProxy::GSSelector m_gs_sel;
    GSDeviceProxy::PSSelector m_ps_sel;

    GSDeviceProxy::PSSamplerSelector m_ps_ssel;
    GSDeviceProxy::OMBlendSelector m_om_bsel;
    GSDeviceProxy::OMDepthStencilSelector m_om_dssel;

    GSDeviceProxy::PSConstantBuffer ps_cb;
    GSDeviceProxy::VSConstantBuffer vs_cb;
    GSDeviceProxy::GSConstantBuffer gs_cb;

public:
    GSRendererProxy();
    virtual ~GSRendererProxy() {}

    void DrawPrims(GSTexture *rt, GSTexture *ds, GSTextureCache::Source *tex) final;

    bool CreateDevice(GSDeviceProxy *dev, void *sharedhandle, void *capturehandle);
	
    void setIsWired(BOOL a_value);

    void setIsTessellated(BOOL a_value);

    void setFXAA(BOOL a_value);
};
