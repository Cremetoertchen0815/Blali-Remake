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

// world matrix
matrix World;

// inverse transpose world matrix
matrix WorldInverseTranspose;

// define white color value
float3 WhiteColor = float3(1, 1, 1);

// ambient light value
float3 AmbientColor = float3(1, 1, 1);

// diffuse color
float3 DiffuseColor = float3(1, 1, 1);

// emissive
float3 EmissiveColor = float3(0, 0, 0);

// fog vector
float4 FogVector;

//fog color
float3 FogColor;

// max intensity, eg allowing lights to overflow original color
float MaxLightIntensity = 1.0f;

// rendering alpha
float Alpha = 1.0f;

// main texture
texture MainTexture;

// normal map texture
texture NormalTexture;

// are we using texture?
bool TextureEnabled = false;

// are we using normal texture?
static bool NormalTextureEnabled = true;

// max lights count
#define MAX_LIGHTS_COUNT 7

// light sources.
// note: 
//	- lights with range 0 = directional lights (in which case light pos is direction).
//	- lights with intensity 0 = disabled lights.
float3 LightColor[MAX_LIGHTS_COUNT];
float3 LightPosition[MAX_LIGHTS_COUNT];
float LightIntensity[MAX_LIGHTS_COUNT];
float LightRange[MAX_LIGHTS_COUNT];
float LightSpecular[MAX_LIGHTS_COUNT];

// how many active lights we have
int ActiveLightsCount = 0;

// how many of the active lights are directional (direction lights come first)
int DirectionalLightsCount = 0;

// main texture sampler
sampler2D MainTextureSampler = sampler_state {
	Texture = (MainTexture);
};

// normal texture sampler
sampler2D NormalTextureSampler = sampler_state {
	Texture = (NormalTexture);
};

// vertex shader input
struct VertexShaderInput
{
	float4 Position : SV_POSITION0;
	float3 Normal : NORMAL0;
	float2 TextureCoordinate : TEXCOORD0;
	float3 Tangent : TANGENT0;
	float3 Binormal : BINORMAL0;
};

// vertex shader output
struct VertexShaderOutputNoFog
{
	float4 Position : SV_POSITION;
	float2 TextureCoordinate : TEXCOORD0;
	float4 WorldPos : TEXCOORD1;
	float3x3 WorldToTangentSpace : TEXCOORD2;
};

// vertex shader output
struct VertexShaderOutputFog
{
	float4 Position : SV_POSITION;
	float3 TextureCoordinate : TEXCOORD0;
	float4 WorldPos : TEXCOORD1;
	float3x3 WorldToTangentSpace : TEXCOORD2;
};

// main vertex shader for flat lighting
VertexShaderOutputNoFog NormalLightingMainVSNoFog(in VertexShaderInput input)
{
	VertexShaderOutputNoFog output;
	output.Position = mul(input.Position, WorldViewProjection);
	output.WorldPos = mul(input.Position, World);
	output.TextureCoordinate = input.TextureCoordinate;
	output.WorldToTangentSpace[0] = mul(float4(normalize(input.Tangent), 0), World).xyz;
	output.WorldToTangentSpace[1] = mul(float4(normalize(input.Binormal), 0), World).xyz;
	output.WorldToTangentSpace[2] = mul(float4(normalize(input.Normal), 0), World).xyz;
	return output;
}

// main vertex shader for flat lighting
VertexShaderOutputFog NormalLightingMainVSFog(in VertexShaderInput input)
{
	VertexShaderOutputFog output;
	output.Position = mul(input.Position, WorldViewProjection);
	output.WorldPos = mul(input.Position, World);
	output.TextureCoordinate = float3(input.TextureCoordinate, saturate(dot(input.Position, FogVector)));
	output.WorldToTangentSpace[0] = mul(float4(normalize(input.Tangent), 0), World).xyz;
	output.WorldToTangentSpace[1] = mul(float4(normalize(input.Binormal), 0), World).xyz;
	output.WorldToTangentSpace[2] = mul(float4(normalize(input.Normal), 0), World).xyz;
	return output;
}

