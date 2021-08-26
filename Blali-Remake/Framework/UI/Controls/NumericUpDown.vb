Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Framework.UI.Controls
    Public Class NumericUpDown
        Inherits GuiControl
        'Shared assets
        Private Shared Loaded As Boolean = False
        Private Shared ArrowA As Texture2D
        Private Shared ArrowB As Texture2D

        'Properties & variables
        Public MinValue As Single = 0
        Public MaxValue As Single = 10
        Public [Step] As Single = 1
        Public Text As String
        Public Value As Single
        Public OutputFormat As OutputFormatter = Function(x) x.ToString
        Public Overrides Property Size As Vector2
            Get
                Dim ms As Vector2 = Font.MeasureString(Text & ": " & Value.ToString)
                Return New Vector2(ms.X + 36, Math.Max(ms.Y, 15))
            End Get
            Set(value As Vector2)

            End Set
        End Property
        Public Overrides ReadOnly Property InnerBounds As Rectangle
            Get
                Return rect
            End Get
        End Property

        'Events & Delegates
        Public Event Clicked(ByVal sender As Object, ByVal e As EventArgs)
        Public Event ValueChanged(ByVal sender As Object, ByVal e As EventArgs)
        Public Delegate Function OutputFormatter(input As Single) As String

        'Private flags
        Private rect As Rectangle
        Private bgrect As Rectangle
        Private par As IParent

        Public Sub New(text As String, location As Vector2)
            Me.Text = text
            Me.Location = location
            Color = Color.White
            Border = New ControlBorder(Color.White, 0)
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
            batcher.DrawRect(bgrect, BackgroundColor)
            batcher.DrawHollowRect(bgrect, color, Border.Width)

            Dim cc As String = OutputFormat(Value)
            batcher.Draw(ArrowA, New Rectangle(rect.Location, New Point(15)), Color.White)
            batcher.DrawString(Font, Text & ": " & cc, rect.Location.ToVector2 + New Vector2(18, -5), Color.White)
            batcher.Draw(ArrowB, New Rectangle(New Point(rect.Location.X + Font.MeasureString(Text & ": " & cc).X + 21, rect.Location.Y), New Point(15)), Color.White)
        End Sub

        Public Overrides Sub Update(mstate As GuiInput, offset As Vector2)
            rect = New Rectangle(Location.X + par.Bounds.Location.X, Location.Y + par.Bounds.Location.Y, Size.X, Size.Y)
            bgrect = New Rectangle(Location.X + par.Bounds.Location.X, Location.Y + par.Bounds.Location.Y - 6, Size.X, Size.Y)

            Dim mstater As Boolean = mstate.LeftClickOneshot Or mstate.LeftClickFullBlast

            Dim mosrectA As New Rectangle(rect.Location - New Point(0, 5), New Point(15))
            Dim mosrectB As New Rectangle(New Point(rect.Location.X + Font.MeasureString(Text & ": " & OutputFormat.Invoke(Math.Round(Value, 3))).X + 21, rect.Location.Y - 5), New Point(15))

            If mstater And rect.Contains(mstate.MousePosition) Then
                RaiseEvent Clicked(Me, New EventArgs)
            End If
            If mstater And mosrectB.Contains(mstate.MousePosition) Then
                If Value < MaxValue Then Value += [Step] : RaiseEvent ValueChanged(Me, New EventArgs)
            End If
            If mstater And mosrectA.Contains(mstate.MousePosition) Then
                If Value - [Step] >= MinValue Then Value -= [Step] : RaiseEvent ValueChanged(Me, New EventArgs)
            End If

            Value = Math.Round(Math.Min(Math.Max(Value, MinValue), MaxValue), 3)
        End Sub
    End Class
End Namespace