Imports System.Collections.Generic
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Framework.Graphics.Animation

    <TestState(TestState.NearCompletion)>
    Public Class AnimatedSpriteSheet

        Public Sub Update(ByVal gameTime As GameTime)
            If AutoUpdate Then
                _durationPassed += gameTime.ElapsedGameTime.TotalMilliseconds

                If _ckeyframe <= _keyframes.Count - 1 Then

                    If _durationPassed >= _keyframes(_ckeyframe).Duration Then
                        _ckeyframe += 1
                        _durationPassed = 0
                    End If
                Else
                    _ckeyframe = 0
                End If

                ActivateKeyframe(_ckeyframe)
            End If
        End Sub

        Public Sub Draw(ByVal Rectangle As Rectangle, color As Color)
            SpriteBat.Draw(_texture, Rectangle, CurrentKeyframe.Rectangle, color)
        End Sub

        Public Sub Draw(ByVal Rectangle As Rectangle, color As Color, rotation As Double, effects As SpriteEffects, layerdepth As Single)
            SpriteBat.Draw(_texture, Rectangle, CurrentKeyframe.Rectangle, color, rotation, New Vector2(0, 0), effects, layerdepth)
        End Sub

        Private _texture As Texture2D
        Private ReadOnly _keyframes As List(Of Keyframe)
        Private _ckeyframe As Integer
        Private _durationPassed As Single

        Public Sub New(texture As Texture2D)
            _texture = texture
            _keyframes = New List(Of Keyframe)()
        End Sub

        Public Property CurrentKeyframe As Keyframe
        Public Property AutoUpdate As Boolean
        Public WriteOnly Property Duration As Double
            Set(value As Double)
                For Each element In _keyframes
                    element.Duration = value
                Next
            End Set
        End Property

        Public Sub Add(ByVal keyframe As Keyframe)
            _keyframes.Add(keyframe)
        End Sub

        Public Sub Remove(ByVal keyframe As Keyframe)
            If _keyframes.Contains(keyframe) Then
                _keyframes.Remove(keyframe)
            End If
        End Sub

        Public Sub RemoveAt(ByVal index As Integer)
            If index < _keyframes.Count - 1 Then
                _keyframes.RemoveAt(index)
            End If
        End Sub

        Public Sub ActivateKeyframe(ByVal index As Integer)
            If index <= _keyframes.Count - 1 Then
                CurrentKeyframe = _keyframes(index)
            End If
        End Sub
    End Class
End Namespace
