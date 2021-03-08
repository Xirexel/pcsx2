struct VS_INPUT
{
	float4 Pos :    POSITION;
	float2 Tex :    TEXCOORD;
    float4 Color:   COLOR;
};

struct VS_OUTPUT
{
    float4 Pos :    SV_POSITION;
    float2 Tex :    TEXCOORD;
    float4 Color :  COLOR;
};

cbuffer PROJECTION_BUFFER : register(b0)
{
    matrix mProj;
};

//--------------------------------------------------------------------------------------
// Vertex Shader
//--------------------------------------------------------------------------------------
VS_OUTPUT VS(VS_INPUT input)
{
    VS_OUTPUT output;
    
    output.Pos = mul(input.Pos, mProj);
    
    output.Tex = input.Tex;
    
    output.Color = input.Color;
    
    return output;
}