Imports Microsoft.Xna.Framework

Namespace Framework.UI.Controls
    Public Class Textbox
        Inherits GuiControl

        Public Text As String
        Public Value As String
        Public ValidCheck As ValidityCheck
        Public ValueChanger As ValueChange
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

        Public Event ValueChanged(ByVal sender As Object, ByVal e As EventArgs)
        Public Delegate Function ValidityCheck(x As String) As Boolean
        Public Delegate Sub ValueChange(x As String)

        Private Shared winopen As Boolean = False
        Private workingtext As String
        Private rect As Rectangle
        Private par As IParent

        Public Sub New(location As Vector2, text As String, value As String, validcheck As ValidityCheck, Optional ValueChanger As ValueChange = Nothing)
            Me.Text = text
            Me.Location = location
            Me.ValidCheck = validcheck
            Me.ValueChanger = ValueChanger
            Me.Value = value
            Color = Color.White
            workingtext = ""
        End Sub

        Public Overrides Sub Init(system As IParent)
            If Font Is Nothing Then Font = system.Font
            par = system
        End Sub

        Public Overrides Sub Render(batcher As Batcher, color As Color)
            batcher.DrawRect(rect, BackgroundColor)
            batcher.DrawHollowRect(rect, color, Border.Width)
            batcher.DrawString(Font, workingtext, rect.Location.ToVector2, color)
        End Sub

        Public Overrides Sub Update(mstate As GuiInput, offset As Vector2)
            workingtext = Text & ": " & Value
            rect = New Rectangle(Location.X + offset.X, Location.Y + offset.Y, Size.X, Size.Y)

            If mstate.LeftClickOneshot And rect.Contains(mstate.MousePosition) Then
                MsgBoxer.OpenInputbox("Enter the new value:", Sub(x, y)
                                                                  If y = 0 AndAlso ValidCheck(x) Then
                                                                      Value = x
                                                                      ValueChanger(x)
                                                                  End If
                                                              End Sub, Value)
            End If
        End Sub
    End Class
End Namespace