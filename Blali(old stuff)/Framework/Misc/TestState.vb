Namespace Framework.Misc
    <TestState(TestState.Finalized)>
    Public Class TestStateAttribute
        Inherits Attribute

        Public Sub New(ByVal state As TestState)
            state = state
        End Sub

        Public Property State As TestState
    End Class
    Public Enum TestState
        DebugOnly = -1
        Placeholder = 0
        WorkInProgress = 1
        NearCompletion = 2
        Finalized = 3
    End Enum
End Namespace
