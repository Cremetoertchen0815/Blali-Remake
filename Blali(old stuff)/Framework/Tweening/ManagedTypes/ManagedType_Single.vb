Namespace Framework.Tweening.ManagedTypes

    <TestState(TestState.Finalized)>
    Friend Class ManagedType_Single
        Implements IManagedType

        Public Function getManagedType() As Type Implements IManagedType.getManagedType
            Return GetType(Single)
        End Function

        Public Function copy(ByVal o As Object) As Object Implements IManagedType.copy
            Dim f As Single = CSng(o)
            Return f
        End Function

        Public Function getIntermediateValue(ByVal start As Object, ByVal [end] As Object, ByVal dPercentage As Double) As Object Implements IManagedType.getIntermediateValue
            Dim fStart As Single = CSng(start)
            Dim fEnd As Single = CSng([end])
            Return interpolate(fStart, fEnd, dPercentage)
        End Function
    End Class
End Namespace
