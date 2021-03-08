
#include "stdafx.h"
#include "Window/GSWndDX.h"
#include "Renderers/DX11/GPURendererDX11.h"
#include "Renderers/DX11/GPUDevice11.h"

#include <shlobj.h>
#include <shellapi.h>

GPURenderer* s_gpu = nullptr;

static UINT32 s_currentTexName = 0;


#define EXPORT_C_(type) extern "C" type __stdcall
#define EXPORT_C EXPORT_C_(void)

static UINT32 s_Texture_count = 1;

void APIENTRY glGenTextures(GLsizei n, GLuint *textures)
{
	for (size_t i = 0; i < n; i++)
	{
		textures[i] = ++s_Texture_count;
	}
}

void APIENTRY glReadBuffer(GLenum mode){}

void APIENTRY glPixelZoom(GLfloat xfactor, GLfloat yfactor) {}

void APIENTRY glDrawPixels(GLsizei width, GLsizei height, GLenum format, GLenum type, const GLvoid *pixels) {}

GLboolean APIENTRY glAreTexturesResident(GLsizei n, const GLuint *textures, GLboolean *residences) { return GL_TRUE; }

void APIENTRY glRasterPos2f(GLfloat x, GLfloat y) {}

void APIENTRY glViewport(GLint x, GLint y, GLsizei width, GLsizei height) {}

void APIENTRY glReadPixels(GLint x, GLint y, GLsizei width, GLsizei height, GLenum format, GLenum type, GLvoid *pixels) {}

void APIENTRY glTexEnvf(GLenum target, GLenum pname, GLfloat param) {}

EXPORT_C swapBuffers()
{
	if (s_gpu != NULL)
		s_gpu->VSync(0);
}

EXPORT_C clearBuffers(DWORD aMask)
{
	if (s_gpu != NULL)
	{
		if (aMask & GL_COLOR_BUFFER_BIT)
			s_gpu->ClearRenderTarget();
		
		if (aMask & GL_DEPTH_BUFFER_BIT)
			s_gpu->ClearDepth();
	}
}

EXPORT_C setClearColor(float aRed, float aGreen, float aBlue, float aAlpha)
{
	if (s_gpu != NULL)
	{
		s_gpu->setClearColor(aRed, aGreen, aBlue, aAlpha);
	}
}

EXPORT_C setOrtho(double aLeft, double aRight, double aBottom, double aTop, double azNear, double azFar)
{
	if (s_gpu != NULL)
	{
		s_gpu->setOrtho(aLeft, aRight, aBottom, aTop, azNear, azFar);
	}
}

EXPORT_C draw(DWORD aDrawMode, void* aPtrVertexes, BOOL aIsTextured)
{
	if (s_gpu != NULL)
	{
		s_gpu->draw(aDrawMode, aPtrVertexes, aIsTextured);
	}
}

EXPORT_C glBindTextureStub(GLenum target, GLuint texture)
{	
	if (s_gpu != NULL)
	{
		s_gpu->setTexture(texture);

		s_currentTexName = texture;
	}
}

EXPORT_C glTexImage2DStub(GLenum target, GLint level, GLint internalformat, GLsizei width, GLsizei height, GLint border, GLenum format, GLenum type, const GLvoid *pixels)
{	   	  	
	if (s_gpu != NULL)
	{
		s_gpu->addTexture(s_currentTexName, internalformat, width, height, border, format, type, pixels);

        s_gpu->setTexture(s_currentTexName);
	}
}

EXPORT_C glAlphaFuncStub(GLenum func, GLclampf ref)
{
	if (s_gpu != NULL)
	{
		s_gpu->setAlphaFunc(func, ref);
	}
}

EXPORT_C glCopyTexSubImage2DStub(GLenum target, GLint level, GLint xoffset, GLint yoffset, GLint x, GLint y, GLsizei width, GLsizei height)
{
	if (s_gpu != NULL)
	{
		s_gpu->copyTexSubImage2D(s_currentTexName, xoffset, yoffset, x, y, width, height);
	}
}

EXPORT_C glTexSubImage2DStub(GLenum target, GLint level, GLint xoffset, GLint yoffset, GLsizei width, GLsizei height, GLenum format, GLenum type, const GLvoid *pixels)
{		
	if (s_gpu != NULL)
	{
		s_gpu->updateTexture(s_currentTexName, xoffset, yoffset, width, height, format, type, pixels);
	}
}

EXPORT_C glDeleteTexturesStub(GLsizei n, const GLuint *textures)
{
	if (s_gpu != NULL)
	{
		for (size_t i = 0; i < n; i++)
		{
			s_gpu->deleteTexture(textures[i]);
		}
	}
}

EXPORT_C glBlendFuncStub(GLenum sfactor, GLenum dfactor)
{
	if (s_gpu != NULL)
	{
		s_gpu->setBlendFunc(sfactor, dfactor);
	}
}

EXPORT_C glBlendEquationEXTExStub(GLenum mode)
{
	if (s_gpu != NULL)
	{
		s_gpu->setBlendEquation(mode);
	}
}

EXPORT_C glEnableStub(GLenum cap)
{
	if (s_gpu != NULL && (cap & GL_BLEND) == GL_BLEND)
	{
		s_gpu->enableBlend(TRUE);
	}

	if (s_gpu != NULL && cap == GL_SCISSOR_TEST)
	{
		s_gpu->enableScissor(TRUE);
	}
}

EXPORT_C glDisableStub(GLenum cap)
{
	if (s_gpu != NULL && (cap & GL_BLEND) == GL_BLEND)
	{
		s_gpu->enableBlend(FALSE);
	}

	if (s_gpu != NULL && cap == GL_SCISSOR_TEST)
	{
		s_gpu->enableScissor(FALSE);
	}
}

EXPORT_C glDepthFuncStub(GLenum func)
{
	if (s_gpu != NULL)
	{
		s_gpu->setDepthFunc(func);
	}
}

EXPORT_C glScissorStub(GLint x, GLint y, GLsizei width, GLsizei height)
{
	if (s_gpu != NULL)
	{
		s_gpu->setScissor(x, y, width, height);
	}
}
