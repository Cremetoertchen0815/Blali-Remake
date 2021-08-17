#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

//Mosaic flags
float horDivide = 400;
float verDivide = 400;
//Very BG flags
Texture2D Skybox;
sampler Skyboxsampler  = sampler_state
{
  Texture = <Skybox>;
};
//Brightness flags
float amount = 1;
float mixingcolor = 0;
//sampler
sampler s0;

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	//Mosaic Shader
	float4 tmp;
	float resX = horDivide * (2.0 - (horDivide / 1920));
	float resY = verDivide * (2.0 - (verDivide / 1080));
	if (horDivide < 1920) {
		int tmpw = (int)floor(input.TextureCoordinates.x * resX);
		int tmph = (int)floor(input.TextureCoordinates.y * resY);
		tmp =  tex2D(s0, float2(tmpw / resX, tmph / resY));
	} else {
		tmp =  tex2D(s0, input.TextureCoordinates);
	}

	//Very BG
	float4 bgtxl = tex2D(Skyboxsampler, input.TextureCoordinates);
	tmp = lerp(bgtxl, tmp, tmp.a);
	
	//Tint
	tmp *= input.Color;
	
	//Brightness
	float4 someOtherColor = float4(mixingcolor / 255, mixingcolor / 255, mixingcolor / 255, 1);
	float4 fin = lerp(tmp * input.Color, someOtherColor, 1 - (amount / 100));
	return fin * input.Color.a;
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};