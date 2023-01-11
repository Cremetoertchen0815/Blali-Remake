Imports Microsoft.Xna.Framework

Namespace Framework.UI
    Public Structure ControlBorder
        Public Sub New(Color As Color, Width As Integer)
            Me.Color = Color
            Me.Width = Width
        End Sub
        Public Property Color As Color
        Public Property Width As Integer
    End Structure
End Namespace