#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
sampler s0;
float ColourAmount;

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 color = tex2D(s0, input.TextureCoordinates);
	float3 tmp = float3(1, 1, 1) - input.Color.rgb;
    float3 colrgb = color.rgb - tmp;
    float greycolor = dot(colrgb, float3(0.3, 0.59, 0.11));

    colrgb.rgb = lerp(dot(greycolor, float3(0.3, 0.59, 0.11)), colrgb, ColourAmount / 100);

    return float4(colrgb.rgb, color.a);
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};