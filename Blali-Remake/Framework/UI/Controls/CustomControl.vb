Imports Microsoft.Xna.Framework

Namespace Framework.UI.Controls
    Public Class CustomControl
        Inherits GuiControl

        Public UpdateSubroutine As ExternalUpdate
        Public DrawSubroutine As ExternalDraw

        Public Overrides ReadOnly Property InnerBounds As Rectangle
            Get
                Return rect
            End Get
        End Property

        Public Delegate Sub ExternalDraw(batcher As Batcher, InnerBounds As Rectangle, color As Color)
        Public Delegate Sub ExternalUpdate(mstate As GuiInput, InnerBounds As Rectangle)

        Private rect As Rectangle
        Private par As IParent

        Public Sub New(draw As ExternalDraw, update As ExternalUpdate, location As Vector2, size As Vector2)
            UpdateSubroutine = update
            DrawSubroutine = draw
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
            DrawSubroutine(batcher, InnerBounds, color)
        End Sub

        Public Overrides Sub Update(mstate As GuiInput, offset As Vector2)
            rect = New Rectangle(Location.X + offset.X, Location.Y + offset.Y, Size.X, Size.Y)
            UpdateSubroutine(mstate, rect)
        End Sub
    End Class
End Namespace