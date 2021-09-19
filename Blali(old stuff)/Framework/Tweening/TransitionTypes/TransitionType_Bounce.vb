Imports System.Collections.Generic

Namespace Framework.Tweening.TransitionTypes

    <TestState(TestState.Finalized)>
    Public Class TransitionType_Bounce
        Inherits TransitionType_UserDefined

        Public Sub New(ByVal iTransitionTime As Integer)
            Dim elements As IList(Of TransitionElement) = New List(Of TransitionElement)()
            elements.Add(New TransitionElement(50, 100, InterpolationMethod.Accleration))
            elements.Add(New TransitionElement(100, 0, InterpolationMethod.Deceleration))
            MyBase.setup(elements, iTransitionTime)
        End Sub
    End Class
End Namespace
