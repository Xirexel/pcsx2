// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//----------------------------------------------------------------------

/* AlphaFunction */
#define GL_NEVER                          0x0200
#define GL_LESS                           0x0201
#define GL_EQUAL                          0x0202
#define GL_LEQUAL                         0x0203
#define GL_GREATER                        0x0204
#define GL_NOTEQUAL                       0x0205
#define GL_GEQUAL                         0x0206
#define GL_ALWAYS                         0x0207

Texture2D tx : register( t0 );
SamplerState samLinear : register(s0);

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
};

//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------
float4 PS(PS_INPUT input) : SV_Target
{
    float4 color = tx.Sample(samLinear, input.Tex) * input.Color;
        
    if (mFunc == GL_GREATER)
    {
        if (color.a <= mValue * 0.4)
            discard;
    }
    else if (mFunc == GL_EQUAL)
    {
        if (color.a != mValue)
            discard;
    }
    else if (mFunc == GL_NOTEQUAL)
    {
        if (color.a == mValue)
            discard;
    }
    
    return float4(color.rgb * 2.0, color.a);
    //return color * 2.0;
}