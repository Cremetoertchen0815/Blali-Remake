Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Framework.Graphics.PostProcessing

    <TestState(TestState.WorkInProgress)>
    Public Class GaussianBlur

        Private Effect As Effect

        Public Property BlurAmount As Single
            Get
                Return _blurAmount
            End Get
            Set(ByVal value As Single)

                If _blurAmount <> value Then
                    If value = 0 Then value = 0.001F
                    _blurAmount = value
                    CalculateSampleWeights()
                End If
            End Set
        End Property

        Public Property HorizontalBlurDelta As Single
            Get
                Return _horizontalBlurDelta
            End Get
            Set(ByVal value As Single)

                If value <> _horizontalBlurDelta Then
                    _horizontalBlurDelta = value
                    SetBlurEffectParameters(_horizontalBlurDelta, 0, _horizontalSampleOffsets)
                End If
            End Set
        End Property

        Public Property VerticalBlurDelta As Single
            Get
                Return _verticalBlurDelta
            End Get
            Set(ByVal value As Single)

                If value <> _verticalBlurDelta Then
                    _verticalBlurDelta = value
                    SetBlurEffectParameters(0, _verticalBlurDelta, _verticalSampleOffsets)
                End If
            End Set
        End Property

        Private _blurAmount As Single = 2.0F
        Private _horizontalBlurDelta As Single = 0.01F
        Private _verticalBlurDelta As Single = 0.01F
        Private _sampleCount As Integer
        Private _sampleWeights As Single()
        Private _verticalSampleOffsets As Vector2()
        Private _horizontalSampleOffsets As Vector2()
        Private _blurWeightsParam As EffectParameter
        Private _blurOffsetsParam As EffectParameter

        Public Sub New()
            Effect = ContentMan.Load(Of Effect)("fx/fx_gaussianblur")
            _blurWeightsParam = Effect.Parameters("_sampleWeights")
            _blurOffsetsParam = Effect.Parameters("_sampleOffsets")
            _sampleCount = _blurWeightsParam.Elements.Count
            _sampleWeights = New Single(_sampleCount - 1) {}
            _verticalSampleOffsets = New Vector2(_sampleCount - 1) {}
            _horizontalSampleOffsets = New Vector2(_sampleCount - 1) {}
            _verticalSampleOffsets(0) = Vector2.Zero
            _horizontalSampleOffsets(0) = Vector2.Zero
            CalculateSampleWeights()
            SetBlurEffectParameters(_horizontalBlurDelta, 0, _horizontalSampleOffsets)
            PrepareForHorizontalBlur()
        End Sub

        Public Sub PrepareForHorizontalBlur()
            _blurOffsetsParam.SetValue(_horizontalSampleOffsets)
        End Sub

        Public Sub PrepareForVerticalBlur()
            _blurOffsetsParam.SetValue(_verticalSampleOffsets)
        End Sub

        Private Sub SetBlurEffectParameters(ByVal dx As Single, ByVal dy As Single, ByVal offsets As Vector2())
            For i = 0 To _sampleCount / 2 - 1
                Dim sampleOffset = i * 2 + 1.5F
                Dim delta = New Vector2(dx, dy) * sampleOffset
                offsets(i * 2 + 1) = delta
                offsets(i * 2 + 2) = -delta
            Next
        End Sub

        Private Sub CalculateSampleWeights()
            _sampleWeights(0) = ComputeGaussian(0)
            Dim totalWeights = _sampleWeights(0)

            For i = 0 To _sampleCount / 2 - 1
                Dim weight = ComputeGaussian(i + 1)
                _sampleWeights(i * 2 + 1) = weight
                _sampleWeights(i * 2 + 2) = weight
                totalWeights += weight * 2
            Next

            For i = 0 To _sampleWeights.Length - 1
                _sampleWeights(i) /= totalWeights
            Next

            _blurWeightsParam.SetValue(_sampleWeights)
        End Sub

        Private Function ComputeGaussian(ByVal n As Single) As Single
            Return CSng(((1.0 / Math.Sqrt(2 * Math.PI * _blurAmount)) * Math.Exp(-(n * n) / (2 * _blurAmount * _blurAmount))))
        End Function
    End Class
End Namespace
