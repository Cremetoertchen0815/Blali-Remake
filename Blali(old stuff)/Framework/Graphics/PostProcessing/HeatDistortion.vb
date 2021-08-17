Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Framework.Graphics.PostProcessing

    <TestState(TestState.WorkInProgress)>
    Public Class HeatDistortionPostProcessor
        Private Effect As Effect
        Private _distortionFactor As Single = 0.005F
        Private _riseFactor As Single = 0.15F
        Private _timeParam As EffectParameter
        Private _distortionFactorParam As EffectParameter
        Private _riseFactorParam As EffectParameter
        Private _totalTime As Integer

        Public Property DistortionFactor As Single
            Get
                Return _distortionFactor
            End Get
            Set(ByVal value As Single)

                If _distortionFactor <> value Then
                    _distortionFactor = value
                    If Effect IsNot Nothing Then _distortionFactorParam.SetValue(_distortionFactor)
                End If
            End Set
        End Property

        Public Property RiseFactor As Single
            Get
                Return _riseFactor
            End Get
            Set(ByVal value As Single)

                If _riseFactor <> value Then
                    _riseFactor = value
                    If Effect IsNot Nothing Then _riseFactorParam.SetValue(_riseFactor)
                End If
            End Set
        End Property

        Public WriteOnly Property DistortionTexture As Texture2D
            Set(ByVal value As Texture2D)
                Effect.Parameters("_distortionTexture").SetValue(value)
            End Set
        End Property


        Public Sub New()
            Effect = ContentMan.Load(Of Effect)("fx/fx_heat_distortion")
            _timeParam = Effect.Parameters("_time")
            _distortionFactorParam = Effect.Parameters("_distortionFactor")
            _riseFactorParam = Effect.Parameters("_riseFactor")
            _distortionFactorParam.SetValue(_distortionFactor)
            _riseFactorParam.SetValue(_riseFactor)
            DistortionTexture = ContentMan.Load(Of Texture2D)("nez/textures/heatDistortionNoise")
        End Sub

        Public Sub Update(gameTime As GameTime)
            _totalTime += gameTime.ElapsedGameTime.TotalMilliseconds
            _timeParam.SetValue(_totalTime)
        End Sub

        Public Sub Process(ByVal source As RenderTarget2D, ByVal destination As RenderTarget2D)
            Graphx.GraphicsDevice.SetRenderTarget(destination)
            SpriteBat.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearWrap, Nothing, Nothing, Effect, MainScalingMatrix)
            SpriteBat.Draw(source, New Rectangle(0, 0, GameSize.X, GameSize.Y), Color.White)
            SpriteBat.End()
        End Sub
    End Class
End Namespace
