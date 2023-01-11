Imports Microsoft.Xna.Framework

Namespace Framework.UI.Controls
    Public Class SimpleContainer
        Inherits GuiControl
        Public Overrides ReadOnly Property InnerBounds As Rectangle
            Get
                Return rect
            End Get
        End Property

        Private rect As Rectangle
        Private par As IParent

        Public Sub New(location As Vector2, size As Vector2)
            Me.Location = location
            Color = Color.White
            Me.Size = size
        End Sub

        Public Overrides Sub Init(system As IParent)
            If Font Is Nothing Then Font = system.Font
            par = system

            For Each element In Children
                element.Init(Me)
            Next
        End Sub

        Public Overrides Sub Unload()
            MyBase.Unload()
            For Each element In Children
                element.Unload()
                element = Nothing
            Next
        End Sub

        Public Overrides Sub Render(batcher As Batcher, color As Color)
            batcher.DrawRect(rect, BackgroundColor)
            batcher.DrawHollowRect(rect, Border.Color, Border.Width)

            For Each element In Children
                If element.Active Then element.Render(batcher, color)
            Next
        End Sub

        Public Overrides Sub Update(mstate As GuiInput, offset As Vector2)
            rect = New Rectangle(Location.X + offset.X, Location.Y + offset.Y, Size.X, Size.Y)

            For Each element In Children
                If element.Active Then element.Update(mstate, rect.Location.ToVector2)
            Next
        End Sub
    End Class
End Namespace