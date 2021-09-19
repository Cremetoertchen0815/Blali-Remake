Imports System.Collections.Generic

Namespace Framework.Tweening

    <TestState(TestState.Finalized)>
    Friend Class TransitionChain
        Public Sub New(ParamArray transitions As Transition(Of Object)())
            For Each transition In transitions
                m_listTransitions.AddLast(transition)
            Next

            runNextTransition()
        End Sub

        Private Sub runNextTransition()
            If m_listTransitions.Count = 0 Then
                Return
            End If

            Dim nextTransition As Transition(Of Object) = m_listTransitions.First.Value
            AddHandler nextTransition.TransitionCompletedEvent, AddressOf onTransitionCompleted
        End Sub

        Private Sub onTransitionCompleted(ByVal sender As Object, ByVal e As EventArgs)
            Dim transition As Transition(Of Object) = CType(sender, Transition(Of Object))
            RemoveHandler transition.TransitionCompletedEvent, AddressOf onTransitionCompleted
            m_listTransitions.RemoveFirst()
            runNextTransition()
        End Sub

        Private m_listTransitions As LinkedList(Of Transition(Of Object)) = New LinkedList(Of Transition(Of Object))()
    End Class
End Namespace
