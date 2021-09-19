Imports System.Collections.Generic

Namespace Framework.Tweening.TransitionTypes

    <TestState(TestState.NearCompletion)>
    Public Class TransitionType_Swing
        Inherits TransitionType_UserDefined

        Public Sub New(ByVal iFrequency As Single, ByVal iTransitionTime As Integer)
            Dim elements As IList(Of TransitionElement) = New List(Of TransitionElement)()

            Dim wavelength As Single = 1 / iFrequency
            If wavelength * 1000 > iTransitionTime Then Throw New InvalidOperationException("Transition time must be greater than or equal the wavelength")
            Dim periods As Integer = Math.Ceiling((iTransitionTime / 1000) / wavelength)
            For i As Integer = 0 To periods - 1
                Dim tA As Single = (i + 0.25) * wavelength
                Dim tB As Single = (i + 0.5) * wavelength
                Dim tC As Single = (i + 0.75) * wavelength
                Dim tD As Single = (i + 1) * wavelength
                elements.Add(New TransitionElement(tA * 1000 / iTransitionTime * 100, 100 - (tA * 1000 / iTransitionTime * 100), InterpolationMethod.Deceleration))
                elements.Add(New TransitionElement(tB * 1000 / iTransitionTime * 100, 0, InterpolationMethod.Accleration))
                elements.Add(New TransitionElement(tC * 1000 / iTransitionTime * 100, -100 + (tA * 1000 / iTransitionTime * 100), InterpolationMethod.Deceleration))
                elements.Add(New TransitionElement(tD * 1000 / iTransitionTime * 100, 0, InterpolationMethod.Accleration))

                If i = 49 Then Console.WriteLine()
            Next
            MyBase.setup(elements, iTransitionTime)
        End Sub
    End Class
End Namespace