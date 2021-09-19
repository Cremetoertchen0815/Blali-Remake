Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Content
Imports Microsoft.Xna.Framework.Graphics

Namespace Framework.Graphics.PostProcessing

    <TestState(TestState.Finalized)>
    Public Class BloomFilter
        ' <summary>
        ' 
        ' Version 1.1, 16. Dez. 2016
        ' 
        ' Bloom / Blur, 2016 TheKosmonaut
        ' 
        ' High-Quality Bloom filter for high-performance applications
        ' 
        ' Based largely on the implementations in Unreal Engine 4 And Call of Duty AW
        ' For more information look for
        ' "Next Generation Post Processing in Call of Duty Advanced Warfare" by Jorge Jimenez
        ' http://www.iryoku.com/downloads/Next-Generation-Post-Processing-in-Call-of-Duty-Advanced-Warfare-v18.pptx
        ' 
        ' The idea Is to have several rendertargets Or one rendertarget with several mip maps
        ' so each mip has half resolution (1/2 width And 1/2 height) of the previous one.
        ' 
        ' 32, 16, 8, 4, 2
        ' 
        ' In the first step we extract the bright spots from the original image. If Not specified otherwise thsi happens in full resolution.
        ' We can do that based on the average RGB value Or Luminance And check whether this value Is higher than our Threshold.
        '     BloomUseLuminance = true / false (default Is true)
        '     BloomThreshold = 0.8f;
        ' 
        ' Then we downscale this extraction layer to the next mip map.
        ' While doing that we sample several pixels around the origin.
        ' We continue to downsample a few more times, defined in
        '     BloomDownsamplePasses = 5 ( default Is 5)
        ' 
        ' Afterwards we upsample again, but blur in this step, too.
        ' The final output should be a blur with a very large kernel And smooth gradient.
        ' 
        ' The output in the draw Is only the blurred extracted texture. 
        ' It can be drawn on top of / merged with the original image with an additive operation for example.
        ' 
        ' If you use ToneMapping you should apply Bloom before that step.
        ' </summary>
        Private _width As Integer
        Private _height As Integer
        Private _bloomRenderTarget2DMip0 As RenderTarget2D
        Private _bloomRenderTarget2DMip1 As RenderTarget2D
        Private _bloomRenderTarget2DMip2 As RenderTarget2D
        Private _bloomRenderTarget2DMip3 As RenderTarget2D
        Private _bloomRenderTarget2DMip4 As RenderTarget2D
        Private _bloomRenderTarget2DMip5 As RenderTarget2D
        Private _renderTargetFormat As SurfaceFormat
        Private _graphicsDevice As GraphicsDevice
        Private _quadRenderer As QuadRenderer
        Private _bloomEffect As Effect
        Private _bloomPassExtract As EffectPass
        Private _bloomPassExtractLuminance As EffectPass
        Private _bloomPassDownsample As EffectPass
        Private _bloomPassUpsample As EffectPass
        Private _bloomPassUpsampleLuminance As EffectPass
        Private _bloomParameterScreenTexture As EffectParameter
        Private _bloomInverseResolutionParameter As EffectParameter
        Private _bloomRadiusParameter As EffectParameter
        Private _bloomStrengthParameter As EffectParameter
        Private _bloomStreakLengthParameter As EffectParameter
        Private _bloomThresholdParameter As EffectParameter
        Private _bloomRadius1 As Single = 1.0F
        Private _bloomRadius2 As Single = 1.0F
        Private _bloomRadius3 As Single = 1.0F
        Private _bloomRadius4 As Single = 1.0F
        Private _bloomRadius5 As Single = 1.0F
        Private _bloomStrength1 As Single = 1.0F
        Private _bloomStrength2 As Single = 1.0F
        Private _bloomStrength3 As Single = 1.0F
        Private _bloomStrength4 As Single = 1.0F
        Private _bloomStrength5 As Single = 1.0F
        Public BloomStrengthMultiplier As Single = 1.0F
        Private _radiusMultiplier As Single = 1.0F
        Public BloomUseLuminance As Boolean = True
        Public BloomDownsamplePasses As Integer = 5

        Public Enum BloomPresets
            Wide
            Focussed
            Small
            SuperWide
            Cheap
            One
        End Enum

        Public Property BloomPreset As BloomPresets
            Get
                Return _bloomPreset
            End Get
            Set(ByVal value As BloomPresets)
                If _bloomPreset = value Then Return
                _bloomPreset = value
                SetBloomPreset(_bloomPreset)
            End Set
        End Property

        Private _bloomPreset As BloomPresets

        Private WriteOnly Property BloomScreenTexture As Texture2D
            Set(ByVal value As Texture2D)
                _bloomParameterScreenTexture.SetValue(value)
            End Set
        End Property

        Private Property BloomInverseResolution As Vector2
            Get
                Return _bloomInverseResolutionField
            End Get
            Set(ByVal value As Vector2)

                If value <> _bloomInverseResolutionField Then
                    _bloomInverseResolutionField = value
                    _bloomInverseResolutionParameter.SetValue(_bloomInverseResolutionField)
                End If
            End Set
        End Property

        Private _bloomInverseResolutionField As Vector2

        Private Property BloomRadius As Single
            Get
                Return _bloomRadius
            End Get
            Set(ByVal value As Single)

                If Math.Abs(_bloomRadius - value) > 0.001F Then
                    _bloomRadius = value
                    _bloomRadiusParameter.SetValue(_bloomRadius * _radiusMultiplier)
                End If
            End Set
        End Property

        Private _bloomRadius As Single

        Private Property BloomStrength As Single
            Get
                Return _bloomStrength
            End Get
            Set(ByVal value As Single)

                If Math.Abs(_bloomStrength - value) > 0.001F Then
                    _bloomStrength = value
                    _bloomStrengthParameter.SetValue(_bloomStrength * BloomStrengthMultiplier)
                End If
            End Set
        End Property

        Private _bloomStrength As Single

        Public Property BloomStreakLength As Single
            Get
                Return _bloomStreakLength
            End Get
            Set(ByVal value As Single)

                If Math.Abs(_bloomStreakLength - value) > 0.001F Then
                    _bloomStreakLength = value
                    _bloomStreakLengthParameter.SetValue(_bloomStreakLength)
                End If
            End Set
        End Property

        Private _bloomStreakLength As Single

        Public Property BloomThreshold As Single
            Get
                Return _bloomThreshold
            End Get
            Set(ByVal value As Single)

                If Math.Abs(_bloomThreshold - value) > 0.001F Then
                    _bloomThreshold = value
                    _bloomThresholdParameter.SetValue(_bloomThreshold)
                End If
            End Set
        End Property

        Private _bloomThreshold As Single

        Public Sub Load(ByVal graphicsDevice As GraphicsDevice, ByVal content As ContentManager, ByVal width As Integer, ByVal height As Integer, ByVal Optional renderTargetFormat As SurfaceFormat = SurfaceFormat.Color, ByVal Optional quadRenderer As QuadRenderer = Nothing)
            _graphicsDevice = graphicsDevice
            UpdateResolution(width, height)
            _quadRenderer = If(quadRenderer, New QuadRenderer(graphicsDevice))
            _renderTargetFormat = renderTargetFormat
            _bloomEffect = ContentMan.Load(Of Effect)("fx\fx_bloom")
            _bloomInverseResolutionParameter = _bloomEffect.Parameters("InverseResolution")
            _bloomRadiusParameter = _bloomEffect.Parameters("Radius")
            _bloomStrengthParameter = _bloomEffect.Parameters("Strength")
            _bloomStreakLengthParameter = _bloomEffect.Parameters("StreakLength")
            _bloomThresholdParameter = _bloomEffect.Parameters("Threshold")
            _bloomParameterScreenTexture = _bloomEffect.Parameters("ScreenTexture")

            If _bloomParameterScreenTexture Is Nothing Then
                _bloomParameterScreenTexture = _bloomEffect.Parameters("LinearSampler+ScreenTexture")
            End If

            _bloomPassExtract = _bloomEffect.Techniques("Extract").Passes(0)
            _bloomPassExtractLuminance = _bloomEffect.Techniques("ExtractLuminance").Passes(0)
            _bloomPassDownsample = _bloomEffect.Techniques("Downsample").Passes(0)
            _bloomPassUpsample = _bloomEffect.Techniques("Upsample").Passes(0)
            _bloomPassUpsampleLuminance = _bloomEffect.Techniques("UpsampleLuminance").Passes(0)
            BloomThreshold = 0.8F
            SetBloomPreset(BloomPreset)
        End Sub

        Private Sub SetBloomPreset(ByVal preset As BloomPresets)
            Select Case preset
                Case BloomPresets.Wide
                    _bloomStrength1 = 0.5F
                    _bloomStrength2 = 1
                    _bloomStrength3 = 2
                    _bloomStrength4 = 1
                    _bloomStrength5 = 2
                    _bloomRadius5 = 4.0F
                    _bloomRadius4 = 4.0F
                    _bloomRadius3 = 2.0F
                    _bloomRadius2 = 2.0F
                    _bloomRadius1 = 1.0F
                    BloomStreakLength = 1
                    BloomDownsamplePasses = 5
                Case BloomPresets.SuperWide
                    _bloomStrength1 = 0.9F
                    _bloomStrength2 = 1
                    _bloomStrength3 = 1
                    _bloomStrength4 = 2
                    _bloomStrength5 = 6
                    _bloomRadius5 = 4.0F
                    _bloomRadius4 = 2.0F
                    _bloomRadius3 = 2.0F
                    _bloomRadius2 = 2.0F
                    _bloomRadius1 = 2.0F
                    BloomStreakLength = 1
                    BloomDownsamplePasses = 5
                    Exit Select
                Case BloomPresets.Focussed
                    _bloomStrength1 = 0.8F
                    _bloomStrength2 = 1
                    _bloomStrength3 = 1
                    _bloomStrength4 = 1
                    _bloomStrength5 = 2
                    _bloomRadius5 = 4.0F
                    _bloomRadius4 = 2.0F
                    _bloomRadius3 = 2.0F
                    _bloomRadius2 = 2.0F
                    _bloomRadius1 = 2.0F
                    BloomStreakLength = 1
                    BloomDownsamplePasses = 5
                    Exit Select
                Case BloomPresets.Small
                    _bloomStrength1 = 0.8F
                    _bloomStrength2 = 1
                    _bloomStrength3 = 1
                    _bloomStrength4 = 1
                    _bloomStrength5 = 1
                    _bloomRadius5 = 1
                    _bloomRadius4 = 1
                    _bloomRadius3 = 1
                    _bloomRadius2 = 1
                    _bloomRadius1 = 1
                    BloomStreakLength = 1
                    BloomDownsamplePasses = 5
                Case BloomPresets.Cheap
                    _bloomStrength1 = 0.8F
                    _bloomStrength2 = 2
                    _bloomRadius2 = 2
                    _bloomRadius1 = 2
                    BloomStreakLength = 1
                    BloomDownsamplePasses = 2
                Case BloomPresets.One
                    _bloomStrength1 = 4.0F
                    _bloomStrength2 = 1
                    _bloomStrength3 = 1
                    _bloomStrength4 = 1
                    _bloomStrength5 = 2
                    _bloomRadius5 = 1.0F
                    _bloomRadius4 = 1.0F
                    _bloomRadius3 = 1.0F
                    _bloomRadius2 = 1.0F
                    _bloomRadius1 = 1.0F
                    BloomStreakLength = 1
                    BloomDownsamplePasses = 5
            End Select
        End Sub

        Public Function Draw(ByVal inputTexture As Texture2D, ByVal width As Integer, ByVal height As Integer) As Texture2D
            If _graphicsDevice Is Nothing Then Throw New Exception("Module not yet Loaded / Initialized. Use Load() first")

            If width <> _width OrElse height <> _height Then
                UpdateResolution(width, height)
                _radiusMultiplier = CSng(width) / inputTexture.Width
                SetBloomPreset(BloomPreset)
            End If

            _graphicsDevice.RasterizerState = RasterizerState.CullNone
            _graphicsDevice.BlendState = BlendState.Opaque
            _graphicsDevice.SetRenderTarget(_bloomRenderTarget2DMip0)
            BloomScreenTexture = inputTexture
            BloomInverseResolution = New Vector2(1.0F / _width, 1.0F / _height)

            If BloomUseLuminance Then
                _bloomPassExtractLuminance.Apply()
            Else
                _bloomPassExtract.Apply()
            End If

            _quadRenderer.RenderQuad(_graphicsDevice, Vector2.One * -1, Vector2.One)

            If BloomDownsamplePasses > 0 Then
                _graphicsDevice.SetRenderTarget(_bloomRenderTarget2DMip1)
                BloomScreenTexture = _bloomRenderTarget2DMip0
                _bloomPassDownsample.Apply()
                _quadRenderer.RenderQuad(_graphicsDevice, Vector2.One * -1, Vector2.One)

                If BloomDownsamplePasses > 1 Then
                    '''                     //Our input resolution is halfed, so our inverse 1/res. must be doubled
                    BloomInverseResolution *= 2

                    ''' 
                    _graphicsDevice.SetRenderTarget(_bloomRenderTarget2DMip2)
                    BloomScreenTexture = _bloomRenderTarget2DMip1
                    _bloomPassDownsample.Apply()
                    _quadRenderer.RenderQuad(_graphicsDevice, Vector2.One * -1, Vector2.One)

                    If BloomDownsamplePasses > 2 Then
                        BloomInverseResolution *= 2

                        _graphicsDevice.SetRenderTarget(_bloomRenderTarget2DMip3)
                        BloomScreenTexture = _bloomRenderTarget2DMip2
                        _bloomPassDownsample.Apply()
                        _quadRenderer.RenderQuad(_graphicsDevice, Vector2.One * -1, Vector2.One)

                        If BloomDownsamplePasses > 3 Then
                            BloomInverseResolution *= 2


                            _graphicsDevice.SetRenderTarget(_bloomRenderTarget2DMip4)
                            BloomScreenTexture = _bloomRenderTarget2DMip3
                            _bloomPassDownsample.Apply()
                            _quadRenderer.RenderQuad(_graphicsDevice, Vector2.One * -1, Vector2.One)

                            If BloomDownsamplePasses > 4 Then
                                BloomInverseResolution *= 2

                                _graphicsDevice.SetRenderTarget(_bloomRenderTarget2DMip5)
                                BloomScreenTexture = _bloomRenderTarget2DMip4
                                _bloomPassDownsample.Apply()
                                _quadRenderer.RenderQuad(_graphicsDevice, Vector2.One * -1, Vector2.One)
                                ChangeBlendState()
                                _graphicsDevice.SetRenderTarget(_bloomRenderTarget2DMip4)
                                BloomScreenTexture = _bloomRenderTarget2DMip5
                                BloomStrength = _bloomStrength5
                                BloomRadius = _bloomRadius5

                                If BloomUseLuminance Then
                                    _bloomPassUpsampleLuminance.Apply()
                                Else
                                    _bloomPassUpsample.Apply()
                                End If

                                _quadRenderer.RenderQuad(_graphicsDevice, Vector2.One * -1, Vector2.One)
                                BloomInverseResolution /= 2

                                ''' 
                            End If

                            ChangeBlendState()
                            _graphicsDevice.SetRenderTarget(_bloomRenderTarget2DMip3)
                            BloomScreenTexture = _bloomRenderTarget2DMip4
                            BloomStrength = _bloomStrength4
                            BloomRadius = _bloomRadius4

                            If BloomUseLuminance Then
                                _bloomPassUpsampleLuminance.Apply()
                            Else
                                _bloomPassUpsample.Apply()
                            End If

                            _quadRenderer.RenderQuad(_graphicsDevice, Vector2.One * -1, Vector2.One)

                            BloomInverseResolution /= 2

                            ''' 
                        End If

                        ChangeBlendState()
                        _graphicsDevice.SetRenderTarget(_bloomRenderTarget2DMip2)
                        BloomScreenTexture = _bloomRenderTarget2DMip3
                        BloomStrength = _bloomStrength3
                        BloomRadius = _bloomRadius3

                        If BloomUseLuminance Then
                            _bloomPassUpsampleLuminance.Apply()
                        Else
                            _bloomPassUpsample.Apply()
                        End If

                        _quadRenderer.RenderQuad(_graphicsDevice, Vector2.One * -1, Vector2.One)

                        BloomInverseResolution /= 2

                        ''' 
                    End If

                    ChangeBlendState()
                    _graphicsDevice.SetRenderTarget(_bloomRenderTarget2DMip1)
                    BloomScreenTexture = _bloomRenderTarget2DMip2
                    BloomStrength = _bloomStrength2
                    BloomRadius = _bloomRadius2

                    If BloomUseLuminance Then
                        _bloomPassUpsampleLuminance.Apply()
                    Else
                        _bloomPassUpsample.Apply()
                    End If

                    _quadRenderer.RenderQuad(_graphicsDevice, Vector2.One * -1, Vector2.One)

                    BloomInverseResolution /= 2

                    ''' 
                End If

                ChangeBlendState()
                _graphicsDevice.SetRenderTarget(_bloomRenderTarget2DMip0)
                BloomScreenTexture = _bloomRenderTarget2DMip1
                BloomStrength = _bloomStrength1
                BloomRadius = _bloomRadius1

                If BloomUseLuminance Then
                    _bloomPassUpsampleLuminance.Apply()
                Else
                    _bloomPassUpsample.Apply()
                End If

                _quadRenderer.RenderQuad(_graphicsDevice, Vector2.One * -1, Vector2.One)
            End If

            Return _bloomRenderTarget2DMip0
        End Function

        Private Sub ChangeBlendState()
            _graphicsDevice.BlendState = BlendState.AlphaBlend
        End Sub

        Public Sub UpdateResolution(ByVal width As Integer, ByVal height As Integer)
            _width = width
            _height = height

            If _bloomRenderTarget2DMip0 IsNot Nothing Then
                Dispose()
            End If

            _bloomRenderTarget2DMip0 = New RenderTarget2D(_graphicsDevice, CInt((width)), CInt((height)), False, _renderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.DiscardContents)
            _bloomRenderTarget2DMip1 = New RenderTarget2D(_graphicsDevice, CInt((width / 2)), CInt((height / 2)), False, _renderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents)
            _bloomRenderTarget2DMip2 = New RenderTarget2D(_graphicsDevice, CInt((width / 4)), CInt((height / 4)), False, _renderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents)
            _bloomRenderTarget2DMip3 = New RenderTarget2D(_graphicsDevice, CInt((width / 8)), CInt((height / 8)), False, _renderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents)
            _bloomRenderTarget2DMip4 = New RenderTarget2D(_graphicsDevice, CInt((width / 16)), CInt((height / 16)), False, _renderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents)
            _bloomRenderTarget2DMip5 = New RenderTarget2D(_graphicsDevice, CInt((width / 32)), CInt((height / 32)), False, _renderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents)
        End Sub

        Public Sub Dispose()
            _bloomRenderTarget2DMip0.Dispose()
            _bloomRenderTarget2DMip1.Dispose()
            _bloomRenderTarget2DMip2.Dispose()
            _bloomRenderTarget2DMip3.Dispose()
            _bloomRenderTarget2DMip4.Dispose()
            _bloomRenderTarget2DMip5.Dispose()
        End Sub
    End Class
End Namespace
