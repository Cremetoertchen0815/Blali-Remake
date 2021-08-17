#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_5_0
    #define PS_SHADERMODEL ps_5_0
#endif

sampler s0;
float horDivide = 400;
float verDivide = 400;

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MosaicFunc(VertexShaderOutput input) : COLOR
{
	if (horDivide < 1920) {
		int tmpw = (int)floor(input.TextureCoordinates.x * horDivide);
		int tmph = (int)floor(input.TextureCoordinates.y * verDivide);
		return tex2D(s0, float2(tmpw / horDivide, tmph / verDivide));
	} else {
		return tex2D(s0, float2(input.TextureCoordinates.x, input.TextureCoordinates.y));
	}

}

technique MosaicShader
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MosaicFunc();
	}
};