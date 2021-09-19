Imports System.Collections.Generic

Namespace Framework.Tweening.TransitionTypes

    <TestState(TestState.Finalized)>
    Public Class TransitionType_Flash
        Inherits TransitionType_UserDefined

        Public Sub New(ByVal iNumberOfFlashes As Integer, ByVal iFlashTime As Integer)
            Dim dFlashInterval As Double = 100.0 / iNumberOfFlashes
            Dim elements As IList(Of TransitionElement) = New List(Of TransitionElement)()

            For i As Integer = 0 To iNumberOfFlashes - 1
                Dim dFlashStartTime As Double = i * dFlashInterval
                Dim dFlashEndTime As Double = dFlashStartTime + dFlashInterval
                Dim dFlashMidPoint As Double = (dFlashStartTime + dFlashEndTime) / 2.0
                elements.Add(New TransitionElement(dFlashMidPoint, 100, InterpolationMethod.EaseInEaseOut))
                elements.Add(New TransitionElement(dFlashEndTime, 0, InterpolationMethod.EaseInEaseOut))
            Next

            MyBase.setup(elements, iFlashTime * iNumberOfFlashes)
        End Sub
    End Class
End Namespace
