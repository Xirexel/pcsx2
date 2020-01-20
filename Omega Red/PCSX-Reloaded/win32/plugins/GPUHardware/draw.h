#pragma once

// internally used defines

#define GPUCOMMAND(x) ((x>>24) & 0xff)
#define RED(x) (x & 0xff)
#define BLUE(x) ((x>>16) & 0xff)
#define GREEN(x) ((x>>8) & 0xff)
#define COLOR(x) (x & 0xffffff)

extern GLbitfield     uiBufferBits;

extern void SetOGLDisplaySettings(BOOL DisplaySet);