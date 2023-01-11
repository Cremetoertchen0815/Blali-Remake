#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif


// world / view / projection matrix
matrix WorldViewProjection;

// ambient light value
float3 AmbientColor = float3(1, 1, 1);

// diffuse color
float3 DiffuseColor = float3(1, 1, 1);

// emissive
float3 EmissiveColor = float3(0, 0, 0);

// rendering alpha
float Alpha = 1.0f;

// main texture
texture MainTexture;

texture ShadowMap;

// are we using texture?
bool TextureEnabled = false;

// main texture sampler
sampler2D MainTextureSampler = sampler_state {
	Texture = (MainTexture);
};

// main texture sampler
sampler2D ShadowMapSampler = sampler_state {
	Texture = (ShadowMap);
};

// vertex shader input
struct VertexShaderInput
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float2 TextureCoordinate : TEXCOORD0;
};

// vertex shader output
struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 TextureCoordinate : TEXCOORD0;
};

// main vertex shader for flat lighting
VertexShaderOutput FlatLightingMainVS(in VertexShaderInput input)
{
	VertexShaderOutput output;
	output.Position = mul(input.Position, WorldViewProjection);
	output.TextureCoordinate = input.TextureCoordinate;
	return output;
}

// main pixel shader for flat lighting
float4 FlatLightingMainPS(VertexShaderOutput input) : COLOR
{
	// pixel base color either from texture if enabled or white
	float4 baseColorA = TextureEnabled ? tex2D(MainTextureSampler, input.TextureCoordinate) : 1.0f;
	float3 baseColor = baseColorA.rgb;
	// apply diffuse to base
	baseColor.rgb = saturate(baseColor.rgb * DiffuseColor);

	float shadowDepth = tex2D(ShadowMapSampler, input.TextureCoordinate).r;
	
	//Calculate if in shadow
	float3 retColor = lerp(baseColor * AmbientColor, baseColor, shadowDepth);
	
	//Apply emissive lighting
	retColor = saturate(retColor + EmissiveColor);

	// return final
	return Alpha * float4(retColor, baseColorA.a);
}

// default technique with flat lighting 
technique FlatLighting
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL FlatLightingMainVS();
		PixelShader = compile PS_SHADERMODEL FlatLightingMainPS();
	}
};
