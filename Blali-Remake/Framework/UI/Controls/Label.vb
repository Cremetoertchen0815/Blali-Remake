Imports Microsoft.Xna.Framework

Namespace Framework.UI.Controls
    Public Class Label
        Inherits GuiControl

        Public Property Text As String
        Public OutputFormat As Func(Of String) = Function() Text
        Public Overrides Property Size As Vector2
            Get
                Return Font.MeasureString(workingtext)
            End Get
            Set(value As Vector2)

            End Set
        End Property
        Public Overrides ReadOnly Property InnerBounds As Rectangle
            Get
                Return rect
            End Get
        End Property

        Public Event Clicked(ByVal sender As Object, ByVal e As EventArgs)
        Private workingtext As String
        Private rect As Rectangle

        Public Sub New(text As String, location As Vector2)
            Me.Text = text
            Me.Location = location
            Color = Color.Transparent
            workingtext = ""
        End Sub

        Public Sub New(output As Func(Of String), location As Vector2)
            OutputFormat = output
            Me.Location = location
            Color = Color.Transparent
            workingtext = ""
        End Sub

        Public Overrides Sub Init(system As IParent)
            If Font Is Nothing Then Font = system.Font
        End Sub

        Public Overrides Sub Render(batcher As Batcher, color As Color)
            batcher.DrawRect(rect, BackgroundColor)
            batcher.DrawHollowRect(rect, color, Border.Width)
            batcher.DrawString(Font, workingtext, rect.Location.ToVector2, If(Me.Color = Color.Transparent, color, Me.Color))
        End Sub

        Public Overrides Sub Update(mstate As GuiInput, offset As Vector2)
            workingtext = OutputFormat()
            rect = New Rectangle(Location.X + offset.X, Location.Y + offset.Y, Size.X, Size.Y)

            If mstate.LeftClickOneshot And rect.Contains(mstate.MousePosition) Then
                RaiseEvent Clicked(Me, New EventArgs())
            End If
        End Sub
    End Class
End Namespace