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
#include "GSRendererProxy.h"

bool GSRendererProxy::CreateDevice(GSDeviceProxy *dev, void *sharedhandle, void *capturehandle, void *directXDeviceNative)
{
    ASSERT(dev);
    ASSERT(!m_dev);

    if (!dev->Create(m_wnd, sharedhandle, capturehandle, directXDeviceNative)) {
        return false;
    }

    m_dev = (GSDevice *)dev;
    m_dev->SetVSync(m_vsync);

    return true;
}

void GSRendererProxy::setFXAA(BOOL a_value)
{
    if (a_value == FALSE)
        m_fxaa = false;
    else
        m_fxaa = true;
}

void GSRendererProxy::setIsWired(BOOL a_value)
{
    GSDeviceProxy *dev = (GSDeviceProxy *)m_dev;

    dev->setIsWired(a_value);
}

void GSRendererProxy::setIsTessellated(BOOL a_value)
{
    GSDeviceProxy *dev = (GSDeviceProxy *)m_dev;

    dev->setIsTessellated(a_value);
}
