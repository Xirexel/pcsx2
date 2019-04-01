
struct VS_OUTPUT
{
    float4 p : SV_Position;
    float4 t : TEXCOORD0;
	float4 tp : TEXCOORD1;
    float4 c : COLOR0;
};

struct PatchTess
{
    float EdgeTess[3] : SV_TessFactor;
    float InsideTess : SV_InsideTessFactor;
};

PatchTess ConstantHS(InputPatch<VS_OUTPUT, 3> patch, uint patchID : SV_PrimitiveID)
{
    PatchTess pt;
	
    pt.EdgeTess[0] = 4;
    pt.EdgeTess[1] = 4;
    pt.EdgeTess[2] = 4;
	
    pt.InsideTess = 4;
	
    return pt;
}



struct HS_OUTPUT
{
    float4 p : SV_Position;
    float4 t : TEXCOORD0;
	float4 tp : TEXCOORD1;
    float4 c : COLOR0;
    //float4 normal : NORMAL;
};

[domain("tri")]
[partitioning("integer")]
[outputtopology("triangle_cw")]
[outputcontrolpoints(3)]
[patchconstantfunc("ConstantHS")]
[maxtessfactor(7.0f)]
HS_OUTPUT hs_main(InputPatch<VS_OUTPUT, 3> patch,
           uint i : SV_OutputControlPointID,
           uint patchId : SV_PrimitiveID)
{
    HS_OUTPUT hout;
	
    hout.p = patch[i].p;

    hout.t = patch[i].t;

    hout.tp = patch[i].tp ;

    hout.c = patch[i].c;

    //float4 vector1 = patch[0].p - patch[1].p;

    //float4 vector2 = patch[0].p - patch[2].p;

    //float3 cross = float3(
    //(vector1.y * vector2.z) - (vector2.y * vector1.z),
    //(vector1.z * vector2.x) - (vector2.z * vector1.x),
    //(vector1.x * vector2.y) - (vector2.x * vector1.y)
    //);

    //hout.normal = float4(normalize(cross), patch[0].p.w);


	
    return hout;
}



struct DS_OUTPUT
{
    float4 p : SV_Position;
    float4 t : TEXCOORD0;
	float4 tp : TEXCOORD1;
    float4 c : COLOR0;
};

// The domain shader is called for every vertex created by the tessellator.  
// It is like the vertex shader after tessellation.
[domain("tri")]
VS_OUTPUT ds_main(PatchTess patchTess,
             float3 BarycentricCoordinates : SV_DomainLocation,
             const OutputPatch<HS_OUTPUT, 3> TrianglePatch)
{
    VS_OUTPUT dout;
        
    float scale = 1 +
    BarycentricCoordinates.x * (1 - BarycentricCoordinates.x) *
    BarycentricCoordinates.y * (1 - BarycentricCoordinates.y) *
    BarycentricCoordinates.z * (1 - BarycentricCoordinates.z) * 100;
	
    dout.p =
    BarycentricCoordinates.x * TrianglePatch[0].p +
    BarycentricCoordinates.y * TrianglePatch[1].p +
    BarycentricCoordinates.z * TrianglePatch[2].p;
    
    dout.p *= scale;

    dout.t =
    BarycentricCoordinates.x * TrianglePatch[0].t +
    BarycentricCoordinates.y * TrianglePatch[1].t +
    BarycentricCoordinates.z * TrianglePatch[2].t;

    dout.tp =
    BarycentricCoordinates.x * TrianglePatch[0].tp +
    BarycentricCoordinates.y * TrianglePatch[1].tp +
    BarycentricCoordinates.z * TrianglePatch[2].tp;

    dout.c =
    BarycentricCoordinates.x * TrianglePatch[0].c +
    BarycentricCoordinates.y * TrianglePatch[1].c +
    BarycentricCoordinates.z * TrianglePatch[2].c;
	
    return dout;
}