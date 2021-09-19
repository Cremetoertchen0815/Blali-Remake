#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler s0;

#define SAMPLE_COUNT 15

float2 _sampleOffsets[SAMPLE_COUNT];
float _sampleWeights[SAMPLE_COUNT];

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};


float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 c = 0;
    
    // Combine a number of weighted image filter taps.
    for( int i = 0; i < SAMPLE_COUNT; i++ )
        c += tex2D( s0, input.TextureCoordinates + _sampleOffsets[i] ) * _sampleWeights[i];
    
    return c;
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};