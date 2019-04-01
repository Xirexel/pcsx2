
/*
*	Copyright (C) 2018 Evgeny Pereguda
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

#include "Window\GSWnd.h"

class GSWndStub : public GSWnd
{
	int width;
	int height;

public:
	GSWndStub();
	virtual ~GSWndStub();

	bool Create(const std::string& title, int w, int h);
	bool Attach(void* handle, bool managed = true);
	void Detach();

	void* GetDisplay();
	void* GetHandle();
	GSVector4i GetClientRect();
	bool SetWindowText(const char* title);

	void Show();
	void Hide();
	void HideFrame();
};