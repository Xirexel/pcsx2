#pragma once

#define width  (512)
#define height (512)

// pixel "width"
#define px (0.001)
#define py (0.001)

/* Parameters */

// for the blur filter
#define mean 0.6
#define dx (mean * px)
#define dy (mean * py)

#define CoefBlur 2
#define CoefOrig (1 + CoefBlur)

// for the sharpen filter
#define SharpenEdge  0.2
#define Sharpen_val0 3
#define Sharpen_val1 ((Sharpen_val0 - 1) / 8.0)

float3 sharpen(Texture2D tx, SamplerState a_sampler, float2 tex : TEXCOORD0) : COLOR
{
	// get original pixel
    float3 orig = tx.Sample(a_sampler, tex);

	// compute blurred image (gaussian filter)
    float3 c1 = tx.Sample(a_sampler, tex + float2(-dx, -dy));
    float3 c2 = tx.Sample(a_sampler, tex + float2(0, -dy));
    float3 c3 = tx.Sample(a_sampler, tex + float2(dx, -dy));
    float3 c4 = tx.Sample(a_sampler, tex + float2(-dx, 0));
    float3 c5 = tx.Sample(a_sampler, tex + float2(dx, 0));
    float3 c6 = tx.Sample(a_sampler, tex + float2(-dx, dy));
    float3 c7 = tx.Sample(a_sampler, tex + float2(0, dy));
    float3 c8 = tx.Sample(a_sampler, tex + float2(dx, dy));

	// gaussian filter
	// [ 1, 2, 1 ]
	// [ 2, 4, 2 ]
	// [ 1, 2, 1 ]
	// to normalize the values, we need to divide by the coeff sum
	// 1 / (1+2+1+2+4+2+1+2+1) = 1 / 16 = 0.0625
    float3 flou = (c1 + c3 + c6 + c8 + 2 * (c2 + c4 + c5 + c7) + 4 * orig) * 0.0625;

	// substract blurred image from original image
    float3 corrected = CoefOrig * orig - CoefBlur * flou;

	// edge detection
	// Get neighbor points
	// [ c1,   c2, c3 ]
	// [ c4, orig, c5 ]
	// [ c6,   c7, c8 ]
    c1 = tx.Sample(a_sampler, tex + float2(-px, -py));
    c2 = tx.Sample(a_sampler, tex + float2(0, -py));
    c3 = tx.Sample(a_sampler, tex + float2(px, -py));
    c4 = tx.Sample(a_sampler, tex + float2(-px, 0));
    c5 = tx.Sample(a_sampler, tex + float2(px, 0));
    c6 = tx.Sample(a_sampler, tex + float2(-px, py));
    c7 = tx.Sample(a_sampler, tex + float2(0, py));
    c8 = tx.Sample(a_sampler, tex + float2(px, py));

	// using Sobel filter
	// horizontal gradient
	// [ -1, 0, 1 ]
	// [ -2, 0, 2 ]
	// [ -1, 0, 1 ]
    float delta1 = (c3 + 2 * c5 + c8) - (c1 + 2 * c4 + c6);

	// vertical gradient
	// [ -1, - 2, -1 ]
	// [  0,   0,  0 ]
	// [  1,   2,  1 ]
    float delta2 = (c6 + 2 * c7 + c8) - (c1 + 2 * c2 + c3);

	// computation
    if (sqrt(mul(delta1, delta1) + mul(delta2, delta2)) > SharpenEdge)
    {
		// if we have an edge, use sharpen
		//return  float4(1,0,0,0);
        return orig * Sharpen_val0 - (c1 + c2 + c3 + c4 + c5 + c6 + c7 + c8) * Sharpen_val1;
    }
    else
    {
		// else return corrected image
        return corrected;
        //return orig;
    }
}