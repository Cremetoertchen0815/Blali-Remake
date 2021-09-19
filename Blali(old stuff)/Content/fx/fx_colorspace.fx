#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_5_0
    #define PS_SHADERMODEL ps_5_0
#endif

Texture2D SpriteTexture;
sampler s0;
float colorbitdepth = 1;

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 LimitColorPalette(VertexShaderOutput input) : COLOR
{
	float4 tx = tex2D(s0, input.TextureCoordinates);
	float colorsteps = pow(2, colorbitdepth);
	
	uint rv = (uint)floor(tx.r * colorsteps);
	uint gv = (uint)floor(tx.g * colorsteps);
	uint bv = (uint)floor(tx.b * colorsteps);

    return float4(rv / colorsteps, gv / colorsteps, bv / colorsteps, tx.a) * input.Color;
}

technique ColorSpaceLimiter
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL LimitColorPalette();
	}
};