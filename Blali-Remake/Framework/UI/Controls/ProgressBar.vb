Imports Microsoft.Xna.Framework

Namespace Framework.UI.Controls
    Public Class ProgressBar
        Inherits GuiControl
        Public Property Progress As Func(Of Single)
        Public Property Max As Single
        Public Overrides ReadOnly Property InnerBounds As Rectangle
            Get
                Return rect
            End Get
        End Property

        Private rect As Rectangle
        Private par As IParent

        Public Sub New(location As Vector2, size As Vector2)
            Progress = Function() 0.5
            Max = 1
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
            batcher.DrawRect(New Rectangle(rect.X + 6, rect.Y + 6, (rect.Width - 12) / Max * Progress.Invoke, rect.Height - 12), color)
        End Sub

        Public Overrides Sub Update(mstate As GuiInput, offset As Vector2)
            rect = New Rectangle(Location.X + offset.X, Location.Y + offset.Y, Size.X, Size.Y)
        End Sub
    End Class
End Namespace