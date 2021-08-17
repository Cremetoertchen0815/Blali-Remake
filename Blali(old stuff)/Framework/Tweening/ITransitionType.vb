Imports System.Runtime.InteropServices

Namespace Framework.Tweening

    <TestState(TestState.Finalized)>
    Public Interface ITransitionType
        Sub onTimer(ByVal iTime As Integer, <Out> ByRef dPercentage As Double, <Out> ByRef bCompleted As Boolean)
    End Interface
End Namespace
