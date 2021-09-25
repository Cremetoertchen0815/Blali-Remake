sampler s0;
float horDivide = 400;
float verDivide = 400;

float4 MainPS(float2 UVCoord:TEXCOORD0) : COLOR0
{
	if (horDivide < 1920) {
		int tmpw = (int)floor(UVCoord.x * horDivide);
		int tmph = (int)floor(UVCoord.y * verDivide);
		return tex2D(s0, float2(tmpw / horDivide, tmph / verDivide));
	} else {
		return tex2D(s0, float2(UVCoord.x, UVCoord.y));
	}
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile ps_3_0 MainPS();
	}
};