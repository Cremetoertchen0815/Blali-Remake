
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Framework.Level.Background.BGs

    <TestState(TestState.NearCompletion)>
    Public Class BackgroundFlex
        Inherits IBackground

        'Background-specific properties
        Public SpeedFactor As Single
        Dim divider As Integer

        Dim xposA As Integer
        Dim xposB As Integer
        Dim rectA As Rectangle
        Dim rectB As Rectangle

        Public Overrides ReadOnly Property Visible As Boolean
            Get
                Return rectA.Top < GameSize.Y And rectA.Bottom > 0
            End Get
        End Property


        'Drawing & Updating methods
        Public Overrides Sub Draw(gameTime As GameTime, Optional drawgreen As Boolean = False)
            If drawgreen Then
                SpriteBat.Draw(Texture, rectA, Nothing, New Color(20, 20, 20), 0, New Vector2, SpriteEffects.None, Layer)
                SpriteBat.Draw(Texture, rectB, Nothing, New Color(20, 20, 20), 0, New Vector2, SpriteEffects.None, Layer)
            Else
                SpriteBat.Draw(Texture, rectA, Nothing, Color, 0, New Vector2, SpriteEffects.None, Layer)
                SpriteBat.Draw(Texture, rectB, Nothing, Color, 0, New Vector2, SpriteEffects.None, Layer)
            End If
        End Sub


        Public Overrides Sub Update(gameTime As GameTime, camloc As Vector2)
            xposA = 0
            xposB = xposA + GameSize.X

            rectA = New Rectangle((Math.Round(-camloc.X * (VectorScale / divider)) + xposA + Location.X), Math.Round(-camloc.Y * (VectorScale / divider) + Location.Y), GameSize.X, Size.Y)
            rectA.X -= Math.Floor(rectA.X / GameSize.X) * GameSize.X
            rectB = New Rectangle(rectA.X - GameSize.X, rectA.Y, GameSize.X, Size.Y)
        End Sub

        Public Sub New(TextureN As Texture2D, scrollingspeed As Single)
            Texture = TextureN
            VectorScale = scrollingspeed
            Color = Color.White
            Type = 1
            divider = 1
        End Sub

        Public Sub New()
            Initialize()
        End Sub

        Public Overrides Function GetDrawRectangle() As Rectangle
            Return New Rectangle(0, rectA.Y, GameSize.X, Size.Y)
        End Function

        Public Overrides Sub Initialize()
            Color = Color.White
            Type = 1
            divider = 1
        End Sub

        Public Overrides Sub Release()
            Texture = Nothing
        End Sub
    End Class
End Namespace
