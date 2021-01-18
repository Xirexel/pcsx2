// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//----------------------------------------------------------------------

#include "CommonShader.h"

//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------
float4 PS(PS_INPUT input) : SV_Target
{    
    float3 color = tx.Sample(samplerLinear, input.Tex).rgb * input.Color.rgb;
        
    float alpha = tx.Sample(samplerPoint, input.Tex).a * input.Color.a;
        
    if (mFunc == GL_GREATER)
    {
        if (alpha <= mValue)
            discard;
    }else
    if (mFunc == GL_EQUAL)
    {
        if (alpha != mValue)
            discard;
    }else 
	if (mFunc == GL_NOTEQUAL)
    {
        if (alpha == mValue)
            discard;
    }
    
    return float4(color.rgb * 2.0, alpha);
}