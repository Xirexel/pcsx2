#pragma once


/* AlphaFunction */
#define GL_NEVER 0x0200
#define GL_LESS 0x0201
#define GL_EQUAL 0x0202
#define GL_LEQUAL 0x0203
#define GL_GREATER 0x0204
#define GL_NOTEQUAL 0x0205
#define GL_GEQUAL 0x0206
#define GL_ALWAYS 0x0207


struct PS_INPUT
{
    float4 Pos : SV_POSITION;
    float2 Tex : TEXCOORD;
    float4 Color : COLOR;
};

cbuffer ALPHA_FUNC_BUFFER : register(b0)
{
    uint mFunc;
    float mValue;
    float padding1;
    float padding2;
};


SamplerState samplerLinear : register(s0);
SamplerState samplerPoint : register(s1);

Texture2D tx : TEXUNIT0;