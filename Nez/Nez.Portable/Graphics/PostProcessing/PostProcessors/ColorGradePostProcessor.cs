using Microsoft.Xna.Framework.Graphics;

namespace Nez
{
	public class ColorGradePostProcessor : PostProcessor
	{

		EffectParameter _LUT;
		EffectParameter _SizeParameter;
		EffectParameter _SizeRootParameter;
		EffectParameter _WidthParameter;
		EffectParameter _HeightParameter;
		float _Size;
		float _SizeRoot;

		public ColorGradePostProcessor(int execOrder) : base(execOrder)
		{

		}

		public override void OnAddedToScene(Scene scene)
		{
			base.OnAddedToScene(scene);

			Effect = scene.Content.LoadEffect<Effect>("LUTColorGrade", EffectResource.LUTColorGrade);
			_LUT = Effect.Parameters["LUT"];
			_SizeParameter = Effect.Parameters["Size"];
			_SizeRootParameter = Effect.Parameters["SizeRoot"];
			_WidthParameter = Effect.Parameters["width"];
			_HeightParameter = Effect.Parameters["height"];

			LUT = Core.Content.LoadTexture("nez/textures/defaultLUT");
			Size = 32;
			SizeRoot = 8;
		}

		public float Size
		{
			get => _SizeParameter.GetValueSingle();
			set
			{
				_Size = value;
				_SizeParameter.SetValue(value);
				RecalcSize();
			}
		}

		public float SizeRoot
		{
			get => _SizeRootParameter.GetValueSingle();
			set
			{
				_SizeRoot = value;
				_SizeRootParameter.SetValue(value);
				RecalcSize();
			}
		}

		private void RecalcSize()
		{
			_WidthParameter.SetValue(Size * SizeRoot);
			_HeightParameter.SetValue(Size * Size / SizeRoot);
		}

		public Texture2D LUT
		{
			set => _LUT.SetValue(value);
		}
	}
}
