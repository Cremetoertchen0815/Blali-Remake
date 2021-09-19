#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif


sampler s0;


Texture2D _distortionTexture;
sampler _distortionTextureSampler = sampler_state
{
    Texture = <_distortionTexture>;
    AddressU = Wrap;
    AddressV = Wrap;
};


float _time; // Time used to scroll the distortion map
float _distortionFactor; // default 0.005. Factor used to control severity of the effect
float _riseFactor; // default 0.15. Factor used to control how fast air rises

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 distortionUV = input.TextureCoordinates;
    distortionUV.y -= _time * -_riseFactor;
    
    // Compute the distortion by reading the distortion map
    float2 distortionMapValue = tex2D( _distortionTextureSampler, distortionUV ).xy;

	// bring it into the -1 to 1 range
    float2 distortionPositionOffset = distortionMapValue;
    distortionMapValue = ( ( distortionMapValue * 2.0 ) - 1.0 );
    
    // The _distortionFactor scales the offset and thus controls the severity
    distortionMapValue *= _distortionFactor;

    // The latter 2 channels of the texture are unused... be creative
    //float2 distortionUnused = distortionMapValue.zw;

    // Since we all know that hot air rises and cools, the effect loses its severity the higher up we get
    // We use the y texture coordinate of the original texture to tell us how "high up" we are and damp accordingly
    distortionMapValue *= ( input.TextureCoordinates.y ); // 1.0 - input.TextureCoordinates.y for actual OpenGL due to input.TextureCoordinates x at the bottom
    
	return tex2D( s0, distortionMapValue + input.TextureCoordinates );
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};