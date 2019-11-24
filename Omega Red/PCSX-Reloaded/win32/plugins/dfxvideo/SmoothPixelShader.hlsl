// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//----------------------------------------------------------------------

Texture2D tx : register( t0 );
SamplerState samLinear : register( s0 );

struct PS_INPUT
{
    float4 Pos : SV_POSITION;
    float2 Tex : TEXCOORD;
};

//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------
float4 SmoothPS(PS_INPUT input) : SV_Target
{
    // TOP ROW
    float4 s11 = tx.Sample(samLinear, input.Tex + float2(-1.0f / 1024.0f, -1.0f / 768.0f)); // LEFT
    float4 s12 = tx.Sample(samLinear, input.Tex + float2(0, -1.0f / 768.0f));               // MIDDLE
    float4 s13 = tx.Sample(samLinear, input.Tex + float2(1.0f / 1024.0f, -1.0f / 768.0f));  // RIGHT

    // MIDDLE ROW
    float4 s21 = tx.Sample(samLinear, input.Tex + float2(-1.0f / 1024.0f, 0)); // LEFT
    float4 col = tx.Sample(samLinear, input.Tex);                              // DEAD CENTER
    float4 s23 = tx.Sample(samLinear, input.Tex + float2(-1.0f / 1024.0f, 0)); // RIGHT

    // LAST ROW
    float4 s31 = tx.Sample(samLinear, input.Tex + float2(-1.0f / 1024.0f, 1.0f / 768.0f)); // LEFT
    float4 s32 = tx.Sample(samLinear, input.Tex + float2(0, 1.0f / 768.0f));               // MIDDLE
    float4 s33 = tx.Sample(samLinear, input.Tex + float2(1.0f / 1024.0f, 1.0f / 768.0f));  // RIGHT

    // Average the color with surrounding samples
    col = (col + s11 + s12 + s13 + s21 + s23 + s31 + s32 + s33) / 9;
    return col;
}