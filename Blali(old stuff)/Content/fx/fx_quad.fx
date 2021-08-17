    #if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0     //_level_9_1
    #define PS_SHADERMODEL ps_4_0     //_level_9_1
#endif

float amount = 100;
float mixingcolor = 0;
float4 tint = float4(1, 1, 1, 1);
float alpha = 1;

matrix World;
matrix View;
matrix Projection;

Texture2D Texture; // primary texture.
sampler s0;
//_______________________________________________________________
// techniques 
// Quad Draw  Position Color Texture
//_______________________________________________________________
struct VsInputQuad
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TexureCoordinateA : TEXCOORD0;
};
struct VsOutputQuad
{
    float4 Position : SV_Position;
    float4 Color : COLOR0;
    float2 TexureCoordinateA : TEXCOORD0;
};
// ____________________________
VsOutputQuad VertexShaderQuadDraw(VsInputQuad input)
{
    VsOutputQuad output;
    float4x4 wvp = mul(World, mul(View, Projection));
    output.Position = mul(input.Position, wvp); // Transform by WorldViewProjection
    output.Color = input.Color;
    output.TexureCoordinateA = input.TexureCoordinateA;
    return output;
}
float4 PixelShaderQuadDraw(VsOutputQuad input) : COLOR
{
	float4 color = tex2D(s0, input.TexureCoordinateA);
	return lerp(color * tint * input.Color, float4(mixingcolor / 255, mixingcolor / 255, mixingcolor / 255, 1), 1 - amount / 100 * alpha) * color.a * input.Color.a;
}

technique QuadDraw
{
    pass
    {
        VertexShader = compile VS_SHADERMODEL VertexShaderQuadDraw();
        PixelShader = compile PS_SHADERMODEL PixelShaderQuadDraw();
    }
}