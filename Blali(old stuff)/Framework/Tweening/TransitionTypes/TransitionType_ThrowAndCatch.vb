Imports System.Collections.Generic

Namespace Framework.Tweening.TransitionTypes

    <TestState(TestState.Finalized)>
    Public Class TransitionType_ThrowAndCatch
        Inherits TransitionType_UserDefined

        Public Sub New(ByVal iTransitionTime As Integer)
            Dim elements As IList(Of TransitionElement) = New List(Of TransitionElement)()
            elements.Add(New TransitionElement(50, 100, InterpolationMethod.Deceleration))
            elements.Add(New TransitionElement(100, 0, InterpolationMethod.Accleration))
            MyBase.setup(elements, iTransitionTime)
        End Sub
    End Class
End Namespace
