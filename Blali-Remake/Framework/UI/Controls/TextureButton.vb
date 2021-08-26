Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Framework.UI.Controls
    Public Class TextureButton
        Inherits GuiControl

        Public Texture As Texture2D

        Public Overrides ReadOnly Property InnerBounds As Rectangle
            Get
                Return rect
            End Get
        End Property

        Public Event Clicked(ByVal sender As Object, ByVal e As EventArgs)
        Private rect As Rectangle
        Private par As IParent

        Public Sub New(text As Texture2D, location As Vector2, size As Vector2)
            Texture = text
            Me.Location = location
            Me.Size = size
            Color = Color.White
            Border = New ControlBorder(Color.White, 2)
            BackgroundColor = New Color(40, 40, 40, 255)
        End Sub

        Public Overrides Sub Init(system As IParent)
            If Font Is Nothing Then Font = system.Font
            par = system
        End Sub

        Public Overrides Sub Render(batcher As Batcher, color As Color)
            batcher.DrawRect(rect, BackgroundColor)
            batcher.DrawHollowRect(rect, color, Border.Width)
            batcher.Draw(Texture, rect, If(Me.Color = Color.Transparent, color, Me.Color))
        End Sub

        Public Overrides Sub Update(mstate As GuiInput, offset As Vector2)
            rect = New Rectangle(Location.X + offset.X, Location.Y + offset.Y, Size.X, Size.Y)
            If mstate.LeftClickOneshot And rect.Contains(mstate.MousePosition) Then
                RaiseEvent Clicked(Me, New EventArgs())
            End If
        End Sub
    End Class
End Namespace