// main pixel shader for flat lighting
float4 NormalLightingMainPSNoFog(VertexShaderOutputNoFog input) : COLOR
{
	// pixel color to return
	float4 retColor;

	// set color either from texture if enabled or white
	retColor = TextureEnabled ? tex2D(MainTextureSampler, input.TextureCoordinate) : 1.0f;

	// get normal from texture
	float3 fragNormal = 2.0 * (tex2D(NormalTextureSampler, input.TextureCoordinate).rgb) - 1.0;
	fragNormal.x *= -1;		// <-- fix X axis to be standard.

	// calculate normal
	fragNormal = normalize(mul(fragNormal, input.WorldToTangentSpace));

	// start calcing lights strength
	float3 LightsColor = AmbientColor + EmissiveColor;

	// process directional lights
	int i = 0;
	for (i = 0; i < DirectionalLightsCount; ++i)
	{
		// calculate angle factor
		float cosTheta = saturate(dot(LightPosition[i], fragNormal));

		// add light to pixel
		LightsColor.rgb += (LightColor[i]) * (cosTheta * LightIntensity[i]);
	}

	// now process all point lights
	for (i = DirectionalLightsCount; i < ActiveLightsCount; ++i)
	{
		// if fully lit stop here
		if (LightsColor.r > 1 && LightsColor.g > 1 && LightsColor.b > 1) { break; }

		// calculate distance and angle factors for point light
		float disFactor = 1.0f - (distance(input.WorldPos.xyz, LightPosition[i]) / LightRange[i]);

		// out of range? skip this light.
		if (disFactor > 0)
		{
			// power distance factor
			disFactor = pow(disFactor, 2);

			// calc with normal factor
			float3 lightDir = normalize(input.WorldPos.xyz - LightPosition[i]);
			float cosTheta = saturate(dot(-lightDir, fragNormal));

			// add light to pixel
			LightsColor.rgb += (LightColor[i]) * (cosTheta * LightIntensity[i] * disFactor);
		}
	}

	// make sure lights doesn't overflow
	LightsColor.rgb = min(LightsColor.rgb, MaxLightIntensity);

	// apply lighting and diffuse on return color
	retColor.rgb = saturate(retColor.rgb * LightsColor * DiffuseColor);

	// apply alpha
	retColor.a *= Alpha;

	// return final
	return retColor;
}


// main pixel shader for flat lighting
float4 NormalLightingMainPSFog(VertexShaderOutputFog input) : COLOR
{
	// pixel color to return
	float4 retColor;

	// set color either from texture if enabled or white
	retColor = TextureEnabled ? tex2D(MainTextureSampler, input.TextureCoordinate.xy) : 1.0f;

	// get normal from texture
	float3 fragNormal = 2.0 * (tex2D(NormalTextureSampler, input.TextureCoordinate.xy).rgb) - 1.0;
	fragNormal.x *= -1;		// <-- fix X axis to be standard.

	// calculate normal
	fragNormal = normalize(mul(fragNormal, input.WorldToTangentSpace));

	// start calcing lights strength
	float3 LightsColor = AmbientColor + EmissiveColor;

	// process directional lights
	int i = 0;
	for (i = 0; i < DirectionalLightsCount; ++i)
	{
		// calculate angle factor
		float cosTheta = saturate(dot(LightPosition[i], fragNormal));

		// add light to pixel
		LightsColor.rgb += (LightColor[i]) * (cosTheta * LightIntensity[i]);
	}

	// now process all point lights
	for (i = DirectionalLightsCount; i < ActiveLightsCount; ++i)
	{
		// if fully lit stop here
		if (LightsColor.r > 1 && LightsColor.g > 1 && LightsColor.b > 1) { break; }

		// calculate distance and angle factors for point light
		float disFactor = 1.0f - (distance(input.WorldPos.xyz, LightPosition[i]) / LightRange[i]);

		// out of range? skip this light.
		if (disFactor > 0)
		{
			// power distance factor
			disFactor = pow(disFactor, 2);

			// calc with normal factor
			float3 lightDir = normalize(input.WorldPos.xyz - LightPosition[i]);
			float cosTheta = saturate(dot(-lightDir, fragNormal));

			// add light to pixel
			LightsColor.rgb += (LightColor[i]) * (cosTheta * LightIntensity[i] * disFactor);
		}
	}

	// make sure lights doesn't overflow
	LightsColor.rgb = min(LightsColor.rgb, MaxLightIntensity);

	// apply lighting and diffuse on return color
	retColor.rgb = saturate(retColor.rgb * LightsColor * DiffuseColor);

	// apply alpha
	retColor.a *= Alpha;
	
	//apply fog
	retColor.rgb = lerp(retColor.rgb, FogColor * retColor.a, input.TextureCoordinate.z);

	// return final
	return retColor;
}

// default technique with normal map lighting and no fog
technique NoFog
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL NormalLightingMainVSNoFog();
		PixelShader = compile PS_SHADERMODEL NormalLightingMainPSNoFog();
	}
};


// default technique with flat lighting and fog
technique Fog
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL NormalLightingMainVSFog();
		PixelShader = compile PS_SHADERMODEL NormalLightingMainPSFog();
	}
};
