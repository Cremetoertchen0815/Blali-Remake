Imports System
Imports System.Collections.Generic

Namespace Framework.UI.Controls
    Public Class TextscrollBox
        Inherits GuiControl

        Public Property Text As String
        Public OutputFormat As Func(Of (String, Color)()) = Function() {(Text, Color.White)}
        Public Overrides ReadOnly Property InnerBounds As Rectangle
            Get
                Return rect
            End Get
        End Property

        Public Event Clicked(ByVal sender As Object, ByVal e As EventArgs)

        Public Property LenLimit As Integer = 30

        Friend ScrollDown As Boolean = False
        Private scrolloffset As Integer = 0
        Private maxlines As Integer = 0
        Private workingtext As (String, Color)()
        Private rect As Rectangle
        Private par As IParent

        Public Sub New(output As Func(Of (String, Color)()), location As Vector2, size As Vector2)
            OutputFormat = output
            Me.Location = location
            Color = Color.White
            Me.Size = size
            workingtext = {}
        End Sub

        Public Overrides Sub Init(system As IParent)
            If Font Is Nothing Then Font = system.Font
            par = system
        End Sub

        Public Overrides Sub Render(batcher As Batcher, color As Color)
            batcher.DrawRect(rect, BackgroundColor)
            batcher.DrawHollowRect(rect, color, Border.Width)

            For i As Integer = scrolloffset To Math.Min(scrolloffset + maxlines, workingtext.Length - 1)
                batcher.DrawString(Font, workingtext(i).Item1, rect.Location.ToVector2 + New Vector2(10, Font.LineSpacing * (i - scrolloffset) + 5), workingtext(i).Item2)
            Next
        End Sub

        Private tmplist As New List(Of (String, Color))
        Public Overrides Sub Update(mstate As GuiInput, offset As Vector2)

            'Prepare text
            Dim ln As (String, Color)() = OutputFormat()
            tmplist.Clear()
            For i As Integer = 0 To ln.Length - 1
                For Each element In WrapTextDifferently(ln(i).Item1, LenLimit, False).Split(Environment.NewLine)
                    tmplist.Add((element.Replace(Microsoft.VisualBasic.vbLf, ""), ln(i).Item2))
                Next
            Next
            workingtext = tmplist.ToArray

            rect = New Rectangle(Location.X + offset.X, Location.Y + offset.Y, Size.X, Size.Y)

            If rect.Contains(mstate.MousePosition) Then
                If mstate.LeftClickOneshot Then RaiseEvent Clicked(Me, New EventArgs())
                If mstate.ScrollDifference < 0 And scrolloffset + maxlines < workingtext.Length - 1 Then scrolloffset += 1
                If mstate.ScrollDifference > 0 Then scrolloffset -= 1
            End If

            If ScrollDown Then
                ScrollDown = False
                Scroll(workingtext.Length - 1)
            End If

            maxlines = Math.Ceiling(Size.Y / Font.LineSpacing - 2)
            scrolloffset = Math.Max(scrolloffset, 0)

        End Sub

        Friend Sub Scroll(index As Integer)
            scrolloffset = Math.Max(0, index - maxlines)
        End Sub
    End Class
End Namespace