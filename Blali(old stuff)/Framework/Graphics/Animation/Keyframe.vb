Imports Microsoft.Xna.Framework

Namespace Framework.Graphics.Animation

    <TestState(TestState.Finalized)>
    Public Class Keyframe
        Public Sub New(ByVal rectangleS As Rectangle, DurationS As Double)
            Rectangle = rectangleS
            Duration = DurationS
        End Sub

        Public Property Rectangle As Rectangle
        Public Property Duration As Double
    End Class
End Namespace
