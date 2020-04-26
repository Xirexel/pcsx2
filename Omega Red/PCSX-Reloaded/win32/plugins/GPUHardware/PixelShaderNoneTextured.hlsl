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
float4 PS_none_textured(PS_INPUT input) : SV_Target
{        
    if (mFunc == GL_GREATER)
    {
        if (input.Color.a <= mValue * 0.4)
            discard;
    }
    else if (mFunc == GL_EQUAL)
    {
        if (input.Color.a != mValue)
            discard;
    }
    else if (mFunc == GL_NOTEQUAL)
    {
        if (input.Color.a == mValue)
            discard;
    }
      
    return float4(input.Color.rgb, input.Color.a);
}