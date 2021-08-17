#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler s0;

Texture2D OverlayTexture;
float Intensity = 0;
sampler OverlaySampler  = sampler_state
{
  Texture = <OverlayTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};


float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 origpxl = tex2D(s0, input.TextureCoordinates);
	float4 overpxl = tex2D(OverlaySampler, input.TextureCoordinates);
	float factorA = Intensity / 100;
	float factorB = 1 - factorA;
	float3 mixed = origpxl.rgb * factorA + overpxl.rgb * factorB;
	mixed *= origpxl.a;
	return float4(mixed, origpxl.a);
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};