Namespace Framework.Tweening

    <TestState(TestState.Finalized)>
    Public Enum InterpolationMethod
        Linear
        Accleration
        Deceleration
        EaseInEaseOut
    End Enum

    <TestState(TestState.Finalized)>
    Public Class TransitionElement
        Public Sub New(ByVal endTime As Double, ByVal endValue As Double, ByVal interpolationMethod As InterpolationMethod)
            Me.EndTime = endTime
            Me.EndValue = endValue
            Me.InterpolationMethod = interpolationMethod
        End Sub

        Public Property EndTime As Double
        Public Property EndValue As Double
        Public Property InterpolationMethod As InterpolationMethod
    End Class
End Namespace
