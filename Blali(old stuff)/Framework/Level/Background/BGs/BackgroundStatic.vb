Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Framework.Level.Background.BGs

    <TestState(TestState.NearCompletion)>
    Public Class BackgroundStatic
        Inherits IBackground

        'Background-specific properties
        Public SpeedFactor As Single
        Dim rect As Rectangle
        Dim basic As New Rectangle(0, 0, GameSize.X, GameSize.Y)

        Public Overrides ReadOnly Property Visible As Boolean
            Get
                Return rect.Intersects(basic)
            End Get
        End Property


        'Drawing & Updating methods
        Public Overrides Sub Draw(gameTime As GameTime, Optional drawgreen As Boolean = False)
            If drawgreen Then
                SpriteBat.Draw(Texture, rect, Nothing, New Color(20, 20, 20), 0, New Vector2, SpriteEffects.None, Layer)
            Else
                SpriteBat.Draw(Texture, rect, Nothing, Color, 0, New Vector2, SpriteEffects.None, Layer)
            End If
        End Sub

        Public Function Clone() As BackgroundStatic
            Return Me.MemberwiseClone
        End Function

        Public Overrides Sub Update(gameTime As GameTime, camloc As Vector2)
            rect = New Rectangle(Location.X - camloc.X * VectorScale + GameSize.X / 2, Location.Y - camloc.Y * VectorScale + GameSize.Y / 2, Size.X, Size.Y)
        End Sub

        Public Sub New()
            Initialize()
        End Sub

        Public Overrides Function GetDrawRectangle() As Rectangle
            Return rect
        End Function

        Public Overrides Sub Initialize()
            Size = Vector2.Zero
            Color = Color.White
            Type = 0
        End Sub

        Public Overrides Sub Release()
            Texture = Nothing
        End Sub
    End Class
End Namespace
