Imports System
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Framework.UI.Controls
    Public Class ListBox
        Inherits GuiControl
        'Shared assets
        Private Shared Loaded As Boolean = False
        Private Shared ArrowA As Texture2D
        Private Shared ArrowB As Texture2D

        Public Output As Func(Of String()) = Function() {""}
        Public EnableSelect As Boolean = False
        Public SelectedIndex As Integer = -1
        Public Overrides ReadOnly Property InnerBounds As Rectangle
            Get
                Return rect
            End Get
        End Property

        Public Event SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        Private workingtext As String()
        Private rect As Rectangle
        Private par As IParent
        Private offsetY As Integer

        Public Sub New(output As Func(Of String()), location As Vector2, size As Vector2)
            Me.Output = output
            Me.Location = location
            Me.Size = size
            Color = Color.White
            Border = New ControlBorder(Color.White, 2)
            workingtext = {""}
        End Sub

        Public Overrides Sub Init(system As IParent)
            If Font Is Nothing Then Font = system.Font
            par = system

            If Not Loaded Then
                ArrowA = Core.Content.Load(Of Texture2D)("ui/general/arrow_left")
                ArrowB = Core.Content.Load(Of Texture2D)("ui/general/arrow_right")
            End If
        End Sub

        Public Overrides Sub Render(batcher As Batcher, color As Color)
            batcher.DrawRect(rect, BackgroundColor)
            batcher.DrawHollowRect(rect, color, Border.Width)
            For i As Integer = 0 To workingtext.Length - 1
                Dim lv As Vector2 = rect.Location.ToVector2 + New Vector2(0, i * 30 + offsetY)
                Dim rc As New Rectangle(lv.X, lv.Y, Size.X, 20)
                If rc.Top >= rect.Top And rc.Bottom <= rect.Bottom Then batcher.DrawString(Font, workingtext(i), lv, Color.White)
                If i = SelectedIndex And EnableSelect Then batcher.DrawHollowRect(New Rectangle(rect.Location + New Point(0, i * 30 + offsetY), New Point(Size.X, 30)), Color.CornflowerBlue, 2)
            Next

            batcher.Draw(ArrowA, New Rectangle(New Point(rect.Right - 25, rect.Top + 10) + New Point(ArrowA.Height / 2), New Point(25)), Nothing, Color.White, Math.PI / 2, New Vector2(ArrowA.Height / 2), SpriteEffects.None, 0)
            batcher.Draw(ArrowB, New Rectangle(New Point(rect.Right - 25, rect.Bottom - 25) + New Point(ArrowA.Height / 2), New Point(25)), Nothing, Color.White, Math.PI / 2, New Vector2(ArrowA.Height / 2), SpriteEffects.None, 0)
        End Sub

        Public Overrides Sub Update(mstate As GuiInput, offset As Vector2)
            workingtext = Output()
            rect = New Rectangle(Location.X + offset.X, Location.Y + offset.Y, Size.X, Size.Y)

            'Select item by mouse
            For i As Integer = 0 To workingtext.Length - 1
                Dim rc As Rectangle = New Rectangle(rect.Location + New Point(0, i * 30 + offsetY), New Point(Size.X, 30))
                If rc.Contains(mstate.MousePosition) And mstate.LeftClickOneshot And SelectedIndex <> i Then
                    SelectedIndex = i
                    RaiseEvent SelectedIndexChanged(Me, New EventArgs())
                End If
            Next


            'Scroll with wheel
            If rect.Contains(mstate.MousePosition) Then
                If mstate.ScrollDifference <> 0 Then offsetY += mstate.ScrollDifference * 20
            End If

            'Scroll by icons
            If New Rectangle(rect.Right - 25, rect.Top + 10, 25, 25).Contains(mstate.MousePosition) And (mstate.LeftClickOneshot Or mstate.LeftClickFullBlast) Then
                offsetY += CInt(Time.DeltaTime * 1000 / 3)
            ElseIf New Rectangle(rect.Right - 25, rect.Bottom - 25, 25, 25).Contains(mstate.MousePosition) And (mstate.LeftClickOneshot Or mstate.LeftClickFullBlast) Then
                offsetY -= CInt(Time.DeltaTime * 1000 / 3)
            End If

            offsetY = Math.Min(Math.Max(0, offsetY), (workingtext.Length - Size.Y / 30) * -20)
        End Sub
    End Class
End Namespace