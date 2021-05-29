using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nez
{
	public class MosaicPostProcessor : PostProcessor
	{

		EffectParameter _horDivide;
		EffectParameter _verDivide;
		Vector2 _Resolution;
		Vector2 _SceneResolution;

		public MosaicPostProcessor(int execOrder) : base(execOrder)
		{

		}

		public override void OnAddedToScene(Scene scene)
		{
			base.OnAddedToScene(scene);

			Effect = scene.Content.LoadEffect<Effect>("Mosaic", EffectResource.Mosaic);
			_horDivide = Effect.Parameters["horDivide"];
			_verDivide = Effect.Parameters["verDivide"];
			_SceneResolution = scene.SceneRenderTargetSize.ToVector2();

			Resolution = new Vector2(50, 50);

		}

		public Vector2 Resolution
		{
			get => _Resolution;
			set
			{
				_horDivide.SetValue(value.X);
				_verDivide.SetValue(value.Y);
				_Resolution = value;
			}
		}

		public float Divide
		{
			get => _SceneResolution.X / Resolution.X;
			set
			{
				_horDivide.SetValue(_SceneResolution.X / value);
				_verDivide.SetValue(_SceneResolution.Y / value);
				Resolution = _SceneResolution / value;
			}
		}

		public override void OnSceneBackBufferSizeChanged(int newWidth, int newHeight)
		{
			base.OnSceneBackBufferSizeChanged(newWidth, newHeight);
			_SceneResolution = new Vector2(newWidth, newHeight);
		}
	}
}
