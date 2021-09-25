using System;
using System.IO;
using Microsoft.Xna.Framework;


namespace Nez
{
	public static class EffectResource
	{
		// sprite effects
		internal static byte[] SpriteBlinkEffectBytes => GetFileResourceBytes("assets/nez/effects/SpriteBlinkEffect.mgfxo");

		internal static byte[] SpriteLinesEffectBytes => GetFileResourceBytes("assets/nez/effects/SpriteLines.mgfxo");

		internal static byte[] SpriteAlphaTestBytes => GetFileResourceBytes("assets/nez/effects/SpriteAlphaTest.mgfxo");

		internal static byte[] CrosshatchBytes => GetFileResourceBytes("assets/nez/effects/Crosshatch.mgfxo");

		internal static byte[] NoiseBytes => GetFileResourceBytes("assets/nez/effects/Noise.mgfxo");

		internal static byte[] TwistBytes => GetFileResourceBytes("assets/nez/effects/Twist.mgfxo");

		internal static byte[] DotsBytes => GetFileResourceBytes("assets/nez/effects/Dots.mgfxo");

		internal static byte[] DissolveBytes => GetFileResourceBytes("assets/nez/effects/Dissolve.mgfxo");

		// post processor effects
		internal static byte[] BloomCombineBytes => GetFileResourceBytes("assets/nez/effects/BloomCombine.mgfxo");

		internal static byte[] BloomExtractBytes => GetFileResourceBytes("assets/nez/effects/BloomExtract.mgfxo");

		internal static byte[] QualityBloom => GetFileResourceBytes("assets/nez/effects/QualityBloom.mgfxo");

		internal static byte[] Mosaic => GetFileResourceBytes("assets/nez/effects/Mosaic.mgfxo");

		internal static byte[] LUTColorGrade => GetFileResourceBytes("assets/nez/effects/LUTColorGrade.mgfxo");

		internal static byte[] GaussianBlurBytes => GetFileResourceBytes("assets/nez/effects/GaussianBlur.mgfxo");

		internal static byte[] VignetteBytes => GetFileResourceBytes("assets/nez/effects/Vignette.mgfxo");

		internal static byte[] LetterboxBytes => GetFileResourceBytes("assets/nez/effects/Letterbox.mgfxo");

		internal static byte[] HeatDistortionBytes => GetFileResourceBytes("assets/nez/effects/HeatDistortion.mgfxo");

		internal static byte[] SpriteLightMultiplyBytes => GetFileResourceBytes("assets/nez/effects/SpriteLightMultiply.mgfxo");

		internal static byte[] PixelGlitchBytes => GetFileResourceBytes("assets/nez/effects/PixelGlitch.mgfxo");

		internal static byte[] StencilLightBytes => GetFileResourceBytes("assets/nez/effects/StencilLight.mgfxo");

		// deferred lighting
		internal static byte[] DeferredSpriteBytes => GetFileResourceBytes("assets/nez/effects/DeferredSprite.mgfxo");

		internal static byte[] DeferredLightBytes => GetFileResourceBytes("assets/nez/effects/DeferredLighting.mgfxo");

		// forward lighting
		internal static byte[] ForwardLightingBytes => GetFileResourceBytes("assets/nez/effects/ForwardLighting.mgfxo");

		internal static byte[] PolygonLightBytes => GetFileResourceBytes("assets/nez/effects/PolygonLight.mgfxo");

		// scene transitions
		internal static byte[] SquaresTransitionBytes => GetFileResourceBytes("assets/nez/effects/transitions/Squares.mgfxo");

		// sprite or post processor effects
		internal static byte[] SpriteEffectBytes => GetMonoGameEmbeddedResourceBytes("Microsoft.Xna.Framework.Graphics.Effect.Resources.SpriteEffect.ogl.mgfxo");

		internal static byte[] MultiTextureOverlayBytes => GetFileResourceBytes("assets/nez/effects/MultiTextureOverlay.mgfxo");

		internal static byte[] ScanlinesBytes => GetFileResourceBytes("assets/nez/effects/Scanlines.mgfxo");

		internal static byte[] ReflectionBytes => GetFileResourceBytes("assets/nez/effects/Reflection.mgfxo");

		internal static byte[] GrayscaleBytes => GetFileResourceBytes("assets/nez/effects/Grayscale.mgfxo");

		internal static byte[] SepiaBytes => GetFileResourceBytes("assets/nez/effects/Sepia.mgfxo");

		internal static byte[] PaletteCyclerBytes => GetFileResourceBytes("assets/nez/effects/PaletteCycler.mgfxo");


		/// <summary>
		/// gets the raw byte[] from an EmbeddedResource
		/// </summary>
		/// <returns>The embedded resource bytes.</returns>
		/// <param name="name">Name.</param>
		static byte[] GetEmbeddedResourceBytes(string name)
		{
			var assembly = typeof(EffectResource).Assembly;
			using (var stream = assembly.GetManifestResourceStream(name))
			{
				using (var ms = new MemoryStream())
				{
					stream.CopyTo(ms);
					return ms.ToArray();
				}
			}
		}


		internal static byte[] GetMonoGameEmbeddedResourceBytes(string name)
		{
			var assembly = typeof(MathHelper).Assembly;
#if FNA
			name = name.Replace( ".ogl.mgfxo", ".fxb" );
#else
			// MG 3.8 decided to change the location of Effecs...sigh.
			if (!assembly.GetManifestResourceNames().Contains(name))
				name = name.Replace(".Framework", ".Framework.Platform");
#endif

			using (var stream = assembly.GetManifestResourceStream(name))
			{
				using (var ms = new MemoryStream())
				{
					stream.CopyTo(ms);
					return ms.ToArray();
				}
			}
		}


		/// <summary>
		/// fetches the raw byte data of a file from the Content folder. Used to keep the Effect subclass code simple and clean due to the Effect
		/// constructor requiring the byte[].
		/// </summary>
		/// <returns>The file resource bytes.</returns>
		/// <param name="path">Path.</param>
		public static byte[] GetFileResourceBytes(string path)
		{
#if FNA
			path = path.Replace( ".mgfxo", ".fxb" );
#endif

			byte[] bytes;
			try
			{
				using (var stream = TitleContainer.OpenStream(path))
				{
					if (stream.CanSeek)
					{
						bytes = new byte[stream.Length];
						stream.Read(bytes, 0, bytes.Length);
					}
					else
					{
						using (var ms = new MemoryStream())
						{
							stream.CopyTo(ms);
							bytes = ms.ToArray();
						}
					}
				}
			}
			catch (Exception e)
			{
				var txt = string.Format(
					"OpenStream failed to find file at path: {0}. Did you add it to the Content folder and set its properties to copy to output directory?",
					path);
				throw new Exception(txt, e);
			}

			return bytes;
		}
	}
}