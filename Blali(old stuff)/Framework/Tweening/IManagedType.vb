

Namespace Framework.Tweening

    <TestState(TestState.Finalized)>
    Public Interface IManagedType
        Function getManagedType() As Type
        Function copy(ByVal o As Object) As Object
        Function getIntermediateValue(ByVal start As Object, ByVal [end] As Object, ByVal dPercentage As Double) As Object
    End Interface
End Namespace
